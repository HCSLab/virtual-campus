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

	bool isSkip = false;
	bool isFinish = false;
	[Header("Text Transition")]
	public float secondsPerCharacter;
	public float secondsPerPauseCharacter;
	public float secondsPerSFX;
	public AudioClip maleVoice, femaleVoice, catVoice, youngVoice, murmuringVoice;

	AudioSource textSFXSource;
	VoiceType voiceType;

	private void Start()
	{
		textSFXSource = UIManager.Instance.textSFXSource;
		if (speaker)
		{
			voiceType = speaker.voiceType;
		}
		else 
		{ 
			voiceType = VoiceType.Young;
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
			LogPanel.Instance.AddLog(nameText.text, sentences.Replace("$", string.Empty), false);
		}

		foreach (var choice in inkStory.currentChoices)
		{
			panelSizedButton.interactable = true;
			panelSizedButton.onClick.RemoveAllListeners(); 

			if (choice.text == "n")
			{
				var path = choice.pathStringOnChoice;
				panelSizedButton.onClick.AddListener(() => { ChoicePathSelected(path); });
			}
			else
			{
				panelSizedButton.onClick.AddListener(
					() =>
					{
						if (!isFinish)
							ChoicePathSelected();
						panelSizedButton.interactable = false;
					}
					);
				var btn = Instantiate(button).GetComponent<Button>();
				btn.transform.SetParent(buttons);
				btn.transform.localScale = Vector3.one;

				var btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
				btnText.text = choice.text;

				var path = choice.pathStringOnChoice;
				btn.onClick.AddListener(() => { ChoicePathSelected(path, true, choice.text); });
			}
		}

		nextStep = false;

		if (firstStep && sentences == "" && inkStory.currentChoices.Count == 1)
		{
			ChoicePathSelected(inkStory.currentChoices[0].pathStringOnChoice, true,
				inkStory.currentChoices[0].text);
		}

		firstStep = false;

		if (!inkStory.canContinue && inkStory.currentChoices.Count == 0)
		{
			EndTalk();
		}


	}

	private void ChoicePathSelected(string path = "", bool isButton = false, string text = "")
	{
		if (!isFinish && !isButton)
		{
			isSkip = true;
		}
		else
		{
			inkStory.ChoosePathString(path);
			inkStory.Continue();
			nextStep = true;

			if (text != "")
			{
				LogPanel.Instance.AddLog("Me", text, false);
			}
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
		isFinish = false;
		for (int i = 0; i < sentences.Length; i++)
		{
			if (sentences[i] == '<' && sentences.IndexOf("<color", i) == i)
			{
				int j = sentences.IndexOf("</color>", i);
				j = j == -1 ? sentences.Length : j + 8;
				stringBuilder.Append(sentences.Substring(i, j - i));
				i = j - 1;
			}
			else if (sentences[i] == '$')
			{
				if (!isSkip) yield return new WaitForSeconds(secondsPerPauseCharacter);
			}
			else
			{
				stringBuilder.Append(sentences[i]);
				text.text = stringBuilder.ToString();
				if (!isSkip) yield return new WaitForSeconds(secondsPerCharacter);
			}
		}

		isFinish = true;
		isSkip = false;
		isSetTextCoroutineExist = false;
	}

	IEnumerator SetTextSFXCoroutine(string sentences)
	{
		switch (voiceType)
		{
			case VoiceType.Male:
				textSFXSource.clip = maleVoice;
				break;
			case VoiceType.Female:
				textSFXSource.clip = femaleVoice;
				break;
			case VoiceType.Cat:
				textSFXSource.clip = catVoice;
				break;
			case VoiceType.Young:
				textSFXSource.clip = youngVoice;
				break;
			case VoiceType.Murmuring:
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
