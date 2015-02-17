using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PathFinding
{

	public static List<GNode> currentPath {get;set;}
	

		public class Path : IEnumerable
		{
				public GNode LastStep { get; private set; }
				public Path PreviousSteps { get; private set; }
				public double TotalCost { get; private set; }
				public int Length { get; private set; }
				private Path (GNode lastStep, Path previousSteps, double totalCost, int length)
				{
						LastStep = lastStep;
						PreviousSteps = previousSteps;
						TotalCost = totalCost;
						Length = length;
				}
				public Path (GNode start) : this(start, null, 0, 0)
				{
				}
				public Path AddStep (GNode step, double stepCost)
				{

						return new Path (step, this, TotalCost + stepCost, Length + 1);
				}
				public IEnumerator GetEnumerator ()
				{
						for (Path p = this; p != null; p = p.PreviousSteps)
								yield return p.LastStep;
				}
				IEnumerator IEnumerable.GetEnumerator ()
				{
						return this.GetEnumerator ();
				}
		}

		public class PriorityQueue<P, V>
		{
				private SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>> ();
				public void Enqueue (P priority, V value)
				{
						Queue<V> q;
						if (!list.TryGetValue (priority, out q)) {
								q = new Queue<V> ();
								list.Add (priority, q);
						}
						q.Enqueue (value);
				}
				public V Dequeue ()
				{
						var pair = list.First ();
						var v = pair.Value.Dequeue ();
						if (pair.Value.Count == 0)
								list.Remove (pair.Key);
						return v;
				}
				public bool IsEmpty {
						get { return !list.Any (); }
				}
		}

		static public double calculateDistance (List<GNode> completedPath)
		{
				double distance = 0;
				for (int i = 0; i < completedPath.Count-1; i++) {
						distance += Vector3.Distance (completedPath [i].getPos (), completedPath [i + 1].getPos ());
				}
				return distance;
		}

		// A-STAR!!!!!!!!!!!!!!!!!!
		// TODO write estimate function. Just make it the distance from the point to the goal.
		static public List<GNode> aStarPath (
		GNode start, 
		GNode goal, 
		Func<GNode, GNode, double> distance)
		{
				HashSet<GNode> closed = new HashSet<GNode> (); // closed set
				PriorityQueue<double, Path> open = new PriorityQueue<double, Path> (); // open set

				open.Enqueue (0, new Path (start));
				while (!open.IsEmpty) {
						Path path = open.Dequeue ();
						if (closed.Contains (path.LastStep))
								continue;

						if (path.LastStep.Equals (goal)) {
								List<GNode> completePath = new List<GNode> ();
								foreach (GNode node in path)
										completePath.Add (node);
								currentPath = completePath;
								return completePath;
						}
						closed.Add (path.LastStep);
						foreach (GNode n in path.LastStep.getNeighbors()) {
								double d = distance (path.LastStep, n);
								Path newPath = path.AddStep (n, d);
				open.Enqueue (newPath.TotalCost + Vector2.Distance (n.getPos (), goal.getPos ()), newPath);
						}
				}
				return null;
		}

	public static void RRT(List<Vector2[]> polygons) {

		Vector3 start = GameManager.start;
		Vector3 goal = GameManager.goal;
		
		Vector3[] bounds = new Vector3[4];
        // PARAMETERS:
        // start, goal, RRT bounds, polygons, close to goal, step size, node min distance to object, 
        // max acceptable angle between nodes, min path distance to object corner

        float acceptableWidth;
        float minAngle;
        if (Agent.type == 0 || Agent.type == 1 || Agent.type == 1) {
            acceptableWidth = 0.0f;
            minAngle = 90f;
        }
        else {
            acceptableWidth = Math.Max(GameObject.FindWithTag ("Agent").transform.localScale.x * 2, GameObject.FindWithTag ("Agent").transform.localScale.y * 2);
            minAngle = 32f;
        }
       
        RRT rrt = new RRT (start, goal, bounds, polygons, 4.0f, 0.4f, acceptableWidth, minAngle, acceptableWidth);

        // Demo small angular threshold
		//RRT rrt = new RRT (start, goal, bounds, polygons, 10.0f, 0.2f, 0.0f, 25f, 2.5f);
		rrt.buildRRT (10000);
		rrt.tree.draw ();
		
		Tuple<GNode, GNode> startGoal = rrt.generateGraph();
		Debug.Log (startGoal.second.getPos ().z);
		List<GNode> path = PathFinding.aStarPath(startGoal.first, startGoal.second, GraphBuilder.distance);		
		currentPath = path;

		draw (path);
	}

	// TODO, ta bort waypoints som inte behöver vara där, alltså ha en raksträcka istället för en kurvig jävel.
	public static void optimizeCurrentPath(List<Vector2[]> polygons) {

		for (int i = 0; i < currentPath.Count ; i++) {
			for (int j = i+2; j < currentPath.Count ; j++) {
				if (hasPathBetween(currentPath[i], currentPath[j], polygons)) {
					currentPath.RemoveAt(j-1);
				}

			}
		}

	}

	// take two lines (end points) and determine if they intersect
	public static bool intersection (Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
	{
		
		Vector3 a = p2 - p1;
		Vector3 b = p3 - p4;
		Vector3 c = p1 - p3;
		
		float alphaNumerator = b.z * c.x - b.x * c.z;
		float alphaDenominator = a.z * b.x - a.x * b.z;
		float betaNumerator = a.x * c.z - a.z * c.x;
		float betaDenominator = alphaDenominator;
		
		bool doIntersect = true;
		
		if (alphaDenominator == 0 || betaDenominator == 0) {
			doIntersect = false;
		} else {
			
			if (alphaDenominator > 0) {
				if (alphaNumerator < 0 || alphaNumerator > alphaDenominator) {
					doIntersect = false;
				}
			} else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator) {
				doIntersect = false;
			}
			
			if (doIntersect && betaDenominator > 0) {
				if (betaNumerator < 0 || betaNumerator > betaDenominator) {
					doIntersect = false;
				}
			} else if (betaNumerator > 0 || betaNumerator < betaDenominator) {
				doIntersect = false;
			}
		}
		
		return doIntersect;
		
	}

	private static bool hasPathBetween (GNode a, GNode b, List<Vector2[]> polygons)
	{
		
		foreach (Vector2[] vertices in polygons) {
			for (int i = 0; i < vertices.Length; i++) {
				
				Vector3 start2 = new Vector3 (vertices [i].x, 0.0f, vertices [i].y);
				Vector3 end2 = Vector3.zero;
				//Debug.Log ("hejsan: " + vertices [i].x);
				if (i == vertices.Length - 1)
					end2 = new Vector3 (vertices [0].x, 0.0f, vertices [0].y);
				else
					end2 = new Vector3 (vertices [i + 1].x, 0.0f, vertices [i + 1].y);
				
				
				if (intersection (a.getPos (), b.getPos (), start2, end2)) {
					return false;
				}
				
			}		
			
		}
		
		return true;
	}

		public static void draw (List<GNode> p)
		{
				GameObject camera = GameObject.FindGameObjectWithTag ("MainCamera");
				Renderer renderer;
		
				if (!(renderer = camera.GetComponent<Renderer> ()))
						renderer = camera.AddComponent<Renderer> ();
				renderer.path = p;
		}

}