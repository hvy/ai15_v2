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

	public void createSimpleStage() {

		clearStage ();
		createEmptyStage ();

		setStart (new Vector2 (10, 10));
		setEnd (new Vector2 (90, 90));
	}

	public void createDiscreteStage() {

		clearStage ();
		createEmptyStage ();

		Vector2 startTile = new Vector2 (1, 1);
		Vector2 endTile = new Vector2 (19, 19);

		int numTilessWidth = 20;
		int numTilesHeight = 20;
		int tileWidth = totWidth / numTilessWidth;
		int tileHeight = totHeight / numTilesHeight;

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
				if (i == startTile.x && j == startTile.y) {

					// Set start
					setStart (new Vector2((i * tileWidth) + (tileWidth / 2),(j * tileHeight) + (tileHeight / 2)));

				} else if (i == endTile.x && j == endTile.y) {

					// Set end
					setEnd (new Vector2((i * tileWidth) + (tileWidth / 2), (j * tileHeight) + (tileHeight / 2)));

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
		createEmptyStage ();

		setStart (new Vector2 (20, 10));
		setEnd (new Vector2 (80, 90));

		// Create the walls
		stage.AddComponent("MeshFilter");
		stage.AddComponent("MeshRenderer");
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.vertices = new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0)};
		mesh.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)};
		mesh.triangles = new int[] {0, 1, 2};
	}

	private void createEmptyStage() {
		stage = GameObject.CreatePrimitive (PrimitiveType.Cube);
		stage.transform.position = new Vector3 (totWidth / 2, 0, totHeight / 2);
		stage.transform.localScale = new Vector3 (totWidth, 1, totHeight);
	}

	private void setStart(Vector2 pos) {

		startPos = pos;

		GameObject start = GameObject.CreatePrimitive (PrimitiveType.Cube);
		start.transform.localScale = new Vector3 (2.5f, 0, 2.5f);
		start.transform.position = new Vector3 (pos.x, 10.0f, pos.y);
		start.renderer.material.color = startColor;
		start.transform.parent = stage.transform;
	}

	private void setEnd(Vector2 pos) {

		endPos = pos;

		GameObject end = GameObject.CreatePrimitive (PrimitiveType.Cube);
		end.transform.localScale = new Vector3 (2.5f, 0, 2.5f);
		end.transform.position = new Vector3 (pos.x, 10.0f, pos.y);
		end.renderer.material.color = endColor;
		end.transform.parent = stage.transform;
	}

	private void clearStage() {
		Object.Destroy (stage);
	}
}
