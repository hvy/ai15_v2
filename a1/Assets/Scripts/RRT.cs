using UnityEngine;
using System.Collections;

public class RRT
{

		public Tree tree{ get; private set; }
		public Vector3[] bounds{ get; set; }
	
		private TNode root;

	
		// initialize the tree
		//TODO denna ska ta parametrar som förändrar beteendet av RRTn, typ bias osv.
		public RRT (TNode start, Vector3[] bounds)
		{
				root = start;
				this.bounds = bounds;
		}

		private bool hasPathBetween (TNode a, TNode b)
		{
				bool hit = Physics.Raycast (a.getPos (), b.getPos (), Vector3.Distance (a.getPos (), b.getPos ()));
				return !hit;
		}

		private bool isInObstacle (TNode a)
		{
				Vector3 up = a.getPos ();
				up.y = 5.0f;
				bool hit = Physics.Raycast (a.getPos (), up, Vector3.Distance (a.getPos (), up));
				return hit;
		}
	
	
		public void buildRRT (int desiredNodes)
		{
				tree = new Tree (new TNode (0, null, new Vector3 (28f, 1.0f, 28f))); // for testing

				for (int i = 0; i < desiredNodes;) {
						TNode rand = getRandomNode ();
						TNode closestNode = tree.findClose (rand.getPos ());
						rand.parent = closestNode;

						if (isInObstacle (rand))
								continue;
						
						if (!hasPathBetween (rand, closestNode)) // denna verkar inte vara perfekt alltså?
								continue;
			
						// TODO ändra så att den inkrementerar i steg typ, så den jobbar sig framåt sakta och inte hoppar hejvilt
						// dvs ha en limit på Distance mellan closest och random node

						tree.addNode (rand);
						i++;
				}
	
		}

		private TNode getRandomNode ()
		{
				TNode random;
			
				// TODO use bounds, and goal bias (RRT*) and such
				float x = UnityEngine.Random.Range (0f, 100f);
				float z = UnityEngine.Random.Range (0f, 100f);
						
				random = new TNode (0, null, new Vector3 (x, 1.0f, z));

				return random;
		}

}
