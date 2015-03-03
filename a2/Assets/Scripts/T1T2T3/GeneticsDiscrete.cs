using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;


public class GeneticsDiscrete {

	HashSet<int[]> hash;
	int[] current_best;
	float current_best_cost;
	List<int[]> population;
	Dictionary<int, GameObject> chromosomeIDs;
	static System.Random _random = new System.Random();

	List<GameObject> agents;
	List<GameObject> customers;
	GNode[,] graph;


	public GeneticsDiscrete (int[] solution, int iterations, int individuals, int tournamentRounds, float mutationRate, List<GameObject> agents, List<GameObject> customers, GNode[,] graph, Dictionary<int, GameObject> chromosomeIDs) {
		this.customers = customers;
		this.agents = agents;
		this.graph = graph;
		this.chromosomeIDs = chromosomeIDs;
		hash = new HashSet<int[]>();

		current_best = solution;
		current_best_cost = cost (current_best).first;
		UnityEngine.Debug.Log("VAFAN: " + solution[0]);

		createPopulation(individuals, solution);



		search (current_best, mutationRate, tournamentRounds, iterations);

	}


	void createPopulation(int N, int[] solution) {
		population = new List<int[]>();
		int[] permutation = solution;
		for (int i = 0; i < N; i++) {
			shuffle (permutation);
			population.Add (permutation);
		}

	}

	public Tuple<float, Dictionary<Agent, List<List<GNode>>>> get_result() {
		return cost (current_best);
	}


	void search(int[] chromosome, float mr, int K, int iterations) {

		List<int[]> parents;
		List<int[]> children;
		//Debug.Log ("Starting search");

		int i = 0;
		while (i < iterations) {
			Stopwatch sw = new Stopwatch();
			
			// your code here
			UnityEngine.Debug.Log ("current best: " + current_best_cost);
			
			sw.Start();
			parents = tournamentSelection(K);
			sw.Stop();
			System.TimeSpan elapsedTime = sw.Elapsed;
			UnityEngine.Debug.Log ("Tournament time: " + elapsedTime.TotalMilliseconds + " ms");

			sw.Start();
			children = crossover(parents);
			sw.Stop();
			elapsedTime = sw.Elapsed;
			UnityEngine.Debug.Log ("Crossover time: " + elapsedTime.TotalMilliseconds + " ms");

			if (_random.NextDouble() <= mr)
				mutate(children);
			sw.Start();
			foreach (int[] child in children) {
				if (hash.Contains(child))
					continue;

				population.RemoveAt(0);
				population.Add (child);
				hash.Add(child);

				float child_cost = cost (child).first;
				if (child_cost < current_best_cost) {
					current_best = child;
					current_best_cost = child_cost;
				}


			} 
			sw.Stop();
			elapsedTime = sw.Elapsed;
			UnityEngine.Debug.Log ("Selection time: " + elapsedTime.TotalMilliseconds + " ms");
//			Debug.Log ("ierations: " + i);
			i++;
		}

	}

	List<int[]> tournamentSelection(int rounds) {
		List<int[]> participants = new List<int[]>();
		participants = copy(population);
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
			int index  = _random.Next (0, customers.Count + agents.Count-2);
			int id_first = parents[i][index];
			int id_second = parents[i][index+1];

			List<int> list = parents[i+1].ToList();

			int value = list[1];
			list.Remove(id_second);
			int idx = list.IndexOf(id_first);
			list.Insert(idx, id_second);
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

	List<int[]> copy(List<int[]> a) {
		List<int[]> newList = new List<int[]>();
		for (int i = 0; i < a.Count; i++) {
			newList.Add (new int[a[i].Length]);
			newList[i] = a[i];
		}
		return newList;
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
//		float distance = 0f;
//		for (int i = 0; i < path.Count-1; i++) {
//			distance += Vector3.Distance(path[i].getPos(), path[i+1].getPos ());
//		}
//		return distance;
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
		float cost = 0f;
		
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
			a.init();
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
				
				if (totalCustomers >= customers.Count)
					break;
				
			}
			
			
			if (distance > cost)
				cost = distance;
			
			i = i + number_of_customers + 1;
			
		}
		
		return new Tuple<float, Dictionary<Agent, List<List<GNode>>>>(cost, result);

	}

}
