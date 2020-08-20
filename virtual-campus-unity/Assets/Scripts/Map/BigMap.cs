using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMap : MonoBehaviour
{

    Coroutine disableBigMapCameraCoroutine = null;
    private void OnEnable()
    {
        CameraManager.Instance.bigMapCamera.SetActive(true);
        if (disableBigMapCameraCoroutine == null)
            disableBigMapCameraCoroutine = StartCoroutine(DisableBigMapCamera());
		else
		{
            StopCoroutine(disableBigMapCameraCoroutine);
            disableBigMapCameraCoroutine = StartCoroutine(DisableBigMapCamera());
        }
    }

    IEnumerator DisableBigMapCamera()
	{
        yield return null;
        yield return null;
        yield return null;
        CameraManager.Instance.bigMapCamera.SetActive(false);
        disableBigMapCameraCoroutine = null;
	}
}
