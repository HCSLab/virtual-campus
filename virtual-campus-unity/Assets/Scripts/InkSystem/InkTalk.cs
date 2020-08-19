using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using System.Security.Cryptography;
using TMPro;
using System;
using System.Text;

public class InkTalk : MonoBehaviour
{
	[HideInInspector] public TextAsset inkFile;
	[HideInInspector] public string executeFunction;

	[HideInInspector] public Story inkStory;
	private bool nextStep;
	private bool firstStep;
	[HideInInspector] public bool notFinished;

	[HideInInspector] public StoryScript storyScript;

	[HideInInspector] public NPCInfo speaker;

	public TextMeshProUGUI text, nameText;
	public Transform buttons;
	public GameObject button;
	public Button panelSizedButton;

	[Header("Text Transition")]
	public float secondsPerCharacter;
	public float secondsPerSFX;
	public AudioClip maleVoice, femaleVoice, catVoice, youngVoice, murmuringVoice;

	AudioSource textSFXSource;
	Voice isVoice;

	private void Start()
	{
		textSFXSource = UIManager.Instance.textSFXSource;
		if (speaker)
		{
			isVoice = speaker.voice;
		}

		panelSizedButton.interactable = false;

		nextStep = true;
		firstStep = true;
		notFinished = false;

		if (storyScript)
		{
			inkStory = storyScript.inkStory;
		}
		else
		{
			inkStory = new Story(inkFile.text);
		}

		PlayerInfo.WriteToInkStory(inkStory);

		if (inkStory.HasFunction(executeFunction))
		{
			inkStory.CheckInFunction(executeFunction);
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
			panelSizedButton.interactable = false;
		}

		string sentences = "";
		while (inkStory.canContinue)
		{
			sentences += inkStory.Continue();
			var tags = inkStory.currentTags;
			if (storyScript)
			{
				foreach (var tag in tags)
				{
					storyScript.InProcessTag(tag, this, inkStory);
				}
			}
			else if ((inkFile.name == "$cat_whitey") || (inkFile.name == "$cat_cutey"))
			{
				foreach (var tag in tags)
				{
					string op, data;
					StoryScript.StandardizationTag(tag, out op, out data);
					if (op == "upd_info")
					{
						PlayerInfo.UpdateFromInkStory(inkStory);
					}
				}
				StoryManager.Instance.refreshFlag = true;
			}
		}
		if (sentences != "")
		{
			SetText(sentences);
			if (speaker)
			{
				SetName(speaker.npcName);
			}
			else
			{
				nameText.text = "";
			}
			LogPanel.Instance.AddLog(nameText.text, sentences, false);
		}

		foreach (var choice in inkStory.currentChoices)
		{
			if (choice.text == "n")
			{
				panelSizedButton.interactable = true;
				var path = choice.pathStringOnChoice;
				panelSizedButton.onClick.RemoveAllListeners();
				panelSizedButton.onClick.AddListener(() => { ChoicePathSelected(path); });
			}
			else
			{
				var btn = Instantiate(button).GetComponent<Button>();
				btn.transform.SetParent(buttons);
				btn.transform.localScale = Vector3.one;

				var btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
				btnText.text = choice.text;

				var path = choice.pathStringOnChoice;
				btn.onClick.AddListener(() => { ChoicePathSelected(path, choice.text); });
			}
		}

		nextStep = false;

		if (firstStep && sentences == "" && inkStory.currentChoices.Count == 1)
		{
			ChoicePathSelected(inkStory.currentChoices[0].pathStringOnChoice,
				inkStory.currentChoices[0].text);
		}

		firstStep = false;

		if (!inkStory.canContinue && inkStory.currentChoices.Count == 0)
		{
			EndTalk();
		}


	}

	private void ChoicePathSelected(string path, string text = "")
	{
		inkStory.ChoosePathString(path);
		inkStory.Continue();
		nextStep = true;

		if (text != "")
		{
			LogPanel.Instance.AddLog("Me", text, false);
		}
	}

	private void EndTalk()
	{
		if (storyScript)
		{
			if (!notFinished)
				storyScript.AddLocalFlag(executeFunction);

			storyScript.talkCount++;
			if (storyScript.talkCount == 1)
			{
				StoryManager.Instance.StartStory(storyScript);
			}

			storyScript.ProcessFunctionHeaderTags();
		}

		NPCManager.Instance.RefreshTalk();

		// 添加一个空行作区隔
		LogPanel.Instance.AddLog(" ", false);

		if (speaker)
		{
			speaker.EndTalkMode();
		}

		// 销毁对话面板
		UIManager.Instance.CloseTalk(gameObject);

		// 如果任务结束，销毁整个任务
		if (storyScript && storyScript.endStory)
		{
			StoryManager.Instance.EndStory(storyScript);
		}
	}

	private void SetName(string name)
	{
		nameText.text = name.Replace('\n', ' ');
	}


	bool isSetTextCoroutineExist = false;
	Coroutine setTextCoroutineInstance = null;
	Coroutine setTextSFXCoroutineInstance = null;
	private void SetText(string sentences)
	{
		if (isSetTextCoroutineExist)
		{
			StopCoroutine(setTextCoroutineInstance);
			StopCoroutine(setTextSFXCoroutineInstance);
		}

		isSetTextCoroutineExist = true;
		setTextCoroutineInstance = StartCoroutine(SetTextCoroutine(sentences));
		setTextSFXCoroutineInstance = StartCoroutine(SetTextSFXCoroutine(sentences));
	}

	IEnumerator SetTextCoroutine(string sentences)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < sentences.Length; i++)
		{
			stringBuilder.Append(sentences[i]);
			text.text = stringBuilder.ToString();
			yield return new WaitForSeconds(secondsPerCharacter);
		}
		isSetTextCoroutineExist = false;
	}

	IEnumerator SetTextSFXCoroutine(string sentences)
	{
		switch (isVoice)
		{
			case Voice.Male:
				textSFXSource.clip = maleVoice;
				break;
			case Voice.Female:
				textSFXSource.clip = femaleVoice;
				break;
			case Voice.Cat:
				textSFXSource.clip = catVoice;
				break;
			case Voice.Young:
				textSFXSource.clip = youngVoice;
				break;
			case Voice.Murmuring:
				textSFXSource.clip = murmuringVoice;
				break;
		}

		while (isSetTextCoroutineExist)
		{
			textSFXSource.Play();
			yield return new WaitForSeconds(secondsPerSFX);
		}
	}
}
