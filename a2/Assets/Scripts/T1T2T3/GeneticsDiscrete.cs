using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;


public class GeneticsDiscrete {

	HashSet<int[]> hash;
	int[] current_best;
	float current_best_cost;
	LinkedList<int[]> population;
	Dictionary<int, GameObject> chromosomeIDs;
	static System.Random _random = new System.Random();

	List<GameObject> agents;
	List<GameObject> customers;
	GNode[,] graph;

	double totalSimulationTime = 0f;


	public GeneticsDiscrete (int[] solution, int iterations, int individuals, int tournamentRounds, float mutationRate, List<GameObject> agents, List<GameObject> customers, GNode[,] graph, Dictionary<int, GameObject> chromosomeIDs) {
		this.customers = customers;
		this.agents = agents;
		this.graph = graph;
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
			UnityEngine.Debug.Log ("Tournament time: " + elapsedTime.Milliseconds + " ms");

			sw = new Stopwatch();
			sw.Start();
			children = crossover(parents);
			sw.Stop();
			elapsedTime = sw.Elapsed;
			UnityEngine.Debug.Log ("Crossover time: " + elapsedTime.Milliseconds + " ms");

			if (_random.NextDouble() <= mr)
				mutate(children);
			sw = new Stopwatch();
			sw.Start();
			foreach (int[] child in children) {
//				if (hash.Contains(child))
//					continue;

				population.RemoveFirst(); // TODO change type of selection?
				population.AddLast (child);
//				hash.Add(child);
			
			} 
			sw.Stop();
			elapsedTime = sw.Elapsed;
			UnityEngine.Debug.Log ("Selection time: " + elapsedTime.Milliseconds + " ms");
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
//			LinkedList<int> linkedList = new LinkedList<int>(parents[i+1]);
			// TODO use LinkedList for easier modification.

			for (int j = 0; j < 2; j++) {
				int index  = _random.Next (0, customers.Count + agents.Count-2);
				int id_first = parents[i][index];
				int id_second = parents[i][index+1];
				
				int value = list[1];
				list.Remove(id_second);
//				linkedList.Remove(id_second);
				int idx = list.IndexOf(id_first);
				list.Insert(idx, id_second);
//				linkedList.AddBefore(id_se)
			}
			int[] res = list.ToArray();
			normalize_chromosome(res);
			children.Add (res);
			//i++;
			

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
		return path.Count;
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
		float totalDistance = 0f;
		
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
			//a.init();
			a.setStart(start.getPos ());
			a.setGoal(start.getPos());
			a.setModel(0); // TOOD denna ska ju vara 0, för att köra discrete model
			
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
		float w2 = 3.0f;
		float w3 = 8.0f;
		return w1*timeCost(res, GameState.Instance.width, GameState.Instance.height) + w2*maxDistance + w3*totalDistance;
	}

