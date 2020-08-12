using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parkour : MonoBehaviour
{
    public List<GameObject> pathPoints;
    [HideInInspector]
    public Timer timer;
    [HideInInspector]
    public GameObject parkourCanvas;
    public float timeLimit;
    [HideInInspector]
    public int nextPathPoint;
    [HideInInspector]
    public bool success;

    public static Parkour Instance;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Instance.Failure();
            Instance = this;
        }
        timer = UIManager.Instance.timer;
        parkourCanvas = UIManager.Instance.parkourCanvas;
        success = false;
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
        UIManager.Instance.successText.SetActive(true);
        UIManager.Instance.counddownSFXSource.PlayOneShot(UIManager.Instance.successSFX);
        timer.start = false;
        success = true;
        End();
    }

    public void Failure()
    {
        UIManager.Instance.failureText.SetActive(true);
        UIManager.Instance.counddownSFXSource.PlayOneShot(UIManager.Instance.failureSFX);
        End();
    }

    public void End()
    {
        StartCoroutine(DelayedCloseTimer());
    }

    IEnumerator DelayedCloseTimer()
    {
        yield return new WaitForSeconds(8);
        timer.gameObject.SetActive(false);
        UIManager.Instance.successText.SetActive(false);
        UIManager.Instance.failureText.SetActive(false);
    }
}
