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
    private AntiPenetration antiPene;
    public float Ysensitivity;

    void Start()
    {
        animator = model.GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        lastMousePosition = Input.mousePosition;
        antiPene = playerCamera.GetComponent<AntiPenetration>();
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

        float cameraRotation = 0f;
        float cameraRotationY = 0f;
        
        if (Input.GetKey(KeyCode.Q)) cameraRotation -= 1f;
        if(Input.GetKey(KeyCode.E)) cameraRotation += 1f;
        if (Input.GetMouseButton(1))
        {
            cameraRotation = (Input.mousePosition - lastMousePosition).x * mouseSensitivity;
            cameraRotationY = (Input.mousePosition - lastMousePosition).y * mouseSensitivity;
        }

        playerCamera.transform.RotateAround(transform.position + new Vector3(0f, 0.5f, 0f), Vector3.up, cameraRotation * cameraRotationSpeed);

        if (cameraRotationY > 0.3f || cameraRotationY < -0.3f)
        {
            if (antiPene.m_distanceUp >= 1 && antiPene.m_distanceUp <= 4.5)
            {
                antiPene.m_distanceUp += cameraRotationY * Ysensitivity;
                antiPene.m_distanceAway = Mathf.Sqrt(antiPene.sqrDist - antiPene.m_distanceUp * antiPene.m_distanceUp);
                if (antiPene.m_distanceUp < 1)
                {
                    antiPene.m_distanceUp = 1;
                    antiPene.m_distanceAway = Mathf.Sqrt(antiPene.sqrDist - antiPene.m_distanceUp * antiPene.m_distanceUp);
                }
                else if (antiPene.m_distanceUp > 4.5)
                {
                    antiPene.m_distanceUp = 4.5f;
                    antiPene.m_distanceAway = Mathf.Sqrt(antiPene.sqrDist - antiPene.m_distanceUp * antiPene.m_distanceUp);
                }
             }
        }

        Vector3 newForward = playerCamera.transform.forward;
        newForward.Scale(new Vector3(1f, 0f, 1f));
        transform.forward = newForward;

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
