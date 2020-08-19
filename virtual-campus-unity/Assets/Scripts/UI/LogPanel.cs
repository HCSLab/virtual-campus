using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class LogPanel : MonoBehaviour
{
	static public LogPanel Instance;

	public Transform logHolderTransform;
	public GameObject logPrefab;
	public string npcNameColor;
	public string achievementColor;

	RectTransform logHolderRectTransform;
	bool enableAchievementLogging;

	private void Awake()
	{
		Instance = this;
		enableAchievementLogging = false;
	}

	void Start()
	{
		logHolderRectTransform = logHolderTransform.GetComponent<RectTransform>();

		LeanTween.delayedCall(1f, () => { enableAchievementLogging = true; });
	}

	public void AddLog(string content, bool popOut = true)
	{
		var log = Instantiate(logPrefab);
		log.transform.SetParent(logHolderTransform);
		log.transform.localScale = Vector3.one;

		log.GetComponent<TextMeshProUGUI>().text = content;

		if (popOut)
		{
			LogNotificationCenter.Instance.Post(content);
		}
	}

	public void AddLog(string npcName, string logContent, bool popOut = true)
	{
		StringBuilder s = new StringBuilder();
		s.Append("<color=");
		s.Append(npcNameColor);
		s.Append(">");
		s.Append(npcName);
		s.Append("</color>: ");
		s.Append(logContent);

		AddLog(s.ToString(), popOut);
	}

	public void AddAchievementFinishLog(string achievementName)
	{
		if (!enableAchievementLogging)
			return;

		var log = Instantiate(logPrefab);
		log.transform.SetParent(logHolderTransform);
		log.transform.localScale = Vector3.one;

		StringBuilder s = new StringBuilder();
		s.Append("你刚刚完成了成就 <color=");
		s.Append(npcNameColor);
		s.Append(">");
		s.Append(achievementName);
		s.Append("</color>!");

		log.GetComponent<TextMeshProUGUI>().text = s.ToString();

		LogNotificationCenter.Instance.Post(s.ToString());

        UIManager.Instance.PlayAchievementSFX();
    }
}
