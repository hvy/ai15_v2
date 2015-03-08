using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicController : MonoBehaviour, MovementModel
{	
	//public float power;
	public float maxA;

	protected List<GNode> path;
	protected Vector3 goal;
	//protected const float max_acceleration = 0.1f;
	protected float velocity = 0;
	//private bool lastGoal = false;
	//private Vector3 destination_;

	protected float initialDistance = 0f;
	public Vector3 direction = new Vector3(0,0,0);
	public Vector3 acceleration = new Vector3(0,0,0);
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


		//Debug.Log ("Moving: " + rigidbody.transform.position);	
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
        float acc = maxA * Time.fixedDeltaTime;

		float distance = Vector3.Distance (rigidbody.position, goal);

		direction = (goal - rigidbody.position).normalized;

		if (acceleration.magnitude > 0.04f) {
			acceleration = Vector3.ClampMagnitude(acceleration, maxA);
			rigidbody.AddRelativeForce(acceleration * Time.deltaTime*10f);
		} else {
			rigidbody.AddRelativeForce(direction * Time.deltaTime*10f);
		}
		
		rigidbody.transform.position = new Vector3(rigidbody.position.x, 0f, rigidbody.position.z);
		       
	}
}
