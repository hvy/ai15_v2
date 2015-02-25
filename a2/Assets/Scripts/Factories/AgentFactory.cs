using UnityEngine;
using System.Collections;

public class AgentFactory : MonoBehaviour {

	public static GameObject createAgent () {
		GameObject prefab = Resources.Load ("Prefabs/Agent", typeof(GameObject)) as GameObject;
		GameObject agent = Instantiate (prefab, Vector3.zero, Quaternion.identity) as GameObject;
		agent.transform.parent = GameObject.FindGameObjectWithTag("Agents").transform;
		return agent;
	}
}
