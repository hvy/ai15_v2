using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class T6GameManager : MonoBehaviour {
	
	public int formationId; // 0 = Leader following, 1 = Virtual structure, 2 = Decentralized local interaction
	public int motionModelId;
	public float width, height, moveSpeed, rotationSpeed;
	
	private int playerControlledAgentId = 0;
	private Formation formation;

	void Start () {
		// Create the stage
		createStage (width, height);

		// Update the camera
		CameraModel.updateOrthoPosition(width, Camera.main.transform.position.y, height);

		// Start the demo given a formation
		formation = setFormation (formationId);
	}

	void Update () {	
		// Listen for keyboard input and move the player controlled agent accordingly
		updatePlayerAgent (); 		

		// Update the positions of the follower agents accordingly
		formation.updateAgents (); 	
	}

	GameObject[] createRandomAgents(int numAgents, int motionModelId) {
		
		GameObject[] agents = createAgents (numAgents, motionModelId);
		
		// Reposition the agents with an offset so that they don't collide
		for (int i = 0; i < agents.Length; i++) {
			agents[i].transform.Translate (new Vector3 (i, 0, width / 2.0f));
		}
		
		// Register the agents in the game state
		for (int i = 0; i < agents.Length; i++) {
			GameState.Instance.addAgent (agents[i].transform.position + new Vector3(0,0,i), (Agent) agents[i].GetComponent (typeof(Agent)));		
		}

		return agents;
	}

	private void updatePlayerAgent () {

		GameObject playerAgent = formation.getAgent (playerControlledAgentId);

		// On mouse click
		if (Input.GetButtonDown ("Fire1")) {

			Plane plane = new Plane (Vector3.up, 0);
			float dist;
			Ray ray = Camera.mainCamera.ScreenPointToRay (Input.mousePosition);
			if (plane.Raycast (ray, out dist)) {
				Vector3 destinationPos = ray.GetPoint (dist);
				Vector3 currentPos = playerAgent.transform.position;
				Vector3 velocity = Vector3.Normalize (destinationPos - currentPos);
				
				// Move the player controlled agent to the clicked position
				Agent agent = (Agent) playerAgent.GetComponent(typeof(Agent));
				agent.setStart (currentPos);
				agent.setGoal (destinationPos);
				agent.setModel (motionModelId);

				Debug.Log ("Velocity: " + playerAgent.GetComponent<Rigidbody> ().rotation);
			}
		}

		// Listen for rotation
		playerAgent.transform.Rotate(0, Input.GetAxis("Horizontal")*rotationSpeed*Time.deltaTime, 0);

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

	private GameObject createStage (float width, float height) {
		StageFactory sf = new StageFactory ();
		return sf.createStage (width, height);
	}

	private GameObject[] createAgents (int numAgents, int motionModelId) {
		GameObject[] agents = new GameObject[numAgents];

		for (int i = 0; i < numAgents; i++) {
			switch (motionModelId) {
			case 0:
				// Discrete
				agents[i] = AgentFactory.createAgent (Vector3.zero, Quaternion.identity, true);		
				break;
			case 1:
				// Kinematic point
				agents[i] = AgentFactory.createAgent (Vector3.zero, Quaternion.identity, true);		
				break;
			case 2:
				// Dynamic point
				agents[i] = AgentFactory.createAgent (Vector3.zero, Quaternion.identity, false);		
				break;
			case 3:
				// Differential drive
				agents[i] = AgentFactory.createCarAgent (Vector3.zero, Quaternion.identity, true);		
				break;
			case 4:
				// Kinematic car
				agents[i] = AgentFactory.createCarAgent (Vector3.zero, Quaternion.identity, true);
				break;
			case 5:
				// Dynamic car
				agents[i] = AgentFactory.createCarAgent (Vector3.zero, Quaternion.identity, false);
				break;
			default:
				Debug.Log ("Cannot create an agent due to invalid motion model");
				break;
			}
		}

		return agents;
	}

	private Formation setFormation (int formationId) {
		Formation formation = null;

		int numAgents = 0;
		GameObject[] agents = null;

		switch (formationId) {
		case 0: // Leader following

			numAgents = 10;
			agents = createRandomAgents (numAgents, motionModelId);

			int[] leaderIds = new int[numAgents];
			leaderIds[0] = -1;
			leaderIds[1] = 0;
			leaderIds[2] = 0;
			leaderIds[3] = 0;
			leaderIds[4] = 1;
			leaderIds[5] = 1;
			leaderIds[6] = 2;
			leaderIds[7] = 2;
			leaderIds[8] = 3;
			leaderIds[9] = 3;

			float yDistToLeader = 3.0f;
			float xDistToNeighborFst = 5.0f;
			float xDistToNeighborSnd = 3.0f;
			Vector3[] formationPositionsFst = new Vector3[3];
			Vector3[] formationPositionsSnd = new Vector3[2];

			formationPositionsFst[0] = new Vector3 (-xDistToNeighborFst , 0, -yDistToLeader);
			formationPositionsFst[1] = new Vector3 (0 , 0, -yDistToLeader);
			formationPositionsFst[2] = new Vector3 (xDistToNeighborFst , 0, -yDistToLeader);

			formationPositionsSnd[0] = new Vector3 (-xDistToNeighborSnd / 2.0f, 0, -yDistToLeader);
			formationPositionsSnd[1] = new Vector3 (xDistToNeighborSnd / 2.0f, 0, -yDistToLeader);

			Dictionary<int, Vector3[]> formationPositionsList = new Dictionary<int, Vector3[]> ();
			formationPositionsList[0] = formationPositionsFst;
			formationPositionsList[1] = formationPositionsSnd;
			formationPositionsList[2] = formationPositionsSnd;
			formationPositionsList[3] = formationPositionsSnd;

			formation = new LeaderFollowerFormation (agents, motionModelId, leaderIds, formationPositionsList);

			break;

		case 1: // Virtual structure

			numAgents = 9;
			agents = createRandomAgents (numAgents, motionModelId);

			// Create a 3x3 grid formation with distance 3 between each agent
			float distance = 3.0f; 
			Vector3[] formationPositions = new Vector3[agents.Length - 1];
			formationPositions[0] = new Vector3 (- distance, 0, distance);
			formationPositions[1] = new Vector3 (0, 0, distance);
			formationPositions[2] = new Vector3 (distance, 0, distance);
			formationPositions[3] = new Vector3 (- distance, 0, 0);
			formationPositions[4] = new Vector3 (distance, 0, 0);
			formationPositions[5] = new Vector3 (- distance, 0, - distance);
			formationPositions[6] = new Vector3 (0, 0, - distance);
			formationPositions[7] = new Vector3 (distance, 0, - distance);

			formation = new VirtualStructureFormation (agents, motionModelId, formationPositions);
			break;

		case 2:

			numAgents = 10;
			agents = createRandomAgents (numAgents, motionModelId);

			Vector3 leaderPos = agents[0].transform.position;
			for (int i = 1; i < agents.Length; i++) {
				GameObject agentObj = agents[i];
				float xOffset = Random.Range (1.0f, 15.0f);
				float yOffset = Random.Range (1.0f, 15.0f);
				agentObj.transform.position = leaderPos + new Vector3 (xOffset, 0.0f, yOffset);
			}

			formation = new DecentralizedLocalInteractionFormation (agents, playerControlledAgentId, motionModelId);
			break;

		default:
			break;
		}

		Vector3 midStagePosition = new Vector3 (width / 2.0f, 0, height / 2.0f);
		setInitialAgentPosition (agents [0], midStagePosition);

		return formation;
	}

	private void setInitialAgentPosition(GameObject agentObj, Vector3 position) {

		agentObj.transform.position = position;

		Agent agent = (Agent) agentObj.GetComponent(typeof(Agent));
		agent.setStart (position);
		agent.setGoal (position);
		agent.setModel (motionModelId);
	}
}
