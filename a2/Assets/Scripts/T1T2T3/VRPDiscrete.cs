using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VRPDiscrete  {

	private float max_astar_distance = 0f;
	int width, height, neighbors;
	static System.Random _random = new System.Random();
	
	private Dictionary<int, GameObject> chromosomeIDs = new Dictionary<int, GameObject>();


	public VRPDiscrete() {
		this.width = GameState.Instance.width;
		this.height = GameState.Instance.height;
		this.neighbors = GameState.Instance.neighbors;
	}

	public Dictionary<Agent, List<List<GNode>>> planVRPPaths (List<GameObject> agents, List<GameObject> customers, List<Vector3> occupiedSlots, int rand_iterations, int GA_iterations, int population, int tournaments) {
	
		
		Dictionary<Agent, List<List<GNode>>> result = new Dictionary<Agent, List<List<GNode>>>();
		Dictionary<Agent, List<List<GNode>>> bestResult = new Dictionary<Agent, List<List<GNode>>>();
		
		int[] chromosome = new int[customers.Count+agents.Count];
		
		int c = 0;
		foreach (GameObject a in agents) {
			chromosome[c] = a.GetInstanceID();
			chromosomeIDs[a.GetInstanceID()] = a;
			c++;
		}
		
		foreach (GameObject a in customers) {
			chromosome[c] = a.GetInstanceID();
			chromosomeIDs[a.GetInstanceID()] = a;
			c++;
		}

		GNode[,] graph = PathPlanner.buildGraph (width, height, neighbors, occupiedSlots);

		
		// Run GA Algorithm
		GeneticsDiscrete genDisc = new GeneticsDiscrete(chromosome, GA_iterations, population, tournaments, 0.1f, agents, customers, graph, chromosomeIDs);
		Debug.Log ("Best result (from GA): " + genDisc.get_result().first);

		bestResult = genDisc.get_result().second;

		
		Dictionary<Agent, List<GNode>> newPaths = PathPlanner.avoidCollision(bestResult, width, height);
		
		foreach(KeyValuePair<Agent, List<GNode>> entry in newPaths)
		{
			Agent a = entry.Key;
			if (entry.Value.Count == 0) {
				a.setStart (a.transform.position);
				a.setGoal (a.transform.position);
			} else {
				a.setStart(entry.Value[entry.Value.Count-1].getPos());
				a.setGoal(entry.Value[entry.Value.Count-1].getPos());
			}
			a.removePaths();
			a.setModel(0);
			a.setPath(entry.Value);
			PathFinding.draw(entry.Value);
		}

		return bestResult;
		
	}
	
	private Dictionary<Agent, List<List<GNode>>> chromosomeToResult(int[] chromosome, List<GameObject> customers, GNode[,] graph) {
		Dictionary<Agent, List<List<GNode>>> result = new Dictionary<Agent, List<List<GNode>>>();
		
		int totalCustomers = 0;
		for (int i = 0; i < chromosome.Length; ) {
			GameObject agent = chromosomeIDs[chromosome[i]];
			
			int x = (int) agent.transform.position.x;
			int z = (int) agent.transform.position.z;
			GNode start = graph [x, z];
			
			int number_of_customers = 0;
			if (totalCustomers >= customers.Count)
				break;
			
			Agent a = (Agent) agent.GetComponent(typeof(Agent));

			GNode previousGoal = start;
			
			float distance = 0f;

			result[a] = new List<List<GNode>>();
			
			while (chromosomeIDs[chromosome[i+number_of_customers+1]].GetComponent("Agent") == null) {
				
				GameObject customer = customers[totalCustomers];
				int c_x = (int) customer.transform.position.x;
				int c_z = (int) customer.transform.position.z;
				GNode goal = graph [c_x, c_z];
				number_of_customers++;
				totalCustomers++;
				

				List<GNode> path = PathFinding.aStarPath(previousGoal, goal, GraphBuilder.distance);
				
				previousGoal = path[0];
				
				result[a].Add (path);
				
				distance += distance_astar_discrete(path); // TODO denna ska väl vara distance_astar_discrete
				
				if (totalCustomers >= customers.Count)
					break;
				
			}
			
			
			if (distance > max_astar_distance)
				max_astar_distance = distance;
			
			i = i + number_of_customers + 1;
			
		}
		
		return result;
	}
	
	private void drawPaths(Dictionary<Agent, List<List<GNode>>> res) {
		foreach(KeyValuePair<Agent, List<List<GNode>>> entry in res)
		{
			Color color = randomizeColor();
			for (int i = 0; i < entry.Value.Count;i++) {
				PathFinding.draw (entry.Value[i], color);
				
			}
		}
	}
	
	private void addPaths(Agent a, List<List<GNode>> paths) {
		List<List<GNode>> copy = new List<List<GNode>>();

		copy = paths.ToList<List<GNode>>();
		a.currentPath = new List<GNode>();
		for (int i = 0; i < copy.Count;i++) {

			//copy[i].RemoveAt(copy[i].Count-1); // remove to avoid duplicates
			a.addPath(copy[i]);
			
		}
	}
	
	private float euclidean_fitness(int[] chromo) {
		float max_distance = 0f;
		float dist = 0;
		
		for (int i = 0; i < chromo.Length - 1; i++) {
			
			if (chromosomeIDs[chromo[i+1]].GetComponent("Agent") != null) {
				GameObject obj = chromosomeIDs[chromo[i]];
				GameObject nextobj = chromosomeIDs[chromo[i+1]];
				dist += Vector3.Distance(obj.transform.position, nextobj.transform.position);
			} else {
				if (dist > max_distance)
					max_distance = dist;
				dist = 0;
			}
			
		}
		return max_distance;
		
	}
	
	private float distance_astar(List<GNode> path) {
		float distance = 0f;
		for (int i = 0; i < path.Count-1; i++) {
			distance += Vector3.Distance(path[i].getPos(), path[i+1].getPos ());
		}
		return distance;
	}
	
	private float distance_astar_discrete(List<GNode> path) {
		return path.Count;
	}
	
	private Color randomizeColor() {
		float r = UnityEngine.Random.Range(0.0f, 1f);
		float b = UnityEngine.Random.Range(0.0f, 1f);
		float g = UnityEngine.Random.Range(0.0f, 1f);
		return new Color (r, g, b, 1.0f);
	}

	void Shuffle(int[] array)
	{
		int n = array.Length;
		for (int i = 0; i < n; i++)
		{
			int r = i + (int)(_random.NextDouble() * (n - i));
			Random hej = new Random();
			int t = array[r];
			array[r] = array[i];
			array[i] = t;
		}
		
		int index_first_agent = 0;
		while (chromosomeIDs[array[index_first_agent]].GetComponent("Agent") == null) {
			index_first_agent++;
		}
		
		int temp = array[index_first_agent];
		array[index_first_agent] = array[0];
		array[0] = temp;
		
		
	}

}
