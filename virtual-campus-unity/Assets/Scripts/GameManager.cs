using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private void Start()
	{
		PlayerPrefs.DeleteAll();

		StartCoroutine(MinuteTick());
	}

	IEnumerator MinuteTick()
	{
		while(true)
		{
			yield return new WaitForSeconds(1f);
			EventCenter.Broadcast(EventCenter.AchievementEvent.TickPerMinute, null);
		}
	}
}
