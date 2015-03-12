using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeaderFollowerFormation : Formation {
	
	private GameObject[] agents;
	private GameObject targetCenter;
	private GameObject[] targets;

	private int leaderId, motionModelId;
	private int[] leaderIds;
	private float distanceToLeader = 10.0f;
	private float maxSeparation = 3.0f;
	private float maxAngularThreshold = Mathf.PI; // TODO Take an angual threshold into account when moving the follower

	private Vector3[] previousPositions;
	private List<int>[] childrenof;
	
	public LeaderFollowerFormation (GameObject[] agents, int motionModelId, int[] leaderIds, Dictionary<int, Vector3[]> formationPositionsList) {

		this.agents = agents;
		this.leaderId = leaderId;
		this.leaderIds = leaderIds;
		this.motionModelId = motionModelId;

		targets = new GameObject[agents.Length];

		childrenof = recomputeChildren (leaderIds);

		setupFormation (agents, childrenof, formationPositionsList);
	}

	void setupFormation (GameObject[] agents, List<int>[] childrenof, Dictionary<int, Vector3[]> formationPositionsList) {

		// The root target always at the same position of the main leader
		targetCenter = new GameObject ();
		targetCenter.transform.position = agents [0].transform.position;
		targetCenter.transform.parent = agents [0].transform;
		targets [0] = targetCenter;

		// For each agent that has at least one follower
		for (int a = 0; a < agents.Length; a++) {

			if (childrenof[a].Count <= 0) {
				continue;
			}

			GameObject rootTarget = new GameObject ();
			rootTarget.transform.position = agents [a].transform.position;
			rootTarget.transform.parent = agents [a].transform;

			Debug.Log ("Registering followers to: " + a);

			List<int> children = childrenof[a];

			for (int childId = 0; childId < children.Count; childId++) {

				int child = children[childId];

				Debug.Log ("\t\tRegistered follower: " + child);
				GameObject childTarget = new GameObject ("" + child);
				childTarget.transform.position = rootTarget.transform.position + formationPositionsList[a][childId];
				childTarget.transform.parent = rootTarget.transform;
				targets[child] = childTarget;
			}
		}
	}
	
	// Implements the interface
	public void updateAgents () {
		for (int i = 1; i < agents.Length; i++) {

			GameObject follower = agents[i];
			Agent agent = (Agent) follower.GetComponent(typeof(Agent));
			agent.setStart (follower.transform.position);
			agent.setGoal (targets[i].transform.position);
			agent.setModel (motionModelId); // 1 = Kinematic poit model		
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