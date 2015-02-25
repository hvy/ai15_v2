using UnityEngine;
using System.Collections;

public class AgentFactory : MonoBehaviour {

    public static Transform agentPreFab;



    public static Transform createAgent(Vector3 pos) {
        return Instantiate(agentPreFab, pos, Quaternion.identity) as Transform;
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
