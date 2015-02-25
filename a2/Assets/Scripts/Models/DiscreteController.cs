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
	public void stepPath(Vector3 goal) {
		counter++;
		if (counter % 50 == 0) {
			rigidbody.transform.position = goal;
			counter = 0;
		}
	}

	// Implements interface member
	public void reset(Vector3 position) {
		steps = 0;
		rigidbody.transform.position = position;
	}
}
