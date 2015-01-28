using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour{

	public GameObject stage;
	public Transform startPrefab, goalPrefab;

	// Discrete stage
	public int numBoxes;
	public Transform boxPrefab;

	// Discrete stage
	public void createDiscreteStage() {

		setStartAndGoal ();

		float boxSize = boxPrefab.transform.localScale.x;
		float halfBoxSize = boxSize / 2.0f;
		
		for (int i = 0; i < numBoxes; i++) {
			int xBoxNum = Random.Range(0, 20);			
			int yBoxNum = Random.Range(0, 20);			
			float x = xBoxNum * boxSize + halfBoxSize;			
			float y = yBoxNum * boxSize + halfBoxSize;
			Transform boxTransform = GameObject.Instantiate(boxPrefab, new Vector3(x, 0, y), Quaternion.identity) as Transform;
			boxTransform.parent = stage.transform;
		}
	}
	
	public void clearStage() {
		
		int childCount = stage.transform.childCount;
		
		for (int i = 0; i < childCount; i++) {
			GameObject child = stage.transform.GetChild(i).gameObject;
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
}
