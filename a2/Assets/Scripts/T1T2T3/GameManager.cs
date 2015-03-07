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
	public int RandomIterations;
	public int GeneticIterations;
	public int GeneticPopulation;
	public int GeneticTournaments;

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
		List<GameObject> obstacles;
		List<Vector2[]> polygons = new List<Vector2[]>();

		if (task < 3)
			obstacles = createRandomObstacles(_width, _height, numObstacles);
		else if (task == 3)
			polygons = createRandomPolygons(_width, _height, numObstacles);

		// Create waypoints
		List<GameObject> waypoints = createRandomWaypoints (_width, _height, numWaypoints, polygons);

		// Create agents
		List<GameObject> agents = createRandomAgents (_width, _height, nr_agents, polygons);


		PathPlanner pp = new PathPlanner ();
		VRPDiscrete vrpDiscrete = new VRPDiscrete();
		VRPContinous vrpContinous = new VRPContinous();

		//List<Vector2[]> polygons = new List<Vector2[]>();

		if (task == 1)
			pp.planDiscretePaths ((int) _width, (int) _height, agents, waypoints, neighbors, GameState.Instance.obstacles);
		else if (task == 2)
			vrpDiscrete.planVRPPaths(agents, waypoints, GameState.Instance.obstacles, RandomIterations, GeneticIterations, GeneticPopulation, GeneticTournaments);
		else if (task == 3)
			vrpContinous.planContinuousVRP(agents, waypoints, polygons, RandomIterations);
			//pp.planContinuousVRP ((int) _width, (int) _height, agents, waypoints, polygons, RandomIterations);
	
	}

	List<Vector2[]> createRandomPolygons(float width, float height, int numberOfObstacles) {

		// TODO skapa polygons på ett smart sätt, så de inte blir för stora etc.

		List<Vector2[]> polygons = new List<Vector2[]>();
//		
//		for (int i = 0; i < numberOfObstacles; i++) {
//
//			Vector2 v1 = new Vector2(Random.Range(0, (int)width), Random.Range(0, (int)height));
//			Vector2 v2 = new Vector2(Random.Range(0, (int)width/2), Random.Range(0, (int)height/2));
//			Vector2 v3 = new Vector2(Random.Range(0, (int)width), Random.Range(0, (int)height));
//			Vector2[] polys = {v1, v2, v3};
//			GameObject polygon = ObstacleFactory.createPolygonalObstacle(polys);
//
//
//
//			polygons.Add (polys);
//			//obstacles.Add (obstacle);
//		}

		 // TODO REMOVE THIS HARDCODED SHIT

		PolygonalLevelParser plp = new PolygonalLevelParser ();
		plp.parse ("polygonal");
		int w = plp.getWidth ();  
		int h = plp.getHeight ();
		int numWaypoints = 0;
		List<Vector2> vertices = plp.getVertices ();
		int scaleDown = 5;
		
		// HARDCODED for this specific input file
		Vector2[] vertices4_1 = new Vector2[4];
		vertices4_1 [0] = vertices [3] / scaleDown;
		vertices4_1 [1] = vertices [2] / scaleDown;
		vertices4_1 [2] = vertices [1] / scaleDown;
		vertices4_1 [3] = vertices [0] / scaleDown;
		GameObject obstacle4_1 = ObstacleFactory.createPolygonalObstacle (vertices4_1);
		
		Vector2[] vertices4_2 = new Vector2[4];
		vertices4_2 [0] = vertices [10] / scaleDown;
		vertices4_2 [1] = vertices [11] / scaleDown;
		vertices4_2 [2] = vertices [12] / scaleDown;
		vertices4_2 [3] = vertices [13] / scaleDown;
		GameObject obstacle4_2 = ObstacleFactory.createPolygonalObstacle (vertices4_2);
		
		Vector2[] vertices4_3 = new Vector2[4];
		vertices4_3 [0] = vertices [14] / scaleDown;
		vertices4_3 [1] = vertices [15] / scaleDown;
		vertices4_3 [2] = vertices [16] / scaleDown;
		vertices4_3 [3] = vertices [17] / scaleDown;
		GameObject obstacle4_3 = ObstacleFactory.createPolygonalObstacle (vertices4_3);
		
		Vector2[] vertices5_1a = new Vector2[3];
		Vector2[] vertices5_1b = new Vector2[4];
		vertices5_1a [0] = vertices [20] / scaleDown;
		vertices5_1a [1] = vertices [21] / scaleDown;
		vertices5_1a [2] = vertices [22] / scaleDown;
		vertices5_1b [0] = vertices [18] / scaleDown;
		vertices5_1b [1] = vertices [19] / scaleDown;
		vertices5_1b [2] = vertices [20] / scaleDown;
		vertices5_1b [3] = vertices [22] / scaleDown;
		GameObject obstacle5_1a = ObstacleFactory.createPolygonalObstacle (vertices5_1a);
		GameObject obstacle5_1b = ObstacleFactory.createPolygonalObstacle (vertices5_1b);
		
		Vector2[] vertices6_1a = new Vector2[4];
		Vector2[] vertices6_1b = new Vector2[4];
		vertices6_1a [0] = vertices [6] / scaleDown;
		vertices6_1a [1] = vertices [7] / scaleDown;
		vertices6_1a [2] = vertices [8] / scaleDown;
		vertices6_1a [3] = vertices [9] / scaleDown;
		vertices6_1b [0] = vertices [4] / scaleDown;
		vertices6_1b [1] = vertices [5] / scaleDown;
		vertices6_1b [2] = vertices [6] / scaleDown;
		vertices6_1b [3] = vertices [9] / scaleDown;
		GameObject obstacle6_1a = ObstacleFactory.createPolygonalObstacle (vertices6_1a);
		GameObject obstacle6_1b = ObstacleFactory.createPolygonalObstacle (vertices6_1b);

		
		polygons.Add (vertices4_1);
		polygons.Add (vertices4_2);
		polygons.Add (vertices4_3);
		polygons.Add (vertices5_1a);
		polygons.Add (vertices5_1b);
		polygons.Add (vertices6_1a);
		polygons.Add (vertices6_1b);
		
		return polygons;
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