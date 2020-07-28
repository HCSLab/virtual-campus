using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Achievement : MonoBehaviour
{
	[Header("Achievement")]
	public EventCenter.AchievementEvent eventToListenTo;
	public GameObject finishedContainer;
	public TextMeshProUGUI descriptionText, nameText;
	public Image fillImage;

	protected bool isFinished;

	protected virtual void Start()
	{
		EventCenter.AddListener(eventToListenTo, OnEventTriggered);
		EventCenter.AddListener(EventCenter.GlobalEvent.Save, Save);

		isFinished = PlayerPrefs.GetInt(SaveSystem.GetAchievementStateName(gameObject), 0) > 0;
	}

	protected virtual void OnEventTriggered(object data)
	{

	}

	protected virtual void Save(object data)
	{
		PlayerPrefs.SetInt(SaveSystem.GetAchievementStateName(gameObject), isFinished ? 1 : 0);
	}

	protected virtual void OnDestroy()
	{
		Save(null);
		EventCenter.RemoveListener(eventToListenTo, OnEventTriggered);
		EventCenter.RemoveListener(EventCenter.GlobalEvent.Save, Save);
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
