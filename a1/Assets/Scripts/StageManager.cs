using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{

		// Input file name
		public string discreteLevelFileName, polygonalLevelFileName;

		// Objects acting as parent object to instantiated obstacles and waypoint. Used for cleanup
		public GameObject stage, waypoints;

		// Prefabs
		public Transform startPrefab, goalPrefab, waypointPrefab;
		public Transform boxPrefab, obstaclePrefab; // Discrete specific prefabs 

		public static List<Vector2[]> polygons;

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
		polygons = new List<Vector2[]> ();

		PolygonalLevelParser plp = new PolygonalLevelParser ();
		plp.parse (polygonalLevelFileName);
		int width = plp.getWidth ();  
		int height = plp.getHeight ();
		int numWaypoints = 0;
		List<Vector2> vertices = plp.getVertices ();
			
		Debug.Log ("Length: " + vertices.Count);

		// HARDCODED for this specific input file
		Vector2[] vertices4_1 = new Vector2[4];
		vertices4_1 [0] = vertices[3];
		vertices4_1 [1] = vertices[2];
		vertices4_1 [2] = vertices[1];
		vertices4_1 [3] = vertices[0];
		GameObject obstacle4_1 = ObstacleFactory.createPolygonalObstacle (vertices4_1);

		Vector2[] vertices4_2 = new Vector2[4];
		vertices4_2 [0] = vertices[10];
		vertices4_2 [1] = vertices[11];
		vertices4_2 [2] = vertices[12];
		vertices4_2 [3] = vertices[13];
		GameObject obstacle4_2 = ObstacleFactory.createPolygonalObstacle (vertices4_2);

		Vector2[] vertices4_3 = new Vector2[4];
		vertices4_3 [0] = vertices[14];
		vertices4_3 [1] = vertices[15];
		vertices4_3 [2] = vertices[16];
		vertices4_3 [3] = vertices[17];
		GameObject obstacle4_3 = ObstacleFactory.createPolygonalObstacle (vertices4_3);

		Vector2[] vertices5_1a = new Vector2[3];
		Vector2[] vertices5_1b = new Vector2[4];
		vertices5_1a [0] = vertices[20];
		vertices5_1a [1] = vertices[21];
		vertices5_1a [2] = vertices[22];
		vertices5_1b [0] = vertices[18];
		vertices5_1b [1] = vertices[19];
		vertices5_1b [2] = vertices[20];
		vertices5_1b [3] = vertices[22];
		GameObject obstacle5_1a = ObstacleFactory.createPolygonalObstacle (vertices5_1a);
		GameObject obstacle5_1b = ObstacleFactory.createPolygonalObstacle (vertices5_1b);

		Vector2[] vertices6_1a = new Vector2[4];
		Vector2[] vertices6_1b = new Vector2[4];
		vertices6_1a [0] = vertices[6];
		vertices6_1a [1] = vertices[7];
		vertices6_1a [2] = vertices[8];
		vertices6_1a [3] = vertices[9];
		vertices6_1b [0] = vertices[4];
		vertices6_1b [1] = vertices[5];
		vertices6_1b [2] = vertices[6];
		vertices6_1b [3] = vertices[9];
		GameObject obstacle6_1a = ObstacleFactory.createPolygonalObstacle (vertices6_1a);
		GameObject obstacle6_1b = ObstacleFactory.createPolygonalObstacle (vertices6_1b);

		obstacle4_1.transform.parent = stage.transform;
		obstacle4_2.transform.parent = stage.transform;
		obstacle4_3.transform.parent = stage.transform;
		obstacle5_1a.transform.parent = stage.transform;
		obstacle5_1b.transform.parent = stage.transform;
		obstacle6_1a.transform.parent = stage.transform;
		obstacle6_1b.transform.parent = stage.transform;

		polygons.Add (vertices4_1);
		polygons.Add (vertices4_2);
		polygons.Add (vertices4_3);
		polygons.Add (vertices5_1a);
		polygons.Add (vertices5_1b);
		polygons.Add (vertices6_1a);
		polygons.Add (vertices6_1b);

			/*
			clearStage ();
			polygons = new List<Vector2[]> ();

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
			*/

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