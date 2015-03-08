using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class T4GameManager : MonoBehaviour {
	
	public int numAgents; // numAgents > 0
	public int motionModelId;
	public float width, height, acceleration;

	List<GameObject> agents;
	private CollisionAvoidance ca;
	private int counter = 0;

	void Start () {
		createStage (width, height);
		CameraModel.updateOrthoPosition(width, Camera.main.transform.position.y, height);

		List<Vector2[]> polygons = new List<Vector2[]>();
		List<GameObject> waypoints = createRandomWaypoints (width, height, numAgents, polygons);
		agents = createRandomAgents (width, height, numAgents, polygons, waypoints);

		agents[0].transform.position = new Vector3(0,0,0);
		agents[1].transform.position = new Vector3(6,0,0);


		ca = new CollisionAvoidance(agents);

	}
	
	void FixedUpdate () {	
		//updatePlayerAgent();
		if (counter % 10 == 0) {
			counter = 0;
			ca.avoidCollisions();
		}
		counter++;
		//Debug.Log (agents[0].rigidbody.velocity);
	}

	private void updatePlayerAgent () {
		
		GameObject playerAgent = agents[0];
		Agent agent = (Agent) playerAgent.GetComponent(typeof(Agent));
		DynamicController dc = (DynamicController) agent.models[motionModelId];
		
		// Listen for rotation
		//playerAgent.transform.Rotate(0, Input.GetAxis("Horizontal")*rotationSpeed*Time.deltaTime, 0);
		
		Vector3 agentTranslation = Vector3.zero;
		
		if (Input.GetKey (KeyCode.UpArrow)) {
			agentTranslation.z += 1.0f;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			agentTranslation.z -= 1.0f;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			agentTranslation.x += 1.0f;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			agentTranslation.x -= 1.0f;
		}
		
		//playerAgent.transform.Translate (agentTranslation * Time.deltaTime * playerAgent.);
		dc.direction = agentTranslation;
		
	}

	
	private GameObject createStage (float width, float height) {
		StageFactory sf = new StageFactory ();
		return sf.createStage (width, height);
	}
	
	List<GameObject> createRandomAgents(float width, float height, int numberOfAgents, List<Vector2[]> polygons, List<GameObject> waypoints) {
		
		List<GameObject> agents = new List<GameObject> ();
		
		for (int i = 0; i < numberOfAgents; i++) {
			GameObject agent = AgentFactory.createAgent();
			agent.transform.position = new Vector3(Random.Range(0, (int)width), 0.0f, Random.Range(0, (int)height));
			while (GameState.Instance.agents.ContainsKey(agent.transform.position) || GameState.Instance.obstacles.Contains(agent.transform.position) || PathFinding.isInObstacle(agent.transform.position, polygons))
				agent.transform.position = new Vector3(Random.Range(0, (int)width), 0.0f, Random.Range(0, (int)height));

			Agent a = (Agent) agent.GetComponent(typeof(Agent));
			GameState.Instance.agents[agent.transform.position] = a;
			agents.Add (agent);
			a.setStart(agent.transform.position);
			a.setGoal(waypoints[i].transform.position);
			a.setModel(motionModelId);
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

			if (i == 0)
				waypoint.transform.position = new Vector3(10, 0, 10);

			if (i==1)
				waypoint.transform.position = new Vector3(3, 0, 4);

			waypoint.transform.parent = parent.transform;
			waypoint.name = "waypoint" + i;
			waypoints.Add (waypoint);
			GameState.Instance.customers[waypoint.transform.position] = waypoint;
		}
		
		return waypoints;
	}
}
