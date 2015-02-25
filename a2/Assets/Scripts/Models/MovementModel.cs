using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface MovementModel {
	void findPath();
	void stepPath(Vector3 goal);
	void reset(Vector3 position);
}
