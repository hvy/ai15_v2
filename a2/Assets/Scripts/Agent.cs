using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour
{
		public Vector3 start, goal; // TODO Make sure start and goal is assignmed by the game manager
	
		private List<MovementModel> models;
		private List<List<GNode>> paths = new List<List<GNode>>();
		private List<GNode> currentPath;
		private List<Vector3> obstacles;
		private List<Agent> collisionAgents = new List<Agent>();
		
		public int type = 1;
		public bool isRunning = false;
		public bool isFinished = false;
		public bool paused = false;
		public bool overridePause = false;


		private float startTime;
		private int currentPathIndex = 0;
		private int steps;
		private bool hasPrintedTime = false;
		
		public int tick = 0;

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
			models.Add (GetComponent<KinematicController> ());
			obstacles = new List<Vector3>();
		
		}

		private void executeStep ()
		{
				float distance = Vector3.Distance (goal, transform.position);
		
				if (distance < 0.1f) {
						steps++;
				}
			
			if (currentPath == null) {
				GameManager.agentPos[transform.position].isFinished = true;
				GameManager.obstacles.Add (transform.position);
				tick = 1000;
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
						GameManager.obstacles.Add (transform.position);
						//tick = 10000;
						return;
				}
		
		}

	private bool recalculatePath() {

		List<GNode> newPath = null;

		if (GameManager.agentPos[goal].paused || GameManager.agentPos[goal].isFinished) {
			obstacles.Add (goal);
			obstacles.AddRange(GameManager.obstacles);
			Debug.Log("Recalculate path: " + GameManager.obstacles.Count);
			newPath = PathPlanner.recalculatePath(this, currentPath[0].getPos (), obstacles);	// recalculate path
		}

//		if () {
//			obstacles.Add (goal);
//			Debug.Log("Recalculate path because of agent paused");
//			obstacles.AddRange(GameManager.obstacles);
//			newPath = PathPlanner.recalculatePath(this, currentPath[0].getPos (), obstacles);	// recalculate path
//
//		}

		if (newPath != null) {
			return true;
		}
		return false;
	}
	
	void FixedUpdate ()
	{
		if (isValidType (type) && goal.x != -1f && !isFinished) {
					
			if (GameManager.agentPos.ContainsKey (goal) && GameManager.agentPos [goal] != this) {
								if (!recalculatePath() && this.tick < GameManager.agentPos [goal].tick) {
									paused = true;
									return; // Pause
								}
						}
						paused = false;
						if (models [type].stepPath (this, goal)) {
							executeStep ();
							overridePause = false;
						}
						else {
							if (!overridePause) {
//								Debug.Log ("reverse!");
//								collisionAgents[0].overridePause = true;
//								models [type].reverse(goal);
							}
							
						}
							

				}

	
				if (isFinished && !hasPrintedTime) {
						Debug.Log ("Total time: " + (Time.time - startTime));
						Debug.Log ("Total ticks: " + tick);
						hasPrintedTime = true;
						tick = 1000;
						//isFinished = false;
            
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

				Vector3 goal;
				List<GNode> path = currentPath;

				if (path == null)
					return transform.position;
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

	public void setCollisionAgents(List<Agent> col) {
		collisionAgents = col;
	}

}