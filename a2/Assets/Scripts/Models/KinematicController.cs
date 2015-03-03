using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinematicController : MonoBehaviour, MovementModel
{
	public float velocity;

	private List<Agent> collisionAgents = new List<Agent>();

	// Implements interface member
	public void findPath() {

	}
	
	// Implements interface member
	virtual public bool stepPath(Agent agent, Vector3 goal) {


		if (clearToMove(agent, goal)) {
			move (goal);
			return true;
		}

		return false;
	}
	
	// Implements interface member
	public void reset(Vector3 position) {
		rigidbody.transform.position = position;
	}

	private bool clearToMove(Agent agent, Vector3 goal) {
		Vector3 fwd = transform.TransformDirection(goal-transform.position);

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.localScale.x/2+0.5f);
		int i = 0;
		bool ret = true;
		collisionAgents.Clear();
		while (i < hitColliders.Length) {
			if (hitColliders[i].transform.gameObject.GetComponent("Agent") != null && hitColliders[i].transform != transform) {
				collisionAgents.Add ((Agent)hitColliders[i].transform.gameObject.GetComponent("Agent"));
				ret = true;
			}
			i++;
		}
		//agent.setCollisionAgents(collisionAgents);
		
		return ret;
	}
	

	protected void move (Vector3 goal)
	{

		float distance = Vector3.Distance (rigidbody.position, goal);

		// interpolate between car and goal, third argument is [0, 1], describing how close to the target we should move.
		// so we basically normalize the fraction with (/ distance) to move in constant speed.
		// Could also use "MoveToward" which is much more straight forward
		rigidbody.transform.position = (Vector3.Lerp (rigidbody.transform.position, goal, velocity * Time.deltaTime / distance));
	}
}
