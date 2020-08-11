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

    public static Parkour Instance;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Instance.Fail();
            Instance = this;
        }
        StartParkour();
    }

    public void StartParkour()
    {
        timer.gameObject.SetActive(true);
        timer.StartTiming(timeLimit);
        nextPathPoint = 0;
        pathPoints[0].SetActive(true);
    }

    public void NextPathPoint()
    {
        pathPoints[nextPathPoint].SetActive(false);
        nextPathPoint++;
        if (nextPathPoint == pathPoints.Count)
        {
            Success();
            return;
        }
        pathPoints[nextPathPoint].SetActive(true);
    }
    public void Success()
    {
        End();
    }

    public void Fail()
    {
        End();
    }

    public void End()
    {
        timer.gameObject.SetActive(false);
    }
    
    void Update()
    {
        //if (pathPoints[nextPathPoint].GetComponent<BoxCollider>())
    }

}
