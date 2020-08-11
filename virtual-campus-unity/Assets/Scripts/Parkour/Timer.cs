using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Timer : MonoBehaviour
{
    [HideInInspector]
    public float time;
    [HideInInspector]
    public bool start;
    [HideInInspector]
    public float maxTime;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI timeLimitText;

    private void Awake()
    {
        time = 0f;
        start = false;
        StartTiming(10);
    }

    public void StartTiming(float max)
    {
        start = true;
        time = 0f;
        maxTime = max;
        SetTimeLimitText();
    }

    public void EndTiming()
    {
        start = false;
        Parkour.Instance.Fail();
    }

    void Update()
    {
        if (start)
        {
            time += Time.deltaTime;         
            if (time > maxTime)
            {
                time = maxTime;
                EndTiming();
            }
            UpdateText();
        }
    }

    private void SetTimeLimitText()
    {
        int minute = Mathf.FloorToInt(maxTime / 60);
        int second = Mathf.FloorToInt(maxTime - 60 * minute);
        int msecond = Mathf.FloorToInt((maxTime - Mathf.FloorToInt(maxTime)) * 100);
        string m, s, ms;
        if (minute < 10)
        {
            m = "0" + minute.ToString();
        }
        else
        {
            m = minute.ToString();
        }
        if (second < 10)
        {
            s = "0" + second.ToString();
        }
        else
        {
            s = second.ToString();
        }
        if (msecond < 10)
        {
            ms = "0" + msecond.ToString();
        }
        else
        {
            ms = msecond.ToString();
        }
        timeLimitText.text = m + ":" + s + ":" + ms;
    }
    private void UpdateText()
    {
        int minute = Mathf.FloorToInt(time / 60);
        int second = Mathf.FloorToInt(time - 60 * minute);
        int msecond = Mathf.FloorToInt((time - Mathf.FloorToInt(time)) * 100);
        string m, s, ms;
        if (minute < 10)
        {
            m = "0" + minute.ToString();
        }
        else
        {
            m = minute.ToString();
        }
        if (second < 10)
        {
            s = "0" + second.ToString();
        }
        else
        {
            s = second.ToString();
        }
        if (msecond < 10)
        {
            ms = "0" + msecond.ToString();
        }
        else
        {
            ms = msecond.ToString();
        }
        timeText.text = m + ":" + s + ":" + ms;
        if (maxTime - time < 3)
        {
            timeText.color = Color.red;
        }
    }
}
