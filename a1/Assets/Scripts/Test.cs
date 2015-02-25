using UnityEngine;
using System.Collections;

// Used for debugging modules
public class Test : MonoBehaviour {
	
	void Start () {

		// Test stage creation
		StageFactory stageFactory = new StageFactory ();
		float width = 100f;
		float height = 100f;
		stageFactory.createStage (width, height);	
	}

	void Update () {
	
	}
}
