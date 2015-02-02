using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Model : MonoBehaviour
{

		public static int type = -1;
		public static Vector2 start = new Vector2(0,0);
		public static Vector2 end = new Vector2(90f, 90f);

		public void useDiscreteModel ()
		{
				type = 0;
				respawnCar ();
		}

		public void useKinematicModel ()
		{
				type = 1;
				respawnCar ();
		}

		public void useDynamicModel ()
		{
				type = 2;
				respawnCar ();	
		}

		public void useDifferentialModel ()
		{
				type = 3;
				respawnCar ();
		}

		public void useCarKinematicModel ()
		{
				type = 4;
				respawnCar ();
		}

		public void useCarDynamicModel ()
		{
				type = 5;
				respawnCar ();
		}

		private void respawnCar() {

			GameObject car = GameObject.Find("Car");
			car.transform.position = new Vector3 (start.x, 2f, start.y);
			Vector3 direction = new Vector3 (0, 0, 0);
			car.transform.rotation = Quaternion.LookRotation (direction);

			CarDynamicController dyncar = car.GetComponent<CarDynamicController> ();
			dyncar.restart ();
			DynamicController dynpoint = car.GetComponent<DynamicController> ();
			dynpoint.restart ();
			DiscreteController discpoint = car.GetComponent<DiscreteController> ();
			discpoint.restart ();
			CarKinematicController kincar = car.GetComponent<CarKinematicController> ();
			kincar.restart ();
			KinematicController kin = car.GetComponent<KinematicController> ();
			kin.restart ();
			DifferentialController diff = car.GetComponent<DifferentialController> ();
			diff.restart ();


		}
		
		public static Vector3 recalculateGoal(int counter) {
			Vector3 goal;
			List<GNode> path = StageManager.aStarPath;
			if (path.Count - counter - 1 < 0)
				return new Vector3(-1f,-1f,-1f);
			goal = path[path.Count-counter-1].getTransform ().position;
			Debug.Log ("NEW WAYPOINT: " + goal.x + " " + goal.z);
			return goal;
		}

		// Use this for initialization
		void Start ()
		{
			
		}
	
		// Update is called once per frame
		void Update ()
		{
	


		}
}
