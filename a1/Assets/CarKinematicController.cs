using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
				if (Model.type == 4) {

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
