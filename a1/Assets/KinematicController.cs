using UnityEngine;
using System.Collections;

public class KinematicController : MonoBehaviour
{
	
		public float speed;

		// for testing	
		protected Vector3 goal = new Vector3 (60.0f, 0.0f, 50.0f);

		void Start ()
		{
		}

		protected void move ()
		{
				float distance = Vector3.Distance (rigidbody.position, goal);
				// interpolate between car and goal
				rigidbody.transform.position = (Vector3.Lerp (rigidbody.position, goal, speed * Time.deltaTime / distance));
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (Model.type == 1) {
						move ();
				}
		}
}
