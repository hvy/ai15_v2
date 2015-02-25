//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class DynamicController : MonoBehaviour, MovementModel
//{	
//	//public float power;
//	public float maxA;
//
//	protected List<GNode> path;
//	protected Vector3 goal;
//	protected const float max_acceleration = 0.1f;
//	protected float velocity = 0;
//	private bool lastGoal = false;
//	private Vector3 destination_;
//
//	protected float initialDistance = 0f;
//	private int steps;
//
//	void Start ()
//	{
//		//goal = Agent.goal;
//		//acceleration = 0.1f;
//		steps = 0;
//	}
//
//	// Implements interface member
//	public void findPath() {
//		path = PathFinding.currentPath;
//
//	}
//
//	// Implements interface member
//	virtual public void stepPath() {
//		//Debug.Log ("Moving: " + rigidbody.transform.position);	
//		float distance = Vector3.Distance (goal, transform.position);
//		
//		if (distance < 4.0f) {
//			steps++;
//			if (path.Count == steps+1) {
//				Debug.Log ("LAST GOAL");
//				lastGoal = true;
//			}
//			initialDistance = Vector3.Distance (goal, transform.position);
//			//acceleration = 0.1f;
//		}
//		goal = Agent.recalculateGoal(steps);
//		destination_ = path [0].getPos ();
//
//        if (!Agent.isRunning)
//            return;
//
//		if (goal.x == -1f) {
//            Agent.isRunning = false;
//            Agent.isFinished = true;
//			return;		
//		}
//
//		move ();
//	}
//
//	// Implements interface member
//	public void reset(Vector3 position) {
//		path = null;
//		steps = 0;
//		rigidbody.transform.position = position;
//        velocity = 0f;
//	}
//
//	protected void move ()
//	{	
////		Vector3 force = goal - rigidbody.position; // allow for slow down
////		float acc = force.magnitude;
////		if (acc > maxA) {
////			acc = maxA;
////		}
//        float acc = maxA * Time.fixedDeltaTime;
//
//		float distance = Vector3.Distance (rigidbody.position, goal);
//
//		if (initialDistance / distance > 2) {
//            velocity -= acc;
//		} else {
//			velocity += acc;
//		}
//
//		float stoppingDistance = Time.deltaTime * (velocity * velocity) / (2 * acc);
//		// TODO change to destination instead of goal to keep velocity at waypoints
//		if (Vector3.Distance (transform.position, goal) <= stoppingDistance) {
//            velocity -= 2*acc;
//		}
//
//		velocity = (velocity == 0) ? 0.1f : velocity;
//		//rigidbody.transform.position = (Vector3.Lerp (rigidbody.transform.position, goal, velocity * Time.deltaTime / distance));
//       
//        //rigidbody.velocity = 
//        Vector3 force = goal - rigidbody.position;
//        Debug.Log (force.x);
//        rigidbody.AddRelativeForce(force * 1f);
//	}
//}
