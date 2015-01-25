using UnityEngine;
using System.Collections;

public class Model : MonoBehaviour
{

		public static int type = 5;

		public void useDiscreteModel ()
		{
				type = 0;
		}

		public void useKinematicModel ()
		{
				type = 1;
		}

		public void useDynamicModel ()
		{
				type = 2;
		}

		public void useDifferentialModel ()
		{
				type = 3;
		}

		public void useCarKinematicModel ()
		{
				type = 4;
		}

		public void useCarDynamicModel ()
		{
				type = 5;
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
