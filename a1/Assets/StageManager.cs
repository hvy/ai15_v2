using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour{

	public GameObject stage;

	// Discrete stage
	public int numBoxes;
	public Transform box;

	// Continuous stage

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

	// Discrete stage
	public void createDiscreteStage() {
		float boxSize = box.transform.localScale.x;
		float halfBoxSize = boxSize / 2.0f;
		
		for (int i = 0; i < numBoxes; i++) {
			int xBoxNum = Random.Range(0, 20);			
			int yBoxNum = Random.Range(0, 20);			
			float x = xBoxNum * boxSize + halfBoxSize;			
			float y = yBoxNum * boxSize + halfBoxSize;
			Transform boxTransform = GameObject.Instantiate(box, new Vector3(x, 0, y), Quaternion.identity) as Transform;
			boxTransform.parent = stage.transform;
		}
	}
}