	// Avoid collision by planning with time (considering pauses)
	private int timeCost(Dictionary<Agent, List<List<GNode>>> paths, int width, int height) {
		Stopwatch sw = new Stopwatch();
		
		// your code here
		//UnityEngine.Debug.Log ("current best: " + current_best_cost);
		
		sw.Start();
		int totalTime = 100;// TODO, how is this determined? Loop until every agent is finished maybe

		int longestTime = 0;
		
		int[,] binGraph = new int[(int)width,(int)height];
		Dictionary<Agent, List<GNode>> new_paths = new Dictionary<Agent, List<GNode>>();
		Dictionary<Agent, List<GNode>> old_paths = new Dictionary<Agent, List<GNode>>();
		Dictionary<Agent, int> recalculatedPathCounter = new Dictionary<Agent, int>();
		Dictionary<Agent, int> steps = new Dictionary<Agent, int>();
		
		foreach(KeyValuePair<Agent, List<List<GNode>>> entry in paths)
		{
			Agent agent = entry.Key;
			
			agent.removePaths();
			
			if (entry.Value.Count != 0)
				addPaths(agent, entry.Value);
			
			new_paths[agent] = new List<GNode>();
			old_paths[agent] = agent.pathsToPath();
			recalculatedPathCounter[agent] = 0;
			steps[agent] = 0;
			binGraph[(int)agent.transform.position.x, (int)agent.transform.position.z] = 1;
		}
		
		for (int i = 0; i < totalTime; i++) {
			foreach(KeyValuePair<Agent, List<List<GNode>>> entry in paths)
			{
				Agent agent = entry.Key;
				
				List<GNode> oldPath = old_paths[agent];
				
				if (oldPath.Count-1 < i) {
					if (oldPath.Count-1 == i-1 && i != 0) {
						binGraph[(int)oldPath[i-1].getPos ().x, (int)oldPath[i-1].getPos ().z] = 2;
					}
					continue;
				}
				
				//printPath (oldPath, "old path");
				
				Vector3 oldPos;
				if (i == 0)
					oldPos = agent.transform.position;
				else 
					oldPos = oldPath[i-1].getPos ();
				
				
				Vector3 newPos = oldPath[i].getPos ();
				
				if (new_paths[agent].Count > longestTime) {
					longestTime = new_paths[agent].Count;
				}

				
				if ((int) newPos.x == -1) { // path complete
					continue;
				}
				
				// Pause, recalculate path or simply add to path
				if (binGraph[(int)newPos.x, (int)newPos.z] == 1 && newPos != oldPos) { // pause
					new_paths[agent].Insert(0, new GNode(0,oldPos, new List<GNode>()));
					oldPath.Insert(i+1, new GNode(0,newPos, new List<GNode>()));
					binGraph[(int)oldPos.x, (int)oldPos.z] = 1;
					binGraph[(int)newPos.x, (int)newPos.z] = 1;
					
				} 
				else if (binGraph[(int)newPos.x, (int)newPos.z] == 2 && oldPath.Count-1 >= i+1) { // recalculate path
					List<Vector3> obstacles = new List<Vector3>();
					obstacles.Add (newPos);
					obstacles.AddRange(GameState.Instance.obstacles);
					
					List<GNode> recalPath = PathPlanner.recalculatePath_noAgentMod(oldPos, oldPath[i+1].getPos (), obstacles);
					
					recalPath.RemoveAt(recalPath.Count-1);
					oldPath.RemoveAt(i+1);
					
					for (int p = 0; p < recalPath.Count; p++) {
						new_paths[agent].Insert(0, recalPath[recalPath.Count-p-1]);
					}
					recalculatedPathCounter[agent]+= recalPath.Count;
					
				} 
				else if (recalculatedPathCounter[agent] > 0) { // currently traversing the recalculated path
					recalculatedPathCounter[agent]--;
					binGraph[(int)newPos.x, (int)newPos.z] = 1;
					binGraph[(int)oldPos.x, (int)oldPos.z] = 0;
					new_paths[agent].Insert(0, new GNode(0, newPos, new List<GNode>()));
					continue;
				} 
				else {// free to move
					binGraph[(int)newPos.x, (int)newPos.z] = 1;
					binGraph[(int)oldPos.x, (int)oldPos.z] = 0;
					new_paths[agent].Insert(0, new GNode(0, newPos, new List<GNode>()));
				}
				
				
			}
		}
		
		sw.Stop();
		System.TimeSpan elapsedTime = sw.Elapsed;
		//UnityEngine.Debug.Log ("Simulation time: " + elapsedTime.TotalMilliseconds + " ms");
		totalSimulationTime += elapsedTime.Milliseconds;

		return longestTime;
		
	}

	private void addPaths(Agent a, List<List<GNode>> paths) {
		for (int i = 0; i < paths.Count;i++) {
			//paths[i].RemoveAt(paths[i].Count-1); // remove to avoid duplicates
			a.addPath(paths[i]);
			
		}
	}

}
