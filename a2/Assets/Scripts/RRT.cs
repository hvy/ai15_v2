using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RRT
{

    public Tree tree{ get; private set; }

    public List<GNode> graph { get; private set; }

    public Vector3[] bounds{ get; set; }
    
    private Vector3 rootPos;
    private Vector3 destinationPos;
    private Tuple<GNode, GNode> startGoal;
    private List<Vector2[]> polygons;
    private float goalThreshold;
    private float stepSize;
    private float pointCloseToLineThreshold;
    private float acceptableAngleBetweenNodes;
    private float pathDistanceToCorner;

    
    // initialize the tree
    //TODO denna ska ta parametrar som förändrar beteendet av RRTn, typ bias osv.
    // TODO ta in en lista med alla linjer som definierar obstacles också.
    public RRT (Vector3 start, Vector3 goal, Vector3[] bounds, List<Vector2[]> polygons, float goalThreshold, float stepSize, float pointCloseToLine, float acceptableAngleBetweenNodes, float pathDistanceToCorner)
    {
        rootPos = start;
        destinationPos = goal;
        this.bounds = bounds;
        this.polygons = polygons;
        this.goalThreshold = goalThreshold;
        this.graph = new List<GNode> ();
        this.stepSize = stepSize;
        this.pointCloseToLineThreshold = pointCloseToLine;
        this.acceptableAngleBetweenNodes = acceptableAngleBetweenNodes;
        this.pathDistanceToCorner = pathDistanceToCorner;
        startGoal = new Tuple<GNode, GNode> ();
    }

    public void buildRRT (int desiredNodes)
    {
        tree = new Tree (new TNode (0, null, rootPos));
            
        int counter = 0;
        for (int i = 0; i < desiredNodes;) {
            TNode rand = getRandomNode ();
            //rand = new TNode (0, null, new Vector3 (90f, 0, 80f));
            TNode closestNode = tree.findClose (rand.getPos ());
            counter++;
                
            if (counter > 500000) {
                Debug.Log ("Out of counter...");
                break;
            }
                
            if (isInObstacle (rand))
                continue;
                
            if (!hasPathBetween (rand, closestNode))
                continue;

            if (!acceptableAngleBetween (closestNode, rand))
                continue;

            Vector3 newPos = closestNode.getPos () + (rand.getPos () - closestNode.getPos ()) * stepSize;
                
            // TODO ändra så att den inkrementerar i steg typ, så den jobbar sig framåt sakta och inte hoppar hejvilt
            // dvs ha en limit på Distance mellan closest och random node
            rand.setPosition (newPos);
            rand.parent = closestNode;
            tree.addNode (rand);
            i++;

            // reached the goal
            if (Vector3.Distance (rand.getPos (), destinationPos) < goalThreshold) {

                // add goal node
                TNode last = new TNode (counter + 1, rand, destinationPos);
                tree.goal = last;
                tree.addNode (last);
                break;
            }
        }

            
    }

    public List<GNode> findPath ()
    {

        List<GNode> graphNodes = new List<GNode> ();
        foreach (TNode node in tree.nodeList) {
                
        }
            
        return graphNodes;
    }

    public Tuple<GNode, GNode> generateGraph ()
    {
        startGoal.first = generateNode (tree.root);

        return startGoal;
    }
        
    private GNode generateNode (TNode root)
    {

        List<GNode> neighbors = new List<GNode> ();
        foreach (TNode child in root.children)
            neighbors.Add (generateNode (child));
        GNode gnode = new GNode (root.getId (), root.getPos (), neighbors);
        graph.Add (gnode);
        if (gnode.getPos () == tree.goal.getPos ()) {
            startGoal.second = gnode;
        }
        return gnode;
    }

    private bool acceptableAngleBetween (TNode a, TNode b)
    {
        if (a == null || a.parent == null)
            return true;
        Vector3 firstVector = a.parent.getPos()-a.getPos ();
        Vector3 secondVector = a.getPos ()-b.getPos();

        float angle = Vector3.Angle(firstVector, secondVector);

        if (angle > acceptableAngleBetweenNodes)
            return false;
        return true;

    }
    
    private bool hasPathBetween (TNode a, TNode b)
    {

        foreach (Vector2[] vertices in polygons) {
            for (int i = 0; i < vertices.Length; i++) {
                            
                Vector3 start2 = new Vector3 (vertices [i].x, 0.0f, vertices [i].y);
                Vector3 end2 = Vector3.zero;

                if (i == vertices.Length - 1)
                    end2 = new Vector3 (vertices [0].x, 0.0f, vertices [0].y);
                else
                    end2 = new Vector3 (vertices [i + 1].x, 0.0f, vertices [i + 1].y);

                                
                if (PathFinding.intersection (a.getPos (), b.getPos (), start2, end2)) {
                    return false;
                }

                if (DistancePointLine(start2, a.getPos(), b.getPos ()) < pathDistanceToCorner || DistancePointLine(end2, a.getPos(), b.getPos ()) < pathDistanceToCorner) {
                    return false;
                }
                                
            }       
            
        }

        return true;
    }

    private bool isInObstacle (TNode a)
    {
        Vector2 point = new Vector2 (a.getPos ().x, a.getPos ().z);
        foreach (Vector2[] vertices in polygons) {
            if (containsPoint (vertices, point))
                return true;
        }

        return false;

    }

    private bool containsPoint (Vector2[] polyPoints, Vector2 p)
    { 
        int j = polyPoints.Length - 1; 
        bool inside = false; 
        for (int i = 0; i < polyPoints.Length; j = i++) { 
            if (((polyPoints [i].y <= p.y && p.y < polyPoints [j].y) || (polyPoints [j].y <= p.y && p.y < polyPoints [i].y)) && 
                (p.x < (polyPoints [j].x - polyPoints [i].x) * (p.y - polyPoints [i].y) / (polyPoints [j].y - polyPoints [i].y) + polyPoints [i].x)) 
                inside = !inside; 

            if (DistancePointLine (p, polyPoints [i], polyPoints [(i + 1) % polyPoints.Length]) < pointCloseToLineThreshold) {
                return true;
            }
        } 
        return inside; 
    }

    private float DistancePointLine (Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        return Vector3.Magnitude (ProjectPointLine (point, lineStart, lineEnd) - point);
    }

    private Vector3 ProjectPointLine (Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 rhs = point - lineStart;
        Vector3 vector2 = lineEnd - lineStart;
        float magnitude = vector2.magnitude;
        Vector3 lhs = vector2;
        if (magnitude > 1E-06f) {
            lhs = (Vector3)(lhs / magnitude);
        }
        float num2 = Mathf.Clamp (Vector3.Dot (lhs, rhs), 0f, magnitude);
        return (lineStart + ((Vector3)(lhs * num2)));
    }
    
    int randomCounter = 0;

    private TNode getRandomNode ()
    {
        TNode random;
            
        // TODO use bounds, and goal bias (RRT*) and such
        //float x = UnityEngine.Random.Range (0f, 100f);
        //float z = UnityEngine.Random.Range (0f, 100f);
        float x = UnityEngine.Random.Range (0f + GameObject.FindWithTag ("Agent").transform.localScale.x * 2, GameManager.width - GameObject.FindWithTag ("Agent").transform.localScale.x * 2);
        float z = UnityEngine.Random.Range (0f + GameObject.FindWithTag ("Agent").transform.localScale.x * 2, GameManager.height - GameObject.FindWithTag ("Agent").transform.localScale.x * 2);
                        
        random = new TNode (randomCounter++, null, new Vector3 (x, 0.0f, z));
        return random;
    }

}
