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
	public int VRPIterations;

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

		//test (agents, waypoints);

		// Run Collision avoidance algo
		PathPlanner pp = new PathPlanner ();
		List<Vector3> obstacles = new List<Vector3>();

		List<List<GNode>> paths;
		if (task == 1)
			 paths = pp.planDiscretePaths ((int) _width, (int) _height, agents, waypoints, neighbors, obstacles);
		else if (task == 2)
			pp.planVRPPaths ((int) _width, (int) _height, agents, waypoints, neighbors, obstacles, VRPIterations);

	
	}

	private void test(List<GameObject> agents, List<GameObject> waypoints) {
		//agents.RemoveRange(1, agents.Count-1);
		//waypoints.RemoveRange(9, waypoints.Count-10);

		agents[0].transform.position = new Vector3(0,0,0);
		agents[1].transform.position = new Vector3(3,0,0);

		waypoints[7].transform.position = new Vector3(1,0,9);
		waypoints[1].transform.position = new Vector3(2,0,2);
		waypoints[2].transform.position = new Vector3(3,0,1);
		waypoints[0].transform.position = new Vector3(1,0,1);
		waypoints[3].transform.position = new Vector3(5,0,2);
		waypoints[4].transform.position = new Vector3(6,0,2);
		waypoints[5].transform.position = new Vector3(9,0,8);
		waypoints[6].transform.position = new Vector3(7,0,3);
		waypoints[8].transform.position = new Vector3(3,0,9);
		waypoints[9].transform.position = new Vector3(2,0,4);


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