using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PolygonalLevelParser {

	private int width, height;
	private List<Vector2> starts, goals, customers;
	private List<List<Vector2>> polygons, triangles;

	public PolygonalLevelParser () {
		clearParser ();
	}

	public int getWidth () {
		return width;
	}

	public int getHeight () {
		return height;
	}

	public List<Vector2> getStarts () {
		return starts;
	}

	public List<Vector2> getGoals () {
		return goals;
	}

	public List<Vector2> getCustomers () {
		return customers;
	}

	public List<List<Vector2>> getPolygons () {
		return polygons;
	}

	public List<List<Vector2>> getTriangles () {
		return triangles;
	}

	public void parse(string fileName) {
		
		StreamReader sr = new StreamReader(Application.dataPath + "/Levels/" + fileName);
		
		string line;
		string[] splitLine;

		line = sr.ReadLine();
		splitLine = line.Split(' ');
		width = int.Parse(splitLine[0]);
		height = int.Parse(splitLine[1]);

		while ((line = sr.ReadLine ()) != null) {

			// Register agent with its start and goal
			if (line.Equals("New agent")) {
				float startX = float.Parse(sr.ReadLine ().Split (' ')[1]);
				float startY = float.Parse(sr.ReadLine ().Split (' ')[1]);
				float goalX = float.Parse(sr.ReadLine ().Split (' ')[1]);
				float goalY = float.Parse(sr.ReadLine ().Split (' ')[1]);
				starts.Add (new Vector2 (startX, startY));
				goals.Add (new Vector2 (goalX, goalY));
				sr.ReadLine (); // Skip the next line "End of agent"
			}


			// Register customer position
			if (line.Equals("New customer")) {
				float customerX = float.Parse(sr.ReadLine ().Split (' ')[1]);
				float customerY = float.Parse(sr.ReadLine ().Split (' ')[1]);
				customers.Add (new Vector2 (customerX, customerY));
				sr.ReadLine (); // Skip the next line "End of customer"
			}

			// Register polygonal obstacle
			if (line.Equals("New polygonal shape")) {
				List<Vector2> polygonVertices = new List<Vector2> ();
				while (true) {
					float x = float.Parse(sr.ReadLine ().Split (' ')[1]);
					float y = float.Parse(sr.ReadLine ().Split (' ')[1]);
					polygonVertices.Add(new Vector2(x, y));
					
					if (sr.Peek () == 69) {
						sr.ReadLine ();
						break;
					}
				}
				polygons.Add (polygonVertices);
			}
		}
		
		sr.Close ();
		
		triangulate ();
	}
	
	public void clearParser ()
	{
		this.width = 0;
		this.height = 0;
		this.starts = new List<Vector2> ();
		this.goals = new List<Vector2> ();
		this.customers = new List<Vector2> ();
		this.polygons = new List<List<Vector2>> ();
		this.triangles = new List<List<Vector2>> ();
	}
	
	// Triangulate the polygons using ear clipping
	void triangulate ()
	{
		for (int i = 0; i < polygons.Count; i++) {
			if (!isClockWise (polygons [i])) {
				polygons[i].Reverse ();	
			}
		}
		
		foreach (List<Vector2> polygonVertices in polygons) {
			// Triangulate the polygon and add them to the list of triangles.
			earClipping(polygonVertices);		
		}
	}
	
	private void earClipping (List<Vector2> polygonVertices) {
		
		Debug.Log ("NEW POLYGON");
		
		// Create a double linked cyclic list with all the polygon vertices
		TriangleList vertices = new TriangleList (polygonVertices);
		int numVertices = polygonVertices.Count;
		
		List<Vector2> reflexes = findReflexes (vertices.first, numVertices);
		List<TriangleListNode> ears = findEars (vertices.first, numVertices, reflexes);
		
		// DEBUG
		Debug.Log ("Number of reflexes: " + reflexes.Count);
		Debug.Log ("Number of ears: " + ears.Count);
		
		while (numVertices > 3) {
			TriangleListNode earToRemove = ears[0];
			ears.RemoveAt(0);
			
			Vector2 v0 = earToRemove.previous.vertex;
			Vector2 v1 = earToRemove.vertex;
			Vector2 v2 = earToRemove.next.vertex;
			
			List<Vector2> triangle = new List<Vector2> ();
			triangle.Add (v0);
			triangle.Add (v1);
			triangle.Add (v2);
			
			// Remove the ear from the linked list
			earToRemove.previous.next = earToRemove.next;
			earToRemove.next.previous = earToRemove.previous;
			
			// Add a triangle!
			if (!isClockWise(triangle)) {
				triangle.Reverse();
			}
			
			Debug.Log ("Adding triangle");
			for (int i = 0; i < triangle.Count; i++) {
				Debug.Log (triangle[i]);
			}
			
			triangles.Add (triangle);
			
			// One vertex is removed
			numVertices--;
			
			reflexes = findReflexes (earToRemove.next, numVertices);
			ears = findEars (earToRemove.next, numVertices, reflexes);
			
			
			// DEBUG
			Debug.Log ("=========================================");
			Debug.Log ("Number of reflexes: " + reflexes.Count);
			Debug.Log ("Number of ears: " + ears.Count);
			Debug.Log ("=========================================");
		}
		
		List<Vector2> lastTriangle = new List<Vector2> ();
		lastTriangle.Add (ears[0].vertex);
		lastTriangle.Add (ears[1].vertex);
		lastTriangle.Add (ears[2].vertex);
		
		if (!isClockWise(lastTriangle)) {
			lastTriangle.Reverse ();
		}
		
		triangles.Add (lastTriangle);
	}
	
	List<TriangleListNode> findEars (TriangleListNode node, int numNodes, List<Vector2> reflexes) {
		
		List<TriangleListNode> ears = new List<TriangleListNode> ();
		
		for (int i = 0; i < numNodes; i++) {
			
			// If it is a reflex, then it is not an ear
			if (reflexes.Contains (node.vertex)) {
				node = node.next;
				continue;
			}
			
			Vector2 v0 = node.previous.vertex;
			Vector2 v1 = node.vertex;
			Vector2 v2 = node.next.vertex;
			
			TriangleListNode testNode = node;
			bool hasVertexInsideTriangle = false;
			for (int j = 0; j < numNodes; j++) {
				if (isInsideTriangle(testNode.vertex, v0, v1, v2)) {
					hasVertexInsideTriangle = true; 					// This is not an ear
					continue;
				}
				testNode = testNode.next;
			}
			
			if (!hasVertexInsideTriangle) {
				ears.Add (node);
			}
			
			node = node.next;
		}
		
		return ears;
	}
	
	// Find reflexes given the first node and the number of nodes
	List<Vector2> findReflexes(TriangleListNode node, int numNodes) {
		
		List<Vector2> reflexes = new List<Vector2> ();
		
		for (int i = 0; i < numNodes; i++) {
			Vector2 v0 = node.previous.vertex;
			Vector2 v1 = node.vertex;
			Vector2 v2 = node.next.vertex;
			
			float angle = Vector2.Angle(v0 - v1, v2 - v1);
			Vector3 cross = Vector3.Cross(v0 - v1, v2 - v1);
			
			if (cross.z < 0) {
				angle = 360 - angle;
			}
			
			if (angle > 180) {
				reflexes.Add (v1);
			}
			
			node = node.next;
		}
		
		return reflexes;
	}
	
	// Help method for isInsideTriangle
	float sign (Vector2 v1, Vector2 v2, Vector2 v3) {
		return (v1.x - v3.x) * (v2.y - v3.y) - (v2.x - v3.x) * (v1.y - v3.y);
	}
	
	bool isInsideTriangle(Vector2 point, Vector2 v0, Vector2 v1, Vector3 v2) {
		
		bool b1, b2, b3;
		
		b1 = sign (point, v0, v1) < 0f;
		b2 = sign (point, v1, v2) < 0f;
		b3 = sign (point, v2, v0) < 0f;
		
		return (b1 == b2) && (b2 == b3);
	}
	
	bool isClockWise(List<Vector2> vertices) {
		
		float sum = 0;
		
		Vector2 v0, v1;
		
		for (int i = 0; i < vertices.Count - 1; i++) {
			v0 = vertices[i];		
			v1 = vertices[i + 1];
			
			sum += ((v1.x - v0.x) * (v1.y + v0.y));
		}
		
		v0 = vertices [vertices.Count - 1];
		v1 = vertices [0];
		sum += ((v1.x - v0.x) * (v1.y + v0.y));
		
		return sum >= 0.0;
	}
	
	// Cyclic linked list to keep track of the polygon vertices
	class TriangleList {
		
		public TriangleListNode first; // The previous node of the first node is the last node
		
		public TriangleList (List<Vector2> vertices) {
			
			first = new TriangleListNode (vertices[0]);
			TriangleListNode current = first;
			
			for (int i = 1; i < vertices.Count; i++) {
				TriangleListNode newNode = new TriangleListNode (vertices[i]);
				newNode.previous = current;
				current.next = newNode;
				current = newNode;
			}
			
			current.next = first;
			first.previous = current;
		}
		
		public void add (TriangleListNode node) {
			node.next = first;
			node.previous = first.previous;
			
			first.previous.next = node;
			first.previous = node;
		}
	}
	
	class TriangleListNode {
		
		public Vector2 vertex;
		public TriangleListNode previous, next;
		
		public TriangleListNode (Vector2 vertex) {
			this.vertex = vertex;
		}
	}
}
