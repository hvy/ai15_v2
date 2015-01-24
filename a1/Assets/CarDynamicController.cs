using UnityEngine;
using System.Collections;

public class CarDynamicController : DynamicController
{

		Vector3 goal = new Vector3 (60.0f, 0.0f, 50.0f);
		private Quaternion lookRotation;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		void rotate ()
		{
				Vector3 direction = (goal - transform.position).normalized;
				lookRotation = Quaternion.LookRotation (direction);
		
				// spherical interpolation
				transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * power);
		}
	
		void move ()
		{
				// TODO, tror nog detta är lite fuskigt. Bör ta hänsyn till styrvinkel och längd, etc.
				// TODO finish implementing this
				Vector3 force = goal - rigidbody.position;
				rigidbody.MovePosition (rigidbody.position + power * force / rigidbody.mass * Time.deltaTime + transform.forward);
				rotate ();
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (Model.type == 5) {
						move ();
				}
		}

}
