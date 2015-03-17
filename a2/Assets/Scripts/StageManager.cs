using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{

		// Input file name
		public string discreteLevelFileName, polygonalLevelFileName, polygonalLevelFileName2, poly3FileName;
		public float width, height;
        public int discreteNeighbors;

		// Objects acting as parent object to instantiated obstacles and waypoint. Used for cleanup
		public GameObject stage, waypoints;

		// Prefabs
		public Transform startPrefab, goalPrefab, waypointPrefab;
		public Transform boxPrefab, obstaclePrefab; // Discrete specific prefabs 

		public static List<Vector2[]> polygons;
		private GraphBuilder graphBuilder = new GraphBuilder () ;
	
		public void createDiscreteStage ()
		{
				/* 
				clearStage ();

                GameManager.discreteNeighbors = discreteNeighbors;

				// Parse data from file
				DiscreteLevelParser dlp = new DiscreteLevelParser ();
				dlp.parse (discreteLevelFileName);
				int width = dlp.getWidth ();  
				int height = dlp.getHeight ();
				int numWaypoints = 0;		
				int numObstacles = dlp.getNumObstacles ();
				List<Vector2> obstaclePositions = dlp.getObstaclePositions ();
				float obstacleWidth = width / (float)width;
		
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
				//GraphBuilder.buildGraphFromScene ();


				*/
		}

	public void createStageFromFile () {

		clearStage ();
		polygons = new List<Vector2[]> ();

		LevelParser lp = new LevelParser ();
		lp.parse (poly3FileName);

		List<List<Vector2>> triangles = lp.getTriangles ();

		foreach (List<Vector2> triangle in triangles) {
			GameObject obstacle = ObstacleFactory.createPolygonalObstacle (triangle.ToArray ());
			//obstacle.transform.parent = stage.transform;
			polygons.Add (triangle.ToArray ());
		}

		// Set the start and goal positions
		Vector2 start2d = lp.getStart ();
		Vector2 goal2d = lp.getGoal ();
		setStart (VectorUtility.toVector3 (start2d));
		setGoal (VectorUtility.toVector3 (goal2d));

		// Tell the GameManager that the width and height is updated
//		GameManager.width = width;
//		GameManager.height = height;
		updateDimensions (width, height);

		// Update the camera position according to the size of the current stage
		CameraModel.updatePosition (width, height);
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
				//GameManager.start = pos;
				addToStage (start);
		}

		private void setGoal (Vector3 pos)
		{
				Transform goal = Instantiate (goalPrefab, pos, Quaternion.identity) as Transform;
				//GameManager.goal = pos;
				addToStage (goal);
		}

		private void addToStage (Transform trans)
		{
				//trans.parent = stage.transform;
		}

		private void updateDimensions(float width, float height) {
			stage.transform.position = new Vector3 (width / 2.0f, -1.0f, height / 2.0f);
			GameObject.Find ("Ground").transform.localScale = new Vector3 (width, 1.0f, height);
		}
}