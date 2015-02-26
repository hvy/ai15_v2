﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour
{
		public Vector3 start, goal; // TODO Make sure start and goal is assignmed by the game manager
	
		private List<MovementModel> models;
		private List<GNode> currentPath;
		private List<Vector3> obstacles;
		public int type = 1;
		public bool isRunning = false;
		public bool isFinished = false;
		private float startTime;
		private int steps;
		public bool paused = false;

		void Start ()
		{
				models = new List<MovementModel> ();
				models.Add (GetComponent<DiscreteController> ());
//				models.Add (GetComponent<DynamicController> ());
				models.Add (GetComponent<KinematicController> ());
		obstacles = new List<Vector3>();
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
		obstacles = new List<Vector3>();
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

	private bool recalculatePath() {

		if (GameManager.agentPos[goal].paused) {
			obstacles.Add (goal);
			Debug.Log("Recalculate path because of agent paused");
			PathPlanner.recalculatePath(this, currentPath[0].getPos (), obstacles);	// recalculate path
		}

		if (GameManager.customerPos.ContainsKey(goal) && goal != currentPath[0].getPos ()) {
			Debug.Log("Recalculate path: " + GameManager.obstacles.Count);
			Debug.Log("Distance to goal: " + currentPath.Count);
			List<GNode> newPath = PathPlanner.recalculatePath(this, currentPath[0].getPos (), GameManager.obstacles);	// recalculate path

			if (newPath == null) {
				return false;
			}
			return true;
			
		}
		return false;
	}
	
	void FixedUpdate ()
	{
		if (isValidType (type) && goal.x != -1f) {
					
						if (GameManager.agentPos.ContainsKey (goal) && GameManager.agentPos [goal] != this) {
								if (!recalculatePath()) {
									paused = true;
									return; // Pause
								}
						}
						paused = false;
						models [type].stepPath (goal);	
						executeStep ();

				}

	
				if (isFinished) {
						//Debug.Log ("Total time: " + (Time.time - startTime));
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

						//Debug.Log ("Using model: " + type);
				}
		}

		public void setPath (List<GNode> path)
		{
				currentPath = path;
				steps = 0;
		}
	
		public Vector3 recalculateGoal (int counter)
		{

				Vector3 goal;
				List<GNode> path = currentPath;
				if (path.Count - counter - 1 < 0) {
						GameManager.obstacles.Add (path[0].getPos());
						return new Vector3 (-1f, -1f, -1f);
				}
				
				goal = path [path.Count - counter - 1].getPos ();
				
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