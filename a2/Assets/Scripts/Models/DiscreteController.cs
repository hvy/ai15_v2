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
//			obstacles.Add (goal);
//			obstacles.AddRange(GameState.Instance.obstacles);
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
		if (counter % 20 == 0) {

			Agent a = agent;

			if (goal.x == -1)
				return false;

			if (reactive) {
				if (reactiveCollision(agent, goal))
					return false;

			}

//			if (GameState.Instance.agents.ContainsKey(goal) && goal != agent.transform.position && goal.x != -1 && agent.tick < GameState.Instance.agents [goal].tick) {
//				Debug.Log("Illegal move, abort " + agent.transform.position + " to  " + goal);
//				if (reactiveCollision(agent, goal)) {
//					agent.tick++;
//					counter = 0;
//					return false;
//				}
//			}

//			Debug.LogError(GameState.Instance.agents.ContainsKey (new Vector3(10, 0, 2)));

			GameState.Instance.agents.Remove(rigidbody.transform.position);
			rigidbody.transform.position = goal;
			GameState.Instance.agents[rigidbody.transform.position] = a;

			counter = 0;
			agent.tick++;

			return true;
			
		}
		return false;
	}

	int pausedTimes = 0;
	bool reactiveCollision(Agent agent, Vector3 goal) {
		if (agent.currentPath == null) {GameState.Instance.getAgent (transform.position).isFinished = true;
//			GameState.Instance.addObstacle (transform.position);
			agent.tick = 1000;
			return true;
		}

		//GameState.Instance.agents.Remove(new Vector3());
		if (GameState.Instance.agents.ContainsKey (goal) && GameState.Instance.agents [goal] != this && goal != agent.transform.position) {
//			if (agent.tick < GameState.Instance.agents [goal].tick) {
			pausedTimes++;
//			if (pausedTimes > 4) {
//				GameState.Instance.agents.Remove(goal);
//				return false;
//			}
				Debug.LogError("Pause");
				return true; // Pause

//			}
		}
		return false;
	}
	
	
	// Implements interface member
	public void reset(Vector3 position) {
		steps = 0;
		rigidbody.transform.position = position;
	}
	
}
