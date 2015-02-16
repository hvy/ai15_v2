using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{

		// Input file name
	public string discreteLevelFileName, polygonalLevelFileName, polygonalLevelFileName2, poly3FileName;

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
				
				// Set start and goal
				Vector2 start2d = dlp.getStart ();
				float start3dx = (float)(obstacleWidth / 2.0f) + ((start2d.x - 1) * obstacleWidth);
				float start3dy = (float)(obstacleWidth / 2.0f) + ((start2d.y - 1) * obstacleWidth);
				setStart (new Vector3 (start3dx, 0, start3dy));

				Vector2 goal2d = dlp.getGoal ();
				float goal3dx = (float)(obstacleWidth / 2.0f) + ((goal2d.x - 1) * obstacleWidth);
				float goal3dy = (float)(obstacleWidth / 2.0f) + ((goal2d.y - 1) * obstacleWidth);
				setGoal (new Vector3 (goal3dx, 0, goal3dy));

				// Find the optimal path and save it in the graph builde
				GraphBuilder.buildGraphFromScene ();
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
				vertices4_1 [0] = vertices [3];
				vertices4_1 [1] = vertices [2];
				vertices4_1 [2] = vertices [1];
				vertices4_1 [3] = vertices [0];
				GameObject obstacle4_1 = ObstacleFactory.createPolygonalObstacle (vertices4_1);

				Vector2[] vertices4_2 = new Vector2[4];
				vertices4_2 [0] = vertices [10];
				vertices4_2 [1] = vertices [11];
				vertices4_2 [2] = vertices [12];
				vertices4_2 [3] = vertices [13];
				GameObject obstacle4_2 = ObstacleFactory.createPolygonalObstacle (vertices4_2);

				Vector2[] vertices4_3 = new Vector2[4];
				vertices4_3 [0] = vertices [14];
				vertices4_3 [1] = vertices [15];
				vertices4_3 [2] = vertices [16];
				vertices4_3 [3] = vertices [17];
				GameObject obstacle4_3 = ObstacleFactory.createPolygonalObstacle (vertices4_3);

				Vector2[] vertices5_1a = new Vector2[3];
				Vector2[] vertices5_1b = new Vector2[4];
				vertices5_1a [0] = vertices [20];
				vertices5_1a [1] = vertices [21];
				vertices5_1a [2] = vertices [22];
				vertices5_1b [0] = vertices [18];
				vertices5_1b [1] = vertices [19];
				vertices5_1b [2] = vertices [20];
				vertices5_1b [3] = vertices [22];
				GameObject obstacle5_1a = ObstacleFactory.createPolygonalObstacle (vertices5_1a);
				GameObject obstacle5_1b = ObstacleFactory.createPolygonalObstacle (vertices5_1b);

				Vector2[] vertices6_1a = new Vector2[4];
				Vector2[] vertices6_1b = new Vector2[4];
				vertices6_1a [0] = vertices [6];
				vertices6_1a [1] = vertices [7];
				vertices6_1a [2] = vertices [8];
				vertices6_1a [3] = vertices [9];
				vertices6_1b [0] = vertices [4];
				vertices6_1b [1] = vertices [5];
				vertices6_1b [2] = vertices [6];
				vertices6_1b [3] = vertices [9];
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

				// Set the start and goal positions
				Vector2 start2d = plp.getStart ();
				Vector2 goal2d = plp.getGoal ();
				setStart (VectorUtility.toVector3 (start2d));
				setGoal (VectorUtility.toVector3 (goal2d));
		}

		public void createContinuousStage2 ()
		{
				clearStage ();
				polygons = new List<Vector2[]> ();
		
				PolygonalLevelParser plp = new PolygonalLevelParser ();
				plp.parse (polygonalLevelFileName2);
				int width = plp.getWidth ();  
				int height = plp.getHeight ();
				float scale = 3.0f;
				int numWaypoints = 0;
				List<Vector2> vertices = plp.getVertices ();
		
				Debug.Log ("Length: " + vertices.Count);
		
				// HARDCODED for this specific input file
				Vector2[] vertices4_1 = new Vector2[4];
				vertices4_1 [0] = vertices [8] / scale;
				vertices4_1 [1] = vertices [11] / scale;
				vertices4_1 [2] = vertices [10] / scale;
				vertices4_1 [3] = vertices [9] / scale;
				GameObject obstacle4_1 = ObstacleFactory.createPolygonalObstacle (vertices4_1);

				Vector2[] vertices4_2a = new Vector2[4];
				vertices4_2a [0] = vertices [1] / scale;
				vertices4_2a [1] = vertices [0] / scale;
				vertices4_2a [2] = vertices [7] / scale;
				vertices4_2a [3] = vertices [6] / scale;
				GameObject obstacle4_2a = ObstacleFactory.createPolygonalObstacle (vertices4_2a);
				Vector2[] vertices4_2b = new Vector2[4];
				vertices4_2b [0] = vertices [1] / scale;
				vertices4_2b [1] = vertices [6] / scale;
				vertices4_2b [2] = vertices [5] / scale;
				vertices4_2b [3] = vertices [2] / scale;
				GameObject obstacle4_2b = ObstacleFactory.createPolygonalObstacle (vertices4_2b);
				Vector2[] vertices4_2c = new Vector2[4];
				vertices4_2c [0] = vertices [5] / scale;
				vertices4_2c [1] = vertices [4] / scale;
				vertices4_2c [2] = vertices [3] / scale;
				vertices4_2c [3] = vertices [2] / scale;
				GameObject obstacle4_2c = ObstacleFactory.createPolygonalObstacle (vertices4_2c);
				
				obstacle4_1.transform.parent = stage.transform;
				obstacle4_2a.transform.parent = stage.transform;
				obstacle4_2b.transform.parent = stage.transform;
				obstacle4_2c.transform.parent = stage.transform;
		
				polygons.Add (vertices4_1);
				polygons.Add (vertices4_2a);
				polygons.Add (vertices4_2b);
				polygons.Add (vertices4_2c);
		
				// Set the start and goal positions
				Vector2 start2d = plp.getStart () / scale;
				Vector2 goal2d = plp.getGoal () / scale;
				setStart (VectorUtility.toVector3 (start2d));
				setGoal (VectorUtility.toVector3 (goal2d));
		}

	public void createContinuousStage3 () {

		clearStage ();
		polygons = new List<Vector2[]> ();

		LevelParser lp = new LevelParser ();
		lp.parse (poly3FileName);

		List<List<Vector2>> triangles = lp.getTriangles ();

		Debug.Log ("Total number of triangles: " + triangles.Count);

		foreach (List<Vector2> triangle in triangles) {
			GameObject obstacle = ObstacleFactory.createPolygonalObstacle (triangle.ToArray ());
			obstacle.transform.parent = stage.transform;
			polygons.Add (triangle.ToArray ());
		}

		// Set the start and goal positions
		Vector2 start2d = lp.getStart ();
		Vector2 goal2d = lp.getGoal ();
		setStart (VectorUtility.toVector3 (start2d));
		setGoal (VectorUtility.toVector3 (goal2d));
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

		private void setStart (Vector3 pos)
		{
				Transform start = Instantiate (startPrefab, pos, Quaternion.identity) as Transform;
				GameManager.start = pos;
				addToStage (start);
		}

		private void setGoal (Vector3 pos)
		{
				Transform goal = Instantiate (goalPrefab, pos, Quaternion.identity) as Transform;
				GameManager.goal = pos;
				addToStage (goal);
		}

		private void addToStage (Transform trans)
		{
				trans.parent = stage.transform;
		}
}