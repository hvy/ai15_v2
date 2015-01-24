using UnityEngine;
using System.Collections;
using System;

public class CarKinematicController : DifferentialController
{

		Vector3 goal = new Vector3 (60.0f, 0.0f, 50.0f);
	
		// Use this for initialization
		void Start ()
		{
	
		}

		void rotate ()
		{
				Vector3 direction = (goal - transform.position).normalized;
				lookRotation = Quaternion.LookRotation (direction);
		
				// spherical interpolation
				transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * rotationSpeed * speed);
		}

		void move ()
		{
				// TODO, tror nog detta är lite fuskigt. Bör ta hänsyn till styrvinkel och längd, etc.
				transform.position += transform.forward * Time.deltaTime * speed;
				rotate ();
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (Model.type == 4) {
						move ();

				}
	
		}
}
