using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using System.Security.Cryptography;
using TMPro;
using System;

public class InkTalk : MonoBehaviour
{
	public TextAsset inkFile;
	public string executeFunction;

	public Story inkStroy;
	private bool nextStep;
	private bool firstStep;
	public bool notFinished;

	public GameObject talk;

	public StoryScript storyScript;

	public GameObject button;

	private TextMeshProUGUI text;
	private Transform buttons;

	private void Start()
	{
		text = talk.transform.Find("Panel/Text").GetComponent<TextMeshProUGUI>();
		buttons = talk.transform.Find("Panel/Buttons");

		nextStep = true;
		firstStep = true;
		notFinished = false;

		inkStroy = new Story(inkFile.text);

		if (inkStroy.HasFunction(executeFunction))
		{
			inkStroy.CheckInFunction(executeFunction);
		}
	}

	private void Update()
	{
		if (!nextStep) return;

		if (!firstStep)
		{
			for (int i = 0; i < buttons.childCount; i++)
			{
				Destroy(buttons.GetChild(i).gameObject);
			}
		}

		text.text = "";
		while (inkStroy.canContinue)
		{
			text.text += inkStroy.Continue();
			var tags = inkStroy.currentTags;
			if (storyScript)
			{
				foreach (var tag in tags)
				{
					storyScript.InProcessTag(tag, this);
				}
			}
		}

		foreach (var choice in inkStroy.currentChoices)
		{
			var btn = Instantiate(button).GetComponent<Button>();
			btn.gameObject.SetActive(true);

			var btnText = btn.transform.Find("Text").GetComponent<TextMeshProUGUI>();
			btnText.text = choice.text;

			btn.transform.SetParent(buttons);
			btn.transform.localScale = Vector3.one;
			//LayoutRebuilder.MarkLayoutForRebuild(buttons.GetComponent<RectTransform>());

			var path = choice.pathStringOnChoice;
			btn.onClick.AddListener(delegate { ChoicePathSelected(path); });
		}

		nextStep = false;
		firstStep = false;

		if (text.text == "" && inkStroy.currentChoices.Count == 1)
		{
			ChoicePathSelected(inkStroy.currentChoices[0].pathStringOnChoice);
		}

		if (!inkStroy.canContinue && inkStroy.currentChoices.Count == 0)
		{
			EndTalk();
		}
	}

	private void ChoicePathSelected(string path)
	{
		inkStroy.ChoosePathString(path);
		inkStroy.Continue();
		nextStep = true;
	}

	private void EndTalk()
	{
		UIManager.Instance.CloseTalk();

		if (!notFinished)
		{
			if (storyScript)
			{
				storyScript.AddFlag(executeFunction + "_done");
			}
		}
	}
}
