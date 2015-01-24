using UnityEngine;
using System.Collections;

public class DynamicController : MonoBehaviour
{
	
	public float power;
	
	// for testing	
	private Vector3 goal = new Vector3 (60.0f, 0.0f, 50.0f);
	
	void Start ()
	{
	}
	
	protected void move ()
	{
		//float distance = Vector3.Distance (rigidbody.position, goal);
		Vector3 force = goal - rigidbody.position;
		//rigidbody.AddForce (v * speed * Time.deltaTime);

		rigidbody.MovePosition (rigidbody.position + power*force/rigidbody.mass * Time.deltaTime);
		// interpolate between car and goal
		//rigidbody.transform.position = (Vector3.Lerp (rigidbody.position, goal, speed * Time.deltaTime / distance));
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Model.type == 2) {
			move ();
		}
	}
	
}
