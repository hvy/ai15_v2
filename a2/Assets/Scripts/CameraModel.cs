using UnityEngine;
using System.Collections;

public class CameraModel : MonoBehaviour {

	private static Camera mainCamera = Camera.main;

	public static void updatePosition (float stageWidth, float stageHeight) {

		Vector3 newCameraPosition = new Vector3 (
				stageWidth / 2.0f,
				Mathf.Sqrt (stageWidth * stageHeight) * 1.8f,
				stageHeight / 2.0f
			);

		mainCamera.transform.position = newCameraPosition;
	}
}

