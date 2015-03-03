using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class T6GameManager : MonoBehaviour {

	public int numAgents; // numAgents > 0
	public int formationId; // 0 = Leader following, 1 = Virtual structure, 2 = Decentralized local interaction
	public float width, height, moveSpeed;
	
	private int playerControlledAgentId = 0;
	private Formation formation;
	
	void Start () {
		createStage (width, height);
		CameraModel.updateOrthoPosition(width, Camera.main.transform.position.y, height);

		// Instantiate the agents and let the player controll the first one
		GameObject[] agents = createAgents (numAgents);
		formation = setFormation (formationId, playerControlledAgentId, agents);
	}

	void Update () {	
		updatePlayerAgent (); // Listen for keyboard input and move the player controlled agent accordingly
		formation.updateAgents (); // Update the positions of the follower agents accordingly
	}

	private void updatePlayerAgent () {

		GameObject playerAgent = formation.getAgent (playerControlledAgentId);

		Vector3 agentTranslation = Vector3.zero;

		if (Input.GetKey (KeyCode.UpArrow)) {
			agentTranslation.z += 1.0f;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			agentTranslation.z -= 1.0f;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			agentTranslation.x += 1.0f;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			agentTranslation.x -= 1.0f;
		}

		playerAgent.transform.Translate (agentTranslation * Time.deltaTime * moveSpeed);
	}

	private void createStage (float width, float height) {
		StageFactory sf = new StageFactory ();
		sf.createStage (width, height);
	}

	private GameObject[] createAgents (int numAgents) {
		GameObject[] agents = new GameObject[numAgents];

		for (int i = 0; i < numAgents; i++) {
			agents[i] = AgentFactory.createAgent ();		
		}

		return agents;
	}

	private Formation setFormation (int formationId, int playerControlledAgentId, GameObject[] agents) {
		Formation formation = null;

		switch (formationId) {
		case 0:
			formation = new LeaderFollowerFormation (agents, playerControlledAgentId);
			break;
		case 1:
			//formation = new VirtualStructureFormation (...);
			break;
		case 2:
			//formation = new DecentralizedLocalInteractionFormation (...);
			break;
		default:
			break;
		}

		return formation;
	}
}
