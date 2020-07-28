using UnityEngine;
using System.Collections;

public class MinimapCamera : MonoBehaviour
{
    protected Camera minimapCamera;
    //private Transform minimapQuad;
    protected Transform player;
    private Vector3 offsetPosition;
    // Use this for initialization
    void Start()
    {
        minimapCamera = GetComponent<Camera>();
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
