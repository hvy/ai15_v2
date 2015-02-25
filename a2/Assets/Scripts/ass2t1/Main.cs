using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
        init();
	
	}

    void init() {
        AgentFactory.createAgent(Vector3.zero);

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
