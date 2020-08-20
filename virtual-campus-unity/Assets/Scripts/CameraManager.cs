using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public GameObject skinPreviewCamera;
    public GameObject skinIconCamera;
    public GameObject bigMapCamera;

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
