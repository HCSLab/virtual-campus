using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AutoNPCController : MonoBehaviour
{
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

		var randomizedWalkSpeedFactor = Random.Range(0.8f, 1.2f);
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
			.setOnComplete(() => {
				isChatting = false;
				otherController.isChatting = false;
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
		if (isChatting)
			model.transform.LookAt(chatTarget.transform);
		else
			model.transform.localRotation = Quaternion.identity;
		if (navMeshAgent.velocity.magnitude < 0.01f)
			animator.SetBool("Walk", false);
		else
			animator.SetBool("Walk", true);
	}

	public Vector3 GetInput()
	{
		return Vector3.zero;
	}
}
