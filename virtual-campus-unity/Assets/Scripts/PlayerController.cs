using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public GestureControl4Game gesture;
    public GameObject model;
    public GameObject playerCamera;
    public GameObject minimapCamera;
    public float cameraRotationSpeed;
    public float cameraScalingSpeed;
    public float mouseSensitivity;
    public float jumpSpeed;
    public GameObject bodySprite;
    Animator animator;
    Rigidbody rigidbody;
    Vector3 cameraPositionOffset;
    string ges;
    void Start()
    {
        animator = model.GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        cameraPositionOffset = playerCamera.transform.position - transform.position;
        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        ges = gesture.GetGesture();
        UpdateCamera();
        UpdatePlayerRotationAndAnimation();
        lastMousePosition = Input.mousePosition;
    }

    void UpdatePlayerRotationAndAnimation()
    {
        var movement2D = GetMovementInput();
        Vector3 movement = Vector3.zero;
        movement.x = -movement2D.y;
        movement.z = movement2D.x;
        if (movement.magnitude > 0.1f)
        {
            if (GetSprintInput())
            {
                animator.SetBool("Run", true);
                animator.SetBool("Walk", false);
            }
            else {
                animator.SetBool("Walk", true);
                animator.SetBool("Run", false);
            }
            model.transform.LookAt(model.transform.position + movement);
            model.transform.Rotate(0f, playerCamera.transform.eulerAngles.y + 90f, 0f);
        }
        else {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }
    }

    Vector3 lastMousePosition;
    void UpdateCamera()
    {
        playerCamera.transform.position = transform.position + cameraPositionOffset;

        float cameraRotation = 0f;
        //if(Input.GetKey(KeyCode.Q)) cameraRotation -= 1f;
        //if(Input.GetKey(KeyCode.E)) cameraRotation += 1f;
        if (gesture.GetGesture()=="RotateQ") cameraRotation -= 0.5f;
        if (gesture.GetGesture() == "RotateE") cameraRotation += 0.5f;
        //if (Input.GetMouseButton(0)) minimapCamera.GetComponent<MinimapCamera>().ZoomInButtonClick();
        if (Input.GetMouseButton(1))
            cameraRotation = (Input.mousePosition - lastMousePosition).x * mouseSensitivity;
       
        playerCamera.transform.RotateAround(transform.position, Vector3.up, cameraRotation * cameraRotationSpeed);

        cameraPositionOffset = playerCamera.transform.position - transform.position;

        Vector3 newForward = playerCamera.transform.forward;
        newForward.Scale(new Vector3(1f, 0f, 1f));
        transform.forward = newForward;

        playerCamera.GetComponent<Camera>().orthographicSize += -Input.mouseScrollDelta.y * cameraScalingSpeed;
    }

    public Vector2 GetMovementInput()
	{
        if (ges == "Right")
            return new Vector2(1, 0);
        else if (ges == "Left")
            return new Vector2(-1, 0);
        else if (ges == "Forward")
            return new Vector2(0, 1);
        else if (ges == "Backward")
            return new Vector2(0, -1);
        else
            return new Vector2(0, 0);
    }

    public bool GetJumpInput()
	{
        if (ges == "Up")
            return true;
        else
            return false;

	}

    public bool GetSprintInput()
	{
        return Input.GetKey(KeyCode.LeftShift);
	}
}
