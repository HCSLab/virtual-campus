using TMPro;
using UnityEngine;

public class LogInNotification : MonoBehaviour
{
	public GameObject image;
	public TextMeshProUGUI text;
	public void StartLoadAnimation()
	{
		LeanTween.scale(image.GetComponent<RectTransform>(), image.GetComponent<RectTransform>().localScale * 0.5f, 0.5f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong();
	}
}