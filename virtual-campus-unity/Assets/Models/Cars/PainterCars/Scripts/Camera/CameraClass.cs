using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClass : MonoBehaviour {

	public Vector3 _position;

	[HideInInspector] public Transform target;
	[HideInInspector] public Transform cameraRotate;

	public virtual void SetPosition () {}
}
