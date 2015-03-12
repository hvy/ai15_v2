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

		float a = 6.0f;
		float D = 1.0f;

		for (int agentId = 0; agentId < agents.Length; agentId++) {
			GameObject agentObj = agents [agentId];
			Agent agent = (Agent)agentObj.GetComponent (typeof(Agent));
			agent.velocity = Vector3.zero;
			//Clone position
			agent.position = new Vector3(agentObj.transform.position.x,agentObj.transform.position.y, agentObj.transform.position.z);
		}

		for (int iter = 0; iter < 1; iter++) {

			for (int agentId = 0; agentId < agents.Length; agentId++) {

				if (agentId == leaderId) {
						continue;
				}

				GameObject agentObj = agents [agentId];
				Agent agent = (Agent)agentObj.GetComponent (typeof(Agent));

				//Collider[] hitColliders = Physics.OverlapSphere (agentObj.transform.position, checkRadius);
				Collider[] hitColliders = Physics.OverlapSphere (agent.position, checkRadius);
				int i = 0;
				int neighborCount = 0;
				while (i < hitColliders.Length) {
					//if (hitColliders[i].transform.gameObject.GetComponent("Agent") != null && hitColliders[i].transform.position != agentObj.transform.position) {
					if (hitColliders [i].transform.gameObject.GetComponent ("Agent") != null) {

						GameObject neighborAgent = hitColliders [i].transform.gameObject;
						Agent neighbor = (Agent)neighborAgent.GetComponent (typeof(Agent));
						
						Vector3 diffVec = neighbor.position - agent.position;
						
						//agent.velocity = D * (1 - Mathf.Exp (-a * (Vector3.Magnitude (diffVec) - minSeparation))) * diffVec.normalized - agent.velocity;
						//agent.position += agent.velocity;
						//float velSize = D * Mathf.Pow((1 - Mathf.Exp (-a * (Vector3.Magnitude (diffVec) - minSeparation))), 2.0f);
						float velSize = D * (1 - Mathf.Exp (-a * (Vector3.Magnitude (diffVec) - minSeparation)));
						agent.velocity += velSize * diffVec.normalized;
						//neighbor.velocity -= velSize * diffVec.normalized;

					}
					i++;
				}
			}
		}


		for (int agentId = 0; agentId < agents.Length; agentId++) {
			
			if (agentId == leaderId) {
					continue;
			}

			GameObject agentObj = agents [agentId];
			// Find the destination position for this follower
			Agent agent = (Agent)agentObj.GetComponent (typeof(Agent));
			agent.setStart (agent.transform.position);
			agent.position += agent.velocity;
			agent.setGoal (agent.position);
			agent.setModel (motionModelId); // 1 = Kinematic poit model

		}

	}

	public GameObject getAgent(int agentId) {
		return agents [agentId];
	}
}
