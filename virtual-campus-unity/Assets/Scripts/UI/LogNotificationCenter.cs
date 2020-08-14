using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogNotificationCenter : MonoBehaviour
{
	static public LogNotificationCenter Instance;

	public GameObject notificationPrefab;
	public float displayDuration;

	bool isLoggingEnabled = false;

	private void Awake()
	{
		Instance = this;
		StartCoroutine(EnableLoggingCoroutine());
	}

	IEnumerator EnableLoggingCoroutine()
	{
		yield return null;
		yield return null;
		yield return null;
		isLoggingEnabled = true;
	}

	public void Post(string content)
	{
		if (!isLoggingEnabled)
			return;

		var notification = Instantiate(notificationPrefab);
		notification.transform.SetParent(gameObject.transform);
		notification.transform.localScale = Vector3.one * 0.8f;

		notification.GetComponentInChildren<TextMeshProUGUI>().text = content;

		LeanTween.scale(
			notification.GetComponent<RectTransform>(),
			notification.GetComponent<RectTransform>().localScale / 0.8f,
			0.2f
			);

		LeanTween.delayedCall(
			displayDuration,
			() =>
			{
				notification.LeanScale(Vector3.one * 0.8f, 0.2f).setOnComplete(() => { Destroy(notification); });
			}
			);
	}
}
