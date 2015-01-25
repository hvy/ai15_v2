using UnityEngine;
using System.Collections;

public class StageGenerator : MonoBehaviour {

	private GameObject stage;

	struct Wall {
		Vector2 u, v;
		GameObject wall;

		public Wall(float fromX, float fromY, float toX, float toY) {
			u = new Vector2(fromX, fromY);
			v = new Vector2(toX, toY);
			wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
			wall.transform.position = new Vector3(fromX, 0, fromY);
		}
	};

	// TODO
	public void createDiscreteStage() {

		int pixelSize = 20;
		int width = 20;
		int height = 20;

		Debug.Log ("Creating discrete stage...");

		// Create the stage
		stage = GameObject.CreatePrimitive (PrimitiveType.Cube);
		float actualWidth = width * pixelSize;
		float actualHeight = height * pixelSize;
		stage.transform.position = new Vector3 (actualWidth / 2, 0, actualHeight / 2);
		stage.transform.localScale = new Vector3 (actualWidth, 1, actualHeight);

		// Create the walls
		// TODO
		/*
		 1     0     1     0     1     1     0     1     0     0     0     0     0     1     0     0     0     0     0     0
	     1     0     0     1     1     1     0     0     0     1     0     1     0     0     1     0     1     1     0     0
	     0     0     0     0     0     0     0     1     0     0     1     1     0     0     1     0     0     1     1     0
	     0     1     0     0     0     0     1     0     0     0     1     0     0     0     1     1     0     1     0     0
	     1     0     0     1     1     0     0     1     1     1     0     0     0     0     0     1     1     0     0     0
	     0     0     1     0     0     0     0     1     0     0     0     0     0     0     0     1     0     1     0     0
	     1     0     0     0     1     0     0     0     0     1     0     0     0     0     0     0     0     1     1     0
	     0     0     0     0     0     0     0     0     1     0     1     1     0     0     1     0     1     0     0     0
	     1     0     0     0     0     0     1     1     0     1     0     1     0     0     0     1     0     0     0     1
	     0     1     0     0     0     0     0     0     0     1     1     0     0     0     0     0     0     0     0     0
	     0     0     0     1     1     1     0     1     1     0     0     0     1     1     0     1     0     1     0     0
	     1     0     0     1     0     0     1     0     0     0     1     0     1     0     0     1     1     0     1     0
	     0     0     1     0     0     0     0     0     1     1     0     0     0     1     0     0     1     0     0     1
	     0     1     0     0     0     1     1     0     0     0     1     0     1     0     0     0     0     0     0     0
	     1     0     0     0     0     0     0     0     0     1     1     0     1     0     1     0     0     0     0     0
	     0     0     0     0     0     0     0     0     0     0     0     0     0     1     0     1     0     0     0     0
	     0     0     0     1     0     0     0     1     0     1     0     0     1     1     0     0     1     0     0     0
	     1     0     0     0     1     1     0     0     0     1     0     0     0     0     0     0     0     0     1     1
	     0     1     0     0     1     0     0     0     0     0     0     0     0     1     1     0     0     0     0     1
	     0     0     1     1     1     0     0     0     0     0     1     0     1     1     0     0     0     0     1     0
	     */

		// Testing 
		bool[,] walkable = new bool[2, 2] {
			{false, true},
			{true, false}
		};

		for (int i = 0; i < walkable.GetLength(0); i++) {
			for (int j = 0; j < walkable.GetLength(1); j++) {
				if (!walkable[i, j]) { 
					GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
					wall.name = "" + i + j;
					wall.transform.localScale = new Vector3(pixelSize, 0.0f, pixelSize);
					wall.transform.position = new Vector3((i * pixelSize) + (pixelSize / 2),  10.0f, (j * pixelSize) + (pixelSize / 2));
				}
			}		
		}
	}

	// TODO
	public void createContinuousStage() {

		Debug.Log ("Creating continuous stage...");

		// Create the stage
		
		// Create the walls
	}
}
