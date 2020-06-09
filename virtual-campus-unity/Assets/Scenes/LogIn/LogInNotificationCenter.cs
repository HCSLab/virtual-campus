using System.Collections.Generic;
using UnityEngine;
using VoxelImporter;

public class LogInNotificationCenter : MonoBehaviour
{
	static public LogInNotificationCenter instance;

	public enum NotificationType
	{
		LoggingIn,
		WrongPasswordOrUsername,
		InternetError
	};

	public GameObject logInPrefab, wrongPasswordOrUsernamePrefab, internetErrorPrefab;

	Dictionary<NotificationType, GameObject> notifications;

	public void Post(NotificationType type)
	{
		GameObject notification;
		switch (type)
		{
			case NotificationType.LoggingIn: notification = Instantiate(logInPrefab); break;
			case NotificationType.WrongPasswordOrUsername: notification = Instantiate(wrongPasswordOrUsernamePrefab); break;
			default: notification = Instantiate(internetErrorPrefab); break;
		}

		notification.transform.SetParent(gameObject.transform);
		notifications[type] = notification;

		notification.transform.localScale *= 0.8f;
		LeanTween.scale(
			notification.GetComponent<RectTransform>(),
			notification.GetComponent<RectTransform>().localScale / 0.8f,
			0.2f
			);

		if(type == NotificationType.LoggingIn)
			notification.GetComponent<LogInNotification>().StartLoadAnimation();
	}
	
	public void Remove(NotificationType type)
	{
		if (notifications.ContainsKey(type)
			&& notifications[type] != null)
		{
			var toRemove = notifications[type];
			notifications[type] = null;

			LeanTween.alpha(
				toRemove.GetComponent<RectTransform>(),
				0,
				0.5f
				).setOnComplete(() => { Destroy(toRemove); });
		}
	}

	void Start()
	{
		if (instance)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;

		notifications = new Dictionary<NotificationType, GameObject>();
	}
}
