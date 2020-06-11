using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTransformObject : MonoBehaviour {

	[SerializeField] private Transform originalObject;
	[SerializeField] private Vector3 offset;

	void LateUpdate () {
		transform.position = originalObject.position;
		transform.localPosition += offset;
		transform.rotation = originalObject.rotation;
	}
}
