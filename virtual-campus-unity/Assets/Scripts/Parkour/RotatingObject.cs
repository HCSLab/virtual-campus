using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    public float angularSpeed;
    public bool horizontal;
    public bool vertical;

    private void Update()
    {
        if (horizontal)
        {
            GetComponent<Rigidbody>().angularVelocity = transform.up * angularSpeed;
            
        }
        else if (vertical)
        {
            GetComponent<Rigidbody>().angularVelocity = transform.forward * angularSpeed;
        }
        else
        {
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
}
