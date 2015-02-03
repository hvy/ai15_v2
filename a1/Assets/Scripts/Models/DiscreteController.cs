using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiscreteController : MonoBehaviour, MovementModel
{
	private List<GNode> path;
	private int steps;

	// Implements interface member
	public void findPath() {
		path = StageManager.aStarPath;
	}

	// Implements interface member
	public void stepPath() {
		if (steps < path.Count) {
			rigidbody.transform.position = path [path.Count - steps - 1].getPos ();
			steps++;
		}
	}

	// Implements interface member
	public void reset(Vector3 position) {
		path = null;
		steps = 0;
		rigidbody.transform.position = position;
	}
}
