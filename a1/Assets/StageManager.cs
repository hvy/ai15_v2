using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageManager : MonoBehaviour{

	public GameObject stage, waypoints;
	public Transform startPrefab, goalPrefab, waypointPrefab;
	public InputField numberOfBoxesInputField;
	
	// Discrete stage
	public Transform boxPrefab;

	// Discrete stage
	public void createDiscreteStage() {

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

		for (int i = 0; i < numBoxes; i++) {
			int xBoxNum = Random.Range(0, 20);			
			int yBoxNum = Random.Range(0, 20);			
			float x = xBoxNum * boxSize + halfBoxSize;			
			float y = yBoxNum * boxSize + halfBoxSize;
			Transform boxTransform = GameObject.Instantiate(boxPrefab, new Vector3(x, 1.7f, y), Quaternion.identity) as Transform;
			boxTransform.parent = stage.transform;

			// Make sure this position is not drivable so that no waypoint is created
			drivable[xBoxNum, yBoxNum] = false;
		}

		// Create the waypoints
		for (int i = 0; i < numBoxesPerSide; i++) {
			for (int j = 0; j < numBoxesPerSide; j++) {
				if(drivable[i,j]) {
					float x = i * boxSize + halfBoxSize;			
					float y = j * boxSize + halfBoxSize;
					setWayPoint(x, y);
				}
			}
		}
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

	private void setWayPoint(float x, float y) {
		Transform waypointTransform = Instantiate(waypointPrefab, new Vector3(x, 1.4f, y), Quaternion.identity) as Transform;

		waypointTransform.parent = waypoints.transform;
	}
}
