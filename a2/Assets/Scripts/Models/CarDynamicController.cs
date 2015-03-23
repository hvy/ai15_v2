using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CarDynamicController : DynamicController, MovementModel {

	public float maxPhi;

	//private const float max_velocity = 100.0f;
	private bool reverse = false;
	private bool keepSteady = true;
	private float reverseCrossThreshold = 0.75f;

	private Vector3 destination;

	private float previousDistance = 1000000.0f;
	private float initialDistance;
	private Vector3 goal;
	private float velocity = 0;
	
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

	void rotate (Vector3 goal) {
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



	void move (Vector3 goal) {

	//            Vector3 force = goal - rigidbody.position; // allow for slow down
	//            float acc = force.magnitude;
	//            if (acc > maxA) {
	//                    acc = maxA;
	//            }
	      float acc = maxA * Time.deltaTime;
	     
	      float distance = Vector3.Distance (rigidbody.position, goal);

	      if (distance < 0.5f && goal == destination)
	              return;

	      if (initialDistance / distance > 2.0 || reverse) {
	              if (initialDistance / distance > 2.0 && reverse)
	                      velocity += acc;
	              else
	                      velocity -= acc;
	      } else {
	                      velocity += acc;
	      }


	      float stoppingDistance = Time.deltaTime * (velocity * velocity) / (2 * acc);
	      //Debug.Log ("distance to goal " + Vector3.Distance (transform.position, destination));
	      //Debug.Log ("stop dist: " + stoppingDistance);

	      // TODO change to destination instead of goal to keep velocity at waypoints
	      if (Vector3.Distance (transform.position, goal) <= stoppingDistance) {
	              if (reverse)
	                      velocity += 2*acc;
	              else
	                      velocity -= 2*acc;
	      }

	//Debug.Log (velocity);

	      // TODO handle last goal, decrease velocity more or something, use force maybe, I don't know.
	      transform.position += transform.forward * Time.deltaTime * velocity;


	      if ((keepSteady && reverse && velocity < 0.0f) || (keepSteady && !reverse && velocity > 0.0f)) {
	              keepSteady = false;
	      }


	      if (!keepSteady)
	              rotate (goal);
	      previousDistance = distance;
	}
}