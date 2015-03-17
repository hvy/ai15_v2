using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{	

	public static int discreteNeighbors;

	public float _width, _height;
	public int nr_agents, numWaypoints, numObstacles;
	public int neighbors;
	public int task;
	public int GeneticIterations;
	public int GeneticPopulation;
	public int GeneticTournaments;
	public bool DrawRRT;

	void Start () 
	{
		CameraModel.updateOrthoPosition(_width, Camera.main.transform.position.y, _height);
		init();
	}


	void init() {

		// Create stage
		StageFactory stageFactory = new StageFactory ();
		stageFactory.createStage(_width, _height);


		GameState.Instance.height = (int)_height;
		GameState.Instance.width = (int)_width;
		GameState.Instance.neighbors = (int)neighbors;

		// Create obstacles
		List<GameObject> obstacles = new List<GameObject>();
 		List<Vector2[]> polygons = new List<Vector2[]>();

		if (task < 3) {
			obstacles = createRandomObstacles (_width, _height, numObstacles);		
		}
		else if (task == 3) {
			// TODO Tobbe, please use the new parser. Hiro
			//polygons = createRandomPolygons(_width, _height, numObstacles);
		}

		// Create waypoints
		// TODO Tobbe, please use the new parser. Hiro
		//List<GameObject> waypoints = createRandomWaypoints (_width, _height, numWaypoints, polygons);
		List<GameObject> waypoints = new List<GameObject> ();

		// Create agents
		// TODO Tobbe, please use the new parser. Hiro
		//List<GameObject> agents = createRandomAgents (_width, _height, nr_agents, polygons);
		//List<GameObject> agents = new List<GameObject> ();
		List<GameObject> agents = new List<GameObject> ();

		if (task == 2) {
		obstacles[0].transform.position = new Vector3(5, 0, 2);
		obstacles[1].transform.position = new Vector3(6, 0, 2);
		obstacles[2].transform.position = new Vector3(7, 0, 2);
		obstacles[3].transform.position = new Vector3(8, 0, 2);
		obstacles[4].transform.position = new Vector3(9, 0, 2);
		obstacles[5].transform.position = new Vector3(10, 0, 2);

		GameState.Instance.obstacles.Clear();
		for (int i = 0; i < obstacles.Count; i++) {
			GameState.Instance.obstacles.Add (obstacles[i].transform.position);
		}


		agents[0].transform.position = new Vector3(0,0,1);
		agents[1].transform.position = new Vector3(5,0,0);
		agents[2].transform.position = new Vector3(6,0,0);
		agents[3].transform.position = new Vector3(7,0,0);
		agents[4].transform.position = new Vector3(10,0,8);

		GameState.Instance.agents.Clear();
		for (int i = 0; i < agents.Count; i++) {
			GameState.Instance.agents[agents[i].transform.position] = (Agent) agents[i].GetComponent(typeof(Agent));
		}

		waypoints[0].transform.position = new Vector3(0,0,7);
		waypoints[1].transform.position = new Vector3(5,0,7);
		waypoints[2].transform.position = new Vector3(6,0,7);
		waypoints[3].transform.position = new Vector3(7,0,7);
		waypoints[4].transform.position = new Vector3(12,0,1);
		waypoints[5].transform.position = new Vector3(12,0,12);
		waypoints[6].transform.position = new Vector3(12,0,10);

		GameState.Instance.customers.Clear();
		for (int i = 0; i < waypoints.Count; i++) {
			GameState.Instance.customers[waypoints[i].transform.position] = waypoints[i];
		}
		}


		PathPlanner pp = new PathPlanner ();
		VRPDiscrete vrpDiscrete = new VRPDiscrete();
		VRPContinous vrpContinous = new VRPContinous();

		//List<Vector2[]> polygons = new List<Vector2[]>();

		if (task == 1)
			pp.planDiscretePaths ((int) _width, (int) _height, agents, waypoints, neighbors, GameState.Instance.obstacles);
		else if (task == 2)
			vrpDiscrete.planVRPPaths(agents, waypoints, GameState.Instance.obstacles, GeneticIterations, GeneticPopulation, GeneticTournaments);
		else if (task == 3)
			vrpContinous.planContinuousVRP(agents, waypoints, polygons, GeneticIterations, GeneticPopulation, GeneticTournaments, DrawRRT);
			//pp.planContinuousVRP ((int) _width, (int) _height, agents, waypoints, polygons, RandomIterations);
	
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