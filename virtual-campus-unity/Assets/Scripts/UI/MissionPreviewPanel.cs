using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPreviewPanel : MonoBehaviour
{
	// 当新任务开始时添加到PreviewPanel底部
	// 如果PreviewPanel中已经有三个任务，则删除最旧的
	// 点击某个Preivew直接打开任务Tab并选择那个任务
	// 如果某个任务完成了，则移除Preview

	static public MissionPreviewPanel Instance;

	public int maxNumOfPreviews;
	public GameObject missionPreviewPrefab;

	List<GameObject> existingPreviews = new List<GameObject>();

	private void Awake()
	{
		Instance = this;
	}

	public void AddMission(string missionName, string missionDescription, int indexInMissionPanel)
	{
		if (existingPreviews.Count >= maxNumOfPreviews)
		{
			existingPreviews[existingPreviews.Count - maxNumOfPreviews].SetActive(false);
		}

		if (existingPreviews.Count == 0)
			gameObject.SetActive(true);

		var preview = Instantiate(missionPreviewPrefab);
		preview.transform.SetParent(transform);
		preview.transform.localScale = Vector3.one;

		preview.GetComponent<MissionPreview>().Initialize(missionName, missionDescription, indexInMissionPanel);

		existingPreviews.Add(preview);
	}

	public void UpdateMission(string missionName, string missionDescription) 
	{
		foreach (GameObject preview in existingPreviews)
		{
			if (preview.GetComponent<MissionPreview>().missionNameText.text == missionName)
			{
				Debug.Log("Preview Updated");
				preview.GetComponent<MissionPreview>().missionDescriptionText.text = missionDescription;
			}
		}
	}
	public void FinishMission(string missionName)
	{
		foreach (GameObject preview in existingPreviews)
		{
			if (preview.GetComponent<MissionPreview>().missionNameText.text == missionName)
			{
				Destroy(preview);
				existingPreviews.Remove(preview);
				break;
			}
		}

		existingPreviews.ForEach((o) => { o.SetActive(false); });
		for (int i = 1; i <= maxNumOfPreviews && i <= existingPreviews.Count; i++)
		{
			existingPreviews[existingPreviews.Count - i].SetActive(true);
		}

		if (existingPreviews.Count == 0)
			gameObject.SetActive(false);
	}
}
