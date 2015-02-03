using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CarKinematicController : DifferentialController
{
	void rotate ()
	{
		Vector3 direction = (goal - transform.position).normalized;
		lookRotation = Quaternion.LookRotation (direction);

		// spherical interpolation
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * rotationSpeed * speed);
	}

	// Implements interface member
	override public void stepPath() 
	{
		float distance = Vector3.Distance (goal, transform.position);
		
		if (distance < 3.2f) {
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
		// TODO, tror nog detta är lite fuskigt. Bör ta hänsyn till styrvinkel och längd, etc.
		transform.position += transform.forward * Time.deltaTime * speed;
		rotate ();
	}
}
