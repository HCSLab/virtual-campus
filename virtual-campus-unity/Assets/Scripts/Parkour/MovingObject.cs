using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public bool xMoving;
    public bool yMoving;
    public bool zMoving;
    public float xSpeed;
    public float ySpeed;
    public float zSpeed;
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;
    public float zMin;
    public float zMax;

    private bool xIncrease;
    private bool yIncrease;
    private bool zIncrease;

    private Rigidbody rb;

    void Start()
    {
        if (xMoving)
        {
            xIncrease = true;
        }
        if (yMoving)
        {
            yIncrease = true;
        }
        if (zMoving)
        {
            zIncrease = true;
        }
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (xMoving)
        {
            if (xIncrease)
            {
                if (transform.position.x >= xMax)
                {
                    xIncrease = false;
                }
                rb.velocity = new Vector3(xSpeed, 0, 0);
            }
            else
            {
                if (transform.position.x <= xMin)
                {
                    xIncrease = true;
                }
                rb.velocity = new Vector3(-xSpeed, 0, 0);
            }
        }

        if (yMoving)
        {
            if (yIncrease)
            {
                if (transform.position.y >= yMax)
                {
                    yIncrease = false;
                }
                rb.velocity = new Vector3(0, ySpeed, 0);
            }
            else
            {
                if (transform.position.y <= yMin)
                {
                    yIncrease = true;
                }
                rb.velocity = new Vector3(0, -ySpeed, 0);
            }
        }

        if (zMoving)
        {
            if (zIncrease)
            {
                if (transform.position.z >= zMax)
                {
                    zIncrease = false;
                }
                rb.velocity = new Vector3(0, 0, zSpeed);
            }
            else
            {
                if (transform.position.z <= zMin)
                {
                    zIncrease = true;
                }
                rb.velocity = new Vector3(0, 0, -zSpeed);
            }
        }
    }
}
