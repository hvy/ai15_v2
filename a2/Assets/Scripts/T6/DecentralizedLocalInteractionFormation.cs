using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DecentralizedLocalInteractionFormation : Formation {

	private GameObject[] agents;
	private int leaderId, motionModelId;
	private float checkRadius = 10.0f;
	private float minSeparation = 5.0f;

	public DecentralizedLocalInteractionFormation (GameObject[] agents, int leaderId, int motionModelId) {

		this.leaderId = leaderId;
		this.agents = agents;
		this.leaderId = leaderId;
		this.motionModelId = motionModelId;
	}

	public void updateAgents () {

		for (int agentId = 0; agentId < agents.Length; agentId++) {

			if (agentId == leaderId) {
				continue;
			}

			GameObject agentObj = agents [agentId];
			Vector3 goal = agentObj.transform.position;

			Collider[] hitColliders = Physics.OverlapSphere(agentObj.transform.position, checkRadius);
			int i = 0;
			int neighborCount = 0;
			while (i < hitColliders.Length) {

				//if (hitColliders[i].transform.gameObject.GetComponent("Agent") != null && hitColliders[i].transform.position != agentObj.transform.position) {
				if (hitColliders[i].transform.gameObject.GetComponent("Agent") != null) {

					GameObject neighborAgent = hitColliders[i].transform.gameObject;
					Vector3 diffVec = neighborAgent.transform.position - agentObj.transform.position;
					goal += (Vector3.Magnitude (diffVec) - minSeparation) * diffVec.normalized;
					//float delta = (Vector3.Magnitude (diffVec) - minSeparation) * Mathf.Pow(2.71828f, (-1 * (Mathf.Pow(Vector3.Magnitude (diffVec), 4))));
					//goal.x += delta;
					//goal.z += delta;
				}
				i++;
			}

			// Find the destination position for this follower
			Agent agent = (Agent) agentObj.GetComponent(typeof(Agent));
			agent.setStart (agent.transform.position);
			agent.setGoal (goal);
			agent.setModel (motionModelId); // 1 = Kinematic poit model
		}
	}

	public GameObject getAgent(int agentId) {
		return agents [agentId];
	}
}
