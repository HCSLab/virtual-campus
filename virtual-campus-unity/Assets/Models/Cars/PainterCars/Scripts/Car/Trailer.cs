using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trailer : MonoBehaviour {
	[SerializeField] private WheelCollider[] wheelColliders;
	[SerializeField] private GameObject[] wheelObjects;
	[SerializeField] private GameObject COM;
	[SerializeField] private CarInputController truckController;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass	= COM.transform.localPosition;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!truckController.ReadyMove) return;
		Vector3 position;
		Quaternion rotation;
		if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f && Mathf.Abs(wheelColliders[1].rpm) < 0.001f) {
			wheelColliders[1].motorTorque = Input.GetAxis("Vertical");
		} else {
			wheelColliders[1].motorTorque = 0;
		}
		for (int i = 0; i < wheelColliders.Length; i ++) {
			wheelColliders[i].GetWorldPose(out position, out rotation);
			wheelObjects[i].transform.position = position;
			wheelObjects[i].transform.localPosition -= wheelColliders[i].center;
			wheelObjects[i].transform.rotation = rotation;
		}
		
	}
}
