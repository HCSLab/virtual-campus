using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class AutoNPCController : MonoBehaviour
{
	[Header("Movement")]
	public float minSpeedFactor;
	public float maxSpeedFactor;
	[Header("Route")]
	public Transform[] checkPoints;
	public float offsetThreshold;
	public bool reverseRoute;
	public bool enableChatting;
	[Range(0f, 1f)] public float chatProbility;
	public float maxChatTime, minChatTime;
	[Header("Model")]
	public GameObject model;

	Animator animator;
	NavMeshAgent navMeshAgent;

	private void Start()
	{
		animator = model.GetComponent<Animator>();
		navMeshAgent = GetComponent<NavMeshAgent>();

		var randomizedWalkSpeedFactor = Random.Range(maxSpeedFactor, minSpeedFactor);
		navMeshAgent.speed *= randomizedWalkSpeedFactor;

		if (reverseRoute)
			Array.Reverse(checkPoints);

		InitializePosition();
	}

	private void Update()
	{
		UpdateNavMeshAgent();
		UpdateAnimationAndRotation();
	}

	// Variables about chatting.
	private bool chatTriggerChecked = false;
	private bool isChatting = false;
	[HideInInspector]
	public float chatCountdown;
	[HideInInspector]
	public GameObject chatTargetModel;
	
	// This property must be updated after other
	// attributes are updated.
	public bool IsChatting
	{
		get
		{
			return isChatting;
		}
		set
		{
			if (value == isChatting)
				return;
			
			// Update the rotation of the model.
			if (value)
				model.transform.LookAt(chatTargetModel.transform);
			else
				LeanTween.rotateLocal(model, Vector3.zero, 0.8f);
			
			isChatting = value;
		}
	}
	
	private void OnTriggerEnter(Collider other)
	{
		// Check whether this NPC can chat.
		if (!enableChatting || isChatting) return;

		// Check whether the collider can chat.
		if (other.tag != "NPC") return;
		var otherController = other.gameObject.GetComponent<AutoNPCController>();
		if (!otherController) return;
		if (!otherController.enableChatting || otherController.isChatting) return;
		if (otherController.chatTriggerChecked) return;

		chatTriggerChecked = true;
		LeanTween.delayedCall(0.5f, () => { chatTriggerChecked = false; });

		// Consider the probility.
		if (Random.value > chatProbility) return;

		chatTargetModel = otherController.model;
		chatCountdown = Random.Range(minChatTime, maxChatTime);
		IsChatting = true; // Remember IsChatting should be updated at last.

		otherController.chatTargetModel = gameObject;
		otherController.chatCountdown = chatCountdown;
		otherController.IsChatting = true;

		LeanTween.value(gameObject, chatCountdown, 0f, chatCountdown)
			.setOnUpdate((float val) => { chatCountdown = val; otherController.chatCountdown = val; })
			.setOnComplete(() => {
				IsChatting = false;
				otherController.IsChatting = false;

				// Disable chatting for 1 seconds
				// to avoid consecutive chats between
				// the same NPCs.
				enableChatting = false;
				otherController.enableChatting = false;
				LeanTween.delayedCall(1f, () => { enableChatting = true; otherController.enableChatting = true; });
			});
	}

	void InitializePosition()
	{
		var initialPosition = checkPoints[0].position;
		initialPosition.y = transform.position.y;
		transform.position = initialPosition;
	}

	// Variables about movement.
	Vector3 nextMovement;
	float angle;
	int nextCheckPointIndex = 0;

	void UpdateNavMeshAgent()
	{
		if (isChatting)
		{
			navMeshAgent.SetDestination(transform.position);
			return;
		}

		var offset = transform.position - checkPoints[nextCheckPointIndex].position;
		offset.Scale(new Vector3(1f, 0f, 1f));
		if (offset.magnitude < offsetThreshold)
			nextCheckPointIndex++;

		if (nextCheckPointIndex == checkPoints.Length - 1)
		{
			Destroy(gameObject);
			return;
		}

		navMeshAgent.SetDestination(checkPoints[nextCheckPointIndex].position);
	}

	void UpdateAnimationAndRotation()
	{
		if (navMeshAgent.velocity.magnitude < 0.01f)
			animator.SetBool("Walk", false);
		else
			animator.SetBool("Walk", true);
	}
}
