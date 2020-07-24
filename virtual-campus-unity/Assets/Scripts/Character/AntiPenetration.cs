using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AntiPenetration : MonoBehaviour
{
    public float m_distanceAway = 4.5f; //Horizontal distance
    public float m_distanceUp = 1.5f; //Vertical distance
    public float m_smooth = 5f;
    public Transform m_player;
    private Transform m_transsform;

    private float maxFieldOfView = 80f;
    private float minFieldOfView = 30f;

    private float maxSD = 20.0f;
    private float minSD = 1f;
    public float offsetY; //offsetY = focus.y - player.y
    public Transform cameraFollow;

    public Camera mainCamera;

    [HideInInspector]
    public float sqrDist;

    void Start()
    {
        m_transsform = this.transform;
        sqrDist = m_distanceAway*m_distanceAway + m_distanceUp*m_distanceUp;
    }


    void Update()
    {
        Zoom();
        CameraSet();
        RaycastHit hit;
        if (Physics.Linecast(m_player.position + Vector3.up, m_transsform.position, out hit))
        {
            string name = hit.collider.gameObject.tag;
            if (name != "MainCamera")
            {
                float currentDistance = Vector3.Distance(hit.point, m_player.position);
                if (currentDistance < m_distanceAway)
                {
                    m_transsform.position = hit.point;
                }
            }
        }
    }

    void CameraSet()
    {
        float m_wangtedRotationAngel = m_player.transform.eulerAngles.y;
        float m_wangtedHeight = m_player.transform.position.y + m_distanceUp;
        float m_currentRotationAngle = m_transsform.eulerAngles.y;
        float m_currentHeight = m_transsform.position.y;
        m_currentRotationAngle = Mathf.LerpAngle(m_currentRotationAngle, m_wangtedRotationAngel, m_smooth * Time.deltaTime);
        m_currentHeight = Mathf.Lerp(m_currentHeight, m_wangtedHeight, m_smooth * Time.deltaTime);
        Quaternion m_currentRotation = Quaternion.Euler(0, m_currentRotationAngle, 0);
        Vector3 m_position = m_player.transform.position + new Vector3(0f, offsetY, 0f);
        m_position -= m_currentRotation * Vector3.forward * m_distanceAway;
        m_position = new Vector3(m_position.x, m_currentHeight, m_position.z);
        m_transsform.position = Vector3.Lerp(m_transsform.position, m_position, Time.time);
        m_transsform.LookAt(cameraFollow);
    }
    void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (mainCamera.fieldOfView <= maxFieldOfView)
            {
                mainCamera.fieldOfView += 2;
            }
            if (mainCamera.orthographicSize <= maxSD)
            {
                mainCamera.orthographicSize += 0.5f;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (mainCamera.fieldOfView >= minFieldOfView)
            {
                mainCamera.fieldOfView -= 2;
            }
            if (mainCamera.orthographicSize >= minSD)
            {
                mainCamera.orthographicSize -= 0.5f;
            }
        }
    }
}