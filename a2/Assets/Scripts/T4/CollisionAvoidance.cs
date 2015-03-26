using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CollisionAvoidance {

	List<GameObject> agents;
	List<Vector2[]> polygons;
	float[,] previousDistances;
	float avoidanceStrength = 170f;
	float avoidanceDistance = 10f;
	float maxA = 10f;

	public CollisionAvoidance(List<GameObject> agents, List<Vector2[]> polygons, float avoidanceStrength, float avoidanceDistance, float acc) {
		this.agents = agents;
		this.polygons = polygons;
		this.avoidanceStrength = avoidanceStrength;
		this.avoidanceDistance = avoidanceDistance;
		this.maxA = acc;
		previousDistances = new float[agents.Count,agents.Count];

		for (int i = 0; i < agents.Count; i++) {
			for (int j = (i+1) % (agents.Count-1); j < agents.Count; j++) {
				if (j == i)
					continue;
				float agentRangeToTarget = Vector3.Distance(agents[i].rigidbody.position, agents[j].rigidbody.position);
				previousDistances[i,j] = agentRangeToTarget;
			}
		}
	}

	public void avoidCollisions() {

		for (int i = 0; i < agents.Count; i++) {
			GameObject agent = agents[i];
			Agent a = (Agent) agent.GetComponent(typeof(Agent));
//			Debug.LogError (i + " " + a.goal);
			Vector3 targetVelocity;
			Vector3 agentVelocity = agent.rigidbody.velocity;
			Vector3 directionToTarget;
			float agentFlightPathAngle = calculateAngle(agentVelocity);
			float targetFlightPathAngle = 0f;
			float agentRangeToTarget = 0f;
			float agentLineOfSight = 0f;
			float targetLineOfSight = 0f;

			Vector3 agentAvoidAcceleration = new Vector3(0,0,0);
			bool ignoreGoalForce = false;

			// Avoid agents
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

				if (acceleration.magnitude < 0.00001f) // perfect collision course
					acceleration += UnityEngine.Random.insideUnitSphere*500f;
//				else // avoid collision acceleration
//					acceleration = acceleration * Math.Min((1/agentRangeToTarget)*50f, 1);

				if (agentRangeToTarget > previousDistances[i,j]) // Moving away from eachother
					acceleration = new Vector3(0,0,0);

				//Debug.LogError ("avoidance distance: " + avoidanceDistance * agent.rigidbody.velocity.magnitude/2);
				if (agentRangeToTarget < avoidanceDistance * Math.Max (agent.rigidbody.velocity.magnitude/2, 0.5f))
					agentAvoidAcceleration += acceleration * avoidanceStrength * Math.Max (target.transform.localScale.x / 5, 1);
				previousDistances[i, j] = agentRangeToTarget;

			}

			// Avoid obstacles
			Vector3 obstAcceleration = new Vector3(0,0,0);

			//new avoidance

			Vector3 dir = (a.goal - agent.transform.position).normalized;
			RaycastHit hit;
			float distToObstacle = 5f;

			Vector3 movementDirection = (agent.rigidbody.velocity + agent.transform.position);
			movementDirection.y = 0f;
			agent.rigidbody.rotation = Quaternion.Euler(movementDirection);
//			agent.transform.forward = movementDirection;

			Vector3 velocityDirection = ((movementDirection)-agent.transform.position).normalized;

			Debug.DrawLine(agent.transform.position, movementDirection, Color.green);
			if (Physics.Raycast(agent.transform.position, velocityDirection, out hit, distToObstacle)) {
				if (hit.transform != agent.transform && hit.transform.rigidbody == null) {
					Debug.DrawLine(agent.transform.position, hit.point, Color.red);
					dir += hit.normal * 20;

//					if (Vector3.Distance(hit.point, agent.transform.position) < 5f)
//						ignoreGoalForce = true;
				}
			}


			Vector3 leftR = agent.transform.position;
			Vector3 rightR = agent.transform.position;

			
			leftR.x -= 2;
			rightR.x += 2;

			if (Physics.Raycast(leftR, velocityDirection, out hit, distToObstacle)) {
				if (hit.transform != agent.transform && hit.transform.rigidbody == null) {
					Debug.DrawLine(agent.transform.position, hit.point, Color.red);
					dir += hit.normal * 20;
					
				}
			}
			
			if (Physics.Raycast(rightR, velocityDirection, out hit, distToObstacle)) {
				if (hit.transform != agent.transform && hit.transform.rigidbody == null) {
					Debug.DrawLine(agent.transform.position, hit.point, Color.red);
					dir += hit.normal * 20;
					
				}
			}

			if (Vector3.Distance(agent.transform.position, a.goal) > 5f)
				obstAcceleration += dir * 200f;

			Quaternion rot = Quaternion.LookRotation(dir);
			agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, rot, Time.deltaTime);
			
			
			Vector3 totalAcceleration = -obstAcceleration + agentAvoidAcceleration;
//			Debug.LogError (obstAcceleration);

			// Apply acceleration to actor
			DynamicController dc = (DynamicController) a.models[2];
			dc.appliedAcceleration = -totalAcceleration;
			dc.ignoreGoalForce = ignoreGoalForce;
			dc.maxA = maxA;
		}

	}

	private float DistancePointLine (Vector3 point, Vector3 lineStart, Vector3 lineEnd)
	{
		return Vector3.Magnitude (ProjectPointLine (point, lineStart, lineEnd) - point);
	}
	
	private Vector3 ProjectPointLine (Vector3 point, Vector3 lineStart, Vector3 lineEnd)
	{
		Vector3 rhs = point - lineStart;
		Vector3 vector2 = lineEnd - lineStart;
		float magnitude = vector2.magnitude;
		Vector3 lhs = vector2;
		if (magnitude > 1E-06f) {
			lhs = (Vector3)(lhs / magnitude);
		}
		float num2 = Mathf.Clamp (Vector3.Dot (lhs, rhs), 0f, magnitude);
		return (lineStart + ((Vector3)(lhs * num2)));
	}

	private float calculateAngle(Vector3 direction) {
		Vector3 norm_direction = direction.normalized;
		Vector3 relative = new Vector3(1,0,0);

		return Vector3.Angle(norm_direction, relative);
	}



}
