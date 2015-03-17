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
		angularThreshold = 0.02f;
	}
	
	// Implements interface member
	override public bool stepPath(Agent agent, Vector3 goal) {

		// Rotate only if the agent hasn't reached the goal
		if (Vector3.Distance (transform.position, goal) > 0.5f) {
			rotate (goal);		

			// Check if the rotation is finished	
			if (Math.Abs(transform.rotation.y - lookRotation.y) < angularThreshold) {
				
				float distance = Vector3.Distance (goal, transform.position);
				
				// Move forward if rotation is finished
				move (goal);
			}
		}

		return true;
	}
	
	protected void rotate(Vector3 goal) {
		Vector3 direction = (goal - transform.position).normalized;
		lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Wmax * Mathf.Rad2Deg * Time.deltaTime);
	}

	protected void move (Vector3 goal) {
		float distance = Vector3.Distance (rigidbody.position, goal);
		//rigidbody.transform.Translate (transform.forward * velocity * Time.deltaTime);
		//rigidbody.transform.position = (Vector3.Lerp (rigidbody.transform.position, goal, velocity * Time.deltaTime / distance));
		rigidbody.transform.position = (Vector3.Lerp (rigidbody.transform.position, rigidbody.transform.position + (rigidbody.transform.forward * distance), velocity * Time.deltaTime / distance));
	}
}