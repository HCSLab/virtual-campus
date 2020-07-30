using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Achievement : SavableMonoBehavior
{
	[Header("Achievement")]
	public EventCenter.AchievementEvent eventToListenTo;
	public GameObject finishedContainer;
	public TextMeshProUGUI descriptionText, nameText;
	public Image fillImage;

	protected bool isFinished;

	protected virtual void Awake()
	{
		EventCenter.AddListener(eventToListenTo, OnEventTriggered);
		isFinished = PlayerPrefs.GetInt(SaveSystem.GetAchievementStateName(gameObject), 0) > 0;
	}

	protected virtual void OnEventTriggered(object data)
	{

	}

	protected override void Save(object data)
	{
		base.Save(data);

		PlayerPrefs.SetInt(SaveSystem.GetAchievementStateName(gameObject), isFinished ? 1 : 0);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		EventCenter.RemoveListener(eventToListenTo, OnEventTriggered);
	}

	protected void Finish()
	{
		// No matter whether finished or not,
		// this does not hurt.
		transform.SetParent(finishedContainer.transform);

		if (isFinished)
			return;

		isFinished = true;

		LogPanel.Instance.AddAchievementFinishLog(nameText.text);
	}
}
