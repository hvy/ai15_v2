using UnityEngine;
using System.Collections;

public class StageGenerator : MonoBehaviour {

	private Vector2 start, end;
	private int totWidth = 100;
	private int totHeight = 100;

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
	
	public void createDiscreteStage() {

		start = new Vector2 (1, 1);
		end = new Vector2 (19, 19);

		int numTilessWidth = 20;
		int numTilesHeight = 20;
		int tileWidth = totWidth / numTilessWidth;
		int tileHeight = totHeight / numTilesHeight;

		// Create the stage
		stage = GameObject.CreatePrimitive (PrimitiveType.Cube);
		float actualWidth = numTilessWidth * tileWidth;
		float actualHeight = numTilesHeight * tileHeight;
		stage.transform.position = new Vector3 (actualWidth / 2, 0, actualHeight / 2);
		stage.transform.localScale = new Vector3 (actualWidth, 1, actualHeight);

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
				if (i == start.x && j == start.y) {
					GameObject waypoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
					waypoint.name = "start";
					waypoint.transform.localScale = new Vector3(tileWidth / 2, 0.0f, tileHeight / 2);
					waypoint.transform.position = new Vector3((i * tileWidth) + (tileWidth / 2),  10.0f, (j * tileHeight) + (tileHeight / 2));
					waypoint.renderer.material.color = Color.blue;
				} else if (i == end.x && j == end.y) {
					GameObject waypoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
					waypoint.name = "end";
					waypoint.transform.localScale = new Vector3(tileWidth / 2, 0.0f, tileHeight / 2);
					waypoint.transform.position = new Vector3((i * tileWidth) + (tileWidth / 2),  10.0f, (j * tileHeight) + (tileHeight / 2));
					waypoint.renderer.material.color = Color.green;
				} else if (!walkable[i, j]) { 
					GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
					wall.name = "" + i + j;
					wall.transform.localScale = new Vector3(tileWidth, 0.0f, tileHeight);
					wall.transform.position = new Vector3((i * tileWidth) + (tileWidth / 2),  10.0f, (j * tileHeight) + (tileHeight / 2));
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
