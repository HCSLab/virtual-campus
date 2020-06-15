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
		var night = GameObject.FindGameObjectWithTag("NightTimeline");
		night.transform.position = Vector3.zero;
	}
}
