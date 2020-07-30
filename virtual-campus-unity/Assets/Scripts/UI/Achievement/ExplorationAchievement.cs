using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationAchievement : Achievement
{
	[Header("Exploration Achievement")]
	public float targetRate;

	int currentProgress, targetProgress, regionCount;

	protected override void Awake()
	{
		base.Awake();

		targetProgress = 100000;
		UpdateProgress(0);
	}

	protected override void Start()
	{
		base.Start();

		targetProgress = Mathf.FloorToInt(CurrentRegion.Instance.regionCount * targetRate);
		regionCount = CurrentRegion.Instance.regionCount;
		UpdateProgress(currentProgress);
	}

	protected override void OnEventTriggered(object data)
	{
		base.OnEventTriggered(data);

		UpdateProgress(currentProgress + 1);
	}

	void UpdateProgress(int newProgress)
	{
		if (newProgress >= targetProgress)
			Finish();

		currentProgress = newProgress;
		fillImage.fillAmount = Mathf.Min(1f, (float)newProgress / targetProgress);
		descriptionText.text =
			"地图探索率 " +
			Mathf.FloorToInt((float)newProgress / regionCount * 100) +
			"% / " +
			Mathf.FloorToInt(targetRate * 100) +
			"%";
	}
}
