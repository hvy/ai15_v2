using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;


public class GeneticsContinous {
	
	HashSet<int[]> hash;
	int[] current_best;
	float current_best_cost;
	LinkedList<int[]> population;
	Dictionary<int, GameObject> chromosomeIDs;
	static System.Random _random = new System.Random();
	
	List<GameObject> agents;
	List<GameObject> customers;
	//GNode[,] graph;
	List<Vector2[]> polygons;
	
	double totalSimulationTime = 0f;
	
	
	public GeneticsContinous (int[] solution, int iterations, int individuals, int tournamentRounds, float mutationRate, List<GameObject> agents, List<GameObject> customers, List<Vector2[]> polygons, Dictionary<int, GameObject> chromosomeIDs) {
		this.customers = customers;
		this.agents = agents;
		this.polygons = polygons;
		this.chromosomeIDs = chromosomeIDs;
		hash = new HashSet<int[]>();
		
		current_best = solution;
		current_best_cost = cost (current_best).first;
		//		UnityEngine.Debug.Log("VAFAN: " + solution[0]);
		
		createPopulation(individuals, solution);
		
		search (current_best, mutationRate, tournamentRounds, iterations);
		
	}
	
	
	void createPopulation(int N, int[] solution) {
		population = new LinkedList<int[]>();
		int[] permutation = solution;
		for (int i = 0; i < N; i++) {
			shuffle (permutation);
			population.AddLast (permutation);
		}
		
	}
	
	public Tuple<float, Dictionary<Agent, List<List<GNode>>>> get_result() {
		return cost (current_best);
	}
	
	
	void search(int[] chromosome, float mr, int K, int iterations) {
		Stopwatch sw = new Stopwatch();
		Stopwatch sw2 = new Stopwatch();
		sw2.Start();
		
		List<int[]> parents;
		List<int[]> children;
		//Debug.Log ("Starting search");
		
		int i = 0;
		while (i < iterations) {
			
			// your code here
			UnityEngine.Debug.Log ("current best: " + current_best_cost);

			sw = new Stopwatch();
			sw.Start();
			parents = tournamentSelection(K);
			sw.Stop();
			System.TimeSpan elapsedTime = sw.Elapsed;
			UnityEngine.Debug.Log ("Tournament time: " + elapsedTime.TotalMilliseconds + " ms");

			sw = new Stopwatch();
			sw.Start();
			children = crossover(parents);
			sw.Stop();
			elapsedTime = sw.Elapsed;
			UnityEngine.Debug.Log ("Crossover time: " + elapsedTime.TotalMilliseconds + " ms");
			
			if (_random.NextDouble() <= mr)
				mutate(children);
			sw = new Stopwatch();
			sw.Start();
			foreach (int[] child in children) {
//				if (hash.Contains(child))
//					continue;
				
				population.RemoveFirst(); // TODO change type of selection?
				population.AddLast(child);
//				hash.Add(child);
				
			} 
			sw.Stop();
			elapsedTime = sw.Elapsed;
			UnityEngine.Debug.Log ("Selection time: " + elapsedTime.TotalMilliseconds + " ms");
			//			Debug.Log ("ierations: " + i);
			i++;
		}
		
		sw2.Stop();
		System.TimeSpan et = sw2.Elapsed;
		UnityEngine.Debug.Log ("Total GA time: " + et.TotalMilliseconds + " ms");
		UnityEngine.Debug.Log ("Total Simulation time: " + totalSimulationTime + " ms");
		
	}
	
	List<int[]> tournamentSelection(int rounds) {
		List<int[]> participants = new List<int[]>();
		participants = population.ToList();
		List<int[]> winners;
		
		for (int k = 0; k < rounds; k++) {
			winners = new List<int[]>();
			for (int i = 0; i < participants.Count - 1; i++) {
				int[] first = participants[i];
				int[] second = participants[i+1];
				
				if (cost(first).first < cost (second).first)
					winners.Add(first);
				else
					winners.Add(second);
				i++;
				
			}
			participants = winners;
		}
		
		return participants;
	}
	
