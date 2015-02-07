using UnityEngine;
using System.Collections;

public class PathManager : MonoBehaviour {


	public void optimizePath() {
		PathFinding.optimizeCurrentPath(StageManager.polygons);
	}

	public void RRT() {
		PathFinding.RRT(StageManager.polygons);
	}
}
