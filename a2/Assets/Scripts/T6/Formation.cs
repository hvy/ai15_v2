using UnityEngine;
using System.Collections;

public interface Formation {
	void updateAgents ();
	GameObject getAgent (int agentId);
}
