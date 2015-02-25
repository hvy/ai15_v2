using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour
{
		public Vector3 start, goal; // TODO Make sure start and goal is assignmed by the game manager
	
		private List<MovementModel> models;
		private List<GNode> currentPath;
		public int type = 1;
		public bool isRunning = false;
		public bool isFinished = false;
		private float startTime;
		private int steps;

		void Start ()
		{
				models = new List<MovementModel> ();
				models.Add (GetComponent<DiscreteController> ());
//				models.Add (GetComponent<DynamicController> ());
				models.Add (GetComponent<KinematicController> ());
//				models.Add (GetComponent<DifferentialController> ());
//				models.Add (GetComponent<CarDynamicController> ());
//				models.Add (GetComponent<CarKinematicController> ());

		}

		public void init ()
		{
				models = new List<MovementModel> ();
				models.Add (GetComponent<DiscreteController> ());
//		models.Add (GetComponent<DynamicController> ());
				models.Add (GetComponent<KinematicController> ());
//		models.Add (GetComponent<DifferentialController> ());
//		models.Add (GetComponent<CarDynamicController> ());
//		models.Add (GetComponent<CarKinematicController> ());
		
		}

		private void executeStep ()
		{
				float distance = Vector3.Distance (goal, transform.position);
		
				if (distance < 0.1f) {
						steps++;
				}
				goal = recalculateGoal (steps);
		
				if (!isRunning)
						return;
		
				if (goal.x == -1f) {
						isRunning = false;
						isFinished = true;
						return;
				}
		
		}
	
		void FixedUpdate ()
		{

		if (isValidType (type) && goal.x != -1f) {
			models [type].stepPath (goal);	
						executeStep ();

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

		public void setPath (List<GNode> path)
		{
				currentPath = path;
		}
	
		public Vector3 recalculateGoal (int counter)
		{

				Vector3 goal;
				List<GNode> path = currentPath;
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

		public void setStart (Vector3 s)
		{
				start = s;
		}

		public void setGoal (Vector3 g)
		{
				goal = g;
		}

}