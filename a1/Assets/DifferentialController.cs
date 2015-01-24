using UnityEngine;
using System.Collections;
using System;

public class DifferentialController : DynamicController {


	// for testing
	Vector3 goal = new Vector3 (60.0f, 0.0f, 50.0f);
	public float rotationSpeed;
	public float angularThreshold;

	private Quaternion lookRotation;

	// Use this for initialization
	void Start () {
	
	}

	void rotate() {
		Vector3 direction = (goal - transform.position).normalized;
		lookRotation = Quaternion.LookRotation(direction);

		// spherical interpolation
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

	}
	
	// Update is called once per frame
	void Update () {
		if (Model.type == 3) {
			rotate ();
			print(Math.Abs(transform.rotation.y - lookRotation.y));
			if (Math.Abs(transform.rotation.y - lookRotation.y) < angularThreshold) {
				move();
			}
		}
	}
}
