using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeMissionAchievement : Achievement
{
	int totalNumberOfMissions, currentProgress;

	protected override void Awake()
	{
		base.Awake();

		totalNumberOfMissions = 9;
		UpdateProgress(0);
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void OnEventTriggered(object data)
	{
		base.OnEventTriggered(data);

		UpdateProgress(currentProgress + 1);
	}

	void UpdateProgress(int newProgress)
	{
		if (newProgress >= totalNumberOfMissions)
			Finish();

		currentProgress = newProgress;
		fillImage.fillAmount = Mathf.Min(1f, (float)currentProgress / totalNumberOfMissions);
		descriptionText.text = "完成所有" + currentProgress + " / " + totalNumberOfMissions + "个 \"问候\" 任务 " ;
	}
}
