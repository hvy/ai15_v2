using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

		public void restart() {
			if (StageManager.aStarPath != null) {
				List<GNode> path = StageManager.aStarPath;
				goal.x = path[path.Count-2].getPos ().x;
				goal.z = path[path.Count-2].getPos ().y;
			}
			
		}
	
		// Update is called once per frame
		int counter = 0;
		void Update ()
		{
				if (Model.type == 1) {
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
