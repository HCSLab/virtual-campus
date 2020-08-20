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

    [HideInInspector] public int isInWater = 0;
    public ParticleSystem sprayParticle;

    [HideInInspector]
    public bool isTakingPhoto = false;

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
            UpdateSpray();
            UpdateHotkeys();
        }

        lastMousePosition = Input.mousePosition;
    }
    void UpdateHotkeys()
    {
        if (GetMapInput())
        {
            if (!UIManager.Instance.fullScreenMapCanvas.activeSelf)
            {
                UIManager.Instance.fullScreenMapCanvas.SetActive(true);
            }
            else
            {
                UIManager.Instance.fullScreenMapCanvas.SetActive(false);
            }
        }
        else if (GetBagInput())
        {
            if (!UIManager.Instance.tabMenuCanvas.activeSelf)
            {
                UIManager.Instance.OpenTab(ItemPanel.Instance.gameObject);
            }
            else
            {
                UIManager.Instance.CloseCanvas();
            }
        }
        else if (GetSettingsInput())
        {
            if (isTakingPhoto)
            {
                return;
            }
            else if (UIManager.Instance.fullScreenMapCanvas.activeSelf)
            {
                UIManager.Instance.fullScreenMapCanvas.SetActive(false);
                return;
            }
            if (!UIManager.Instance.tabMenuCanvas.activeSelf)
            {
                UIManager.Instance.OpenTab(SettingsPanel.Instance.gameObject);
                UIManager.Instance.resetSettingsPanel();
            }
            else
            {
                UIManager.Instance.CloseCanvas();
            }
        }
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

    void UpdateSpray()
    {
        if (isInWater > 0)
        {
            if (!sprayParticle.isPlaying)
                sprayParticle.Play();
        }
        else
        {
            if (sprayParticle.isPlaying)
                sprayParticle.Stop();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
            isInWater += 1;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water")
            isInWater -= 1;
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

    public bool GetMapInput()
    {
        return Input.GetKeyDown(KeyCode.M);
    }

    public bool GetBagInput()
    {
        return Input.GetKeyDown(KeyCode.B);
    }

    public bool GetSettingsInput()
    {
        return Input.GetKeyDown(KeyCode.Escape);
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
    }
}
