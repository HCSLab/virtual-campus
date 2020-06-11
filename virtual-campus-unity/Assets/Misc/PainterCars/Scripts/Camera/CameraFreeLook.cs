using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFreeLook : CameraClass {

	[SerializeField] private float sensitivity = 1;
    private float cameraDistance = -5;
    private GameObject cameraGO;
    private RaycastHit rayHit;
    void Start () {
        cameraGO = GetComponentInChildren<Camera>().gameObject;
	}
	
	void FixedUpdate () {
        transform.position = target.transform.position;
        if (!Input.GetKey(KeyCode.LeftControl)){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * sensitivity, Vector3.up);
            transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * sensitivity, Vector3.left);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                cameraDistance += Input.GetAxis("Mouse ScrollWheel");
            }
            cameraGO.transform.localPosition = new Vector3(0, 0, cameraDistance);
            if (Physics.Linecast(transform.position, cameraGO.transform.position, out rayHit, LayerMask.GetMask("Ground")))
            {
                if (rayHit.distance > cameraDistance)
                {
                    cameraGO.transform.position = rayHit.point;
                    cameraGO.transform.localPosition = new Vector3(0, 0, cameraGO.transform.localPosition.z + 0.1f);
                }
            }
        } else{
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OnDisable(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
