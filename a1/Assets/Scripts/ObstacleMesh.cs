using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleMesh : MonoBehaviour {

	private int numTopVertices;
	private float obstacleHeight, obstacleMaxEdge, boardWidth, boardHeight;
	private Mesh mesh;
	private List<Vector3> vertices;
	private List<int> triangles;

	void configure ()
	{
		obstacleHeight = 10.0f;
		obstacleMaxEdge = 70.0f;
		mesh = GetComponent<MeshFilter> ().mesh;
		boardWidth = GameObject.Find ("Ground").transform.localScale.x;
		boardHeight = GameObject.Find ("Ground").transform.localScale.z;
	}

	void recreate() 
	{
		vertices.Clear ();
		triangles.Clear ();
	}

	Vector3 getNextGroundVertex (Vector3 from)
	{
		throw new System.NotImplementedException ();
	}

	void Start () {

		vertices = new List<Vector3> ();
		triangles = new List<int> ();

		configure ();
		recreate ();

		Vector3 heightVector = new Vector3 (0, obstacleHeight, 0);

		vertices.Add (getRndGroundVertex ());
		vertices.Add (getSndRndGroundVertex (vertices[0], obstacleMaxEdge));
		vertices.Add (getTrdRndGroundVertex (vertices[0], vertices[1], obstacleMaxEdge));
		vertices.Add (vertices[0] + heightVector);
		vertices.Add (vertices[1] + heightVector);
		vertices.Add (vertices[2] + heightVector);

		triangles.Add (3);
		triangles.Add (4);
		triangles.Add (5);

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals ();

		Debug.Log ("Fst: " + vertices[3]);
		Debug.Log ("Snd: " + vertices[4]);
		Debug.Log ("Trd: " + vertices[5]);

		/*

		obstacleHeight = 10.0f;
		boardWidth = GameObject.Find ("Ground").transform.localScale.x;
		boardHeight = GameObject.Find ("Ground").transform.localScale.z;

		vertices = new List<Vector3> ();
		normals = new List<Vector3> ();
		triangles = new List<int> ();

		mesh = GetComponent<MeshFilter> ().mesh;

		// Randomize the shape of the mesh given the number of vertices
		Vector3 fstVertex = getRndVertex ();
		vertices.Add (fstVertex);
		Vector3 sndVertex = getRndVertex (fstVertex, 50.0f);
		vertices.Add (sndVertex);
		Vector3 trdVertex = getRndVertex (sndVertex, 50.0f);
		vertices.Add (trdVertex);

		Vector3 fstVertexBottom = fstVertex;
		fstVertexBottom.z -= obstacleHeight;
		Vector3 sndVertexBottom = sndVertex;
		sndVertexBottom.z -= obstacleHeight;
		Vector3 trdVertexBottom = trdVertex;
		sndVertexBottom.z -= obstacleHeight;

		vertices.Add (fstVertexBottom);
		vertices.Add (sndVertexBottom);
		vertices.Add (trdVertexBottom);

		triangles.Add (0);
		triangles.Add (1);
		triangles.Add (2);

		triangles.Add (3);
		triangles.Add (4);
		triangles.Add (5);

		triangles.Add (0);
		triangles.Add (3);
		triangles.Add (1);

		triangles.Add (1);
		triangles.Add (3);
		triangles.Add (4);

		triangles.Add (1);
		triangles.Add (4);
		triangles.Add (2);

		triangles.Add (2);
		triangles.Add (4);
		triangles.Add (5);

		triangles.Add (0);
		triangles.Add (2);
		triangles.Add (3);

		triangles.Add (2);
		triangles.Add (5);
		triangles.Add (3);


		//normals.Add (Vector3.Cross(sndVertex - fstVertex, trdVertex - fstVertex).normalized);
		//normals.Add (Vector3.Cross(fstVertex - sndVertex, trdVertex - sndVertex).normalized);
		//normals.Add (Vector3.Cross(fstVertex - trdVertex, sndVertex - trdVertex).normalized);

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		//mesh.normals = normals.ToArray();
		mesh.RecalculateNormals ();
		*/
	}

	private Vector3 getRndGroundVertex() {
		return new Vector3(random(0.0f, boardWidth), 0, random(0.0f, boardHeight));
	}

	private Vector3 getSndRndGroundVertex(Vector3 from, float maxDistance) {
		float maxX = from.x + maxDistance > boardWidth ? boardWidth : from.x + maxDistance;
		float maxZ = from.z + maxDistance > boardHeight ? boardHeight : from.z + maxDistance;
		return new Vector3(random(from.x, maxX), 0, random(from.z, maxZ));
	}

	private Vector3 getTrdRndGroundVertex(Vector3 fromFst, Vector3 fromSnd, float maxDistance) {
		float minZ = fromSnd.z - maxDistance < 0 ? 0.0f : fromSnd.z - maxDistance;
		float maxX = fromFst.x + maxDistance > boardWidth ? boardWidth : fromFst.x + maxDistance;
		return new Vector3(random(fromFst.x, maxX), 0, random(minZ, fromSnd.z));
	}

	private float random(float min, float max) {
		return UnityEngine.Random.Range (min, max);
	}
}
