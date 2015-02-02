using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DifferentialController : KinematicController {


	// for testing
	//Vector3 goal = new Vector3 (60.0f, 0.0f, 50.0f);
	public float rotationSpeed;
	private float angularThreshold;

	protected Quaternion lookRotation;

	// Use this for initialization
	void Start () {
		angularThreshold = 0.01f;
	}

	protected void rotate() {
		Vector3 direction = (goal - transform.position).normalized;
		lookRotation = Quaternion.LookRotation(direction);

		// spherical interpolation
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

	}

	public void restart() {
		if (StageManager.aStarPath != null) {
			List<GNode> path = StageManager.aStarPath;
			goal.x = path[path.Count-2].getTransform ().position.x;
			goal.z = path[path.Count-2].getTransform ().position.y;
		}
		
	}
	
	// Update is called once per frame
	int counter = 0;
	void Update () {
		if (Model.type == 3) {
			rotate ();
			if (Math.Abs(transform.rotation.y - lookRotation.y) < angularThreshold) {
				float distance = Vector3.Distance (goal, transform.position);
				
				if (distance < 3.2f) {
					counter++;
					goal = Model.recalculateGoal(counter);
				}
				
				if (goal.x == -1f)
					return;
				move ();
			}
		}
	}
}