	List<int[]> crossover(List<int[]> parents) {
		List<int[]> children = new List<int[]>();
		
		// TODO do a cool crossover
		
		for (int i = 0; i < parents.Count-1; i++) {
			List<int> list = parents[i+1].ToList();
//			LinkedList<int> list2 = parents[i+1].ToList();
			//LinkedList<int> linkedList = new LinkedList<int>(parents[i+1]);
			
			for (int j = 0; j < 2; j++) {
				int index  = _random.Next (0, customers.Count + agents.Count-2);
				int id_first = parents[i][index];
				int id_second = parents[i][index+1];
				
				
				int value = list[1];
				list.Remove(id_second);
				int idx = list.IndexOf(id_first);
				list.Insert(idx, id_second);
			}
			int[] res = list.ToArray();
			normalize_chromosome(res);
			children.Add (res);
			i++;
			
			
		}
		return children;
		
	}
	
	void mutate(List<int[]> children) {
		// TODO
	}
	
	void shuffle(int[] array)
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
	
	private float distance_astar(List<GNode> path) {
		float distance = 0f;
		for (int i = 0; i < path.Count-1; i++) {
			distance += Vector3.Distance(path[i].getPos(), path[i+1].getPos ());
		}
		return distance;
	}
	
	void normalize_chromosome(int[] chromo) {
		int index_first_agent = 0;
		while (chromosomeIDs[chromo[index_first_agent]].GetComponent("Agent") == null) {
			index_first_agent++;
		}
		
		int temp = chromo[index_first_agent];
		chromo[index_first_agent] = chromo[0];
		chromo[0] = temp;
	}
	
	
	Tuple<float, Dictionary<Agent, List<List<GNode>>>>  cost(int[] chromosome) {
		
		Dictionary<Agent, List<List<GNode>>> result = new Dictionary<Agent, List<List<GNode>>>();

		float maxDistance = 0f;
		int totalCustomers = 0;
		float totalDistance = 0f;
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
				
				RRT rrt = new RRT (previousStart, customer.transform.position, bounds, polygons, 100.0f, 1f, acceptableWidth, minAngle, acceptableWidth, GameState.Instance.width, GameState.Instance.height);
				
				rrt.buildRRT (10000);
				//rrt.tree.draw ();
				
				
				Tuple<GNode, GNode> startGoal = rrt.generateGraph();
				List<GNode> path = PathFinding.aStarPath(startGoal.first, startGoal.second, GraphBuilder.distance);		
				
				PathFinding.optimizePath(polygons, path);
				PathFinding.optimizePath(polygons, path);
				PathFinding.optimizePath(polygons, path);
				
				previousStart = path[0].getPos();
				
				result[a].Add (path);
				
				distance += distance_astar(path);
				totalDistance += distance;
				
				if (totalCustomers >= customers.Count)
					break;
				
			}
			
			
			if (distance > maxDistance)
				maxDistance = distance;
			
			i = i + number_of_customers + 1;
			
		}

		// Calculate the fitness of the result
		float cost = fitness (result, maxDistance, totalDistance);

		if (cost < current_best_cost) {
			current_best = chromosome;
			current_best_cost = cost;
		}
		
		return new Tuple<float, Dictionary<Agent, List<List<GNode>>>>(cost, result);
		
	}
	
	private float fitness(Dictionary<Agent, List<List<GNode>>> res, float maxDistance, float totalDistance) {
		float w1 = 10.0f;
		float w2 = 2.0f;
		return w1*maxDistance + w2*totalDistance;
	}

	
	private void addPaths(Agent a, List<List<GNode>> paths) {
		for (int i = 0; i < paths.Count;i++) {
			a.addPath(paths[i]);
			
		}
	}
	
}
