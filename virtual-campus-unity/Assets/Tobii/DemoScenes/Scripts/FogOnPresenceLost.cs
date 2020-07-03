using UnityEngine;
using Tobii.Gaming;

public class FogOnPresenceLost : MonoBehaviour
{
	// Update is called once per frame
	void Update()
	{
		var presence = TobiiAPI.GetUserPresence();
		if (presence.IsUserPresent())
		{
			RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, 30, Time.deltaTime);
		}
		else
		{
			RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, 0, Time.deltaTime * 10);
		}
	}
}
