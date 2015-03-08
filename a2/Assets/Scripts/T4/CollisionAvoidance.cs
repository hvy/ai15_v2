using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CollisionAvoidance {

	List<GameObject> agents;

	public CollisionAvoidance(List<GameObject> agents) {
		this.agents = agents;
	}

	public void avoidCollisions() {

		for (int i = 0; i < agents.Count; i++) {
			GameObject agent = agents[i];
			Vector3 targetVelocity;
			Vector3 agentVelocity = agent.rigidbody.velocity;
			Vector3 directionToTarget;
			float agentFlightPathAngle = calculateAngle(agentVelocity);
			float targetFlightPathAngle = 0f;
			float agentRangeToTarget = 0f;
			float agentLineOfSight = 0f;
			float targetLineOfSight = 0f;

			Vector3 totalAcceleration = new Vector3(0,0,0);

			for (int j = (i+1) % (agents.Count-1); j < agents.Count; j++) {
				if (j == i)
					continue;
				GameObject target = agents[j];
				targetVelocity = target.rigidbody.velocity;
				targetFlightPathAngle = calculateAngle(targetVelocity);
				agentRangeToTarget = Vector3.Distance(agent.rigidbody.position, target.rigidbody.position);
				directionToTarget = target.rigidbody.position - agent.rigidbody.position;
				agentLineOfSight = calculateAngle(directionToTarget);
				targetLineOfSight = calculateAngle(target.transform.position);
				Vector3 acceleration;

//				Debug.Log ("line of sight: " + agentLineOfSight);

				Vector3 targetVelocityRelativeToAgent = (targetVelocity - agentVelocity);
				Vector3 rotationVector = Vector3.Cross(directionToTarget, targetVelocityRelativeToAgent) / Vector3.Dot(directionToTarget, directionToTarget);
				acceleration = Vector3.Cross(targetVelocityRelativeToAgent, rotationVector);

				if (agentRangeToTarget > 3f) // radius of consideration to avoid other agents
					acceleration = new Vector3(0,0,0);
				acceleration = acceleration * Math.Min(1/agentRangeToTarget, 1);

				totalAcceleration += acceleration;

			}

			Agent a = (Agent) agent.GetComponent(typeof(Agent));
			DynamicController dc = (DynamicController) a.models[2];
			dc.acceleration = -totalAcceleration;
//			Debug.Log ("acceleration: " + totalAcceleration);
//			Debug.Log ("acceleration mag: " + totalAcceleration.magnitude);
		}

	}

	private float calculateAngle(Vector3 direction) {
		Vector3 norm_direction = direction.normalized;
		Vector3 relative = new Vector3(1,0,0);

		return Vector3.Angle(norm_direction, relative);
	}



}
