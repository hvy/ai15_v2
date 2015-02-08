using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphBuilder
{

	//public static List<GNode> aStarPath {get;set;}
		private static GNode start, end;

		// Fetches the waypoint positions from the scene and generates a graph
		public static void buildGraphFromScene ()
		{
				GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag ("Waypoint");

				Dictionary<GameObject, GNode> nodes = new Dictionary<GameObject, GNode> ();

				for (int i = 0; i < waypointObjects.Length; i++) {
						List<GNode> neighbors = new List<GNode> ();		
						GameObject waypoitObject = waypointObjects [i];
						nodes [waypoitObject] = new GNode (i, waypoitObject.transform.position, neighbors);
				}

				// Ray length
				float obstacleLength = 5.0f; // hard coded
				float l = (float)System.Math.Sqrt (obstacleLength * obstacleLength * 2);

				Vector3[] rayDirections = new Vector3[8];
				rayDirections [0] = new Vector3 (1.0f, 0, 0);
				rayDirections [1] = new Vector3 (0, 0, 1.0f);
				rayDirections [2] = new Vector3 (-1.0f, 0, 0);
				rayDirections [3] = new Vector3 (0, 0, -1.0f);
				rayDirections [4] = new Vector3 (1.0f, 0, 1.0f);
				rayDirections [5] = new Vector3 (-1.0f, 0, 1.0f);
				rayDirections [6] = new Vector3 (-1.0f, 0, -1.0f);
				rayDirections [7] = new Vector3 (1.0f, 0, -1.0f);

				for (int i = 0; i < waypointObjects.Length; i++) {
						for (int j = 0; j < rayDirections.Length; j++) {
								RaycastHit[] hits;
								hits = Physics.RaycastAll (waypointObjects [i].transform.position, rayDirections [j], l);
								int hitIdx = 0;
								while (hitIdx < hits.Length) {
										RaycastHit hit = hits [hitIdx];
					
										if (hit.collider.tag == "Waypoint") {
												nodes [waypointObjects [i]].addNeighbor (nodes [hit.transform.gameObject]);
										}
					
										hitIdx++;
								}
						}
				}

				// Find the start and the goal waypoints
				for (int i = 0; i < waypointObjects.Length; i++) {
					if (waypointObjects[i].transform.position.x == GameManager.start.x && 
					    waypointObjects[i].transform.position.z == GameManager.start.z) {
						start = nodes [waypointObjects [i]];
					} else if (waypointObjects[i].transform.position.x == GameManager.goal.x &&
					           waypointObjects[i].transform.position.z == GameManager.goal.z) {
						end = nodes [waypointObjects [i]];
					}		
				}
				
				PathFinding.aStarPath (start, end, distance);
				PathFinding.draw (PathFinding.currentPath);
		}
	
		public static double distance (GNode a, GNode b)
		{
				return Vector2.Distance (a.getPos (), b.getPos ());
		}
	
		
}
