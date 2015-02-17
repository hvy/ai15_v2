using UnityEngine;
using System.Collections;

public class SecondCamera : MonoBehaviour {

    Vector3 offset;

	// Use this for initialization
	void Start () {

        offset = transform.position;
	
	}
	
	// Update is called once per frame
	void Update () {


	
	}

    void LateUpdate() {

        transform.position = GameObject.FindWithTag ("Agent").transform.position + offset;
        transform.LookAt(GameObject.FindWithTag ("Agent").transform);
        //Quaternion newRot = Quaternion.Euler(new Vector3(transform.rotation.x, GameObject.FindWithTag ("Agent").transform.rotation.y, GameObject.FindWithTag ("Agent").transform.rotation.z));
        //transform.rotation = newRot;
    }
}
