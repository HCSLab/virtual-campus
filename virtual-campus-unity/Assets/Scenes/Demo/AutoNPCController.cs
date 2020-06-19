using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoNPCController : MonoBehaviour
{
	[Header("Movement")]
	public float rotateSpeed;
	[Header("Route")]
	public Transform[] checkPoints;
	public float offsetThreshold, smoothSteerThreshold;
	[Header("Model")]
	public GameObject model;

	[HideInInspector]
	public float randomizedWalkSpeedFactor;
	Animator animator;
	Rigidbody rb;

	private void Awake()
	{
		randomizedWalkSpeedFactor = Random.Range(0.8f, 1.2f);
	}

	private void Start()
	{
		animator = model.GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();

		InitializePositionAndRotation();
	}

	void InitializePositionAndRotation()
	{
		var initialPosition = checkPoints[0].position;
		initialPosition.y = transform.position.y;
		transform.position = initialPosition;

		var dir = checkPoints[1].position - model.transform.position;
		dir.Scale(new Vector3(1f, 0f, 1f));
		model.transform.forward = dir;
	}

	Vector3 nextMovement;
	float angle;
	int nextCheckPointIndex = 0;

	public Vector3 GetInput()
	{
		var offset = transform.position - checkPoints[nextCheckPointIndex].position;
		offset.Scale(new Vector3(1f, 0f, 1f));
		if (offset.magnitude < offsetThreshold)
			nextCheckPointIndex++;

		if (nextCheckPointIndex == checkPoints.Length - 1)
		{
			Destroy(gameObject);
			return Vector2.zero;
		}

		var nextCheckPoint = checkPoints[nextCheckPointIndex];
		var bodyDir = model.transform.forward;
		var targetDir = nextCheckPoint.position - transform.position;

		// By default, the forward of the model is not the
		// direction that it is facing.
		angle = Vector3.SignedAngle(bodyDir, targetDir, Vector3.up);

		targetDir.Scale(new Vector3(1f, 0f, 1f));
		nextMovement = targetDir.normalized;

		UpdateAnimationAndRotation();
		
		return nextMovement;
	}

	void UpdateAnimationAndRotation()
	{
		// Update animation.
		if (nextMovement.magnitude < 0.5f)
		{
			animator.SetBool("Walk", false);
			return;
		}
		animator.SetBool("Walk", true);

		// Update rotation.
		if (Mathf.Abs(angle) > smoothSteerThreshold)
		{
			model.transform.Rotate(Vector3.up, angle < 0 ? -rotateSpeed : rotateSpeed);
		}
		else
		{
			model.transform.Rotate(Vector3.up, angle);
		}
	}
}
