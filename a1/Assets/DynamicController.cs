using UnityEngine;
using System.Collections;

public class DynamicController : MonoBehaviour
{
	
		public float power;
	
		// for testing	
		protected Vector3 goal = new Vector3 (Model.end.x, 2.0f, Model.end.y);
		protected float acceleration;
		protected const float max_acceleration = 5.0f;

		void Start ()
		{
		acceleration = 0;
		}
	
		protected void move ()
		{
				Vector3 force = goal - rigidbody.position; // allow for slow down
				acceleration += 0.03f;
				if (acceleration > max_acceleration) {
					acceleration = max_acceleration;
				}
				rigidbody.MovePosition (rigidbody.position + power * force * Time.deltaTime * acceleration / rigidbody.mass);
		}

		public void restart() {
			acceleration = 0;
		}	
	
		// Update is called once per frame
		void FixedUpdate ()
		{
		//goal = new Vector3 (Model.end.x, 2.0f, Model.end.y);
				if (Model.type == 2) {
						move ();
				}
		}
	
}
