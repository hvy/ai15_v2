using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{	
	// TODO FIX
	//public static Vector3 start, goal;
	public static int discreteNeighbors;
	//public static float width, height;


	public static Dictionary<Vector3, Agent> agentPos;
	public static Dictionary<Vector3, GameObject> customerPos;
	public static List<Vector3> obstacles;
	public static int gameTick = 0;

	public float _width, _height;
	public int nr_agents, numWaypoints;
	public int neighbors;
	public int task;

	void Start () 
	{
		CameraModel.updateOrthoPosition(_width, Camera.main.transform.position.y, _height);
		agentPos = new Dictionary<Vector3, Agent>();
		customerPos = new Dictionary<Vector3, GameObject>();
		obstacles = new List<Vector3>();
		init();
	}


	void init() {

		// Create stage
		StageFactory stageFactory = new StageFactory ();
		stageFactory.createStage(_width, _height);

		// Create waypoints
		List<GameObject> waypoints = createRandomWaypoints (_width, _height, numWaypoints);

		// Create agents
		List<GameObject> agents = createRandomAgents (_width, _height, nr_agents);
		

		// Run Collision avoidance algo
		PathPlanner pp = new PathPlanner ();
		List<Vector3> obstacles = new List<Vector3>();

		List<List<GNode>> paths;
		if (task == 1)
			 paths = pp.planDiscretePaths ((int) _width, (int) _height, agents, waypoints, neighbors, obstacles);
		else if (task == 2)
			pp.planVRPPaths ((int) _width, (int) _height, agents, waypoints, neighbors, obstacles);

	
	}
	
	List<GameObject> createRandomAgents(float width, float height, int numberOfAgents) {

		List<GameObject> agents = new List<GameObject> ();

		for (int i = 0; i < numberOfAgents; i++) {
			GameObject agent = AgentFactory.createAgent();
			agent.transform.position = new Vector3(Random.Range(0, (int)width), 0.0f, Random.Range(0, (int)height));
			while (agentPos.ContainsKey(agent.transform.position))
				agent.transform.position = new Vector3(Random.Range(0, (int)width), 0.0f, Random.Range(0, (int)height));

			agentPos[agent.transform.position] = (Agent) agent.GetComponent(typeof(Agent));
			agents.Add (agent);
		}

		return agents;
	}

	List<GameObject> createRandomWaypoints(float width, float height, int numberOfWaypoints) {

		GameObject parent = GameObject.Find ("Waypoints"); // Empty GameObject that acts as a parent for the waypoint objects

		List<GameObject> waypoints = new List<GameObject> ();

		for (int i = 0; i < numberOfWaypoints; i++) {
			GameObject waypoint = WaypointFactory.createWaypoint ();

			// Randomize the position of the waypoint
			float x = Random.Range(0, (int)width);
			float y = 0.0f;
			float z = Random.Range(0, (int)height);
			waypoint.transform.position = new Vector3 (x, y, z);

			while (customerPos.ContainsKey(waypoint.transform.position))
				waypoint.transform.position = new Vector3(Random.Range(0, (int)width), 0.0f, Random.Range(0, (int)height));

			waypoint.transform.parent = parent.transform;
			waypoint.name = "waypoint" + i;
			waypoints.Add (waypoint);
			customerPos[waypoint.transform.position] = waypoint;
		}

		return waypoints;
	}


	
}