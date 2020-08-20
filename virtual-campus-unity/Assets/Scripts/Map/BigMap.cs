using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMap : MonoBehaviour
{
    private void OnEnable()
    {
        CameraManager.Instance.bigMapCamera.SetActive(true);
    }

    private void OnDisable()
    {
        CameraManager.Instance.bigMapCamera.SetActive(false);
    }
}
