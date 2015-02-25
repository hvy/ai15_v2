using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{	

	public static Vector3 start, goal;
	public static float width, height;
	public static int discreteNeighbors;

	public static int nr_agents;


	AgentFactory agentFact;

	void Start () 
	{
		init();
	}

	void init() {

		agentFact = new AgentFactory();

		for (int i = 0; i < nr_agents; i++) {
			//GameObject agent = agentFact.createAgent();
			//agent.transform.position = new Vector3(Random.Range(0, 100), 0.0f, Random.Range(0, 100));
		}
	}


	
}