using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LikenessAchievement : Achievement
{
	[Header("Likeness")]
	public int targetLikeness;
	int currentProgress = 0;

	protected override void Start()
	{
		base.Start();
		UpdateProgress(PlayerInfo.Instance.likeness);
	}

	protected override void OnEventTriggered(object data)
	{
		base.OnEventTriggered(data);
		UpdateProgress((int)data);
	}

	void UpdateProgress(int newProgress)
	{
		if (newProgress > targetLikeness)
			Finish();

		currentProgress = newProgress;
		fillImage.fillAmount = Mathf.Min(1f, (float)currentProgress / targetLikeness);
		descriptionText.text = "猫咪好感度 " + currentProgress + " / " + targetLikeness;
	}
}
