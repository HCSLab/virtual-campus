using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogPanel : MonoBehaviour
{
	static public LogPanel Instance;

	public Transform logHolderTransform;
	public GameObject logPrefab;

	RectTransform logHolderRectTransform;
	
	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		logHolderRectTransform = logHolderTransform.GetComponent<RectTransform>();
	}

	public void AddLog(string logContent)
	{
		var log = Instantiate(logPrefab);
		log.transform.SetParent(logHolderTransform);
		log.transform.localScale = Vector3.one;
		log.GetComponent<TextMeshProUGUI>().text = logContent;
	}
}
