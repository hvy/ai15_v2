using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{

		// Input file name
		public string discreteLevelFileName;

		// Objects acting as parent object to instantiated obstacles and waypoint. Used for cleanup
		public GameObject stage, waypoints;

		// Prefabs
		public Transform startPrefab, goalPrefab, waypointPrefab;
		public Transform boxPrefab, obstaclePrefab; // Discrete specific prefabs 

		private GraphBuilder graphBuilder = new GraphBuilder () ;
	
		public void createDiscreteStage ()
		{

				clearStage ();

				// Parse data from file
				DiscreteLevelParser dlp = new DiscreteLevelParser ();
				dlp.parse (discreteLevelFileName);
				int width = dlp.getWidth ();  
				int height = dlp.getHeight ();
				int numWaypoints = 0;		
				int numObstacles = dlp.getNumObstacles ();
				List<Vector2> obstaclePositions = dlp.getObstaclePositions ();
				float obstacleWidth = GameManager.width / (float)width;
		
				bool[,] hasObstacle = new bool[width, height];

				// Place the obstacles
				foreach (Vector2 obstaclePosition in obstaclePositions) {
						hasObstacle [(int)obstaclePosition.x, (int)obstaclePosition.y] = true;
						float x = (float)(obstacleWidth / 2.0f) + (obstaclePosition.x * obstacleWidth);
						float y = 0.0f;
						float z = (float)(obstacleWidth / 2.0f) + (obstaclePosition.y * obstacleWidth);
						Transform obstacle = Instantiate (boxPrefab, new Vector3 (x, y, z), Quaternion.identity) as Transform;
						obstacle.parent = stage.transform;
				}

				// Instantiate the waypoints
				for (int i = 0; i < width; i++) {
						for (int j = 0; j < height; j++) {
								if (!hasObstacle [i, j]) {
										float x = (float)(obstacleWidth / 2.0f) + (i * obstacleWidth);
										float y = 1.7f;
										float z = (float)(obstacleWidth / 2.0f) + (j * obstacleWidth);
										Transform waypoint = Instantiate (waypointPrefab, new Vector3 (x, y, z), Quaternion.identity) as Transform;
										waypoint.parent = waypoints.transform;
								}
						}		
				}

				// Find the optimal path and save it in the graph builde
				GraphBuilder.buildGraphFromScene ();
				//testShit ();
		}
	


		public void createContinuousStage ()
		{

				clearStage ();
				List<Vector2[]> polygons = new List<Vector2[]> ();

				// Test obstacle with 3 vertices
				Vector2[] vertices3 = new Vector2[3];
				vertices3 [0] = new Vector2 (60, 50);
				vertices3 [1] = new Vector2 (55, 70);
				vertices3 [2] = new Vector2 (90, 60);
				GameObject obstacleA = ObstacleFactory.createPolygonalObstacle (vertices3);
				

				// Test obstacle with 4 vertices
				Vector2[] vertices4 = new Vector2[4];
				vertices4 [0] = new Vector2 (20, 0);
				vertices4 [1] = new Vector2 (20, 75);
				vertices4 [2] = new Vector2 (30, 75);
				vertices4 [3] = new Vector2 (30, 0);
				GameObject obstacleB = ObstacleFactory.createPolygonalObstacle (vertices4);

				Vector2[] vertices4_2 = new Vector2[4];
				vertices4_2 [0] = new Vector2 (40, 20);
				vertices4_2 [1] = new Vector2 (40, 100);
				vertices4_2 [2] = new Vector2 (50, 100);
				vertices4_2 [3] = new Vector2 (50, 20);
				GameObject obstacleC = ObstacleFactory.createPolygonalObstacle (vertices4_2);
				
				polygons.Add (vertices4);
				polygons.Add (vertices4_2);
				polygons.Add (vertices3);

				PathFinding.RRT(polygons, Vector3.zero, Vector3.zero);
		}

		public void clearStage ()
		{
				clearChildrenOf (stage);
				clearChildrenOf (waypoints);
		}

		private void clearChildrenOf (GameObject gameObject)
		{
				int childCount = gameObject.transform.childCount;
		
				for (int i = 0; i < childCount; i++) {
						GameObject child = gameObject.transform.GetChild (i).gameObject;
						if (child.name == "Static") {
								continue; // Do not destroy static objects such as the ground and the walls
						}		
						Object.Destroy (child);
				}
		}
}