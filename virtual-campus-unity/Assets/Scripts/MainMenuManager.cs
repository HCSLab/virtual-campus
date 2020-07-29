using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
	[Header("Skybox Animation")]
	public Skybox skybox;
	public float cycle;

	private void Start()
	{
		LeanTween.value(
			gameObject,
			(x) => { skybox.material.SetFloat("_Rotation", x); },
			0f,
			360f,
			cycle
			).setEaseLinear().setRepeat(-1);
	}
}
