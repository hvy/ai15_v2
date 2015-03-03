﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour
{
		public Vector3 start, goal;
	
		private List<MovementModel> models;
		private List<List<GNode>> paths = new List<List<GNode>>();
		public List<GNode> currentPath;
		
		public int type = 1;
		public bool isRunning = false;
		public bool isFinished = false;
		public bool paused = false;
		public bool overridePause = false;


		private float startTime;
		private int currentPathIndex = 0;
		private int steps;
		private bool hasPrintedTime = false;
		
		public int tick = -1;

		void Start ()
		{
				models = new List<MovementModel> ();
				models.Add (GetComponent<DiscreteController> ());
				models.Add (GetComponent<KinematicController> ());
				models.Add (GetComponent<DynamicController> ());
//				models.Add (GetComponent<DifferentialController> ());
//				models.Add (GetComponent<CarDynamicController> ());
//				models.Add (GetComponent<CarKinematicController> ());

		}


		private void executeStep ()
		{
			float distance = Vector3.Distance (goal, transform.position);
		
			if (distance < 0.1f) {
					steps++;
			}
			
			if (currentPath == null) {
				return;
			}
			
			if (currentPath.Count - steps - 1 < 0 && paths != null && currentPathIndex < paths.Count - 1) {
				currentPathIndex++;
				currentPath = paths[currentPathIndex];
				steps = 0;
			}

			goal = recalculateGoal (steps);
		
			if (!isRunning)
					return;
	
			if (goal.x == -1f) {
					isRunning = false;
					isFinished = true;
					GameState.Instance.obstacles.Add (transform.position);
					//tick = 10000;
					return;
			}
		
		}


	
	void FixedUpdate () {
		if (isValidType (type) && goal.x != -1f && !isFinished) {
			if (models [type].stepPath (this, goal)) {
				paused = false;
				executeStep ();
				overridePause = false;
			} else {
				paused = true;
			}
		}

		if (isFinished && !hasPrintedTime) {
			Debug.Log ("Total time: " + (Time.time - startTime) + "  Total ticks: " + tick);
			hasPrintedTime = true;
			tick = 1000;
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
							}
		}

		public void setPath (List<GNode> path)
		{
				currentPath = path;
				//paths[0] = path;
				steps = 0;
		}

		public void addPath(List<GNode> path) {				
			paths.Add (path);
			currentPath = paths[0];
			steps = 0;
		}

		public void removePaths() {
			paths  = new List<List<GNode>>();
			currentPath = null;
		}

		public Vector3 recalculateGoal (int counter)
		{

				Vector3 pos;
				List<GNode> path = currentPath;

				if (path == null)
					return transform.position;
				if (path.Count - counter - 1 < 0) {
						GameState.Instance.obstacles.Add (path[0].getPos());
						return new Vector3 (-1f, -1f, -1f);
				}
				
				pos = path [path.Count - counter - 1].getPos ();
				
				return pos;
		}

		public void updatePath(List<GNode> pathSegment, int step) {
			step = currentPath.Count - step - 1;
			currentPath.RemoveAt(step);
			foreach (GNode node in pathSegment)
				currentPath.Insert(step-1,node);
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