using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeAchievement : Achievement
{
	[Header("Game Time")]
    public int targetMinute;
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
		if (newProgress > targetMinute)
			Finish();

		currentProgress = newProgress;
		fillImage.fillAmount = Mathf.Min(1f, (float)currentProgress / targetMinute);
		descriptionText.text = "总游戏时间 " + currentProgress + " 分钟 / " + targetMinute + " 分钟";
	}
}
