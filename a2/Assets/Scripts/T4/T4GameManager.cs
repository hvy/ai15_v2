using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class T4GameManager : MonoBehaviour {
	
	//public int numAgents; // numAgents > 0
	public int motionModelId;
	public float acceleration;
	public string file;

	private float width, height;
	List<GameObject> agents;
	private CollisionAvoidance ca;
	public int offset;

	void Awake () {

		Triple<List<GameObject>, List<GameObject>, List<List<Vector2>>> tripleOfAgents;
		tripleOfAgents = initPolyStage ();
		
		// Create stage
		StageFactory stageFactory = new StageFactory ();
		stageFactory.createStage(width, height);
		CameraModel.updateOrthoPosition(width, Camera.main.transform.position.y, height);

		GameState.Instance.height = (int)height;
		GameState.Instance.width = (int)width;
		
		// Create waypoints
		List<GameObject> agents = new List<GameObject> ();
		List<GameObject> waypoints = new List<GameObject> ();
		List<List<Vector2>> polys = new List<List<Vector2>> ();
		
		
		agents = tripleOfAgents.first;
		waypoints = tripleOfAgents.second;
		polys = tripleOfAgents.third;
		
		List<Vector2[]> polygons = new List<Vector2[]>();
		
		for (int i = 0; i < polys.Count; i++) {
			polygons.Add(polys[i].ToArray());
		}

		//Debug.LogError ("waypoints: " + waypoints.Count);
			
			ca = new CollisionAvoidance(agents, polygons);

	}
	
	void FixedUpdate () {	
		ca.avoidCollisions();
	}


	
	private GameObject createStage (float width, float height) {
		StageFactory sf = new StageFactory ();
		return sf.createStage (width, height);
	}

	private Triple<List<GameObject>, List<GameObject>, List<List<Vector2>>> initPolyStage() {
		
		PolygonalLevelParser plp = new PolygonalLevelParser();
		
		plp.parse(file);
		
		width = plp.getWidth();
		height = plp.getHeight();
		List<Vector2> starts = plp.getStarts ();
		List<Vector2> goals = plp.getGoals ();
		List<Vector2> customers = plp.getCustomers ();
		
		
		List<List<Vector2>> triangles = plp.getTriangles();
		
		StageFactory sf = new StageFactory ();
		sf.createStage (width, height);
		
		
		List<GameObject> agents = new List<GameObject> ();
		
		Debug.Log ("starts: "+ starts.Count);
		for (int i = 0; i < starts.Count; i++) {
			GameObject agent = AgentFactory.createAgent();
			agent.transform.position = new Vector3(starts[i].x-1+offset, 0.0f, starts[i].y-1+offset);
			Debug.Log ("Starts: " + starts[i]);

			Agent a = (Agent) agent.GetComponent(typeof(Agent));
			a.setStart(agent.transform.position);
			a.setModel(motionModelId);
			GameState.Instance.agents[agent.transform.position] = a;
			agents.Add (agent);

		}
		
		GameObject parent = GameObject.Find ("Waypoints"); // Empty GameObject that acts as a parent for the waypoint objects
		
		List<GameObject> waypoints = new List<GameObject> ();

			for (int i = 0; i < goals.Count; i++) {
				GameObject waypoint = WaypointFactory.createWaypoint ();
				
				// Randomize the position of the waypoin
				waypoint.transform.position = new Vector3 (goals[i].x-1+offset, 0.0f, goals[i].y-1+offset);
				
				waypoint.transform.parent = parent.transform;
				waypoint.name = "waypoint" + i;
				waypoints.Add (waypoint);

				GameObject agent = agents[i];
				Agent a = (Agent) agent.GetComponent(typeof(Agent));
				a.setGoal(waypoint.transform.position);
				
				GameState.Instance.customers[waypoint.transform.position] = waypoint;

			}

		for (int i = 0; i < triangles.Count; i++) {
			ObstacleFactory.createPolygonalObstacle(triangles[i].ToArray());
		}
		
		return new Triple<List<GameObject>, List<GameObject>, List<List<Vector2>>> (agents, waypoints, triangles);
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
				waypoint.transform.position = new Vector3(80, 0, 80);

			if (i==1)
				waypoint.transform.position = new Vector3(0, 0, 80);

			waypoint.transform.parent = parent.transform;
			waypoint.name = "waypoint" + i;
			waypoints.Add (waypoint);
			GameState.Instance.customers[waypoint.transform.position] = waypoint;
		}
		
		return waypoints;
	}
}
