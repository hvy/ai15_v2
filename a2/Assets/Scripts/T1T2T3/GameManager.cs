using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{	

	public static int discreteNeighbors;

	private float _width, _height;
	private int nr_agents, numWaypoints, numObstacles;
	private int neighbors = 8;
	public int task;
	public int GeneticIterations;
	public int GeneticPopulation;
	public int GeneticTournaments;
	public bool DrawRRT;
	public bool NoFrontCollisions;
	public string file;

	void Start () 
	{
		init();
	}


	void init() {

		Triple<List<GameObject>, List<GameObject>, List<List<Vector2>>> tripleOfAgents;

		if (task < 3)
			tripleOfAgents = initDiscreteStage ();
		else
			tripleOfAgents = initPolyStage ();

		// Create stage
		StageFactory stageFactory = new StageFactory ();
		stageFactory.createStage(_width, _height);
		CameraModel.updateOrthoPosition(_width, Camera.main.transform.position.y, _height);


//		_height = _height - 1;
//		_width = _width - 1;
		GameState.Instance.height = (int)_height;
		GameState.Instance.width = (int)_width;
		GameState.Instance.neighbors = (int)neighbors;

		// Create waypoints
		List<GameObject> agents = new List<GameObject> ();
		List<GameObject> waypoints = new List<GameObject> ();
		List<List<Vector2>> polys = new List<List<Vector2>> ();


		agents = tripleOfAgents.first;
		waypoints = tripleOfAgents.second;
		polys = tripleOfAgents.third;

		List<Vector2[]> polygons = new List<Vector2[]>();
		if (task >= 3) {
			for (int i = 0; i < polys.Count; i++) {
				polygons.Add(polys[i].ToArray());
			}
		}


		PathPlanner pp = new PathPlanner ();
		VRPDiscrete vrpDiscrete = new VRPDiscrete();
		VRPContinous vrpContinous = new VRPContinous();

		//List<Vector2[]> polygons = new List<Vector2[]>();
		if (task == 1)
			pp.planDiscretePaths ((int) _width, (int) _height, agents, waypoints, neighbors, GameState.Instance.obstacles, NoFrontCollisions);
		else if (task == 2)
			vrpDiscrete.planVRPPaths(agents, waypoints, GameState.Instance.obstacles, GeneticIterations, GeneticPopulation, GeneticTournaments);
		else if (task == 3)
			vrpContinous.planContinuousVRP(agents, waypoints, polygons, GeneticIterations, GeneticPopulation, GeneticTournaments, DrawRRT);
			//pp.planContinuousVRP ((int) _width, (int) _height, agents, waypoints, polygons, RandomIterations);
	
	}

	private Triple<List<GameObject>, List<GameObject>, List<List<Vector2>>> initPolyStage() {

		PolygonalLevelParser plp = new PolygonalLevelParser();

		plp.parse(file);

		_width = plp.getWidth();
		_height = plp.getHeight();
		List<Vector2> starts = plp.getStarts ();
		List<Vector2> goals = plp.getGoals ();
		List<Vector2> customers = plp.getCustomers ();


		List<List<Vector2>> triangles = plp.getTriangles();
	
		StageFactory sf = new StageFactory ();
		sf.createStage (_width, _height);
		
		
		List<GameObject> agents = new List<GameObject> ();
		
		Debug.Log ("starts: "+ starts.Count);
		for (int i = 0; i < starts.Count; i++) {
			GameObject agent = AgentFactory.createAgent();
			agent.transform.position = new Vector3(starts[i].x-1, 0.0f, starts[i].y-1);
			
			GameState.Instance.agents[agent.transform.position] = (Agent) agent.GetComponent(typeof(Agent));
			agents.Add (agent);
		}
		
		GameObject parent = GameObject.Find ("Waypoints"); // Empty GameObject that acts as a parent for the waypoint objects
		
		List<GameObject> waypoints = new List<GameObject> ();
		
		if (task == 3) {
			for (int i = 0; i < customers.Count; i++) {
				GameObject waypoint = WaypointFactory.createWaypoint ();
				
				// Randomize the position of the waypoin
				waypoint.transform.position = new Vector3 (customers[i].x-1, 0.0f, customers[i].y-1);
				
				waypoint.transform.parent = parent.transform;
				waypoint.name = "waypoint" + i;
				waypoints.Add (waypoint);
				
				GameState.Instance.customers[waypoint.transform.position] = waypoint;
			}
		}

		for (int i = 0; i < triangles.Count; i++) {
			ObstacleFactory.createPolygonalObstacle(triangles[i].ToArray());
		}
		
		return new Triple<List<GameObject>, List<GameObject>, List<List<Vector2>>> (agents, waypoints, triangles);
	}

	private Triple<List<GameObject>, List<GameObject>, List<List<Vector2>>> initDiscreteStage() {

		DiscreteLevelParser dlp = new DiscreteLevelParser();
		dlp.parse(file);

		_width = dlp.getWidth ();
		_height = dlp.getHeight ();
		List<Vector2> starts = dlp.getStarts ();
		List<Vector2> goals = dlp.getGoals ();
		List<Vector2> customers = dlp.getCustomers ();
		List<Vector2> obstaclePositions = dlp.getObstaclePositions ();
		
		Debug.Log ("Width:\t" + _width);
		Debug.Log ("Height:\t" + _height);
		Debug.Log ("Number of starting positions:\t" + starts.Count);
		Debug.Log ("Number of goal positions:\t" + goals.Count);
		
		StageFactory sf = new StageFactory ();
		sf.createStage (_width, _height);


		List<GameObject> agents = new List<GameObject> ();

		Debug.Log ("starts: "+ starts.Count);
		for (int i = 0; i < starts.Count; i++) {
			GameObject agent = AgentFactory.createAgent();
			agent.transform.position = new Vector3(starts[i].x-1, 0.0f, starts[i].y-1);

			GameState.Instance.agents[agent.transform.position] = (Agent) agent.GetComponent(typeof(Agent));
			agents.Add (agent);
		}

		GameObject parent = GameObject.Find ("Waypoints"); // Empty GameObject that acts as a parent for the waypoint objects
		
		List<GameObject> waypoints = new List<GameObject> ();

		if (task == 1 || task == 3) {
			for (int i = 0; i < goals.Count; i++) {
				GameObject waypoint = WaypointFactory.createWaypoint ();
				
				// Randomize the position of the waypoin
				waypoint.transform.position = new Vector3 (goals[i].x-1, 0.0f, goals[i].y-1);

				waypoint.transform.parent = parent.transform;
				waypoint.name = "waypoint" + i;
				waypoints.Add (waypoint);

				GameState.Instance.customers[waypoint.transform.position] = waypoint;
			}
		}

		if (task == 2) {

			for (int i = 0; i < customers.Count; i++) {
				GameObject waypoint = WaypointFactory.createWaypoint ();
				
				// Randomize the position of the waypoin
				waypoint.transform.position = new Vector3 (customers[i].x-1, 0.0f, customers[i].y-1);
				
				waypoint.transform.parent = parent.transform;
				waypoint.name = "waypoint" + i;
				waypoints.Add (waypoint);
				
				GameState.Instance.customers[waypoint.transform.position] = waypoint;
			}

		}
		

//		List<GameObject> obstacles = new List<GameObject> ();
		
		for (int i = 0; i < obstaclePositions.Count; i++) {
			GameObject obstacle = ObstacleFactory.createDiscreteObstacle(new Vector3(obstaclePositions[i].x-1, 0.0f, obstaclePositions[i].y-1));

			GameState.Instance.obstacles.Add (obstacle.transform.position);
//			obstacles.Add (obstacle);
		}
		

		return new Triple<List<GameObject>, List<GameObject>, List<List<Vector2>>> (agents, waypoints, null);


	}

	List<GameObject> createRandomObstacles(float width, float height, int numberOfObstacles) {
		
		List<GameObject> obstacles = new List<GameObject> ();
		
		for (int i = 0; i < numberOfObstacles; i++) {
			GameObject obstacle = ObstacleFactory.createDiscreteObstacle(new Vector3(Random.Range(0, (int)width), 0.0f, Random.Range(0, (int)height)));
			while (GameState.Instance.obstacles.Contains(obstacle.transform.position) || GameState.Instance.obstacles.Contains(obstacle.transform.position))
				obstacle.transform.position = new Vector3(Random.Range(0, (int)width), 0.0f, Random.Range(0, (int)height));
			
			GameState.Instance.obstacles.Add (obstacle.transform.position);
			obstacles.Add (obstacle);
		}
		
		return obstacles;
	}

	List<GameObject> createRandomAgents(float width, float height, int numberOfAgents, List<Vector2[]> polygons) {

		List<GameObject> agents = new List<GameObject> ();

		for (int i = 0; i < numberOfAgents; i++) {
			GameObject agent = AgentFactory.createAgent();
			agent.transform.position = new Vector3(Random.Range(0, (int)width), 0.0f, Random.Range(0, (int)height));
			while (GameState.Instance.agents.ContainsKey(agent.transform.position) || GameState.Instance.obstacles.Contains(agent.transform.position) || PathFinding.isInObstacle(agent.transform.position, polygons))
				agent.transform.position = new Vector3(Random.Range(0, (int)width), 0.0f, Random.Range(0, (int)height));

			GameState.Instance.agents[agent.transform.position] = (Agent) agent.GetComponent(typeof(Agent));
			agents.Add (agent);
		}

		return agents;
	}

	List<GameObject> createRandomWaypoints(float width, float height, int numberOfWaypoints, List<Vector2[]> polygons) {

		GameObject parent = GameObject.Find ("Waypoints"); // Empty GameObject that acts as a parent for the waypoint objects

		List<GameObject> waypoints = new List<GameObject> ();

		for (int i = 0; i < numberOfWaypoints; i++) {
			GameObject waypoint = WaypointFactory.createWaypoint ();

			// Randomize the position of the waypoint
			float x = Random.Range(0, (int)width);
			float y = 0.0f;
			float z = Random.Range(0, (int)height);
			waypoint.transform.position = new Vector3 (x, y, z);

			while (GameState.Instance.customers.ContainsKey(waypoint.transform.position) || GameState.Instance.obstacles.Contains(waypoint.transform.position) || PathFinding.isInObstacle(waypoint.transform.position, polygons))
				waypoint.transform.position = new Vector3(Random.Range(0, (int)width), y, Random.Range(0, (int)height));

			waypoint.transform.parent = parent.transform;
			waypoint.name = "waypoint" + i;
			waypoints.Add (waypoint);
			GameState.Instance.customers[waypoint.transform.position] = waypoint;
		}

		return waypoints;
	}


	
}