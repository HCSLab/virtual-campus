using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{

    public float minDistance = 0.2f;
    public float maxDistance = 2.0f;
    public float smooth = 5.0f;
    Vector3 dollyDir;
    public Vector3 dollyDirAdjusted;
    public float distance;

    void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
        StartCoroutine(DelayedSetSmooth());
    }

    private IEnumerator DelayedSetSmooth()
    {
        yield return new WaitForSeconds(2);
        SetSmooth();
    }
    private void SetSmooth()
    {
        smooth = 10;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit))
        {
            distance = Mathf.Clamp(hit.distance * 0.85f, minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);

    }
}