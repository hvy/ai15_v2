using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static Vector2 start, goal;
	public static float width, height;

	void Start () {
		start = new Vector2 (10.0f, 10.0f);
		goal = new Vector2 (90.0f, 90.0f);
		width = GameObject.Find ("Ground").transform.localScale.x;
		height = GameObject.Find ("Ground").transform.localScale.z;
	}
}
