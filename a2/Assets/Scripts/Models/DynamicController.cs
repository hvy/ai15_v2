using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicController : MonoBehaviour, MovementModel
{	
	public float maxA;
	public float toVel; 
	public float maxVel;
	public float gain;

	protected List<GNode> path;
	protected Vector3 goal;
	protected float velocity = 0;

	protected float initialDistance = 0f;
	public Vector3 direction = new Vector3(0,0,0);
	public Vector3 appliedAcceleration = new Vector3(0,0,0);
	private int steps;

	void Start () {
		steps = 0;
		direction = (goal - rigidbody.position).normalized;
	}

	// Implements interface member
	public void findPath() {
		path = PathFinding.currentPath;
	}

	// Implements interface member
	virtual public bool stepPath(Agent agent, Vector3 goal) {


//		Debug.Log ("Moving: " + rigidbody.transform.position);	
		float distance = Vector3.Distance (goal, transform.position);
		
		if (distance < 4.0f) {
			steps++;
		}
		this.goal = goal;

        if (!agent.isRunning)
            return false;

		if (goal.x == -1f) {
            agent.isRunning = false;
            agent.isFinished = true;
			return false;		
		}

		move ();
		return true;
	}

	// Implements interface member
	public void reset(Vector3 position) {
		path = null;
		steps = 0;
		rigidbody.transform.position = position;
        velocity = 0f;
		direction = (goal - rigidbody.position).normalized;
		initialDistance = Vector3.Distance(goal, rigidbody.position);
	}

	protected void move ()
	{	
		if (appliedAcceleration.magnitude > 0.0003f) {
//			Debug.Log ("repelling");
//			Debug.Log ("acc: " + appliedAcceleration);
			Vector3 acc = Vector3.ClampMagnitude(appliedAcceleration, maxA*rigidbody.mass);
			rigidbody.AddForce(acc * Time.deltaTime);
		} 

		Vector3 dist = goal - transform.position;

		// calc a target vel proportional to distance (clamped to maxVel)
		Vector3 tgtVel = Vector3.ClampMagnitude(toVel * dist, maxVel);
		// calculate the velocity error
		Vector3 error = tgtVel - rigidbody.velocity;
		// calc a force proportional to the error (clamped to maxForce)
		Vector3 force = Vector3.ClampMagnitude(gain * error, maxA*rigidbody.mass);

		rigidbody.AddRelativeForce(force * Time.deltaTime);
		
		rigidbody.transform.position = new Vector3(rigidbody.position.x, 0f, rigidbody.position.z);
//		Debug.Log (rigidbody.velocity);
		       
	}
}
