//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Update the textviews of the scene based on the selected GazeChallenge
/// </summary>
public class SceneContentController : MonoBehaviour
{
	public SceneContent[] SceneChapters;
	public Text TextViewHeadline;
	public Text TextViewDescription;
	public Text TextViewInteraction;
	public GameObject GameobjectInteractionText;

	private int _activeSceneChapter = 0;

	void Start()
	{
		StartCoroutine(WaitForStart());
	}

	/// <summary>
	/// Update the Views of the SceneContent
	/// </summary>
	/// <param name="steps"></param>
	public void GoToChapter(int steps)
	{
		/*
		foreach (SceneContent scenes in SceneChapters)
		{
			scenes.gameObject.SetActive(false);
		}

		_activeSceneChapter += steps;

		if (_activeSceneChapter > SceneChapters.Length - 1)
		{
			_activeSceneChapter = 0;
		}
		else if (_activeSceneChapter < 0)
		{
			_activeSceneChapter = SceneChapters.Length - 1;
		}
        */
		_activeSceneChapter = steps;

		SceneChapters[_activeSceneChapter].gameObject.SetActive(true);
		TextViewHeadline.text = SceneChapters[_activeSceneChapter].Headline;

		TextViewDescription.text = SceneChapters[_activeSceneChapter].Description.Replace("/n", "\n");

		GameobjectInteractionText.SetActive(SceneChapters[_activeSceneChapter].HasSuggestion);
		TextViewInteraction.text = SceneChapters[_activeSceneChapter].InteractionTip;
	}

	IEnumerator WaitForStart()
	{
		yield return new WaitForFixedUpdate();
		for (int i = 0; i < SceneChapters.Length; i++)
		{
			if (SceneChapters[i].gameObject.activeInHierarchy)
			{
				GoToChapter(i);
			}
		}
	}
}
