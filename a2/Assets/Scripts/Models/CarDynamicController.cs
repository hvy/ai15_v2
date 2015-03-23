using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CarDynamicController : MonoBehaviour, MovementModel
{

	public float maxPhi;

	public float maxA;
	public float toVel; 
	public float maxVel;
	public float gain;
	public float rotationSpeed;

	public bool ignoreGoalForce = false;

	public Vector3 appliedAcceleration = new Vector3(0,0,0);

	//private const float max_velocity = 100.0f;
	private bool reverse = false;
	private bool keepSteady = true;
	private float reverseCrossThreshold = 1.5f;
	private Vector3 destination;
	private float previousDistance = 1000000.0f;
	private float initialDistance;
	private Vector3 goal;
	private float velocity = 0;

	public void findPath ()
	{
		
	}

	virtual public bool stepPath (Agent agent, Vector3 goal)
	{

		float distance = Vector3.Distance (goal, transform.position);
		
		if (Vector3.Distance (transform.position, goal) > 0.8f) {
			rotate (goal);
			move (goal);
		}
		
		return true;
	}
	
	public void reset (Vector3 position)
	{
		rigidbody.transform.position = position;
	}

	void rotate (Vector3 goal)
	{
		Vector3 targetDir = goal - transform.position;
		Vector3 localTarget = transform.InverseTransformPoint(goal);
		float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
		Vector3 eulerAngleVelocity = new Vector3 (0, angle, 0);
		Quaternion deltaRotation = Quaternion.Euler(rotationSpeed * eulerAngleVelocity * Time.deltaTime );

		rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
	}

	public void move (Vector3 goal)
	{
		Vector3 dist = goal - transform.position;
		
		// calc a target vel proportional to distance (clamped to maxVel)
		Vector3 tgtVel = Vector3.ClampMagnitude(toVel * dist, maxVel);
		
		// calculate the velocity error
		Vector3 error = tgtVel - rigidbody.velocity;
		
		// calc a force proportional to the error (clamped to maxForce)
		//Vector3 force = Vector3.ClampMagnitude(gain * error, maxA * rigidbody.mass);
		Vector3 force = Vector3.ClampMagnitude(gain * Vector3.forward.normalized, maxA * rigidbody.mass);
		Debug.Log ("Force: " + force);

		rigidbody.AddRelativeForce(force);
		//rigidbody.AddRelativeForce (transform.right * 1000000f);
		//rigidbody.AddRelativeForce (-transform.right * 1000000f);
		
		// Make sure that it stays on the plane
		//rigidbody.transform.position = new Vector3(rigidbody.position.x, 0f, rigidbody.position.z);
	}
}