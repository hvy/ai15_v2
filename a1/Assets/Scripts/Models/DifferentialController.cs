using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DifferentialController : KinematicController
{
	public float Wmax; // rotation speed in rad/sec

	private float angularThreshold;

	protected Quaternion lookRotation;

	void Start() {
		angularThreshold = 0.03f;
	}

	// Implements interface member
	public void findPath() {
		path = GraphBuilder.aStarPath;
		
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

//	Vector3 motion;
//	Vector3 rotation;

	protected void rotate() {
		// NEW CODE
//		float v = 1;
//		float w = 1f;
//
//		float theta = Mathf.Deg2Rad*transform.eulerAngles.y;
//		motion.z=motion.z+v*Mathf.Cos(theta);
//		motion.x=motion.x+v*Mathf.Sin(theta);
//		rotation.y=rotation.y+w*Mathf.Rad2Deg;
//
//		rigidbody.MovePosition (rigidbody.position + motion);
//		rigidbody.MoveRotation (rigidbody.rotation * Quaternion.Euler (rotation));
//
//		motion = Vector3.zero;
//		rotation = Vector3.zero;

		
		Vector3 direction = (goal - transform.position).normalized;
		lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Wmax*Mathf.Rad2Deg*Time.deltaTime);

	}
}