using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishSpecificMissionAchievement : Achievement
{
	protected override void Start()
	{
		base.Start();

		fillImage.fillAmount = 0f;
	}

	protected override void OnEventTriggered(object data)
	{
		base.OnEventTriggered(data);
		UpdateProgress();
	}

	void UpdateProgress()
	{
		Finish();

		fillImage.fillAmount = 1f;
	}
}
