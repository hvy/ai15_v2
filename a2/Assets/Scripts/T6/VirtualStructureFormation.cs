using UnityEngine;
using System.Collections;

public class VirtualStructureFormation : MonoBehaviour, Formation {

	private int motionModelId;
	private float distance = 20.0f;
	private GameObject leader;
	private GameObject[] followers;
	private GameObject targetCenter;
	private GameObject[] targets;
	private GameObject[] agents;

	Vector3 prevLeaderPosition, prevLeaderVelocity;

	public VirtualStructureFormation (GameObject[] agents, int motionModelId, Vector3[] formationPositions) {

		this.agents = agents;
		this.motionModelId = motionModelId;

		leader = agents[0];
		followers = new GameObject[agents.Length - 1];
		for (int i = 1; i < agents.Length; i++) {
			followers[i - 1] = agents[i];
		}

		targets = new GameObject[followers.Length];
		targetCenter = new GameObject ("Target Center");
		for (int i = 0; i < formationPositions.Length; i++) {
			GameObject target = new GameObject ();
			target.transform.parent = targetCenter.transform;
			target.transform.position = formationPositions[i];
			targets[i] = target;
		}

		targetCenter.transform.position = leader.transform.position;
		targetCenter.transform.parent = leader.transform;
		prevLeaderPosition = agents [0].transform.position;
		prevLeaderVelocity = new Vector3 (0, 0, 1.0f);
	}
	
	// Implements the interface
	public void updateAgents () {

		/* NOT USED SINCE THE FORMATION OBJECT IS ATTACHED TO THE ACTUAL AGENT, WHICH ROTATES ACCORDING TO USER INPUT

		// Rotate the formation structure according to the leader velocity
		if ((leader.transform.position - prevLeaderPosition).magnitude > 0) {
			Vector3 currentLeaderPosition = leader.transform.position;
			Vector3 velocity = currentLeaderPosition - prevLeaderPosition;

			Debug.Log (Vector3.Angle (velocity, prevLeaderVelocity));

			velocity /= Time.deltaTime;

			if (Vector3.Angle (velocity, prevLeaderVelocity) != 0) {
				prevLeaderPosition = currentLeaderPosition;
				targetCenter.transform.rotation = Quaternion.LookRotation (velocity.normalized);
				prevLeaderVelocity = velocity;
			}
		}
		*/

		for (int i = 0; i < followers.Length; i++) {
			GameObject follower = followers[i];
			Agent agent = (Agent) follower.GetComponent(typeof(Agent));
			agent.setStart (follower.transform.position);
			agent.setGoal (targets[i].transform.position);
			agent.setModel (motionModelId);	
		}
	}

	public GameObject getAgent(int agentId) {
		return agents [agentId];
	}
}
