using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RRT
{

		public Tree tree{ get; private set; }
		public Vector3[] bounds{ get; set; }
	
		private TNode root;
		private TNode destination;
		private List<Vector2[]> polygons;
		private float goalThreshold;

	
		// initialize the tree
		//TODO denna ska ta parametrar som förändrar beteendet av RRTn, typ bias osv.
		// TODO ta in en lista med alla linjer som definierar obstacles också.
	public RRT (TNode start, TNode goal, Vector3[] bounds, List<Vector2[]> polygons, float goalThreshold)
		{
				root = start;
				destination = goal;
				this.bounds = bounds;
				this.polygons = polygons;
				this.goalThreshold = goalThreshold;
		}

		public void buildRRT (int desiredNodes)
		{
			tree = new Tree (new TNode (0, null, new Vector3 (0f, 0.0f, 0f))); // for testing
			
			int counter = 0;
			for (int i = 0; i < desiredNodes;) {
				TNode rand = getRandomNode ();
				//rand = new TNode (0, null, new Vector3 (90f, 0, 80f));
				TNode closestNode = tree.findClose (rand.getPos ());
				counter++;
				
				if (counter > 10000)
					break;
				
				if (isInObstacle (rand))
					continue;
				
				//						bool intersection = hasPathBetween (rand, closestNode);
				if (!hasPathBetween (rand, closestNode)) // denna verkar inte vara perfekt alltså?
					continue;

				Vector3 newPos = closestNode.getPos () + (rand.getPos () - closestNode.getPos ()) * 0.1f;
				
				// TODO ändra så att den inkrementerar i steg typ, så den jobbar sig framåt sakta och inte hoppar hejvilt
				// dvs ha en limit på Distance mellan closest och random node
				rand.setPosition(newPos);
				rand.parent = closestNode;
				tree.addNode (rand);
				i++;

				// reached the goal
				if (Vector3.Distance (rand.getPos (), destination.getPos ()) < goalThreshold)
					break;
			}
			
		}


		// take two lines (end points) and determine if they intersect
		private bool intersection (Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
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
	
	
		private bool hasPathBetween (TNode a, TNode b)
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

		private bool isInObstacle (TNode a)
		{
			Vector2 point = new Vector2(a.getPos ().x, a.getPos().z);
			foreach (Vector2[] vertices in polygons) {
				if (containsPoint(vertices, point))
					return true;
			}

		return false;

		}

		private bool containsPoint (Vector2[] polyPoints, Vector2 p) { 
			int j = polyPoints.Length-1; 
			bool inside = false; 
			for (int i = 0; i < polyPoints.Length; j = i++) { 
				if ( ((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)) && 
				    (p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x)) 
					inside = !inside; 
			} 
			return inside; 
		}
	


		int randomCounter = 0;
		private TNode getRandomNode ()
		{
				TNode random;
			
				// TODO use bounds, and goal bias (RRT*) and such
				float x = UnityEngine.Random.Range (0f, 100f);
				float z = UnityEngine.Random.Range (0f, 100f);
						
				random = new TNode (randomCounter++, null, new Vector3 (x, 0.0f, z));
				return random;
		}

}
