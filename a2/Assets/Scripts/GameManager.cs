using UnityEngine;
using System.Collections;

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
		for (int i = 0; i < numWaypoints; i++) {
			WaypointFactory.createWaypoint();
		}

		agentFact = new AgentFactory();

		for (int i = 0; i < nr_agents; i++) {
			//GameObject agent = agentFact.createAgent();
			//agent.transform.position = new Vector3(Random.Range(0, 100), 0.0f, Random.Range(0, 100));
		}
	}


	
}