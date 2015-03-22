using UnityEngine;
using System.Collections;

public class AgentFactory : MonoBehaviour {

	Random random;

	public static GameObject createAgent () {
		GameObject prefab = Resources.Load ("Prefabs/Agent", typeof(GameObject)) as GameObject;
		GameObject agent = Instantiate (prefab, Vector3.one, Quaternion.identity) as GameObject;
		agent.transform.parent = GameObject.FindGameObjectWithTag("Agents").transform;
		agent.renderer.material.color = PathPlanner.randomizeColor();
		return agent;
	}

	public static GameObject createAgent (Vector3 position, Quaternion rotation) {
		GameObject prefab = Resources.Load ("Prefabs/Agent", typeof(GameObject)) as GameObject;
		GameObject agent = Instantiate (prefab, position, rotation) as GameObject;
		agent.transform.parent = GameObject.FindGameObjectWithTag("Agents").transform;
		return agent;
	}

	public static GameObject createCarAgent () {
		GameObject prefab = Resources.Load ("Prefabs/CarAgent", typeof(GameObject)) as GameObject;
		GameObject agent = Instantiate (prefab, Vector3.one, Quaternion.identity) as GameObject;
		agent.transform.parent = GameObject.FindGameObjectWithTag("Agents").transform;
		return agent;
	}

	public static GameObject createCarAgent (Vector3 position, Quaternion rotation) {
		GameObject prefab = Resources.Load ("Prefabs/CarAgent", typeof(GameObject)) as GameObject;
		GameObject agent = Instantiate (prefab, position, rotation) as GameObject;
		agent.transform.parent = GameObject.FindGameObjectWithTag("Agents").transform;
		return agent;
	}
}
