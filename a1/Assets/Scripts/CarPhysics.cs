using UnityEngine;
using System.Collections;

public class CarPhysics : MonoBehaviour {

	// Collision detection with waypoint GameObjects
	void OnCollisionEnter (Collision col) {
		if (col.gameObject.tag == "Waypoint") {
			Object.Destroy(col.gameObject);		
		}
	}
}
