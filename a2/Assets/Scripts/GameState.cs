using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour {

	public Dictionary<Vector3, Agent> agents;
	public Dictionary<Vector3, GameObject> customers;
	public List<Vector3> obstacles;

	private static GameState instance;

	public static GameState Instance {
		get {
			if(instance == null) {
				instance = new GameObject("GameState").AddComponent<GameState>();
				instance.init ();
			}
			return instance;
		}
	}	

	public void OnApplicationQuit() {
		instance = null;
	}
		
	public void init () {
		agents = new Dictionary<Vector3, Agent> ();
		customers = new Dictionary<Vector3, GameObject> ();
		obstacles = new List<Vector3> ();
	}

	public Dictionary<Vector3, Agent> getAgents () {
		return agents;
	}

	public Agent getAgent (Vector3 position) {
		return agents[position];
	}

	public Dictionary<Vector3, Agent> getCustomers () {
		return agents;
	}
	
	public GameObject getACustomer (Vector3 position) {
		return customers[position];
	}

	public bool addAgent(Vector3 position, Agent agent) {
		if (agents.ContainsValue (agent)) {
			return false;		
		}

		agents [position] = agent;

		return true;
	}

	public bool addCustomer(Vector3 position, GameObject customer) {
		if (customers.ContainsKey (position)) {
			return false;		
		}
		
		customers [position] = customer;

		return true;
	}

	public void addObstacle (Vector3 position) {
		obstacles.Add (position);
	}
}
