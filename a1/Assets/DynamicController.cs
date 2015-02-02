using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
			if (StageManager.aStarPath != null) {
				List<GNode> path = StageManager.aStarPath;
				goal.x = path[path.Count-2].getPos().x;
				goal.z = path[path.Count-2].getPos().y;
			}
			acceleration = 0;
		}	
	
		// Update is called once per frame
		int counter = 0;
		void FixedUpdate ()
		{
		//goal = new Vector3 (Model.end.x, 2.0f, Model.end.y);
				if (Model.type == 2) {
					float distance = Vector3.Distance (goal, transform.position);
					
					if (distance < 3.2f) {
						counter++;
						goal = Model.recalculateGoal(counter);
					}
					
					if (goal.x == -1f)
						return;
					move ();
				}
		}
	
}
