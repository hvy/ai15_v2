using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public void setAgentModel(int type) {
		GameObject.FindWithTag ("Agent").GetComponent<Agent> ().setModel (type);
	}
}
