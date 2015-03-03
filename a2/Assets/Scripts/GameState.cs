using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour {

	public static Dictionary<Vector3, Agent> agentPos;
	public static Dictionary<Vector3, GameObject> customerPos;

	void Start () {
		agentPos = new Dictionary<Vector3, Agent> ();
		customerPos = new Dictionary<Vector3, GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
