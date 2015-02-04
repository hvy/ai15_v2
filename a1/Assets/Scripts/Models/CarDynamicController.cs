using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CarDynamicController : DynamicController
{
	//private const float max_velocity = 100.0f;
	private float acceleration;
	private bool reverse = false;
	private bool switching = false;
	private float reverseCrossThreshold = 0.7f;

	private Vector3 destination;

	public float maxPhi;

	// Use this for initialization
	void Start ()
	{
		acceleration = 0.1f;
	}


	// Implements interface member
	public void findPath() {
		path = StageManager.aStarPath;
		
		goal.x = path[path.Count-1].getPos ().x;
		goal.z = path[path.Count-1].getPos ().y;
	}
	
	// Implements interface member
	override public void stepPath() 
	{
		// TODO check if has reached waypoint. If so, update and assign new goal.
		float distance = Vector3.Distance (goal, transform.position);
		
		if (distance < 1.7f) {
			steps++;
			goal = Agent.recalculateGoal(steps);
			initialDistance = Vector3.Distance (goal, transform.position);
			acceleration = 0f;
		}

		destination = path [0].getPos ();

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
		switching = previous != reverse ? true : false;

		bool reverseToGoal = false;
		if (Vector3.Dot(direction, transform.forward) < -0.95) {
			reverse = true; // goal is behind the car
			reverseToGoal = true;
		}
		
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

		float acc = maxA;
		
		float distance = Vector3.Distance (rigidbody.position, goal);

		if (initialDistance / distance > 2.0 || reverse) {
				velocity -= acc;
		} else {
				velocity += acc;
		}

		float stoppingDistance = Time.deltaTime * (velocity * velocity) / (2 * acc);
		//Debug.Log ("distance to goal " + Vector3.Distance (transform.position, destination));
		//Debug.Log ("stop dist: " + stoppingDistance);
		if (Vector3.Distance (transform.position, destination) <= stoppingDistance) {
			velocity -= 2*acc;
		}

		// TODO handle last goal, decrease velocity more or something, use force maybe, I don't know.
		transform.position += transform.forward * Time.deltaTime * velocity;


		if ((switching && reverse && velocity < 0.0f) || (switching && !reverse && velocity > 0.0f)) {
			switching = false;
		}


		if (!switching)
			rotate ();
		previousDistance = distance;

	}
}
