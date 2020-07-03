//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

public class SimpleMoveController : MonoBehaviour
{

	public float maxMoveDelta = 5.0f;
	public float clampY = 180.0f;

	Vector2 _deltaMouse;
	Vector2 _smoothMouse;
	CharacterController _characterController;

	public Vector2 sensitivity = new Vector2(2.0f, 2.0f);
	public Vector2 smoothing = new Vector2(3.0f, 3.0f);
	public Vector2 moveSpeed = new Vector2(1.0f, 1.0f);

	public bool enablePitch = true;
	public bool enableYaw = true;
	public bool enableMove = true;

	private void Awake()
	{
		_characterController = GetComponent<CharacterController>();
		_deltaMouse = new Vector2(transform.eulerAngles.y, transform.eulerAngles.x);
	}

	private void Update()
	{
		if (Cursor.visible) return;

		var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
		mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));
		_smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
		_smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);
		_deltaMouse += _smoothMouse;

		_deltaMouse.y = Mathf.Clamp(_deltaMouse.y, -clampY * 0.5f, clampY * 0.5f);

		ApplyRotatation();

		if (enableMove)
		{
			var moveX = Input.GetKey(KeyCode.A) ? -moveSpeed.x : Input.GetKey(KeyCode.D) ? moveSpeed.x : 0.0f;
			var moveY = Input.GetKey(KeyCode.W) ? moveSpeed.y : Input.GetKey(KeyCode.S) ? -moveSpeed.y : 0.0f;
			var deltaPos = transform.right * moveX + transform.forward * moveY;
			deltaPos.y = 0.0f;
			_characterController.SimpleMove(deltaPos);
		}
	}

	public void SetRotation(Quaternion rotation)
	{
		_deltaMouse.x = rotation.eulerAngles.y;
		var pitch = rotation.eulerAngles.x;
		_deltaMouse.y = -pitch;

		ApplyRotatation();
	}

	private void ApplyRotatation()
	{
		while (_deltaMouse.x < -180) _deltaMouse.x += 360;
		while (_deltaMouse.x > 180) _deltaMouse.x -= 360;

		while (_deltaMouse.y < -180) _deltaMouse.y += 360;
		while (_deltaMouse.y > 180) _deltaMouse.y -= 360;

		if (enablePitch)
		{
			var xRotation = Quaternion.AngleAxis(-_deltaMouse.y, Vector3.right);
			transform.localRotation = xRotation;
		}
		else
		{
			transform.localRotation = Quaternion.identity;
		}

		if (enableYaw)
		{
			var yRotation = Quaternion.AngleAxis(_deltaMouse.x, transform.InverseTransformDirection(Vector3.up));
			transform.localRotation *= yRotation;
		}
	}
}
