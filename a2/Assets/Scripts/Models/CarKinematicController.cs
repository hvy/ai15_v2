using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CarKinematicController : KinematicController, MovementModel {

	public float maxPhi;

	private bool reverse = false;
	private float reverseCrossThreshold = 0.75f;
	
	void rotate (Vector3 goal)
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

		reverse = Math.Abs (cross.y) > reverseCrossThreshold ? true : false;

		bool reverseToGoal = false;
		if (Vector3.Dot(direction, transform.forward) < -0.85) {
			reverse = true; // goal is behind the car
			reverseToGoal = true;
		}

		phi = Mathf.Abs(phi) > maxPhi ? Mathf.Sign(phi) * maxPhi : phi; // steering angle
		phi = reverseToGoal ? -phi : phi;
		float theta = ((velocity / transform.localScale.z) * Mathf.Tan (phi)); // moving angle

		transform.RotateAround (pivot.position, Vector3.up, theta * Mathf.Rad2Deg * Time.deltaTime); // backwheels as pivot
	}

	// Implements interface member
	override public bool stepPath(Agent agent, Vector3 goal) {

		float distance = Vector3.Distance (goal, transform.position);

		if (Vector3.Distance (transform.position, goal) > 0.8f) {
			move (goal);
		}

		return true;
	}

    public void reset(Vector3 position) {
        rigidbody.transform.position = position;
    }

	void move (Vector3 goal) {

		float distance = Vector3.Distance (goal, transform.position);

		transform.position += reverse ? transform.forward * Time.deltaTime * -velocity : transform.forward * Time.deltaTime * velocity;

		rotate (goal);
	}
}
