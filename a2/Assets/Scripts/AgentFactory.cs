using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentFactory : MonoBehaviour{

	private Transform parentTransform;
	public GameObject prefab;
	

	public AgentFactory() {
		parentTransform = GameObject.FindGameObjectWithTag("Agents").transform;

	}

    void Awake() {
		Debug.Log ("Creating agent");

		for (int i = 0; i < GameManager.nr_agents; i++) {
			GameObject a = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
			a.transform.parent = parentTransform;
		}
    }

}
