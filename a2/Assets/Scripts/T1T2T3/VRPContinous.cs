using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class VRPContinous {

	private float max_astar_distance = 0f;
	static System.Random _random = new System.Random();
	float width, height;
	
	private Dictionary<int, GameObject> chromosomeIDs = new Dictionary<int, GameObject>();

	public VRPContinous() {
		this.width = GameState.Instance.width;
		this.height = GameState.Instance.height;
	}


	public void planContinuousVRP (List<GameObject> agents, List<GameObject> customers, List<Vector2[]> polygons, int iterations) {
		
		Dictionary<Agent, List<List<GNode>>> result = new Dictionary<Agent, List<List<GNode>>>();
		Dictionary<Agent, List<List<GNode>>> bestResult = new Dictionary<Agent, List<List<GNode>>>();
		
		int[] chromosome = new int[customers.Count+agents.Count];
		
		int c = 0;
		foreach (GameObject a in agents) {
			chromosome[c] = a.GetInstanceID();
			chromosomeIDs[a.GetInstanceID()] = a;
			//Debug.Log ("id: " + a.GetInstanceID());
			//Debug.Log ("start: " + chromosomeIDs[a.GetInstanceID()].transform.position.x + " " + chromosomeIDs[a.GetInstanceID()].transform.position.z);
			Agent hej = (Agent) chromosomeIDs[a.GetInstanceID()].GetComponent(typeof(Agent));
			hej.start = a.transform.position;
			c++;
		}
		
		foreach (GameObject a in customers) {
			chromosome[c] = a.GetInstanceID();
			chromosomeIDs[a.GetInstanceID()] = a;
			c++;
		}
		
		
		float current_best = 100000000f;
		for (int iter = 0; iter < iterations; iter++) {
			Shuffle(chromosome);
			
			max_astar_distance = 0f;
			
			result = chromosomeToResultContinous(chromosome, customers, polygons, width, height);
			
			
			if (current_best > max_astar_distance) {
				current_best = max_astar_distance;
				Debug.Log ("current best: " + current_best);
				bestResult = result;
				foreach(KeyValuePair<Agent, List<List<GNode>>> entry in bestResult)
				{
					entry.Key.removePaths();
					//entry.Key.setModel(2);
					if (entry.Value.Count != 0)
						addPaths(entry.Key, entry.Value);
					PathFinding.clearDrawnPaths();
					drawPaths (bestResult);
				}
				
			}
		}
		
		
	}
	
	private Dictionary<Agent, List<List<GNode>>> chromosomeToResultContinous(int[] chromosome, List<GameObject> customers, List<Vector2[]> polygons, float width, float height) {
		Dictionary<Agent, List<List<GNode>>> result = new Dictionary<Agent, List<List<GNode>>>();
		
		int totalCustomers = 0;
		//Debug.Log ("______________________");
		for (int i = 0; i < chromosome.Length; ) {
			GameObject agent = chromosomeIDs[chromosome[i]];
			
			int number_of_customers = 0;
			if (totalCustomers >= customers.Count)
				break;
			
			Agent a = (Agent) agent.GetComponent(typeof(Agent));
			//a.init();
			a.setModel(2); // TOOD denna ska ju vara 0, för att köra discrete model
			
			Vector3 previousStart = a.start;
			a.setStart(previousStart);
			a.setGoal(previousStart);
			//			Debug.Log ("id: " + chromosome[i]);
			//			Debug.Log ("start: " + previousStart.x + " " + previousStart.z);
			
			float distance = 0f;
			
			result[a] = new List<List<GNode>>();
			
			while (chromosomeIDs[chromosome[i+number_of_customers+1]].GetComponent("Agent") == null) {
				
				GameObject customer = customers[totalCustomers];
				number_of_customers++;
				totalCustomers++;
				
				float acceptableWidth;
				float minAngle;
				
				//acceptableWidth = System.Math.Max(GameObject.FindWithTag ("Agent").transform.localScale.x * 2, GameObject.FindWithTag ("Agent").transform.localScale.y * 2) + 0.5f;
				acceptableWidth = 0.0f;
				minAngle = 360f;
				Vector3[] bounds = new Vector3[4];
				
				RRT rrt = new RRT (previousStart, customer.transform.position, bounds, polygons, 1.0f, 1f, acceptableWidth, minAngle, acceptableWidth, width, height);
				
				rrt.buildRRT (10000);
				rrt.tree.draw ();
				
				
				Tuple<GNode, GNode> startGoal = rrt.generateGraph();
				List<GNode> path = PathFinding.aStarPath(startGoal.first, startGoal.second, GraphBuilder.distance);		
				
				PathFinding.optimizePath(polygons, path);
				PathFinding.optimizePath(polygons, path);
				PathFinding.optimizePath(polygons, path);
				
				previousStart = path[0].getPos();
				
				result[a].Add (path);
				
				distance += distance_astar(path);
				
				if (totalCustomers >= customers.Count)
					break;
				
			}
			
			
			if (distance > max_astar_distance)
				max_astar_distance = distance;
			
			i = i + number_of_customers + 1;
			
		}
		
		return result;
	}
	
	public static void printPath(List<GNode> p, string name) {
		StringBuilder builder = new StringBuilder();
		foreach (GNode n in p)
		{
			builder.Append(n.getPos()).Append(" ");
		}
		string result = builder.ToString();
		Debug.Log ( name + ": " + result);
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

	public static void drawPaths(Dictionary<Agent, List<List<GNode>>> res) {
		foreach(KeyValuePair<Agent, List<List<GNode>>> entry in res)
		{
			Color color = randomizeColor();
			for (int i = 0; i < entry.Value.Count;i++) {
				PathFinding.draw (entry.Value[i], color);
				
			}
		}
	}
	
	public static void addPaths(Agent a, List<List<GNode>> paths) {
		for (int i = 0; i < paths.Count;i++) {
			a.addPath(paths[i]);
			
		}
	}

	
	public static float distance_astar(List<GNode> path) {
		float distance = 0f;
		for (int i = 0; i < path.Count-1; i++) {
			distance += Vector3.Distance(path[i].getPos(), path[i+1].getPos ());
		}
		return distance;
	}
	
	public static float distance_astar_discrete(List<GNode> path) {
		return path.Count;
	}
	
	public static Color randomizeColor() {
		float r = UnityEngine.Random.Range(0.0f, 1f);
		float b = UnityEngine.Random.Range(0.0f, 1f);
		float g = UnityEngine.Random.Range(0.0f, 1f);
		return new Color (r, g, b, 1.0f);
	}


}
