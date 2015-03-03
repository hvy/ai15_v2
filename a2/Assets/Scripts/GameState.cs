using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour {

	public static Dictionary<Vector3, Agent> agentPosistions;
	public static Dictionary<Vector3, GameObject> customerPosistions;

	void Start () {
		agentPosistions = new Dictionary<Vector3, Agent> ();
		customerPosistions = new Dictionary<Vector3, GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
