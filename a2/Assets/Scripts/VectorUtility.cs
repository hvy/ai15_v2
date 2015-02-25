using UnityEngine;
using System.Collections;

public class VectorUtility : MonoBehaviour {

	public static Vector3 toVector3(Vector2 v) {
		return new Vector3(v.x, 0, v.y);
	}
}
