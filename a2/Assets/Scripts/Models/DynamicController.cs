using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicController : MonoBehaviour, MovementModel {	

	public float maxA;
	public float toVel; 
	public float maxVel;
	public float gain;
	
	public Vector3 appliedAcceleration = new Vector3(0,0,0);
	public bool ignoreGoalForce = false;
	
	public void findPath() {

	}

	virtual public bool stepPath(Agent agent, Vector3 goal) {

		float distance = Vector3.Distance (goal, transform.position);

		if (distance > 1.3f) {
			move (goal);		
		}

		return true;
	}
	
	public void reset(Vector3 position) {
		rigidbody.transform.position = position;
	}

	virtual public void move (Vector3 goal) {	

//		Vector3 movementDirection = rigidbody.velocity.normalized;
//		rigidbody.rotation = Quaternion.Euler(movementDirection);


		// Collision avoidence
		if (appliedAcceleration.magnitude > 0.0003f) {
			Vector3 acc = Vector3.ClampMagnitude(appliedAcceleration, maxA * rigidbody.mass);
			rigidbody.AddForce(acc);
		} 

		if (ignoreGoalForce)
			return;

		Vector3 dist = goal - transform.position;

		// calc a target vel proportional to distance (clamped to maxVel)
		Vector3 tgtVel = Vector3.ClampMagnitude(toVel * dist, maxVel);

		// calculate the velocity error
		Vector3 error = tgtVel - rigidbody.velocity;

		// calc a force proportional to the error (clamped to maxForce)
		Vector3 force = Vector3.ClampMagnitude(gain * error, maxA * rigidbody.mass);

		rigidbody.AddRelativeForce(force);

		// Make sure that it stays on the plane
		rigidbody.transform.position = new Vector3(rigidbody.position.x, 0f, rigidbody.position.z);


	}
}
