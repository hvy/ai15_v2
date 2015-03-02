using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface MovementModel {
	void findPath();
	bool stepPath(Agent agent, Vector3 goal);
	void reset(Vector3 position);
	void reverse(Vector3 goal);
}
