using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeaderFollowerFormation : Formation {
	
	private GameObject[] agents;
	private int leaderId, motionModelId;
	private int[] leaderIds;
	private float distanceToLeader = 10.0f;
	private float maxSeparation = 3.0f;
	private float maxAngularThreshold = Mathf.PI; // TODO Take an angual threshold into account when moving the follower

	private Vector3[] previousPositions;
	private List<int>[] childrenof;
	
	public LeaderFollowerFormation (GameObject[] agents, int motionModelId, int[] leaderIds) {
		this.agents = agents;
		this.leaderId = leaderId;
		this.motionModelId = motionModelId;
		
		//leaderIds = assignSingleLeader (agents, leaderId);
		//leaderIds = assignRandomLeaders (agents, leaderId);
		this.leaderIds = leaderIds;

		childrenof = recomputeChildren (leaderIds);

		/*
		for (int i = 0; i < agents.Length; i++) {
			GameObject agent = agents [i];
			agent.AddComponent<Rigidbody> ();
			agent.GetComponent<Rigidbody> ().useGravity = false;
			agent.GetComponent<Rigidbody> ().isKinematic = false;
			agent.GetComponent<Rigidbody> ().detectCollisions = true;
		}
		*/

		previousPositions = new Vector3[agents.Length];
		for (int i = 0; i < agents.Length; i++) {
			previousPositions[i] = agents [i].transform.position;		
		}

	}
	
	// Implements the interface
	public void updateAgents () {

		//Debug.Log ("Leader velocity: " + (agents [0].transform.position - previousLeaderPosition));


		//followLeader (0);

		// Keep track of this position of the leader to compute the velocity in the next update
		//previousPositions[0] = agents [0].transform.position;

		/*
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
			agent.setStart (follower.transform.position);
			agent.setGoal (follower.transform.position + (directionOfLeader * moveDistance));
			agent.setModel (motionModelId); // 1 = Kinematic poit model
		}
		*/

		for (int agentId = 0; agentId < agents.Length; agentId++) {

			if (leaderIds[agentId] == -1) {

				previousPositions[agentId] = agents[agentId].transform.position;

			} else {

				GameObject follower = agents [agentId];
				GameObject leader = agents [leaderIds[agentId]];
				
				// Update the positions of the followers
				Vector3 directionOfLeader = Vector3.Normalize (leader.transform.position - follower.transform.position);
				float moveDistance = Vector3.Distance (leader.transform.position, follower.transform.position) - distanceToLeader;  
				
				Vector3 goal = follower.transform.position + (directionOfLeader * moveDistance);

				Vector3 leaderVelocity = leader.transform.position - previousPositions[leaderIds[agentId]];

				previousPositions[agentId] = follower.transform.position;

				Debug.Log ("leader: " + leaderVelocity);

				if (leaderVelocity.magnitude > 0) {

					Debug.Log("Leade velocity magnitude " + leaderVelocity.magnitude);

					Vector3 tv = leaderVelocity * -1;
					tv = tv.normalized * distanceToLeader;
					Vector3 behind = leader.transform.position + tv;
					
					goal = behind;
					
					Vector3 force = goal - follower.transform.position;
					
					int neighborCount = 0;
					//foreach (int neighborId in childrenof[leaderIds[i]]) {
					for (int neighborId = 0; neighborId < agents.Length; neighborId++) {
						if (agentId != neighborId && Vector3.Distance(follower.transform.position, agents[neighborId].transform.position) < 2.0f) { // Check with min separtion
							force.x += agents[neighborId].transform.position.x - follower.transform.position.x;
							force.z += agents[neighborId].transform.position.z - follower.transform.position.z;
							neighborCount++;
						}
					}
					
					if (neighborCount > 0) {
						force.x /= neighborCount;
						force.z /= neighborCount;
						force *= -1;
					}
					
					force = force.normalized;
					force *= maxSeparation;
					
					// Find the destination position for this follower
					Agent agent = (Agent) follower.GetComponent(typeof(Agent));
					agent.setStart (follower.transform.position);
					agent.setGoal (follower.transform.position + force);
					agent.setModel (motionModelId); // 1 = Kinematic poit model
				}
			}
		}
	}
	
	List<int>[] recomputeChildren (int[] leaderIds) {

		List<int>[] childrenOf = new List<int>[leaderIds.Length];
		
		for (int i = 0; i < leaderIds.Length; i++) {
			childrenOf[i] = new List<int> ();
		}
		
		for (int i = 0; i < leaderIds.Length; i++) {
			
			int leaderId = leaderIds[i];
			
			if (leaderId != -1) { // leader id for the main agent is -1
				childrenOf[leaderId].Add (i);
			}
		}
		
		return childrenOf;
	}

	public GameObject getAgent(int agentId) {
		return agents [agentId];
	}
}