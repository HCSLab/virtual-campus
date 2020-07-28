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

	protected bool isFinished = false;

    protected virtual void Start()
	{
		EventCenter.AddListener(eventToListenTo, OnEventTriggered);
		EventCenter.AddListener(EventCenter.GlobalEvent.Save, Save);
	}

    protected virtual void OnEventTriggered(object data)
	{

	}

	protected virtual void Save(object data)
	{

	}

	protected virtual void OnDestroy()
	{
		Save(null);
		EventCenter.RemoveListener(eventToListenTo, OnEventTriggered);
		EventCenter.RemoveListener(EventCenter.GlobalEvent.Save, Save);
	}

	protected void Finish(bool addToLog)
	{
		if (isFinished)
			return;

		isFinished = true;

		transform.SetParent(finishedContainer.transform);

		if(addToLog)
			LogPanel.Instance.AddAchievementFinishLog(nameText.text);
	}
}
