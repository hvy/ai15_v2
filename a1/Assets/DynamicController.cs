using UnityEngine;
using System.Collections;

public class DynamicController : MonoBehaviour
{
	
		public float power;
	
		// for testing	
		private Vector3 goal = new Vector3 (60.0f, 0.0f, 50.0f);

	private float acceleration;
	
		void Start ()
		{
		acceleration = 0;
		}
	
		protected void move ()
		{
				float distance = Vector3.Distance (transform.position, goal);
				Vector3 force = goal - rigidbody.position;
				acceleration += 0.04f;
				float ad = acceleration * distance;
				rigidbody.MovePosition (rigidbody.position + power * (force / rigidbody.mass) * Time.deltaTime * acceleration);
		}
	
		// Update is called once per frame
		void FixedUpdate ()
		{
				if (Model.type == 2) {
						move ();
				}
		}
	
}
