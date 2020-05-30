using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject model;
    public GameObject playerCamera;
    public float cameraRotationSpeed;
    public float cameraScalingSpeed;
    public float mouseSensitivity;
    Animator animator;
    Rigidbody rigidbody;
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
        Vector3 movement = new Vector3(-Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));
        if (movement.magnitude > 0.1f)
        {
            animator.SetBool("Walk", true);
            model.transform.LookAt(model.transform.position + movement);
            model.transform.Rotate(0f, playerCamera.transform.eulerAngles.y, 0f);
        }
        else {
            animator.SetBool("Walk", false);
        }
    }


    Vector3 lastMousePosition;
    void UpdateCamera()
    {
        Vector3 modelEulerAngles = model.transform.eulerAngles;

        playerCamera.transform.position = transform.position + cameraPositionOffset;

        float cameraRotation = 0f;
        if(Input.GetKey(KeyCode.Q)) cameraRotation -= 1f;
        if(Input.GetKey(KeyCode.E)) cameraRotation += 1f;
        if(Input.GetMouseButton(1)){
            cameraRotation = (Input.mousePosition - lastMousePosition).x * mouseSensitivity;
            
        }
        playerCamera.transform.RotateAround(transform.position, Vector3.up, cameraRotation * cameraRotationSpeed);

        cameraPositionOffset = playerCamera.transform.position - transform.position;

        Vector3 newForward = playerCamera.transform.forward;
        newForward.Scale(new Vector3(1f, 0f, 1f));
        transform.forward = newForward;

        playerCamera.GetComponent<Camera>().orthographicSize += -Input.mouseScrollDelta.y * cameraScalingSpeed;

        model.transform.eulerAngles = modelEulerAngles;
    }
}
