using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleFactory : MonoBehaviour {

	public static float obstacleHeight = 3.0f;
	public static ObstacleFactory OF;

	void Awake () {
		if (OF != null) {
			GameObject.Destroy (OF);		
		} else {
			OF = this;				
		}

		DontDestroyOnLoad (this);
	}

	public static GameObject createDiscreteObstacle(Vector3 pos) 
	{
		GameObject prefab = Resources.Load ("Prefabs/Box", typeof(GameObject)) as GameObject;
		GameObject obstacle = Instantiate (prefab, new Vector3(1f, 0f, 1f), Quaternion.identity) as GameObject;
	
		obstacle.transform.localScale = new Vector3(1f, 0.1f, 1f);

		
		obstacle.AddComponent<BoxCollider> ();
		obstacle.name = "DiscreteObstacle";
		obstacle.transform.position = pos;
		
		return obstacle;
	}


	// The vertices need to be ordered clockwise
	public static GameObject createPolygonalObstacle(Vector2[] vertices) 
	{
		if (vertices.Length < 3) {
			return null;
		}

		GameObject obstacle = new GameObject ();
		obstacle.AddComponent<MeshFilter> ();
		obstacle.AddComponent<MeshRenderer> ();

		List<Vector3> meshVertices = new List<Vector3> ();
		List<int> meshTriangles = new List<int> ();
		
		// Compute the vertices and the triandles and add them to the argument lists
		computeMeshComponents(vertices, meshVertices, meshTriangles);

		Mesh mesh = obstacle.GetComponent<MeshFilter> ().mesh;
		mesh.vertices = meshVertices.ToArray();
		mesh.triangles = meshTriangles.ToArray();
		mesh.RecalculateNormals ();

		obstacle.AddComponent<MeshCollider> (); // Make sure collision is enabled
		obstacle.name = "PolygonalObstacle";

		//Material obstacleMaterial = (Material)Resources.Load("Materials/White", typeof(Material));

		obstacle.renderer.receiveShadows = false;
		obstacle.renderer.castShadows = false;

		return obstacle;
	}
	
	static void computeMeshComponents (Vector2[] vertices, List<Vector3> meshVertices, List<int> meshTriangles)
	{
		addVerticesWithHeight (vertices, meshVertices, obstacleHeight);
		addVerticesWithHeight (vertices, meshVertices, 0.0f);

		// Add the triangles to all faces except the bottom
		if (vertices.Length == 3) {
		
			// Top face
			meshTriangles.Add (0);
			meshTriangles.Add (1);
			meshTriangles.Add (2);

			// Side faces
			for (int i = 0; i < 3; i++) {
					meshTriangles.Add (i);
					meshTriangles.Add (i + 3);
					meshTriangles.Add (i + 1);
			}

			// Side faces
			for (int i = 5; i > 2; i--) {
					meshTriangles.Add (i);
					meshTriangles.Add (i - 3);
					meshTriangles.Add (i - 1);
			}

		} else if (vertices.Length == 4) {

			// Top face
			meshTriangles.Add (0);
			meshTriangles.Add (1);
			meshTriangles.Add (2);
			meshTriangles.Add (2);
			meshTriangles.Add (3);
			meshTriangles.Add (0);

			// Side faces
			for (int i = 0; i < 4; i++) {
				meshTriangles.Add (i);
				meshTriangles.Add (i + 4);
				meshTriangles.Add (i + 1);
			}
			
			// Side faces
			for (int i = 7; i > 3; i--) {
				meshTriangles.Add (i);
				meshTriangles.Add (i - 4);
				meshTriangles.Add (i - 1);
			}

		}
	}

	static void addVerticesWithHeight (Vector2[] vertices, List<Vector3> meshVertices, float height)
	{
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 vertex = toVector3(vertices[i], height);
			meshVertices.Add(vertex);
		}
	}

	private static Vector3 toVector3(Vector2 v, float y) {
		return new Vector3 (v.x, y, v.y);
	}
}
