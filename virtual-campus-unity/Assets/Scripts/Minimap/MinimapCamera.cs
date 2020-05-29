using UnityEngine;
using System.Collections;

public class MinimapCamera : MonoBehaviour
{
    private Camera minimapCamera;
    //private Transform minimapQuad;
    private Transform player;
    private Vector3 offsetPosition;
    // Use this for initialization
    void Start()
    {
        minimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offsetPosition = minimapCamera.transform.position - player.position;
        //minimapQuad = GameObject.FindGameObjectWithTag("Player").GetComponent<MinimapQuad>().transform;
    }

    void Update()
    {
        minimapCamera.transform.position = offsetPosition + player.position;
    }
    public void ZoomInButtonClick()
    {
        Debug.Log("ZoomIn");
        if (minimapCamera.orthographicSize > 6)
        {
            minimapCamera.orthographicSize -= 3;
            //minimapQuad.localScale /= 2;
        }
    }

    public void ZoomOutButtonClick()
    {
        if (minimapCamera.orthographicSize < 27)
        {
            minimapCamera.orthographicSize += 3;
            //minimapQuad.localScale *= 2;
        }
    }
}
