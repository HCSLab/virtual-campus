using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionPanel : MonoBehaviour
{
	static public MissionPanel Instance;

	public Color inProgressButtonColor, finishedButtonColor;
	public Transform buttonHolder;
	public GameObject buttonPrefab;
	public TextMeshProUGUI missionName, missionDescription;
	public GameObject inProgressFlag, finishedFlag;
	public Transform inProgressButtonHolder, finishedButtonHolder;

	List<string> missionNames = new List<string>();
	List<string> missionDescriptions = new List<string>();
	List<bool> missionStates = new List<bool>();
	List<GameObject> missionButtons = new List<GameObject>();

	int nextMissionIndex = 0;

	private void Awake()
	{
		Instance = this;

		inProgressFlag.SetActive(false);
		finishedFlag.SetActive(false);
		missionName.text = string.Empty;
		missionDescription.text = string.Empty;
	}

	public void AddMission(string missionName, string missionDescription, bool isFinished)
	{
		if (missionName == "")
		{
			return;
		}
		else if (missionName.EndsWith("宝箱"))
		{
			return;
		}

		var currentIndex = nextMissionIndex;
		nextMissionIndex++;

		missionNames.Add(missionName);
		missionDescriptions.Add(missionDescription);
		missionStates.Add(isFinished);

		var newButton = Instantiate(buttonPrefab);
		if (isFinished)
		{
			newButton.transform.SetParent(finishedButtonHolder);
		}
		else
		{
			newButton.transform.SetParent(inProgressButtonHolder);
		}
		newButton.transform.localScale = Vector3.one;

		newButton.GetComponentInChildren<TextMeshProUGUI>().text = missionName;
		newButton.GetComponent<Image>().color = isFinished ? finishedButtonColor : inProgressButtonColor;
		newButton.GetComponent<Button>().onClick.AddListener(() => { SelectMission(currentIndex); });

		missionButtons.Add(newButton);

		if (!isFinished)
			MissionPreviewPanel.Instance.AddMission(missionName, missionDescription, currentIndex);
		else
			EventCenter.Broadcast(EventCenter.AchievementEvent.OneMissionFinished, null);
	}

	public void UpdateMissionDescription(string updatedMissionName, string updatedDescription)
	{
		if (!missionNames.Contains(updatedMissionName))
		{
			return;
		}
		for (int i = 0; i < missionNames.Count; i++)
		{
			if (missionNames[i] == updatedMissionName)
			{
				missionDescriptions[i] = updatedDescription;
				LogNotificationCenter.Instance.Post(
					"任务 <color=blue>" + updatedMissionName + "</color> 已更新。"
					);
				return;
			}
		}
	}

	public void FinishMission(string finishedMissionName)
	{
		if (finishedMissionName.EndsWith("宝箱"))
		{
			EventCenter.Broadcast(EventCenter.AchievementEvent.OneTreasureFound, null);
			UIManager.Instance.missionFinishedSource.PlayOneShot(UIManager.Instance.missionFinishedSFX);
		}
		if (!missionNames.Contains(finishedMissionName))
		{
			return;
		}

		EventCenter.Broadcast(EventCenter.AchievementEvent.OneMissionFinished, null);
		UIManager.Instance.missionFinishedSource.PlayOneShot(UIManager.Instance.missionFinishedSFX);
		MissionPreviewPanel.Instance.FinishMission(finishedMissionName);
		LogNotificationCenter.Instance.Post(
			"你刚刚完成了任务 <color=blue>" + finishedMissionName + "</color>！"
			);

		for (int i = 0; i < missionNames.Count; i++)
		{
			if (missionNames[i] == finishedMissionName)
			{
				missionStates[i] = true;
				missionButtons[i].GetComponent<Image>().color = finishedButtonColor;
				missionButtons[i].transform.SetParent(finishedButtonHolder);
				string temp = missionDescriptions[i];
				missionDescriptions[i] = "<color=grey>" + temp + "</color>";

				// Update detail if the mission is currently selected.
				if (missionName.text == finishedMissionName)
				{
					inProgressFlag.SetActive(false);
					finishedFlag.SetActive(true);
				}

				return;
			}
		}
	}

	public void SelectMission(int index)
	{
		missionName.text = missionNames[index];
		missionDescription.text = missionDescriptions[index];
		inProgressFlag.SetActive(!missionStates[index]);
		finishedFlag.SetActive(missionStates[index]);
	}
}
