using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PathFinding {
	

	public class Path : IEnumerable
	{
		public GNode LastStep { get; private set; }
		public Path PreviousSteps { get; private set; }
		public double TotalCost { get; private set; }
		public int Length { get; private set; }
		private Path(GNode lastStep, Path previousSteps, double totalCost, int length)
		{
			LastStep = lastStep;
			PreviousSteps = previousSteps;
			TotalCost = totalCost;
			Length = length;
		}
		public Path(GNode start) : this(start, null, 0, 0) {}
		public Path AddStep(GNode step, double stepCost)
		{

			return new Path(step, this, TotalCost + stepCost, Length+1);
		}
		public IEnumerator GetEnumerator()
		{
			for (Path p = this; p != null; p = p.PreviousSteps)
				yield return p.LastStep;
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}

	public class PriorityQueue<P, V>
	{
		private SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>>();
		public void Enqueue(P priority, V value)
		{
			Queue<V> q;
			if (!list.TryGetValue(priority, out q))
			{
				q = new Queue<V>();
				list.Add(priority, q);
			}
			q.Enqueue(value);
		}
		public V Dequeue()
		{
			var pair = list.First();
			var v = pair.Value.Dequeue();
			if (pair.Value.Count == 0)
				list.Remove(pair.Key);
			return v;
		}
		public bool IsEmpty
		{
			get { return !list.Any(); }
		}
	}

	static public double calculateDistance(List<GNode> completedPath) {
		double distance = 0;
		for (int i = 0; i < completedPath.Count-1; i++) {
			distance += Vector3.Distance(completedPath[i].getPos(), completedPath[i+1].getPos());
		}
		return distance;
	}

	// A-STAR!!!!!!!!!!!!!!!!!!
	// TODO write estimate function. Just make it the distance from the point to the goal.
	static public List<GNode> aStarPath(
		GNode start, 
		GNode goal, 
		Func<GNode, GNode, double> distance, 
		Func<GNode, double> estimate)
	{
		HashSet<GNode> closed = new HashSet<GNode>(); // closed set
		PriorityQueue<double, Path> open = new PriorityQueue<double, Path>(); // open set

		open.Enqueue(0, new Path(start));
		while (!open.IsEmpty)
		{
			Path path = open.Dequeue();
			if (closed.Contains(path.LastStep))
				continue;

			if (path.LastStep.Equals(goal)) {
				List<GNode> completePath = new List<GNode>();
				foreach (GNode node in path)
					completePath.Add (node);
				return completePath;
			}
			closed.Add(path.LastStep);
			foreach(GNode n in path.LastStep.getNeighbors())
			{
				double d = distance(path.LastStep, n);
				Path newPath = path.AddStep(n, d);
				open.Enqueue(newPath.TotalCost + estimate(n), newPath);
			}
		}
		return null;
	}

}