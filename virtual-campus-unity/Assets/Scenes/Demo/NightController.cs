using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class NightController : MonoBehaviour
{
	public GameObject dayWorld;
    public void OnLightMapChange()
	{
		dayWorld.SetActive(false);
		GameObject.FindGameObjectWithTag("NightTimeline").GetComponent<PlayableDirector>().Play();
	}
}
