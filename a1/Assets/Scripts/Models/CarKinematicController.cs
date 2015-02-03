using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CarKinematicController : KinematicController
{

	public float maxPhi;


	void rotate ()
	{
		Vector3 rotation = Vector3.zero;
		Vector3 direction = (goal - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (direction);

		Transform pivot = transform.Find("Pivot");

		Vector3 cross = Vector3.Cross(-transform.forward, direction);
		
		float phi;
		if (cross.y < 0) { // turn right
			phi = Quaternion.Angle(transform.rotation, lookRotation) * Mathf.Deg2Rad;
		} else { // turn left
			phi = -Quaternion.Angle(transform.rotation, lookRotation) * Mathf.Deg2Rad;
		}

		if (Math.Abs (cross.y) < 0.05f) // to prevent flickering with the steering wheel
			return;

		//Debug.Log (cross.y);

		phi = Mathf.Abs(phi) > maxPhi ? Mathf.Sign(phi) * maxPhi : phi; // steering angle
		float theta = ((velocity / transform.localScale.z) * Mathf.Tan (phi)); // moving angle

		// TODO motsvarar detta rad/sec?. Vi kanske får skita i pivot point vid bakhjulen?
		transform.RotateAround (pivot.position, Vector3.up, theta * Mathf.Rad2Deg * Time.deltaTime); // backwheels as pivot

//		rotation.y = rotation.y + theta;
//		rigidbody.MoveRotation (rigidbody.rotation * Quaternion.Euler (rotation));
		
	}

	// Implements interface member
	override public void stepPath() 
	{
		float distance = Vector3.Distance (goal, transform.position);
		
		if (distance < 4.0f) {
			steps++;
			goal = Agent.recalculateGoal(steps);
		}
		
		if (goal.x == -1f)
			return;
		move ();

	}

	void move ()
	{
		float distance = Vector3.Distance (goal, transform.position);
		if (distance < 0.8f)
			return;

		transform.position += transform.forward * Time.deltaTime * velocity;


		rotate ();
	}
}
