using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parkour : MonoBehaviour
{
    public List<GameObject> pathPoints;
    public Timer timer;
    public float timeLimit;
    [HideInInspector]
    public int nextPathPoint;

    void Start()
    {
        
    }

    public void StartParkour()
    {
        timer.StartTiming(timeLimit);
        nextPathPoint = 0;
        pathPoints[0].SetActive(true);
    }

    public void NextPathPoint()
    {
        pathPoints[nextPathPoint].SetActive(false);
        nextPathPoint++;
        pathPoints[nextPathPoint].SetActive(true);
    }
    public void Success()
    {
        
    }

    public void Fail()
    {

    }
    
    void Update()
    {
        //if (pathPoints[nextPathPoint].GetComponent<BoxCollider>())
    }
}
