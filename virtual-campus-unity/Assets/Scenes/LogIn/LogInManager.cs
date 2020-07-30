using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogInManager : MonoBehaviour
{
	enum LogInResult
	{
		Success,
		WrongPasswordOrUsername,
		InternetError
	};

	public GameObject logInButton;
	public void LogIn()
	{
		LogInNotificationCenter.instance.Remove(LogInNotificationCenter.NotificationType.WrongPasswordOrUsername);
		LogInNotificationCenter.instance.Remove(LogInNotificationCenter.NotificationType.InternetError);

		logInButton.GetComponent<Button>().interactable = false;

		LogInNotificationCenter.instance.Post(LogInNotificationCenter.NotificationType.LoggingIn);

		var result = LogInResult.Success;

		switch (result)
		{
			case LogInResult.Success:
				LeanTween.delayedCall(2f, () =>
				{
					LogInNotificationCenter.instance.Remove(LogInNotificationCenter.NotificationType.LoggingIn);
					SceneLoadingManager.Instance.LoadGame();
				});
				break;
			case LogInResult.WrongPasswordOrUsername:
				LeanTween.delayedCall(2f, () =>
				{
					LogInNotificationCenter.instance.Remove(LogInNotificationCenter.NotificationType.LoggingIn);
					LogInNotificationCenter.instance.Post(LogInNotificationCenter.NotificationType.WrongPasswordOrUsername);
					logInButton.GetComponent<Button>().interactable = true;
				});
				break;
			case LogInResult.InternetError:
				LeanTween.delayedCall(2f, () =>
				{
					LogInNotificationCenter.instance.Remove(LogInNotificationCenter.NotificationType.LoggingIn);
					LogInNotificationCenter.instance.Post(LogInNotificationCenter.NotificationType.InternetError);
					logInButton.GetComponent<Button>().interactable = true;
				});
				break;
		}
	}
}
