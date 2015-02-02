using UnityEngine;
using System.Collections;
using System;

public class CarKinematicController : DifferentialController
{

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
				float distance = Vector3.Distance (goal, transform.position);
				if (distance < 0.8f)
					return;
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
