using UnityEngine;
using System.Collections;

public class KinematicController : MonoBehaviour
{
	
		public float speed;

		// for testing	
		protected Vector3 goal = new Vector3 (Model.end.x, 2.0f, Model.end.y);

		void Start ()
		{
		}

		protected void move ()
		{
				float distance = Vector3.Distance (rigidbody.position, goal);
				// interpolate between car and goal, third argument is [0, 1], describing how close to the target we should move.
				// so we basically normalize the fraction with (/ distance) to move in constant speed.
				// Could also use "MoveToward" which is much more straight forward
				rigidbody.transform.position = (Vector3.Lerp (rigidbody.transform.position, goal, speed * Time.deltaTime / distance));
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (Model.type == 1) {
						move ();
				}
		}
}
