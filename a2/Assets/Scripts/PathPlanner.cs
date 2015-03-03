using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class PathPlanner
{

	private float max_astar_distance = 0f;
	static System.Random _random = new System.Random();

	private Dictionary<int, GameObject> chromosomeIDs = new Dictionary<int, GameObject>();
	

	// TODO ta hänsyn till tid också
	public List<List<GNode>> planDiscretePaths (int width, int height, List<GameObject> agents, List<GameObject> customers, int neighbors, List<Vector3> occupiedSlots) {

		GNode[,] graph = buildGraph (width, height, neighbors, occupiedSlots);
		Dictionary<Agent, List<List<GNode>>> result = new Dictionary<Agent, List<List<GNode>>>();

		List<List<GNode>> paths = new List<List<GNode>> ();

		while (customers.Count > 0) {

			GameObject customer = customers[0];

			int x = (int) customer.transform.position.x;
			int z = (int) customer.transform.position.z;

			GNode goal = graph [x, z];

			// Find the closest agent
			int index = 0;
			float shortestDistance = 10000f;
			for (int i = 0; i < agents.Count; i++) {
				float dist = Vector3.Distance(agents[i].transform.position, customer.transform.position);
				if (dist < shortestDistance) {
					shortestDistance = dist;
					index = i;
				}
			}

			GameObject agent = agents[index];
			x = (int) agent.transform.position.x;
			z = (int) agent.transform.position.z;

			GNode start = graph [x, z];

			agents.RemoveAt(index);
			customers.RemoveAt(0);

			List<GNode> path = PathFinding.aStarPath(start, goal, GraphBuilder.distance); // TODO Change the heuristic function, remove dependency

			if (path == null)
				continue;

			PathFinding.draw (path);


			Agent a = (Agent) agent.GetComponent(typeof(Agent));
			//a.init();
			a.setStart(path[path.Count-1].getPos());
			a.setGoal(path[path.Count-1].getPos());
			a.setModel(0); // TOOD denna ska ju vara 0, för att köra discrete model
			a.setPath(path);

			paths.Add (path);
			result[a] = new List<List<GNode>>();
			result[a].Add (path);
		}

		foreach(KeyValuePair<Agent, List<List<GNode>>> entry in result) {
			Agent a = entry.Key;
			DiscreteController dc = (DiscreteController) a.models[0];
			if (dc.reactive)
				return paths;

		}

		Dictionary<Agent, List<GNode>> newPaths = avoidCollision(result, width, height);
		PathFinding.clearDrawnPaths();
		foreach(KeyValuePair<Agent, List<GNode>> entry in newPaths)
		{
			Agent a = entry.Key;

//			if (entry.Value.Count > 5)
//				Debug.Log ("path: " + entry.Value[0].getPos() + " " + entry.Value[1].getPos () + " " + entry.Value[2].getPos () + " " + entry.Value[3].getPos() + " " + entry.Value[4].getPos());
			a.setPath(entry.Value);
			a.setStart(entry.Value[entry.Value.Count-1].getPos());
			a.setGoal(entry.Value[entry.Value.Count-1].getPos());
			PathFinding.draw(entry.Value);
		}

	
		return paths;

	}

	// Avoid collision by planning with time (considering pauses)
	public static Dictionary<Agent, List<GNode>> avoidCollision(Dictionary<Agent, List<List<GNode>>> paths, int width, int height) {
		int totalTime = 100;// TODO, how is this determined? Loop until every agent is finished maybe


		int[,] binGraph = new int[(int)width,(int)height];
		Dictionary<Agent, List<GNode>> new_paths = new Dictionary<Agent, List<GNode>>();
		Dictionary<Agent, int> recalculatedPathCounter = new Dictionary<Agent, int>();

		foreach(KeyValuePair<Agent, List<List<GNode>>> entry in paths)
		{
			Agent agent = entry.Key;
			new_paths[agent] = new List<GNode>();
			recalculatedPathCounter[agent] = 0;
			binGraph[(int)agent.transform.position.x, (int)agent.transform.position.z] = 1;
		}
		
		for (int i = 0; i < totalTime; i++) {
			foreach(KeyValuePair<Agent, List<List<GNode>>> entry in paths)
			{
					Agent agent = entry.Key;
					//List<List<GNode>> p = entry.Value;
					
					Vector3 oldPos;
					if (i == 0)
						oldPos = agent.transform.position;
					else
						oldPos = agent.recalculateGoal(i-1);
						//oldPos = agent.currentPath [agent.currentPath.Count - i].getPos ();

					Vector3 newPos = agent.recalculateGoal(i);
					
					if ((int) newPos.x == -1) {
						if ((int)oldPos.x != -1)
							binGraph[(int)oldPos.x, (int)oldPos.z] = 2;
						continue;
					}
					
					
					
					// Pause, recalculate path or simply add to path
					if (binGraph[(int)newPos.x, (int)newPos.z] == 1 && newPos != oldPos && newPos != agent.transform.position) {
						Debug.Log ("PAUSA " + agent.GetInstanceID());
						new_paths[agent].Insert(0, new GNode(0,oldPos, new List<GNode>()));
						new_paths[agent].Insert(0, new GNode(0,newPos, new List<GNode>()));
						Debug.Log ("paus pos: " + oldPos.x + " " + oldPos.z + " tid: " + i);
						binGraph[(int)oldPos.x, (int)oldPos.z] = 1;
					} else if (binGraph[(int)newPos.x, (int)newPos.z] == 2) { // recalculate path
						Debug.Log ("RECALCULATE ASTAR PATH");
						List<Vector3> obstacles = new List<Vector3>();
						obstacles.Add (newPos);
						obstacles.AddRange(GameState.Instance.obstacles);
							
						List<GNode> recalPath = recalculatePath(oldPos, agent.recalculateGoal(i+1), obstacles);


						recalPath.RemoveAt(recalPath.Count-1);
						agent.updatePath(recalPath, i);

						for (int p = 0; p < recalPath.Count; p++) {
							new_paths[agent].Insert(0, recalPath[recalPath.Count-p-1]);
						}
						recalculatedPathCounter[agent]+= recalPath.Count;

					}  else if (recalculatedPathCounter[agent] > 0) { // currently traversing the recalculated path
						recalculatedPathCounter[agent]--;
						binGraph[(int)newPos.x, (int)newPos.z] = 1;
						binGraph[(int)oldPos.x, (int)oldPos.z] = 0;
						continue;
					}else {
						binGraph[(int)newPos.x, (int)newPos.z] = 1;
						binGraph[(int)oldPos.x, (int)oldPos.z] = 0;
						new_paths[agent].Insert(0, new GNode(0, newPos, new List<GNode>()));
					}


			}
		}

		return new_paths;

	}

	static List<GNode> recalculatePath(Vector3 _start, Vector3 _goal, List<Vector3> obstacles) {

		GNode[,] graph = buildGraph ((int)GameState.Instance.width, (int)GameState.Instance.height, GameState.Instance.neighbors, obstacles);
		
		int x = (int) _goal.x;
		int z = (int) _goal.z;

//		Debug.Log ("goal: " + _goal);
//		Debug.Log ("graph: " + graph.Length);
		GNode goal = graph [x, z];
		
		GNode start = graph [(int)_start.x, (int)_start.z];
		
		
		List<GNode> path = PathFinding.aStarPath(start, goal, GraphBuilder.distance); // TODO Change the heuristic function, remove dependency
		
		if (path == null) {
			Debug.Log ("Didn't find path!");
			return null;
		}
		
		//PathFinding.draw (path);
		
		
		return path;
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
		//Debug.Log ("max distance: " + max_distance);
		return max_distance;

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

	public static List<GNode> recalculatePath(Agent _agent, Vector3 _goal, List<Vector3> obstacles) {
//		PathPlanner pp = new PathPlanner ();
		List<GameObject> agents = new List<GameObject>();
		List<GameObject> waypoints = new List<GameObject>();
		agents.Add (_agent.gameObject);

		waypoints.Add (GameState.Instance.customers[_goal]);

		GNode[,] graph = buildGraph ((int)GameState.Instance.width, (int)GameState.Instance.height, GameState.Instance.neighbors, obstacles);
							
			
			int x = (int) _goal.x;
			int z = (int) _goal.z;
			GNode goal = graph [x, z];
			
			// Find the closest agent
			
			GameObject agent = _agent.gameObject;
			x = (int) agent.transform.position.x;
			z = (int) agent.transform.position.z;
			
			GNode start = graph [x, z];

			
			List<GNode> path = PathFinding.aStarPath(start, goal, GraphBuilder.distance); // TODO Change the heuristic function, remove dependency
			
			if (path == null) {
			Debug.Log ("Didn't find path!");
			return null;
		}

			PathFinding.draw (path);
			
			
			Agent a = _agent;
			//a.init();
			a.setStart(path[path.Count-1].getPos());
			a.setGoal(path[path.Count-1].getPos ());
			a.setModel(0); // TOOD denna ska ju vara 0, för att köra discrete model
			a.setPath(path);
			
			

		return path;
	}



	
	public void planContinuousVRP (float width, float height, List<GameObject> agents, List<GameObject> customers, List<Vector2[]> polygons, int iterations) {

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
		
		
		// PERFORM EUCLIDEAN FITNESS
		float bestFitness = 1000000000f;
		for (int i = 0; i < iterations*5; i++) {
			Shuffle (chromosome);
			float newFitness = euclidean_fitness(chromosome);
			if (newFitness < bestFitness)
				bestFitness = newFitness;
		}
		
		//GNode[,] graph = buildGraph (width, height, neighbors, occupiedSlots);
	

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
			a.setModel(1); // TOOD denna ska ju vara 0, för att köra discrete model
			
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
				
				acceptableWidth = System.Math.Max(GameObject.FindWithTag ("Agent").transform.localScale.x * 2, GameObject.FindWithTag ("Agent").transform.localScale.y * 2) + 0.5f;
				minAngle = 180f;
				Vector3[] bounds = new Vector3[4];
				
				RRT rrt = new RRT (previousStart, customer.transform.position, bounds, polygons, 5.0f, 1f, acceptableWidth, minAngle, acceptableWidth, width, height);
				
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
				
				if (totalCustomers >= customers.Count)
					break;
				
			}
			
			
			if (distance > max_astar_distance)
				max_astar_distance = distance;
			
			i = i + number_of_customers + 1;
			
		}
		
		return result;
	}

	public static GNode[,] buildGraph (int width, int height, int neighbors, List<Vector3> occupiedSlots)
	{
		GNode[,] gnodes = new GNode[width, height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				Vector3 position = new Vector3 (i, 0, j);
				gnodes[i, j] = new GNode(i, position, new List<GNode> ());
			}
		}

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {

				GNode node = gnodes[i, j];

				// Up
				if (j > 0 && !occupiedSlots.Contains(gnodes[i, j - 1].getPos())) {
					node.addNeighbor (gnodes[i, j - 1]);
				}


				// Right
				if (i < width - 1 && !occupiedSlots.Contains(gnodes[i+1, j].getPos())) {
					node.addNeighbor (gnodes[i + 1, j]);
				}

				// Down
				if (j < height - 1 && !occupiedSlots.Contains(gnodes[i, j + 1].getPos())) {
					node.addNeighbor (gnodes[i, j + 1]);
				}

				// Left
				if (i > 0 && !occupiedSlots.Contains(gnodes[i - 1, j].getPos())) {
					node.addNeighbor (gnodes[i - 1, j]);
				}

				if (neighbors != 8)
					continue;

				// North West
				if (i > 0 && j > 0 && !occupiedSlots.Contains(gnodes[i - 1, j - 1].getPos()))
					node.addNeighbor (gnodes[i - 1, j - 1]);

				// North East
				if (i < width - 1 && j > 0 && !occupiedSlots.Contains(gnodes[i + 1, j - 1].getPos()))
					node.addNeighbor (gnodes[i + 1, j - 1]);

				// South West
				if (i > 0 && j < height -1 && !occupiedSlots.Contains(gnodes[i - 1, j + 1].getPos()))
					node.addNeighbor (gnodes[i - 1, j + 1]);

				// South East
				if (i < width - 1 && j < height - 1 && !occupiedSlots.Contains(gnodes[i + 1, j + 1].getPos()))
					node.addNeighbor (gnodes[i + 1, j + 1]);
			}
		}

		return gnodes;
	}
}
