using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour{
	
	public GameObject stage, waypoints;
	public Transform startPrefab, goalPrefab, waypointPrefab;
	public InputField numberOfBoxesInputField;
	public InputField numberOfObstaclesInputField;

	public static List<GNode> aStarPath;
	private GNode start, end;
	
	// Discrete stage
	public Transform boxPrefab, obstaclePrefab;
	
	private int numWaypoints = 0;
	
	// Discrete stage
	public void createDiscreteStage() {
		
		numWaypoints = 0;
		clearStage ();
		setStartAndGoal ();
		
		int numBoxes = int.Parse(numberOfBoxesInputField.text);
		float boxSize = boxPrefab.transform.localScale.x;
		int numBoxesPerSide = (int) (GameObject.Find ("Ground").transform.localScale.x / boxSize);
		
		float halfBoxSize = boxSize / 2.0f;

		bool[,] drivable = new bool[numBoxesPerSide,numBoxesPerSide];

		for (int i = 0; i < numBoxesPerSide; i++) {
			for (int j = 0; j < numBoxesPerSide; j++) {
				drivable[i,j] = true;
			}
		}
		
		Transform[] boxes = new Transform[numBoxes];
		for (int i = 0; i < numBoxes; i++) {
			int xBoxNum = UnityEngine.Random.Range(0, 20);			
			int yBoxNum = UnityEngine.Random.Range(0, 20);			
			float x = xBoxNum * boxSize + halfBoxSize;			
			float y = yBoxNum * boxSize + halfBoxSize;

			bool boxExists = false;
			for (int j = 0; j < i; j++) {
				if (x == boxes[j].position.x && y == boxes[j].position.z) {
					i--;
					boxExists = true;
					break;
				}
			}
			if (boxExists) continue;
			
			Transform boxTransform = GameObject.Instantiate(boxPrefab, new Vector3(x, 1.7f, y), Quaternion.identity) as Transform;
			boxTransform.parent = stage.transform;
			boxes[i] = boxTransform;
			
			// Make sure this position is not drivable so that no waypoint is created
			drivable[xBoxNum, yBoxNum] = false;
		}
		
		Debug.Log ("Total number of boxes: " + numBoxes);
		
		// Create the waypoints
		int waypointIdx = 0;
		Transform[] waypoints = new Transform[numBoxesPerSide * numBoxesPerSide - numBoxes];
		
		Debug.Log ("Total number of waypoints to create: " + waypoints.Length);
		
		int count = 0;
		Debug.Log (drivable.Length);

		for (int i = 0; i < numBoxesPerSide; i++) {
			for (int j = 0; j < numBoxesPerSide; j++) {
				if (!drivable[i,j]) {
					count++;
				}
			}
		}
		Debug.Log ("Num waypoints created: " + count);

		Dictionary<Transform, GNode> nodes = new Dictionary<Transform, GNode> ();

		for (int i = 0; i < numBoxesPerSide; i++) {
			for (int j = 0; j < numBoxesPerSide; j++) {
				if(drivable[i,j]) {
					float x = i * boxSize + halfBoxSize;			
					float y = j * boxSize + halfBoxSize;

					waypoints[waypointIdx] = setWayPoint(x, y);

					List<GNode> neighbors = new List<GNode>();
					nodes[waypoints[waypointIdx]] =  new GNode(waypointIdx, waypoints[waypointIdx].position, neighbors);

					waypointIdx++;


					//waypoints[waypointIdx++] = new Vector2(x,y);
				}
			}
		}

		// create discrete graph
		//Dictionary<int, Neighbors> neighborTable = new Dictionary<int, Neighbors> ();

		Vector3[] rayDirections = new Vector3[8];
		rayDirections [0] = new Vector3 (1.0f, 0, 0);
		rayDirections [1] = new Vector3 (0, 0, 1.0f);
		rayDirections [2] = new Vector3 (-1.0f, 0, 0);
		rayDirections [3] = new Vector3 (0, 0, -1.0f);
		rayDirections [4] = new Vector3 (1.0f, 0, 1.0f);
		rayDirections [5] = new Vector3 (-1.0f, 0, 1.0f);
		rayDirections [6] = new Vector3 (-1.0f, 0, -1.0f);
		rayDirections [7] = new Vector3 (1.0f, 0, -1.0f);
		for (int i = 0; i < waypoints.Length; i++) {

			for (int j = 0; j < 8; j++) {
				RaycastHit[] hits;
				float l = (float) System.Math.Sqrt(boxSize*boxSize*2);
				hits = Physics.RaycastAll(waypoints[i].position, rayDirections[j], l);
				int hitIdx = 0;
				while(hitIdx < hits.Length) {
					RaycastHit hit = hits[hitIdx];

					if (hit.collider.tag == "Waypoint") {
						// draw edge
						nodes[waypoints[i]].addNeighbor(nodes[hit.transform]);
						//neighborTable[i].Add(hit.transform);
						//neighborTable[i].Add(hit.transform);
					}

					hitIdx++;
				}
			}
		}


		// A-STAR PATH (for testing)
		Debug.Log ("Nodes: " + nodes.Count);

	 	start = nodes [waypoints [0]];
		end = nodes [waypoints [waypoints.Length - 1]];
		aStarPath = PathFinding.FindPath (start, end, distance, estimate);
	}

	public void createContinuousStage() {

		clearStage ();
		setStartAndGoal ();

		/*
		int numObstacles = int.Parse (numberOfObstaclesInputField.text);

		for (int i = 0; i < numObstacles; i++) {
			Transform obstacleTransform = Instantiate(obstaclePrefab) as Transform;
			obstacleTransform.parent = stage.transform;
		}
		*/

		// Test obstacle with 3 vertices
		Vector2[] vertices3 = new Vector2[3];
		vertices3 [0] = new Vector2 (0, 0);
		vertices3 [1] = new Vector2 (0, 10);
		vertices3 [2] = new Vector2 (10, 0);
		GameObject obstacleA = ObstacleFactory.createPolygonalObstacle (vertices3);
		obstacleA.transform.Translate (new Vector3(50.0f, 0, 50.0f));

		// Test obstacle with 4 vertices
		Vector2[] vertices4 = new Vector2[4];
		vertices4 [0] = new Vector2 (3, 0);
		vertices4 [1] = new Vector2 (0, 10);
		vertices4 [2] = new Vector2 (10, 20);
		vertices4 [3] = new Vector2 (10, 0);
		GameObject obstacleB = ObstacleFactory.createPolygonalObstacle (vertices4);
	}

	private double distance(GNode a, GNode b) {
		return Vector2.Distance(a.getPos (), b.getPos ());
	}

	private double estimate(GNode a) {
		return Vector2.Distance(a.getPos (), end.getPos ());
	}
	
	public void clearStage() {
		clearChildrenOf (stage);
		clearChildrenOf (waypoints);
	}

	private void clearChildrenOf(GameObject gameObject) {
		int childCount = gameObject.transform.childCount;
		
		for (int i = 0; i < childCount; i++) {
			GameObject child = gameObject.transform.GetChild(i).gameObject;
			if (child.name == "Static") {
				continue; // Do not destroy static objects such as the ground and the walls
			}		
			Object.Destroy(child);
		}
	}

	private void setStartAndGoal() {
//		Vector3 start = GameManager.start;
//		Vector3 goal = GameManager.goal;
//		
//		Transform startTransform = Instantiate (startPrefab, start, Quaternion.identity) as Transform;
//		Transform goalTransform = Instantiate (goalPrefab, goal, Quaternion.identity) as Transform;
//		
//		startTransform.parent = stage.transform;
//		goalTransform.parent = stage.transform;
	}

	private Transform setWayPoint(float x, float y) {
		Transform waypointTransform = Instantiate(waypointPrefab, new Vector3(x, 1.4f, y), Quaternion.identity) as Transform;

		waypointTransform.parent = waypoints.transform;
		waypointTransform.gameObject.name = "" + numWaypoints;

		numWaypoints++;
		return waypointTransform;
	}
}