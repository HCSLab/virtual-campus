using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeAchievement : Achievement
{
	[Header("Game Time")]
    public int targetMinute;
	int progress = 0;

	protected override void Start()
	{
		base.Start();
		UpdateProgress(PlayerPrefs.GetInt(SaveSystem.GetAchievementProgressName(gameObject), 0), false);
	}

	protected override void OnEventTriggered(object data)
	{
		base.OnEventTriggered(data);
		UpdateProgress(progress + 1);
	}

	protected override void Save(object data)
	{
		base.Save(data);
		PlayerPrefs.SetInt(SaveSystem.GetAchievementProgressName(gameObject), progress);
	}

	void UpdateProgress(int newProgress, bool enableLogging = true)
	{
		if (newProgress > targetMinute)
			Finish(enableLogging);

		progress = newProgress;
		fillImage.fillAmount = Mathf.Min(1f, (float)progress / targetMinute);
		descriptionText.text = "总游戏时间 " + progress + " Min / " + targetMinute + " Min";
	}
}
