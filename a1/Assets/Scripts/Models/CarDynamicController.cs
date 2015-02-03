using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CarDynamicController : DynamicController
{
	private Quaternion wheelDir;
	private float L;
	private const float max_wheel_turn = 2.0f;
	private const float max_velocity = 100.0f;
	private float acceleration;
	public float power;

	// Use this for initialization
	void Start ()
	{
		L = transform.localScale.z;
		wheelDir = Quaternion.LookRotation (transform.forward);
		acceleration = 0.1f;
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
		// TODO check if has reached waypoint. If so, update and assign new goal.
		float distance = Vector3.Distance (goal, transform.position);
		
		if (distance < 2.5f) {
			steps++;
			goal = Agent.recalculateGoal(steps);
			acceleration = 0.5f;
		}
		
		if (goal.x == -1f)
			return;
		move ();
	}
	
	// Implements interface member
	public void reset(Vector3 position) {
		path = null;
		acceleration = 0.1f;
		steps = 0;
		rigidbody.transform.position = position;
	}

	void rotate ()
	{
		Vector3 direction = (goal - transform.position).normalized;
		wheelDir = Quaternion.LookRotation (direction);

		// spherical interpolation
		// TOOD, let wheels rotate seperately and when calculate direction when starting to move.
		rigidbody.rotation = Quaternion.Slerp (transform.rotation, wheelDir, Time.deltaTime * max_wheel_turn);
	}

	void move ()
	{
		float distance = Vector3.Distance (goal, transform.position);
		if (distance < 0.8f)
				return;
		acceleration += 0.03f;
		if (acceleration > max_acceleration) {
			acceleration = max_acceleration;
		}
		float ad = acceleration * distance;
		if (ad > max_velocity)
				ad = max_velocity;
		rigidbody.MovePosition (rigidbody.position + power * (transform.forward / rigidbody.mass) * Time.deltaTime * ad);
		rotate ();

	}
}
