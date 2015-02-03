using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicController : MonoBehaviour, MovementModel
{	
	//public float power;
	public float maxA;

	protected List<GNode> path;
	protected Vector3 goal;
	protected int steps;
	protected const float max_acceleration = 0.1f;
	private bool lastGoal = false;

	protected float initialDistance = 0f;

	void Start ()
	{
		goal = Agent.goal;
		//acceleration = 0.1f;
		steps = 0;
	}

	// Implements interface member
	public void findPath() {
		path = StageManager.aStarPath;

		goal.x = path[path.Count-2].getPos().x;
		goal.z = path[path.Count-2].getPos().y;
	}

	// Implements interface member
	virtual public void stepPath() {
		//Debug.Log ("Moving: " + rigidbody.transform.position);	
		float distance = Vector3.Distance (goal, transform.position);
		
		if (distance < 0.01f) {
			steps++;
			if (path.Count == steps+1) {
				Debug.Log ("LAST GOAL");
				lastGoal = true;
			}
			goal = Agent.recalculateGoal(steps);
			initialDistance = Vector3.Distance (goal, transform.position);
			//acceleration = 0.1f;
		}
		
		if (goal.x == -1f) {
			return;		
		}

		move ();
	}

	// Implements interface member
	public void reset(Vector3 position) {
		path = null;
		steps = 0;
		rigidbody.transform.position = position;
	}

	float velocity = 0;
	protected void move ()
	{	
		Vector3 force = goal - rigidbody.position; // allow for slow down
		float acc = force.magnitude;
		if (acc > maxA) {
			acc = maxA;
		}

		float distance = Vector3.Distance (rigidbody.position, goal);

		if (initialDistance / distance > 2) {
			velocity -= acc;
		} else {
			velocity += acc;
		}

		velocity = (velocity == 0) ? 0.1f : velocity;

		if (!lastGoal)
			rigidbody.transform.position = (Vector3.Lerp (rigidbody.transform.position, goal, velocity * Time.deltaTime / distance));
		else {
			if (distance > 10.0f)
				rigidbody.transform.position = (Vector3.Lerp (rigidbody.transform.position, goal, velocity * Time.deltaTime / distance));
			else
				rigidbody.MovePosition (rigidbody.position + force * Time.deltaTime);
		}
	}
}