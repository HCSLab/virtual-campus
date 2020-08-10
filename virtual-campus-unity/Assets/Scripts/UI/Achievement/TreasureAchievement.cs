using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureAchievement : Achievement
{
    [Header("Treasure")]
    public int targetTreasure;
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
        if (newProgress >= targetTreasure)
            Finish();

        currentProgress = newProgress;
        fillImage.fillAmount = Mathf.Min(1f, (float)currentProgress / targetTreasure);
        descriptionText.text = "已发现的宝藏 " + currentProgress + " / " + targetTreasure;
    }
}
