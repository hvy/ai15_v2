using UnityEngine;
using System.Collections;

public class Model : MonoBehaviour
{

		public static int type = -1;
		public static Vector2 start = new Vector2(10,10);
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


		}

		// Use this for initialization
		void Start ()
		{

		}
	
		// Update is called once per frame
		void Update ()
		{
	
		// TODO check if has reached waypoint. If so, update and assign new goal.
		float distance = Vector3.Distance (end, transform.position);
		if (distance < 0.8f) {
			//end = 
		}

		}
}
