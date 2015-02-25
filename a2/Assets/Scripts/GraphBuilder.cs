using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphBuilder
{

    //public static int numNeighbors = 16;
    //public static List<GNode> aStarPath {get;set;}
    private static GNode start, end;

    // Fetches the waypoint positions from the scene and generates a graph
    public static void buildGraphFromScene ()
    {
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag ("Waypoint");

        Dictionary<GameObject, GNode> nodes = new Dictionary<GameObject, GNode> ();

        for (int i = 0; i < waypointObjects.Length; i++) {
            List<GNode> neighbors = new List<GNode> ();     
            GameObject waypoitObject = waypointObjects [i];
            nodes [waypoitObject] = new GNode (i, waypoitObject.transform.position, neighbors);
        }

        // Ray length
        float obstacleLength = 5.0f; // hard coded
        float l = (float)System.Math.Sqrt (obstacleLength * obstacleLength * 2);

        Vector3[] rayDirections = new Vector3[16];
        rayDirections [0] = new Vector3 (1.0f, 0, 0);
        rayDirections [1] = new Vector3 (0, 0, 1.0f);
        rayDirections [2] = new Vector3 (-1.0f, 0, 0);
        rayDirections [3] = new Vector3 (0, 0, -1.0f);
        rayDirections [4] = new Vector3 (1.0f, 0, 1.0f);
        rayDirections [5] = new Vector3 (-1.0f, 0, 1.0f);
        rayDirections [6] = new Vector3 (-1.0f, 0, -1.0f);
        rayDirections [7] = new Vector3 (1.0f, 0, -1.0f);
        rayDirections [8] = new Vector3 (1.0f, 0, 2.0f);
        rayDirections [9] = new Vector3 (1.0f, 0, -2.0f);
        rayDirections [10] = new Vector3 (2.0f, 0, 1.0f);
        rayDirections [11] = new Vector3 (2.0f, 0, -1.0f);
        rayDirections [12] = new Vector3 (-1.0f, 0, 2.0f);
        rayDirections [13] = new Vector3 (-1.0f, 0, -2.0f);
        rayDirections [14] = new Vector3 (-2.0f, 0, 1.0f);
        rayDirections [15] = new Vector3 (-2.0f, 0, -1.0f);

        for (int i = 0; i < waypointObjects.Length; i++) {
            for (int j = 0; j < GameManager.discreteNeighbors; j++) {
                RaycastHit[] hits;

                if (GameManager.discreteNeighbors < 9)
                    hits = Physics.RaycastAll (waypointObjects [i].transform.position, rayDirections [j], l);
                else
                    hits = Physics.RaycastAll (waypointObjects [i].transform.position, rayDirections [j], 12f);

                int hitIdx = 0;
                                
                bool collision = false;
                GameObject obj = null;
                int collisionCount = 0; // 16 neighbors

                while (hitIdx < hits.Length) {
                    RaycastHit hit = hits [hitIdx];

                    if (GameManager.discreteNeighbors < 9) {
                        if (hit.collider.tag == "Waypoint") {
                            obj = hit.transform.gameObject;
                            nodes [waypointObjects [i]].addNeighbor (nodes [obj]);
                        }
                    } else {
                        if (hit.collider.tag == "Obstacle") {
                            collisionCount++;
                        }
                        
                        if (hit.collider.tag == "Waypoint") {
                            obj = hit.transform.gameObject;
                   
                        }
                    }

                    hitIdx++;
                }

                if (GameManager.discreteNeighbors > 8 && collisionCount <= 1 && obj != null) {
                    if (System.Math.Abs(rayDirections [j].x) >= 2 || System.Math.Abs(rayDirections [j].z) >= 2)
                    nodes [waypointObjects [i]].addNeighbor (nodes [obj]);
                }
            }
        }

//        // Find the start and the goal waypoints
//        for (int i = 0; i < waypointObjects.Length; i++) {
//            if (waypointObjects [i].transform.position.x == GameManager.start.x && 
//                waypointObjects [i].transform.position.z == GameManager.start.z) {
//                start = nodes [waypointObjects [i]];
//            } else if (waypointObjects [i].transform.position.x == GameManager.goal.x &&
//                waypointObjects [i].transform.position.z == GameManager.goal.z) {
//                end = nodes [waypointObjects [i]];
//            }       
//        }
//                
//        PathFinding.aStarPath (start, end, distance);
//        PathFinding.draw (PathFinding.currentPath);
    }
    
    public static double distance (GNode a, GNode b)
    {
        return Vector2.Distance (a.getPos (), b.getPos ());
    }
    
        
}
