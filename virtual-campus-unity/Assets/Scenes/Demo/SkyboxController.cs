using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

public class SkyboxController: MonoBehaviour
{
	public Skybox skybox;

	public Material dayToSunset, sunsetToNight;

	public void Start()
	{
		skybox.material.SetFloat("_Blend", 0f);
	}

	public void OnNightStart()
	{
		StartCoroutine(BlendSkybox());
	}

	IEnumerator BlendSkybox()
	{
		yield return new WaitForSeconds(3f);

		// Day to sunset
		LeanTween.value(
			gameObject,
			(float x) => { skybox.material.SetFloat("_Blend", x); },
			0f,
			1f,
			3f
			);

		yield return new WaitForSeconds(3.1f);

		// SunsetToNight
		skybox.material = sunsetToNight;
		skybox.material.SetFloat("_Blend", 0f);
		LeanTween.value(
			gameObject,
			(float x) => { skybox.material.SetFloat("_Blend", x); },
			0f,
			1f,
			3f
			);
	}
}
