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
			// will throw if there isn’t any first element!
			var pair = list.First();
			var v = pair.Value.Dequeue();
			if (pair.Value.Count == 0) // nothing left of the top priority.
				list.Remove(pair.Key);
			return v;
		}
		public bool IsEmpty
		{
			get { return !list.Any(); }
		}
	}

	static public List<GNode> FindPath(
		GNode start, 
		GNode destination, 
		Func<GNode, GNode, double> distance, 
		Func<GNode, double> estimate)
	{
		var closed = new HashSet<GNode>();
		var queue = new PriorityQueue<double, Path>();
		queue.Enqueue(0, new Path(start));
		while (!queue.IsEmpty)
		{
			Path path = queue.Dequeue();
			if (closed.Contains(path.LastStep))
				continue;
			if (path.LastStep.Equals(destination)) {
				List<GNode> completePath = new List<GNode>();
				foreach (GNode node in path)
					completePath.Add (node);
				return completePath;

			//	return path;
			}
			closed.Add(path.LastStep);
			foreach(GNode n in path.LastStep.getNeighbors())
			{
				double d = distance(path.LastStep, n);
				var newPath = path.AddStep(n, d);
				queue.Enqueue(newPath.TotalCost + estimate(n), newPath);
			}
		}
		return null;
	}

}