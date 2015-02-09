using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CarDynamicController : DynamicController
{
	//private const float max_velocity = 100.0f;
	private float acceleration;
	private bool reverse = false;
	private bool keepSteady = true;
	private float reverseCrossThreshold = 0.75f;

	private Vector3 destination;

	public float maxPhi;

	private int steps_;

	// Use this for initialization
	void Start ()
	{
		acceleration = 0.1f;
		steps_ = 0;
	}


	// Implements interface member
	public void findPath() {
		path = PathFinding.currentPath;
	}
	
	// Implements interface member
	override public void stepPath() 
	{
		// TODO check if has reached waypoint. If so, update and assign new goal.
		float distance = Vector3.Distance (goal, transform.position);
		
		if (distance < 3.0f && goal != destination) {
			steps_++;
			initialDistance = Vector3.Distance (goal, transform.position);
			acceleration = 0f;
		}

		goal = Agent.recalculateGoal(steps_);
		destination = path [0].getPos ();

        if (!Agent.isRunning)
            return;

		if (goal.x == -1f) {
            Agent.isRunning = false;
            Agent.isFinished = true;
			return;
        }
		move ();
	}
	
	// Implements interface member
	public void reset(Vector3 position) {
		path = null;
		acceleration = 0.1f;
		steps_ = 0;
		rigidbody.transform.position = position;
	}

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
		
//		if (Math.Abs (cross.y) < 0.05f) // to prevent flickering with the steering wheel
//			return;

		bool previous = reverse;
		reverse = Math.Abs (cross.y) > reverseCrossThreshold ? true : false;
		keepSteady = previous == reverse ? true : false;

		if (!keepSteady) {
			initialDistance = Vector3.Distance (goal, transform.position);
			//Debug.Log ("NEW INITIAL DISTANCE!");
		}

		bool reverseToGoal = false;
		if (Vector3.Dot(direction, transform.forward) < -0.80) {
			reverse = true; // goal is behind the car
			reverseToGoal = true;
		}

		Debug.Log ("revser to goal: " + reverseToGoal);
		Debug.Log ("reverse: " + reverse);
		Debug.Log ("keepSteady: " + keepSteady);
		
		//Debug.Log ("Cross: " + cross.y);
		
		phi = Mathf.Abs(phi) > maxPhi ? Mathf.Sign(phi) * maxPhi : phi; // steering angle
		phi = reverse && !reverseToGoal ? -phi : phi;
		float theta = ((velocity / transform.localScale.z) * Mathf.Tan (phi)); // moving angle
		
		// TODO motsvarar detta rad/sec?. Vi kanske får skita i pivot point vid bakhjulen?
		transform.RotateAround (pivot.position, Vector3.up, theta * Mathf.Rad2Deg * Time.deltaTime); // backwheels as pivot
	}

	float previousDistance = 1000000.0f;
	void move ()
	{



//		Vector3 force = goal - rigidbody.position; // allow for slow down
//		float acc = force.magnitude;
//		if (acc > maxA) {
//			acc = maxA;
//		}
		float acc = maxA;
		
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

		// TODO handle last goal, decrease velocity more or something, use force maybe, I don't know.
		transform.position += transform.forward * Time.deltaTime * velocity;


		if ((keepSteady && reverse && velocity < 0.0f) || (keepSteady && !reverse && velocity > 0.0f)) {
			keepSteady = false;
		}


		if (!keepSteady)
			rotate ();
		previousDistance = distance;

	}
}
