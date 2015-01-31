using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiscreteController : MonoBehaviour
{

		protected Vector3 goal = new Vector3 (Model.end.x, 2.0f, Model.end.y);
		private int counter = 0;
		// Use this for initialization
		void Start ()
		{

			//restart ();

		}

		void move(Vector3 position) {
			rigidbody.transform.position = position;
		}

		void move ()
		{

		List<GNode> path = StageManager.aStarPath;
		if (path.Count - counter - 1 < 0)
						return;
		move (path [path.Count-counter-1].getTransform ().position);
			counter++;

				
//				float distance_x = goal.x - rigidbody.transform.position.x;
//				float distance_z = goal.z - rigidbody.transform.position.z;
//
//				if (distance_x > 5.0f) {
//						stepRight ();
//				} else if (distance_x < -5.0f) {
//						stepLeft ();
//				} else if (distance_z < -5.0f) {
//						stepDown ();
//				} else if (distance_z > 5.0f) {
//						stepUp ();
//				} else {
//						CancelInvoke ("move");
//				}
		}

		void stepRight ()
		{
				Vector3 step = new Vector3 (10.0f, 0.0f, 0.0f);
				rigidbody.transform.position = rigidbody.position + step;

		}

		void stepLeft ()
		{
				Vector3 step = new Vector3 (-10.0f, 0.0f, 0.0f);
				rigidbody.transform.position = rigidbody.position + step;
		
		}

		void stepUp ()
		{
				Vector3 step = new Vector3 (0.0f, 0.0f, 10.0f);
				rigidbody.transform.position = rigidbody.position + step;
		
		}

		void stepDown ()
		{
				Vector3 step = new Vector3 (0.0f, 0.0f, -10.0f);
				rigidbody.transform.position = rigidbody.position + step;
		
		}

		public void restart() {
		counter = 0;
			CancelInvoke ("move");
		if (Model.type == 0) {
			InvokeRepeating ("move", 0.2f, 0.2f);			
		}
			
		}

		// Update is called once per frame
		void Update ()
		{


		}

		// Physics update
		void FixedUpdate ()
		{

		}

}
