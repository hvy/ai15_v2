using UnityEngine;
using System.Collections;

public class WaypointFactory : MonoBehaviour {

	public static GameObject createWaypoint () {
		GameObject prefab = Resources.Load ("Prefabs/Waypoint", typeof(GameObject)) as GameObject;
		GameObject waypoint = Instantiate (prefab, Vector3.zero, Quaternion.identity) as GameObject;
		return waypoint;
	}

}