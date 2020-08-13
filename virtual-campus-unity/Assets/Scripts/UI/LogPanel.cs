using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class LogPanel : SavableMonoBehavior
{
	static public LogPanel Instance;

	public Transform logHolderTransform;
	public GameObject logPrefab;
	public string npcNameColor;
	public string achievementColor;

	RectTransform logHolderRectTransform;
	bool enableAchievementLogging;

	private List<string> records = new List<string>();

	private void Awake()
	{
		Instance = this;
		enableAchievementLogging = false;
	}

	protected override void Start()
	{
		base.Start();

		logHolderRectTransform = logHolderTransform.GetComponent<RectTransform>();

		LeanTween.delayedCall(1f, () => { enableAchievementLogging = true; });

		int logCount = PlayerPrefs.GetInt(SaveSystem.GetDialogueLogCountName());
		for (int i = 0; i < logCount; i++)
		{
			AddLog(PlayerPrefs.GetString(SaveSystem.GetIthDialogueLogName(i)), false);
		}
	}

	public void AddLog(string content, bool popOut = true)
	{
		var log = Instantiate(logPrefab);
		log.transform.SetParent(logHolderTransform);
		log.transform.localScale = Vector3.one;

		log.GetComponent<TextMeshProUGUI>().text = content;

		records.Add(content);

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

	protected override void Save(object data)
	{
		base.Save(data);

		PlayerPrefs.SetInt(
			SaveSystem.GetDialogueLogCountName(),
			records.Count
			);
		for (int i = 0; i < records.Count; i++)
		{
			PlayerPrefs.SetString(
				SaveSystem.GetIthDialogueLogName(i),
				records[i]
				);
		}
	}
}
