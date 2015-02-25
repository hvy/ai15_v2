using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Used for debugging modules
public class Test : MonoBehaviour {

	void Start () {

		// Test stage creation
		StageFactory stageFactory = new StageFactory ();
		float width = 100f;
		float height = 100f;
		stageFactory.createStage (width, height);	


		//WaypointFactory waypointFactory = new WaypointFactory ();

		int numWaypoints = 100;
		List<GameObject> waypoints = new List<GameObject> ();

		//WaypointFactory waypointFactory = new WaypointFactory ();

		for (int i = 0; i < numWaypoints; i++) {
			GameObject waypoint = WaypointFactory.createWaypoint();
			waypoints.Add(waypoint);
		}

	}

	void Update () {
	
	}
}
