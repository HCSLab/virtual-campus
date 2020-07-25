using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
	public float animationTime;
	public float scaleAmplitude;
	public LeanTweenType easeType;
	private void OnEnable()
	{
		transform.LeanScale(Vector3.one * scaleAmplitude, animationTime).setEase(easeType);
	}
}
