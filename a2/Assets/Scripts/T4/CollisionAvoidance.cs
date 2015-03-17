using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CollisionAvoidance {

	List<GameObject> agents;
	List<Vector2[]> polygons;
	float[,] previousDistances;
	float avoidanceStrength = 150f;

	public CollisionAvoidance(List<GameObject> agents, List<Vector2[]> polygons) {
		this.agents = agents;
		this.polygons = polygons;
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
			Vector3 targetVelocity;
			Vector3 agentVelocity = agent.rigidbody.velocity;
			Vector3 directionToTarget;
			float agentFlightPathAngle = calculateAngle(agentVelocity);
			float targetFlightPathAngle = 0f;
			float agentRangeToTarget = 0f;
			float agentLineOfSight = 0f;
			float targetLineOfSight = 0f;

			Vector3 totalAcceleration = new Vector3(0,0,0);

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

				if (agentRangeToTarget < 30f)
					totalAcceleration += acceleration * avoidanceStrength * Math.Max (target.transform.localScale.x / 5, 1);
				previousDistances[i, j] = agentRangeToTarget;

			}

			// Avoid obstacles
			Vector3 obstAcceleration = new Vector3(0,0,0);
			float shortestDistance = 100000f;
			for (int j = 0; j < polygons.Count; j++) {
				for (int k = 0; k < polygons[j].Length; k++) {

					Vector2 p1 = polygons[j][k];
					Vector2 p2 = polygons[j][(k+1)%polygons[j].Length];

					Vector2 v = p2 - p1;
					Vector2	outLeft = new Vector2(-v.y, v.x);
					outLeft /= (float) Math.Sqrt(v.x*v.x + v.y*v.y);

					// distance between agent and obstacle edge/line
					float distance = DistancePointLine(agent.transform.position, new Vector3(p1.x, 0.0f, p1.y), new Vector3(p2.x, 0.0f, p2.y));
					//if (distance < shortestDistance)
					if (distance < 50f && Vector3.Distance(agent.transform.position, a.goal) > 15f)
						obstAcceleration += -(new Vector3(outLeft.x, 0.0f, outLeft.y) - new Vector3(p1.x, 0.0f, p1.y)) * (1/distance) * 15f * agent.rigidbody.velocity.magnitude;
				}

			}
			totalAcceleration += obstAcceleration;

			// Apply acceleration to actor
			DynamicController dc = (DynamicController) a.models[2];
			dc.appliedAcceleration = -totalAcceleration;
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
