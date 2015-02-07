using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinematicController : MonoBehaviour, MovementModel
{
	public float velocity;

	protected List<GNode> path;
	protected Vector3 goal;
	protected int steps;

	// Implements interface member
	public void findPath() {
		path = PathFinding.currentPath;

	}
	
	// Implements interface member
	virtual public void stepPath() {
		float distance = Vector3.Distance (goal, transform.position);
		
		if (distance < 3.2f) {
			steps++;
		}
		goal = Agent.recalculateGoal(steps);
		
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
		float distance = Vector3.Distance (rigidbody.position, goal);

		// interpolate between car and goal, third argument is [0, 1], describing how close to the target we should move.
		// so we basically normalize the fraction with (/ distance) to move in constant speed.
		// Could also use "MoveToward" which is much more straight forward
		rigidbody.transform.position = (Vector3.Lerp (rigidbody.transform.position, goal, velocity * Time.deltaTime / distance));
	}
}
