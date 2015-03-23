using UnityEngine;
using System.Collections;

public class RigidbodyDynamicCarTest : MonoBehaviour {

	public float width, height;
	public int motionModelId;

	private GameObject carObj;
	private Agent carAgent;

	void Start () {

		StageFactory sf = new StageFactory ();
		sf.createStage (width, height);
		CameraModel.updateOrthoPosition(width, Camera.main.transform.position.y, height);

		this.carObj = AgentFactory.createCarAgent (false);
		this.carAgent = carObj.GetComponent<Agent> ();

		carObj.transform.position = new Vector3 (width / 2.0f, 0, height / 2.0f);

		carAgent.setStart (carObj.transform.position);
		carAgent.setGoal (carObj.transform.position);
		carAgent.setModel (motionModelId);
	}

	void Update () {

		if (Input.GetButtonDown ("Fire1")) {
			
			Plane plane = new Plane (Vector3.up, 0);
			float dist;
			Ray ray = Camera.mainCamera.ScreenPointToRay (Input.mousePosition);
			if (plane.Raycast (ray, out dist)) {
				Vector3 destinationPos = ray.GetPoint (dist);
				Vector3 currentPos = carObj.transform.position;
				Vector3 velocity = Vector3.Normalize (destinationPos - currentPos);

				carAgent.setStart (currentPos);
				carAgent.setGoal (destinationPos);
				carAgent.setModel (motionModelId);
			}
		}
	}
}
