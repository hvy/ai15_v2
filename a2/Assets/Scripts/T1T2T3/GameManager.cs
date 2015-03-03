using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{	

	public static int discreteNeighbors;

	public float _width, _height;
	public int nr_agents, numWaypoints;
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

		// Create waypoints
		List<GameObject> waypoints = createRandomWaypoints (_width, _height, numWaypoints);

		// Create agents
		List<GameObject> agents = createRandomAgents (_width, _height, nr_agents);



		// Run Collision avoidance algo
		PathPlanner pp = new PathPlanner ();


		List<Vector2[]> polygons = new List<Vector2[]>();
		Debug.Log ("KLAR");
		if (task == 1)
			pp.planDiscretePaths ((int) _width, (int) _height, agents, waypoints, neighbors, GameState.Instance.obstacles);
		else if (task == 2)
			pp.planVRPPaths ((int) _width, (int) _height, agents, waypoints, neighbors, GameState.Instance.obstacles, RandomIterations, GeneticIterations, GeneticPopulation, GeneticTournaments);
		else if (task == 3)
			pp.planContinuousVRP ((int) _width, (int) _height, agents, waypoints, polygons, RandomIterations);
	
	}

	List<GameObject> createRandomAgents(float width, float height, int numberOfAgents) {

		List<GameObject> agents = new List<GameObject> ();

		for (int i = 0; i < numberOfAgents; i++) {
			GameObject agent = AgentFactory.createAgent();
			agent.transform.position = new Vector3(Random.Range(0, (int)width), 0.0f, Random.Range(0, (int)height));
			while (GameState.Instance.agents.ContainsKey(agent.transform.position) || GameState.Instance.obstacles.Contains(agent.transform.position))
				agent.transform.position = new Vector3(Random.Range(0, (int)width), 0.0f, Random.Range(0, (int)height));

			GameState.Instance.agents[agent.transform.position] = (Agent) agent.GetComponent(typeof(Agent));
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

			while (GameState.Instance.customers.ContainsKey(waypoint.transform.position) || GameState.Instance.obstacles.Contains(waypoint.transform.position))
				waypoint.transform.position = new Vector3(Random.Range(0, (int)width), y, Random.Range(0, (int)height));

			waypoint.transform.parent = parent.transform;
			waypoint.name = "waypoint" + i;
			waypoints.Add (waypoint);
			GameState.Instance.customers[waypoint.transform.position] = waypoint;
		}

		return waypoints;
	}


	
}