using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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


		Dictionary<Agent, List<GNode>> newPaths = PathPlanner.avoidCollision(result, width, height);
		
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

	
		return paths;

	}

	// Avoid collision by planning with time (considering pauses)
	public static Dictionary<Agent, List<GNode>> avoidCollision(Dictionary<Agent, List<List<GNode>>> paths, int width, int height) {
		int totalTime = 100;// TODO, how is this determined? Loop until every agent is finished maybe


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
						if (oldPath.Count-1 == i-1 && i != 0)
							binGraph[(int)oldPath[i-1].getPos ().x, (int)oldPath[i-1].getPos ().z] = 2;
						continue;
					}

					//printPath (oldPath, "old path");

					Vector3 oldPos;
					if (i == 0)
						oldPos = agent.transform.position;
					else 
						oldPos = oldPath[i-1].getPos ();
					
					Vector3 newPos = oldPath[i].getPos ();

					// Pause, recalculate path or simply add to path
					if (binGraph[(int)newPos.x, (int)newPos.z] == 1 && newPos != oldPos) { // pause
						new_paths[agent].Insert(0, new GNode(0,oldPos, new List<GNode>()));
						oldPath.Insert(i+1, new GNode(0,newPos, new List<GNode>()));
						//new_paths[agent].Insert(0, new GNode(0,newPos, new List<GNode>()));
						Debug.Log ("Pause at: " + oldPos.x + " " + oldPos.z + " tid: " + i);
						binGraph[(int)oldPos.x, (int)oldPos.z] = 1;
						binGraph[(int)newPos.x, (int)newPos.z] = 1;
						
					} 
					else if (binGraph[(int)newPos.x, (int)newPos.z] == 2 && oldPath.Count-1 >= i+1) { // recalculate path
						Debug.Log ("RECALCULATE ASTAR PATH");
						List<Vector3> obstacles = new List<Vector3>();
						obstacles.Add (newPos);
						obstacles.AddRange(GameState.Instance.obstacles);
							
						List<GNode> recalPath = recalculatePath_noAgentMod(oldPos, oldPath[i+1].getPos (), obstacles);

						recalPath.RemoveAt(recalPath.Count-1);
						oldPath.RemoveAt(i+1);
						//recalPath.RemoveAt(0);
					
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

		return new_paths;

	}

	public static List<GNode> recalculatePath_noAgentMod(Vector3 _start, Vector3 _goal, List<Vector3> obstacles) {

		GNode[,] graph = buildGraph ((int)GameState.Instance.width, (int)GameState.Instance.height, GameState.Instance.neighbors, obstacles);
		
		int x = (int) _goal.x;
		int z = (int) _goal.z;


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
	

	public static Color randomizeColor() {
		float r = UnityEngine.Random.Range(0.0f, 1f);
		float b = UnityEngine.Random.Range(0.0f, 1f);
		float g = UnityEngine.Random.Range(0.0f, 1f);
		return new Color (r, g, b, 1.0f);
	}

	public static List<GNode> recalculatePath(Agent _agent, Vector3 _goal, List<Vector3> obstacles) {

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
			path.RemoveAt(path.Count-1);
			path.RemoveAt(path.Count-1);
			//path.RemoveAt(0);
			
			
			Agent a = _agent;
			//a.init();
			a.setStart(path[path.Count-1].getPos());
			a.setGoal(path[path.Count-1].getPos ());
			a.setModel(0); // TOOD denna ska ju vara 0, för att köra discrete model
			a.setPath(path);
			
			

		return path;
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
