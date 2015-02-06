using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour
{
		public static Vector3 start, goal;
	
		private List<MovementModel> models;

		private int type = -1;

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
		}

		// Initiated from the GUI using the buttons
		public void setModel (int newType)
		{
				if (isValidType (newType)) {
						// Find the optimal path for the new model
						models [newType].reset (GameManager.start);
						models [newType].findPath ();

						type = newType;		

						Debug.Log ("Using model: " + type);
				}
		}
	
		public static Vector3 recalculateGoal (int counter)
		{

				Vector3 goal;
				List<GNode> path = GraphBuilder.aStarPath;
				if (path.Count - counter - 1 < 0)
						return new Vector3 (-1f, -1f, -1f);
				goal = path [path.Count - counter - 1].getPos ();
				Debug.Log ("NEW WAYPOINT: " + goal.x + " " + goal.z);
				return goal;
		}

		private bool isValidType (int type)
		{
				return type > -1 && type < models.Count;
		}
}