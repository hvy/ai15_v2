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

		// Use this for initialization
		void Start ()
		{
				L = transform.localScale.z;
				wheelDir = Quaternion.LookRotation (transform.forward);
				acceleration = 0.1f;
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

		public void restart() {
			if (StageManager.aStarPath != null) {
				List<GNode> path = StageManager.aStarPath;
				goal.x = path[path.Count-2].getPos ().x;
				goal.z = path[path.Count-2].getPos ().y;
			}

			acceleration = 0.1f;
			counter = 0;
		}
	
		// Update is called once per frame
		void FixedUpdate ()
		{
				if (Model.type == 5) {
						// TODO check if has reached waypoint. If so, update and assign new goal.
						float distance = Vector3.Distance (goal, transform.position);
						
						if (distance < 2.5f) {
							counter++;
							goal = Model.recalculateGoal(counter);
							acceleration = 0.5f;
						}

						if (goal.x == -1f)
							return;
						move ();
				}
		}

}
