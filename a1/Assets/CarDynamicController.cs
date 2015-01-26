using UnityEngine;
using System.Collections;
using System;

public class CarDynamicController : DynamicController
{

		//Vector3 goal = new Vector3 (60.0f, 0.0f, 50.0f);
		Vector3 start;
		private Quaternion wheelDir;
		private float L;
		private const float max_wheel_turn = 0.4f;
		private const float max_velocity = 200.0f;
		
		// Use this for initialization
		void Start ()
		{
				L = transform.localScale.z;
				wheelDir = Quaternion.LookRotation (transform.forward);
				start = transform.position;
				acceleration = 0;
		}
	
		void rotate ()
		{
				Vector3 direction = (goal - transform.position).normalized;
				wheelDir = Quaternion.LookRotation (direction);
		
				// spherical interpolation
				transform.rotation = Quaternion.Slerp (transform.rotation, wheelDir, Time.deltaTime * power * max_wheel_turn);
		}
	
		void move ()
		{
				float distance = Vector3.Distance (goal, transform.position);
				if (distance < 0.8f)
						return;
				acceleration += 0.04f;
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
			acceleration = 0;
		}
	
		// Update is called once per frame
		void FixedUpdate ()
		{
				if (Model.type == 5) {
						move ();
				}
		}

}
