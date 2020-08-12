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

    void Start()
    {
        if (xMoving)
        {
            transform.position = new Vector3(xMin, transform.position.y, transform.position.z);
            xIncrease = true;
        }
        if (yMoving)
        {
            transform.position = new Vector3(transform.position.x, yMin, transform.position.z);
            yIncrease = true;
        }
        if (zMoving)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zMin);
            zIncrease = true;
        }
    }

    void Update()
    {
        
        if (xMoving)
        {
            if (xIncrease)
            {
                float newX = transform.position.x + Time.deltaTime * xSpeed;
                if (newX >= xMax)
                {
                    newX = xMax;
                    xIncrease = false;
                }
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            }
            else
            {
                float newX = transform.position.x - Time.deltaTime * xSpeed;
                if (newX <= xMin)
                {
                    newX = xMin;
                    xIncrease = true;
                }
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            }
        }
        if (yMoving)
        {
            if (yIncrease)
            {
                float newY = transform.position.y + Time.deltaTime * ySpeed;
                if (newY >= yMax)
                {
                    newY = yMax;
                    yIncrease = false;
                }
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
            else
            {
                float newY = transform.position.y - Time.deltaTime * ySpeed;
                if (newY <= yMin)
                {
                    newY = yMin;
                    yIncrease = true;
                }
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }
        if (zMoving)
        {
            if (zIncrease)
            {
                float newZ = transform.position.z + Time.deltaTime * zSpeed;
                if (newZ >= zMax)
                {
                    newZ = zMax;
                    zIncrease = false;
                }
                transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
            }
            else
            {
                float newZ = transform.position.z - Time.deltaTime * zSpeed;
                if (newZ <= zMin)
                {
                    newZ = zMin;
                    zIncrease = true;
                }
                transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
            }
        }
    }
}
