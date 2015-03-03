using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiscreteController : MonoBehaviour, MovementModel
{
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
		
		if (GameManager.agentPos[goal].paused || GameManager.agentPos[goal].isFinished) {
			obstacles.Add (goal);
			obstacles.AddRange(GameManager.obstacles);
			Debug.Log("Recalculate path: " + GameManager.obstacles.Count);
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

//			if (agent.currentPath == null) {GameState.Instance.getAgent (transform.position).isFinished = true;
//				GameState.Instance.addObstacle (transform.position);
//				agent.tick = 1000;
//				return false;
//			}
//
//			if (GameManager.agentPos.ContainsKey (goal) && GameManager.agentPos [goal] != this) {
//				if (!recalculatePath(goal, agent) && agent.tick < GameManager.agentPos [goal].tick) {
//					Debug.LogError("Pause");
//					return false; // Pause
//				}
//			}

			Agent a = agent;
//			if (GameManager.agentPos.ContainsKey(goal) && GameManager.agentPos[goal].tick > agent.tick) {
//				Debug.LogError("Illegal move, abort");
//				agent.tick++;
//				counter = 0;
//				return false;
//			}
			GameManager.agentPos.Remove(rigidbody.transform.position);
			Debug.Log (goal);
			rigidbody.transform.position = goal;
			GameManager.agentPos[rigidbody.transform.position] = a;

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

	public void reverse(Vector3 goal) {
		
	}
}
