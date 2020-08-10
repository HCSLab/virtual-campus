using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionFinishRateAchievement : Achievement
{
	int totalNumberOfMissions, currentProgress;
    public int treasureCount;

	protected override void Awake()
	{
		base.Awake();

		totalNumberOfMissions = 100000;
		UpdateProgress(0);
	}

	protected override void Start()
	{
		base.Start();

        treasureCount = StoryManager.Instance.treasureCount;
        totalNumberOfMissions = StoryManager.Instance.stories.Count - treasureCount;
		UpdateProgress(currentProgress);
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
		descriptionText.text = "已经完成了 " + currentProgress + " / " + totalNumberOfMissions + " 个任务";
	}
}
