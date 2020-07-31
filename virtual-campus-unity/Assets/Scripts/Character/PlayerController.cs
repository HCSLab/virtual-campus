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
    public float Ysensitivity;
    private Vector3 forward;

    private bool active = true;

    void Start()
    {
        animator = model.GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        if (active)
        {
            UpdateCamera();
            UpdatePlayerRotationAndAnimation();
        }

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
            transform.forward = forward;
            //playerCamera.GetComponent<ThirdPersonCamera.DisableFollow>().moving = true;
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
            //playerCamera.GetComponent<ThirdPersonCamera.DisableFollow>().moving = false;
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }
    }


    Vector3 lastMousePosition;

    void UpdateCamera()
    {
        float cameraRotation = 0f;
        
        if (Input.GetKey(KeyCode.Q)) cameraRotation -= 1f;
        if(Input.GetKey(KeyCode.E)) cameraRotation += 1f;

        playerCamera.transform.RotateAround(transform.position + new Vector3(0f, 0.5f, 0f), Vector3.up, cameraRotation * cameraRotationSpeed);

        forward = playerCamera.transform.forward;
        forward.Scale(new Vector3(1f, 0f, 1f));

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

    public void FreezeUnfreezePlayer(bool isFreeze)
    {
        active = !isFreeze;
        if (active == false)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }

        GetComponent<ScriptedFirstPersonAIO>().playerCanMove = !isFreeze;
        GetComponent<Rigidbody>().isKinematic = isFreeze;
    }
}
