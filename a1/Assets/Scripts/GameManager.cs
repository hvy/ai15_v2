using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{	
	public static Vector3 start, goal;
	public static float width, height;

	void Start () 
	{
		start = new Vector3 (2.5f, 0, 2.5f);
		goal = new Vector3 (87.5f, 0, 87.5f);

		width = GameObject.Find ("Ground").transform.localScale.x;
		height = GameObject.Find ("Ground").transform.localScale.z;

		resetAgent ();
	}

	public static void resetAgent() 
	{
		Agent.start = start;
		Agent.goal = goal;

		GameObject.FindWithTag ("Agent").transform.position = start;
	}
}