using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private float CameraMoveSpeed = 120.0f;
    public GameObject CameraFollowObj;
    Vector3 FollowPOS;
    public float ClampAngle = 80.0f;
    public float InputSensitivity = 150.0f;
    public GameObject CameraObj;
    public GameObject PlayerObj;
    public float camDistanceXtoPlayer;
    public float camDistanceYtoPlayer;
    public float camDistanceZtoPlayer;
    public float mouseX;
    public float mouseY;
    public float finalInputX;
    public float finalInputZ;
    public float smoothX;
    public float smoothY;
    private float rotY = 0.0f;
    private float rotX = 0.0f;
    private Vector3 cameraPositionOffset;
    private CameraCollision cameraCollision;
    public float minMaxDist;
    public float maxMaxDist;
    public float zoomSensitivity;

    // Use this for initialization
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraPositionOffset = transform.position - CameraFollowObj.transform.position;
        cameraCollision = CameraObj.GetComponent<CameraCollision>();
    }

    // Update is called once per frame
    void Update()
    {

        //float inputX = Input.GetAxis("RightStickHorizontal");
        //float inputZ = Input.GetAxis("RightStickVertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        //finalInputX = inputX + mouseX;
        //finalInputZ = inputZ + mouseY;
        finalInputX = mouseX;
        finalInputZ = mouseY;

        rotY += finalInputX * InputSensitivity * Time.deltaTime;
        rotX += finalInputZ * InputSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -ClampAngle, ClampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
        Zoom();

        transform.position = CameraFollowObj.transform.position + cameraPositionOffset;
    }

    void LateUpdate()
    {
        CameraUpdater();
    }

    void CameraUpdater()
    {
        Transform target = CameraFollowObj.transform;

        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }


    void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (cameraCollision.maxDistance <= maxMaxDist)
            {
                cameraCollision.maxDistance += zoomSensitivity;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (cameraCollision.maxDistance >= minMaxDist)
            {
                cameraCollision.maxDistance -= zoomSensitivity;
            }
        }
    }

}