using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourAchievement : Achievement
{
    [Header("Parkour")]
    public int targetParkour;
    int currentProgress = 0;

    protected override void Start()
    {
        base.Start();
        UpdateProgress(PlayerPrefs.GetInt(SaveSystem.GetAchievementProgressName(gameObject), 0));
    }

    protected override void OnEventTriggered(object data)
    {
        base.OnEventTriggered(data);
        UpdateProgress(currentProgress + 1);
    }

    protected override void Save(object data)
    {
        base.Save(data);
        PlayerPrefs.SetInt(SaveSystem.GetAchievementProgressName(gameObject), currentProgress);
    }

    void UpdateProgress(int newProgress)
    {
        if (newProgress >= targetParkour)
            Finish();

        currentProgress = newProgress;
        fillImage.fillAmount = Mathf.Min(1f, (float)currentProgress / targetParkour);
        descriptionText.text = "已完成的跑酷 " + currentProgress + " / " + targetParkour;
    }
}
