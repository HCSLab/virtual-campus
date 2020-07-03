//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

[ExecuteInEditMode]
public class FollowTransform : MonoBehaviour
{
	public Transform otherTransform;
	public Vector3 positionOffset;
	public Vector3 rotationOffset;

	// Update is called once per frame
	void Update()
	{
		transform.position = otherTransform.position + otherTransform.TransformDirection(positionOffset);
		transform.rotation = otherTransform.rotation * Quaternion.Euler(rotationOffset);
	}
}