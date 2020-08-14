using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parkour : MonoBehaviour
{
    public GameObject startPoint;
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
    //public GameObject paths;

    public Material pathPointMaterial;
    public Material endPointMaterial;

    public static Parkour Instance;

    private bool alphaIncrease;
    public float alphaSpeed;
    public float maxAlpha;
    public float minAlpha;

    void Start()
    {
        Init();
    }

    void Init()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
        foreach (GameObject pathPoint in pathPoints)
        {
            pathPoint.SetActive(false);
        }
        startPoint.SetActive(true);
        UIManager.Instance.promptText.SetActive(true);
        timer = UIManager.Instance.timer;
        parkourCanvas = UIManager.Instance.parkourCanvas;
        success = false;
        timer.gameObject.SetActive(false);
        UIManager.Instance.successText.SetActive(false);
        UIManager.Instance.failureText.SetActive(false);
        alphaIncrease = false;
        Color c = pathPointMaterial.color;
        Color d = endPointMaterial.color;
        c.a = (maxAlpha + minAlpha) / 2;
        d.a = (maxAlpha + minAlpha) / 2;
        pathPointMaterial.color = c;
        endPointMaterial.color = d;
    }

    public void StartParkour()
    {
        timer.gameObject.SetActive(true);
        timer.StartTiming(timeLimit);
        timer.giveUpButton.SetActive(true);
        nextPathPoint = 0;
        pathPoints[0].SetActive(true);
        startPoint.SetActive(false);
        UIManager.Instance.promptText.SetActive(false);
    }

    public void NextPathPoint()
    {
        pathPoints[nextPathPoint].GetComponent<ParkourPath>().pathPoint.SetActive(false);
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
        timer.giveUpButton.SetActive(false);
        if (nextPathPoint < pathPoints.Count)
        {
            pathPoints[nextPathPoint].GetComponent<ParkourPath>().pathPoint.SetActive(false);
        }
        pathPoints[pathPoints.Count - 1].GetComponent<ParkourPath>().pathPoint.SetActive(false);
        UIManager.Instance.promptText.SetActive(false);
        StartCoroutine(DelayedDestroy());
        //startPoint.SetActive(false);
        //paths.SetActive(false);
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(8);
        Destroy(gameObject);
        timer.gameObject.SetActive(false);
        UIManager.Instance.successText.SetActive(false);
        UIManager.Instance.failureText.SetActive(false);
    }

    private void Update()
    {
        Color c = pathPointMaterial.color;
        Color d = endPointMaterial.color;
        float alpha = c.a;
        if (alphaIncrease)
        {
            alpha += Time.deltaTime * alphaSpeed;
            if (alpha >= maxAlpha)
            {
                alpha = maxAlpha;
                alphaIncrease = false;
            }
        }
        else
        {
            alpha -= Time.deltaTime * alphaSpeed;
            if (alpha <= minAlpha)
            {
                alpha = minAlpha;
                alphaIncrease = true;
            }
        }
        c.a = alpha;
        d.a = alpha;
        pathPointMaterial.color = c;
        endPointMaterial.color = d;
    }
}
