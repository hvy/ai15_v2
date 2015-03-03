using UnityEngine;
using System.Collections;

public class VirtualStructureFormation : MonoBehaviour, Formation {

	private int motionModelId;
	private float distance = 10.0f;
	private GameObject leader;
	private GameObject[] followers;
	private GameObject targetCenter;
	private GameObject[] targets;
	private GameObject[] agents;
	
	public VirtualStructureFormation (GameObject[] agents, int motionModelId, Vector3[] formationPositions) {

		this.agents = agents;
		this.motionModelId = motionModelId;

		leader = agents[0];
		followers = new GameObject[agents.Length - 1];
		for (int i = 1; i < agents.Length; i++) {
			followers[i - 1] = agents[i];
		}

		targets = new GameObject[followers.Length];
		targetCenter = new GameObject ();
		for (int i = 0; i < formationPositions.Length; i++) {
			GameObject target = new GameObject ();
			target.transform.parent = targetCenter.transform;
			target.transform.position = formationPositions[i];
			targets[i] = target;

			Debug.Log ("Position: " + target.transform.position); // Print
		}

		targetCenter.transform.position = leader.transform.position;
		targetCenter.transform.parent = leader.transform;
	}
	
	// Implements the interface
	public void updateAgents () {
		for (int i = 0; i < followers.Length; i++) {
			GameObject follower = followers[i];
			Agent agent = (Agent) follower.GetComponent(typeof(Agent));
			agent.setStart (follower.transform.position);
			agent.setGoal (targets[i].transform.position);
			agent.setModel (motionModelId); // 1 = Kinematic poit model		
		}
	}

	public GameObject getAgent(int agentId) {
		return agents [agentId];
	}
}
