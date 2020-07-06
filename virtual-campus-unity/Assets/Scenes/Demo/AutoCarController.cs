using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCarController : MonoBehaviour
{
	public enum LoopType
	{
		Loop,
		RestartOnFinish,
		None
	};

	[Header("Car Parameters")]
	[SerializeField] private float maxSpeed;
	[SerializeField] private float powerEngine;
	[SerializeField] private float brakePower;
	[SerializeField] private Transform COM;
	[SerializeField] private Wheels[] wheels;

	[Header("Sounds")]
	[SerializeField] private AudioClip StartEngineClip;
	[SerializeField] private AudioClip WorkingEngineClip;

	Rigidbody rb;
	AudioSource audioSource;
	bool engineWorking;

	[System.Serializable]
	public class Wheels
	{
		[Space(20), Header("Parametrs Wheel")]
		public WheelCollider wheelCollider;
		public GameObject wheelObject;
		public float angleTurningWheel;
		[Range(0, 100)] public float percentMotorPower;
		[HideInInspector] public float wheelPower;
	}

	[Header("Route")]
	public int startingPointIndex;
	public float offsetThreshold, steerThreshold;
	public LoopType loopType;
	public Transform[] checkPoints;


	[Header("Auto Brake")]
	public Transform pedestrianDetector;

	float initialY;
	int nextCheckPointIndex;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();

		// Resetting the COM causes a weird bug
		// in the bus model of PainterCars
		rb.centerOfMass = COM.localPosition;

		initialY = transform.position.y;
		nextCheckPointIndex = (startingPointIndex + 1) % checkPoints.Length;

		StartEngine();
		InitializePositionAndRotation();
	}

	void FixedUpdate()
	{
		CalculateInput();
		UpdateWheels();
	}

	// Pedestrian detection
	[HideInInspector]
	public int carAheadCounter = 0;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Car") return;
		if (other.gameObject.GetInstanceID() < GetInstanceID())
		{
			carAheadCounter++;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag != "Car") return;
		LeanTween.delayedCall(1f, () => { carAheadCounter = Mathf.Max(0, carAheadCounter - 1); });
	}

	void InitializePositionAndRotation()
	{
		var initialPosition = checkPoints[startingPointIndex].position;
		initialPosition.y = initialY;
		transform.position = initialPosition;

		transform.LookAt(checkPoints[nextCheckPointIndex]);
	}

	public void StartEngine()
	{
		StartCoroutine(StartEngineCor());
	}

	public void StopEngine()
	{
		audioSource.Stop();
		engineWorking = false;
	}

	IEnumerator StartEngineCor()
	{
		if (StartEngineClip != null)
		{
			audioSource.clip = StartEngineClip;
			audioSource.Play();
			yield return new WaitForSeconds(StartEngineClip.length);
		}
		if (WorkingEngineClip != null)
		{
			audioSource.clip = WorkingEngineClip;
			audioSource.loop = true;
			audioSource.Play();
		}
		engineWorking = true;
	}

	// Inputs to the car from the controller.
	float inputPower;
	float inputSteerAngle;
	float currentMaxSpeed;

	void CalculateInput()
	{
		var offset = transform.position - checkPoints[nextCheckPointIndex].position;
		offset.Scale(new Vector3(1f, 0f, 1f));
		if (offset.magnitude < offsetThreshold)
			nextCheckPointIndex = (nextCheckPointIndex + 1) % checkPoints.Length;

		if (nextCheckPointIndex == checkPoints.Length - 1)
		{
			if (loopType == LoopType.None)
			{
				Destroy(gameObject);
				return;
			}
			else if (loopType == LoopType.RestartOnFinish)
			{
				startingPointIndex = 0;
				nextCheckPointIndex = 1;
				InitializePositionAndRotation();
			}
			else
			{
				// Do nothing, because the car is looping by default.
			}
		}

		var nextCheckPoint = checkPoints[nextCheckPointIndex];
		var carDir = transform.forward;
		var targetDir = nextCheckPoint.position - transform.position;
		var angle = Vector3.SignedAngle(carDir, targetDir, Vector3.up);

		// Debug.Log("Angle: " + angle + "; nextCheckPointIndex: " + nextCheckPointIndex);

		if (Mathf.Abs(angle) < steerThreshold)
		{
			inputPower = 1;
			inputSteerAngle = angle;
			currentMaxSpeed = maxSpeed;
		}
		else
		{
			inputPower = 1;
			inputSteerAngle = angle;
			currentMaxSpeed = 0.5f * maxSpeed;
		}

		if (carAheadCounter > 0)
		{
			inputPower = 0;
			currentMaxSpeed = 0f;
		}

		// Update Pedestrian Detector.
		var toLookAt = nextCheckPoint.position;
		if (nextCheckPointIndex + 1 < checkPoints.Length)
		{
			toLookAt += checkPoints[nextCheckPointIndex + 1].position;
			toLookAt *= 0.5f;
		}
		toLookAt.y = pedestrianDetector.transform.position.y;
		pedestrianDetector.LookAt(toLookAt);
	}

	void UpdateWheels()
	{
		Vector3 position;
		Quaternion rotation;
		float currentSpeed = rb.velocity.magnitude;

		for (int i = 0; i < wheels.Length; i++)
		{
			if (wheels[i].wheelCollider == null) continue;

			// Check speed limit.
			if (currentSpeed > currentMaxSpeed)
			{
				wheels[i].wheelPower = 0;
				WheelBrake(wheels[i].wheelCollider);
				wheels[i].wheelCollider.motorTorque = 0f;
			}
			else
			{
				wheels[i].wheelPower = powerEngine * (wheels[i].percentMotorPower * 0.1f);
				
				// Update motor.
				if (engineWorking)
				{
					float inputVertical = inputPower;
					if (wheels[i].wheelCollider.rpm < 0.01f && inputVertical < 0f || wheels[i].wheelCollider.rpm >= -0.01f && inputVertical >= 0f)
					{
						wheels[i].wheelCollider.brakeTorque = 0;
						wheels[i].wheelCollider.motorTorque = inputVertical * wheels[i].wheelPower;
					}
					else
					{
						wheels[i].wheelCollider.motorTorque = 0;
						WheelBrake(wheels[i].wheelCollider);
					}
				}
				else
				{
					wheels[i].wheelCollider.motorTorque = 0;
				}
			}

			// Update steering.
			if (inputSteerAngle > 0f)
				wheels[i].wheelCollider.steerAngle = Mathf.Min(wheels[i].angleTurningWheel, inputSteerAngle);
			else
				wheels[i].wheelCollider.steerAngle = Mathf.Max(-wheels[i].angleTurningWheel, inputSteerAngle);

			// Update the mesh of the wheel.
			wheels[i].wheelCollider.GetWorldPose(out position, out rotation);
			wheels[i].wheelObject.transform.position = position;
			wheels[i].wheelObject.transform.localPosition -= wheels[i].wheelCollider.center;
			wheels[i].wheelObject.transform.rotation = rotation;
		}

		// Update the engine sound.
		if (audioSource != null)
			audioSource.pitch = 1 + (currentSpeed * 0.03f);
	}

	void WheelBrake(WheelCollider wheelCollider)
	{
		wheelCollider.brakeTorque = brakePower;
	}
}
