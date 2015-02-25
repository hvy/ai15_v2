using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour
{
		public static Vector3 start, goal; // TODO Make sure start and goal is assignmed by the game manager
	
		private List<MovementModel> models;

		public static int type = -1;

        public static bool isRunning = false;
        public static bool isFinished = false;
        private float startTime;

		void Start ()
		{
				models = new List<MovementModel> ();
				models.Add (GetComponent<DiscreteController> ());
				models.Add (GetComponent<DynamicController> ());
				models.Add (GetComponent<KinematicController> ());
				models.Add (GetComponent<DifferentialController> ());
				models.Add (GetComponent<CarDynamicController> ());
				models.Add (GetComponent<CarKinematicController> ());

		}

		void FixedUpdate ()
		{

				if (isValidType (type)) {
						models [type].stepPath ();	
				}
        if (isFinished) {
            Debug.Log ("Total time: " + (Time.time - startTime));
            isFinished = false;
            
        }
    }
    
    // Initiated from the GUI using the buttons
		public void setModel (int newType)
		{
            
                if (isValidType (newType)) {
                        if (!isRunning && !isFinished) {
                            startTime = Time.time;
                            isRunning = true;
                         }

						// Find the optimal path for the new model
						models [newType].reset (start);
						models [newType].findPath ();

						type = newType;		

						Debug.Log ("Using model: " + type);
				}
		}
	
		public static Vector3 recalculateGoal (int counter)
		{

				Vector3 goal;
				List<GNode> path = PathFinding.currentPath;
				if (path.Count - counter - 1 < 0)
						return new Vector3 (-1f, -1f, -1f);
				goal = path [path.Count - counter - 1].getPos ();
				//Debug.Log ("NEW WAYPOINT: " + goal.x + " " + goal.z);
				return goal;
		}

		private bool isValidType (int type)
		{
				return type > -1 && type < models.Count;
		}
}