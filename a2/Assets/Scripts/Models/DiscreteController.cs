using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiscreteController : MonoBehaviour, MovementModel
{
	public bool reactive;
	private int steps;
	int counter = 0;
	private List<Vector3> obstacles = new List<Vector3>();

	// Implements interface member
	public void findPath() {
		//Debug.Log ("Path distance: " + PathFinding.calculateDistance(path));
	}

	// MOVE TO DISCRETE
	private bool recalculatePath(Vector3 goal, Agent agent) {
		
		List<GNode> newPath = null;
		
		if (GameState.Instance.agents[goal].paused || GameState.Instance.agents[goal].isFinished) {
			obstacles.Add (goal);
			obstacles.AddRange(GameState.Instance.obstacles);
			Debug.Log("Recalculate path: " + GameState.Instance.obstacles.Count);
			newPath = PathPlanner.recalculatePath(agent, agent.currentPath[0].getPos (), obstacles);	// recalculate path
		}
		
		if (newPath != null) {
			return true;
		}
		return false;
	}

	// Implements interface member
	public bool stepPath(Agent agent, Vector3 goal) {
		counter++;
		if (counter % 50 == 0) {

			Agent a = agent;
			if (reactive) {
				if (agent.currentPath == null) {GameState.Instance.getAgent (transform.position).isFinished = true;
					GameState.Instance.addObstacle (transform.position);
					agent.tick = 1000;
					return false;
				}

				if (GameState.Instance.agents.ContainsKey (goal) && GameState.Instance.agents [goal] != this && goal != agent.transform.position) {
					if (!recalculatePath(goal, agent) && agent.tick < GameState.Instance.agents [goal].tick) {
						Debug.LogError("Pause");
						return false; // Pause
					}
				}

				if (GameState.Instance.agents.ContainsKey(goal) && GameState.Instance.agents[goal].tick > agent.tick && goal != agent.transform.position) {
					Debug.LogError("Illegal move, abort");
					agent.tick++;
					counter = 0;
					return false;
				}
			}

			GameState.Instance.agents.Remove(rigidbody.transform.position);
			rigidbody.transform.position = goal;
			GameState.Instance.agents[rigidbody.transform.position] = a;

			counter = 0;
			agent.tick++;

			return true;
			
		}
		return false;
	}
	

	// Implements interface member
	public void reset(Vector3 position) {
		steps = 0;
		rigidbody.transform.position = position;
	}
	
}
