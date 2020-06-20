using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AutoNPCController : MonoBehaviour
{
	[Header("Movement")]
	public float rotateSpeed;
	[Header("Route")]
	public Transform[] checkPoints;
	public float offsetThreshold, smoothSteerThreshold;
	public bool reverseRoute;
	public bool enableChatting;
	[Range(0f, 1f)] public float chatProbility;
	public float maxChatTime, minChatTime;
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

		if (reverseRoute)
			Array.Reverse(checkPoints);

		InitializePositionAndRotation();
	}

	// Variables about chatting.
	bool chatTriggerChecked = false;
	bool isChatting = false;
	float chatCountdown;
	GameObject chatTarget;
	private void OnTriggerEnter(Collider other)
	{
		// Check whether this NPC can chat.
		if (!enableChatting || isChatting) return;

		// Check whether the collider can chat.
		if (other.tag != "NPC") return;
		var otherController = other.gameObject.GetComponent<AutoNPCController>();
		if (!otherController.enableChatting || otherController.isChatting) return;
		if (otherController.chatTriggerChecked) return;

		chatTriggerChecked = true;
		LeanTween.delayedCall(0.5f, () => { chatTriggerChecked = false; });

		// Consider the probility.
		if (Random.value > chatProbility) return;

		isChatting = true;
		chatCountdown = Random.Range(minChatTime, maxChatTime);
		chatTarget = other.gameObject;

		otherController.isChatting = true;
		otherController.chatCountdown = chatCountdown;
		otherController.chatTarget = gameObject;

		LeanTween.value(gameObject, chatCountdown, 0f, chatCountdown)
			.setOnUpdate((float val) => { chatCountdown = val; otherController.chatCountdown = val; })
			.setOnComplete(() => { isChatting = false; otherController.isChatting = false; })
			;
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

	// Variables about movement.
	Vector3 nextMovement;
	float angle;
	int nextCheckPointIndex = 0;

	public Vector3 GetInput()
	{
		if (isChatting)
		{
			nextMovement = Vector3.zero;
			UpdateAnimationAndRotation();
			return Vector3.zero;
		}

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
		if (isChatting)
		{
			animator.SetBool("Walk", false);
			model.transform.LookAt(chatTarget.transform);
			return;
		}

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
