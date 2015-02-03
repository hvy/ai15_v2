using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface MovementModel {
	void findPath();
	void stepPath();
	void reset(Vector3 position);
}
