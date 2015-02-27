using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiscreteController : MonoBehaviour, MovementModel
{
	private int steps;
	int counter = 0;

	// Implements interface member
	public void findPath() {
		//Debug.Log ("Path distance: " + PathFinding.calculateDistance(path));
	}

	// Implements interface member
	public void stepPath(Agent agent, Vector3 goal) {
		counter++;
		if (counter % 50 == 0) {

			//Agent a = GameManager.agentPos[rigidbody.transform.position];
			Agent a = agent;
			GameManager.agentPos.Remove(rigidbody.transform.position);
			rigidbody.transform.position = goal;
			GameManager.agentPos[rigidbody.transform.position] = a;

			counter = 0;
			agent.tick++;

		}
	}

	// Implements interface member
	public void reset(Vector3 position) {
		steps = 0;
		rigidbody.transform.position = position;
	}
}
