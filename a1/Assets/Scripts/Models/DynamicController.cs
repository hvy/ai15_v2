using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicController : MonoBehaviour, MovementModel
{	
	public float power;

	protected List<GNode> path;
	protected Vector3 goal;
	protected int steps;
	protected float acceleration;
	protected const float max_acceleration = 5.0f;
	protected const float max_acceleration = 0.1f;

	void Start ()
	{
		goal = Agent.goal;
		acceleration = 0.1f;
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
		Debug.Log ("Moving: " + rigidbody.transform.position);	
		float distance = Vector3.Distance (goal, transform.position);
		
		if (distance < 3.2f) {
			steps++;
			goal = Agent.recalculateGoal(steps);
			acceleration = 0.1f;
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

	protected void move ()
	{	
		Vector3 force = goal - rigidbody.position; // allow for slow down
		acceleration += 0.03f;
		if (acceleration > max_acceleration) {
			acceleration = max_acceleration;
		}
		rigidbody.MovePosition (rigidbody.position + power * force * Time.deltaTime * acceleration / rigidbody.mass);
	}
}
