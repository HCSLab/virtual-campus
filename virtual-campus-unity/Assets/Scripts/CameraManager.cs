using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject skinPreviewCamera;
    public GameObject skinIconCamera;
    public GameObject bigMapCamera;

    public static CameraManager Instance;
    void Awake()
    {
        Instance = this;
        DisableAllCamera();
    }

    public void DisableAllCamera()
    {
        skinPreviewCamera.SetActive(false);
        skinIconCamera.SetActive(false);
        bigMapCamera.SetActive(false);
    }

}
