using UnityEngine;
using System.Collections;

public class LeaderFollowerFormation : Formation {

	private GameObject[] agents;
	private int leaderId, motionModelId;
	private int[] leaderIds;
	private float distanceToLeader = 10.0f;
	private float maxAngularThreshold = Mathf.PI; // TODO Take an angual threshold into account when moving the follower

	public LeaderFollowerFormation (GameObject[] agents, int leaderId, int motionModelId) {
		this.agents = agents;
		this.leaderId = leaderId;
		this.motionModelId = motionModelId;

		//leaderIds = assignSingleLeader (agents, leaderId);
		leaderIds = assignRandomLeaders (agents, leaderId);

		initialRepositioning ();
	}

	// Implements the interface
	public void updateAgents () {

		for (int i = 0; i < agents.Length; i++) {

			if (leaderIds[i] == -1) {
				continue;
			} 

			GameObject follower = agents [i];
			GameObject leader = agents [leaderIds[i]];
		
			// Update the positions of the followers
			Vector3 directionOfLeader = Vector3.Normalize (leader.transform.position - follower.transform.position);
			float moveDistance = Vector3.Distance (leader.transform.position, follower.transform.position) - distanceToLeader;  

			// Find the destination position for this follower
			Agent agent = (Agent) follower.GetComponent(typeof(Agent));
			agent.init ();
			agent.setStart (follower.transform.position);
			agent.setGoal (follower.transform.position + (directionOfLeader * moveDistance));
			agent.setModel (motionModelId); // 1 = Kinematic poit model
		}
	}

	// Implements the interface
	public GameObject getAgent(int agentId) {
		return agents [agentId];
	}

	// Implements the interface
	public GameObject[] getAgents() {
		return agents;
	}

	private void initialRepositioning () {

	}

	private int[] assignRandomLeaders (GameObject[] agents, int leaderId) {
		int[] randomLeaderIds = new int[agents.Length];
		
		for (int i = 0; i < agents.Length; i++) {
			if (i == leaderId) {
				randomLeaderIds[i] = -1;
			} else {
				int randomLeaderId = Random.Range (0, agents.Length - 1);
				randomLeaderIds[i] = randomLeaderId;
				Debug.Log ("Leader: " + randomLeaderId + " Follower: " + i);
			}
		}
		
		return randomLeaderIds;
	}

	private int[] assignSingleLeader (GameObject[] agents, int leaderId) {
		int[] leaderIds = new int[agents.Length];
		
		for (int i = 0; i < agents.Length; i++) {
			if (i == leaderId) {
				leaderIds[i] = -1;
			} else {
				leaderIds[i] = leaderId;
			}
		}
		
		return leaderIds;
	}
}
