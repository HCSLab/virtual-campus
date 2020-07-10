using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public GameObject model;
    public GameObject playerCamera;
    public GameObject minimapCamera;
    public float cameraRotationSpeed;
    public float cameraScalingSpeed;
    public float mouseSensitivity;
    Animator animator;
    new Rigidbody rigidbody;
    Vector3 cameraPositionOffset;

    void Start()
    {
        animator = model.GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        cameraPositionOffset = playerCamera.transform.position - transform.position;
        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
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
                animator.SetBool("Walk", true);
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
        if(Input.GetKey(KeyCode.Q)) cameraRotation -= 1f;
        if(Input.GetKey(KeyCode.E)) cameraRotation += 1f;
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
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public bool GetJumpInput()
	{
        return Input.GetKeyDown(KeyCode.Space);
	}

    public bool GetSprintInput()
	{
        return Input.GetKey(KeyCode.LeftShift);
	}
}
