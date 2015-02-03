using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DifferentialController : KinematicController
{
	public float rotationSpeed;

	private float angularThreshold;

	protected Quaternion lookRotation;

	void Start() {
		angularThreshold = 0.03f;
	}

	// Implements interface member
	public void findPath() {
		path = StageManager.aStarPath;
		
		goal.x = path[path.Count-2].getPos ().x;
		goal.z = path[path.Count-2].getPos ().y;
	}
	
	// Implements interface member
	override public void stepPath() 
	{
		rotate ();

		// Check if the rotation is finished	
		if (Math.Abs(transform.rotation.y - lookRotation.y) < angularThreshold) {
			float distance = Vector3.Distance (goal, transform.position);
			
			if (distance < 2.2f) {
				steps++;
				goal = Agent.recalculateGoal(steps);
			}
			
			if (goal.x == -1f) {
				return;
			}

			// Move forward if rotation is finished
			move ();
		}
	}
	
	// Implements interface member
	public void reset(Vector3 position) {
		path = null;
		steps = 0;
		rigidbody.transform.position = position;
	}

	protected void rotate() {
		Vector3 direction = (goal - transform.position).normalized;
		lookRotation = Quaternion.LookRotation(direction);

		// Spherical interpolation
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
	}
}