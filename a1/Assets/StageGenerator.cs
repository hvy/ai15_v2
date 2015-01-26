using UnityEngine;
using System.Collections;

public class StageGenerator : MonoBehaviour {

	private GameObject stage;
	private Vector2 startPos, endPos;

	private int totWidth = 100;
	private int totHeight = 100;
	private Color startColor = Color.red;
	private Color endColor = Color.green;

	// Not used at the moment. Might use for Continuous stage
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

	public void createEmptyStage() {

		clearStage ();

		// Create the stage
		stage = GameObject.CreatePrimitive (PrimitiveType.Cube);
		stage.transform.position = new Vector3 (totWidth / 2, 0, totHeight / 2);
		stage.transform.localScale = new Vector3 (totWidth, 1, totHeight);

		// Create the start and the end points
		GameObject start = GameObject.CreatePrimitive (PrimitiveType.Cube);
		start.transform.localScale = new Vector3 (5, 0, 5);
		start.transform.position = new Vector3 (10, 10, 10);
		start.renderer.material.color = startColor;
		start.transform.parent = stage.transform;

		GameObject end = GameObject.CreatePrimitive (PrimitiveType.Cube);
		end.transform.localScale = new Vector3 (5, 0, 5);
		end.transform.position = new Vector3 (90, 10, 90);
		end.renderer.material.color = endColor;
		end.transform.parent = stage.transform;
	}

	public void createDiscreteStage() {

		clearStage ();

		startPos = new Vector2 (1, 1);
		endPos = new Vector2 (19, 19);

		int numTilessWidth = 20;
		int numTilesHeight = 20;
		int tileWidth = totWidth / numTilessWidth;
		int tileHeight = totHeight / numTilesHeight;

		// Create the stage
		stage = GameObject.CreatePrimitive (PrimitiveType.Cube);
		//float actualWidth = numTilessWidth * tileWidth;
		//float actualHeight = numTilesHeight * tileHeight;
		//stage.transform.position = new Vector3 (actualWidth / 2, 0, actualHeight / 2);
		//stage.transform.localScale = new Vector3 (actualWidth, 1, actualHeight);

		stage.transform.position = new Vector3 (totWidth / 2, 0, totHeight / 2);
		stage.transform.localScale = new Vector3 (totWidth, 1, totHeight);

		// Create the walls
		bool[,] walkable = new bool[20, 20] {
			{false,true,false,true,false,false,true,false,true,true,true,true,true,false,true,true,true,true,true,true},
			{false,true,true,false,false,false,true,true,true,false,true,false,true,true,false,true,false,false,true,true},
			{true,true,true,true,true,true,true,false,true,true,false,false,true,true,false,true,true,false,false,true},
			{true,false,true,true,true,true,false,true,true,true,false,true,true,true,false,false,true,false,true,true},
			{false,true,true,false,false,true,true,false,false,false,true,true,true,true,true,false,false,true,true,true},
			{true,true,false,true,true,true,true,false,true,true,true,true,true,true,true,false,true,false,true,true},
			{false,true,true,true,false,true,true,true,true,false,true,true,true,true,true,true,true,false,false,true},
			{true,true,true,true,true,true,true,true,false,true,false,false,true,true,false,true,false,true,true,true},
			{false,true,true,true,true,true,false,false,true,false,true,false,true,true,true,false,true,true,true,false},
			{true,false,true,true,true,true,true,true,true,false,false,true,true,true,true,true,true,true,true,true},
			{true,true,true,false,false,false,true,false,false,true,true,true,false,false,true,false,true,false,true,true},
			{false,true,true,false,true,true,false,true,true,true,false,true,false,true,true,false,false,true,false,true},
			{true,true,false,true,true,true,true,true,false,false,true,true,true,false,true,true,false,true,true,false},
			{true,false,true,true,true,false,false,true,true,true,false,true,false,true,true,true,true,true,true,true},
			{false,true,true,true,true,true,true,true,true,false,false,true,false,true,false,true,true,true,true,true},
			{true,true,true,true,true,true,true,true,true,true,true,true,true,false,true,false,true,true,true,true},
			{true,true,true,false,true,true,true,false,true,false,true,true,false,false,true,true,false,true,true,true},
			{false,true,true,true,false,false,true,true,true,false,true,true,true,true,true,true,true,true,false,false},
			{true,false,true,true,false,true,true,true,true,true,true,true,true,false,false,true,true,true,true,false},
			{true,true,false,false,false,true,true,true,true,true,false,true,false,false,true,true,true,true,false,true}
		};

		for (int i = 0; i < walkable.GetLength(0); i++) {
			for (int j = 0; j < walkable.GetLength(1); j++) {
				if (i == startPos.x && j == startPos.y) {

					// Create the start point
					GameObject start = GameObject.CreatePrimitive(PrimitiveType.Cube);
					start.transform.localScale = new Vector3(tileWidth / 2, 0.0f, tileHeight / 2);
					start.transform.position = new Vector3((i * tileWidth) + (tileWidth / 2),  10.0f, (j * tileHeight) + (tileHeight / 2));
					start.renderer.material.color = startColor;
					start.transform.parent = stage.transform;

				} else if (i == endPos.x && j == endPos.y) {

					// Create the end point
					GameObject end = GameObject.CreatePrimitive(PrimitiveType.Cube);
					end.transform.localScale = new Vector3(tileWidth / 2, 0.0f, tileHeight / 2);
					end.transform.position = new Vector3((i * tileWidth) + (tileWidth / 2),  10.0f, (j * tileHeight) + (tileHeight / 2));
					end.renderer.material.color = endColor;
					end.transform.parent = stage.transform;

				} else if (!walkable[i, j]) { 

					// Create obstacle
					GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
					wall.transform.localScale = new Vector3(tileWidth, 0.0f, tileHeight);
					wall.transform.position = new Vector3((i * tileWidth) + (tileWidth / 2),  10.0f, (j * tileHeight) + (tileHeight / 2));
					wall.transform.parent = stage.transform;
				}
			}		
		}
	}

	// TODO
	public void createContinuousStage() {

		clearStage ();
		
		startPos = new Vector2 (20, 10);
		endPos = new Vector2 (80, 90);

		// Create the stage
		
		// Create the walls
	}

	private void clearStage() {
		Object.Destroy (stage);
	}
}
