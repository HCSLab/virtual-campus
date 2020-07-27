using UnityEngine;
using System.Collections;

public class Reset : MonoBehaviour 
{
    private Vector3 startPos;
    private Quaternion startRot;

	// Use this for initialization
	void Start () 
    {
        startPos = transform.position;
        startRot = transform.rotation;	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            transform.position = startPos;
            transform.rotation = startRot;
        }	
	}
}
