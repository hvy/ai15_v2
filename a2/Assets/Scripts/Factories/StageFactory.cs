using UnityEngine;
using System.Collections;

public class StageFactory : MonoBehaviour {

	public StageFactory () {

	}

	public GameObject createStage(float width, float height) {

		Debug.Log ("Creating stage W: " + width + " H: " + height);

		GameObject stage = GameObject.CreatePrimitive (PrimitiveType.Cube);
		stage.transform.position = new Vector3 (width / 2.0f, -0.5f, height / 2.0f);
		stage.transform.localScale = new Vector3 (width, -1.0f, height);
		stage.name = "Stage";
		return stage;
	}
}
