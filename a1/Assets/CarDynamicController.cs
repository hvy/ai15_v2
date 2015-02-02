using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CarDynamicController : DynamicController
{

		private Quaternion wheelDir;
		private float L;
		private const float max_wheel_turn = 1.5f;
		private const float max_velocity = 200.0f;

		// Use this for initialization
		void Start ()
		{
				L = transform.localScale.z;
				wheelDir = Quaternion.LookRotation (transform.forward);
				acceleration = 0;
		}
	
		void rotate ()
		{
				Vector3 direction = (goal - transform.position).normalized;
				wheelDir = Quaternion.LookRotation (direction);
		
				// spherical interpolation
				transform.rotation = Quaternion.Slerp (transform.rotation, wheelDir, Time.deltaTime * max_wheel_turn);
		}
	
		void move ()
		{
				float distance = Vector3.Distance (goal, transform.position);
				if (distance < 0.8f)
						return;
				acceleration += 0.08f;
				if (acceleration > max_acceleration) {
					acceleration = max_acceleration;
				}
				float ad = acceleration * distance;
				if (ad > max_velocity)
						ad = max_velocity;
				rigidbody.MovePosition (rigidbody.position + power * (transform.forward / rigidbody.mass) * Time.deltaTime * ad);
				rotate ();

		}

		public void restart() {
		if (StageManager.aStarPath != null) {
			List<GNode> path = StageManager.aStarPath;
			Debug.Log ("Changed goal and start");
			goal.x = path[path.Count-2].getTransform ().position.x;
			goal.z = path[path.Count-2].getTransform ().position.y;
			Debug.Log (goal.x);
			Debug.Log (goal.z);
		}

			acceleration = 0;
		}
	
		// Update is called once per frame
	int counter = 0;
		void FixedUpdate ()
		{
				if (Model.type == 5) {
						// TODO check if has reached waypoint. If so, update and assign new goal.
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
