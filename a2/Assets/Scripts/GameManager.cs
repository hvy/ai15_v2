using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{	
	// TODO FIX
	//public static Vector3 start, goal;
	public static int discreteNeighbors;
	//public static float width, height;

	public float _width, _height;


	public int nr_agents, numWaypoints;
	
	AgentFactory agentFact;

	void Start () 
	{
		init();
	}

	void init() {

		// Create stage
		StageFactory stageFactory = new StageFactory ();
		stageFactory.createStage(_width, _height);

		// Create waypoints
		List<GameObject> waypoints = createRandomWaypoints (_width, _height, numWaypoints);

		agentFact = new AgentFactory();

		for (int i = 0; i < nr_agents; i++) {
			GameObject agent = AgentFactory.createAgent();
			agent.transform.position = new Vector3(Random.Range(0, (int)_width), 0.0f, Random.Range(0, (int)_height));
		}

		List<GameObject> agents = null;

		PathPlanner pp = new PathPlanner ();
		List<List<GNode>> paths = pp.planDiscretePaths ((int) _width, (int) _height, agents, waypoints);
	}

	List<GameObject> createRandomWaypoints(float width, float height, int numberOfWaypoints) {

		GameObject parent = GameObject.Find ("Waypoints"); // Empty GameObject that acts as a parent for the waypoint objects

		List<GameObject> waypoints = new List<GameObject> ();

		for (int i = 0; i < numberOfWaypoints; i++) {
			GameObject waypoint = WaypointFactory.createWaypoint ();

			// Randomize the position of the waypoint
			float x = Random.Range(0, (int)width);
			float y = 1.0f;
			float z = Random.Range(0, (int)height);
			waypoint.transform.position = new Vector3 (x, y, z);
			waypoint.transform.parent = parent.transform;
			waypoint.name = "waypoint" + i;
			waypoints.Add (waypoint);
		}

		return waypoints;
	}


	
}