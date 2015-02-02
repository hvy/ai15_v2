﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour{
	
	public GameObject stage, waypoints;
	public Transform startPrefab, goalPrefab, waypointPrefab;
	public InputField numberOfBoxesInputField;
	public InputField numberOfObstaclesInputField;

	public static List<GNode> aStarPath;
	
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
					nodes[waypoints[waypointIdx]] =  new GNode(waypointIdx, waypoints[waypointIdx], neighbors);

					waypointIdx++;


					//waypoints[waypointIdx++] = new Vector2(x,y);
				}
			}
		}

		// create discrete graph
		//Dictionary<int, Neighbors> neighborTable = new Dictionary<int, Neighbors> ();

		Vector3[] rayDirections = new Vector3[4];
		rayDirections [0] = new Vector3 (1.0f, 0, 0);
		rayDirections [1] = new Vector3 (0, 0, 1.0f);
		rayDirections [2] = new Vector3 (-1.0f, 0, 0);
		rayDirections [3] = new Vector3 (0, 0, -1.0f);
		for (int i = 0; i < waypoints.Length; i++) {

			for (int j = 0; j < 4; j++) {
				RaycastHit[] hits;
				hits = Physics.RaycastAll(waypoints[i].position, rayDirections[j], boxSize);
				int hitIdx = 0;
				while(hitIdx < hits.Length) {
					RaycastHit hit = hits[hitIdx];
					Debug.Log ("Hit name: " + hit.collider.gameObject.name);
					//if (hit.collider.gameObject)

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

		GNode start = nodes [waypoints [0]];
		GNode end = nodes [waypoints [waypoints.Length - 1]];
		aStarPath = PathFinding.FindPath (start, end, distance, estimate);
	}

	public void createContinuousStage() {

		clearStage ();
		setStartAndGoal ();

		int numObstacles = int.Parse (numberOfObstaclesInputField.text);

		for (int i = 0; i < numObstacles; i++) {
			Transform obstacleTransform = Instantiate(obstaclePrefab) as Transform;
			obstacleTransform.parent = stage.transform;
		}

	}

	private double distance(GNode a, GNode b) {
		return Vector2.Distance(a.getTransform().position, b.getTransform().position);
	}

	private double estimate(GNode a) {
		// TODO
		return 0;
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
		Vector2 start = GameManager.start;
		Vector2 goal = GameManager.goal;
		
		Transform startTransform = Instantiate (startPrefab, new Vector3(start.x, 0.0f, start.y), Quaternion.identity) as Transform;
		Transform goalTransform = Instantiate (goalPrefab, new Vector3(goal.x, 0.0f, goal.y), Quaternion.identity) as Transform;
		
		startTransform.parent = stage.transform;
		goalTransform.parent = stage.transform;
	}

	private Transform setWayPoint(float x, float y) {
		Transform waypointTransform = Instantiate(waypointPrefab, new Vector3(x, 1.4f, y), Quaternion.identity) as Transform;

		waypointTransform.parent = waypoints.transform;
		waypointTransform.gameObject.name = "" + numWaypoints;

		numWaypoints++;
		return waypointTransform;
	}
}