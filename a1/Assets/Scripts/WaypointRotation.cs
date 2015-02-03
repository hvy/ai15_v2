using UnityEngine;
using System.Collections;

public class WaypointRotation : MonoBehaviour {

	public Vector3 RotateAmount;
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(RotateAmount * Time.deltaTime);
	}
}
