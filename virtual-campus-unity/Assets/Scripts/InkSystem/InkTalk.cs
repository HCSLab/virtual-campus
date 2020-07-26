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
    [HideInInspector] public TextAsset inkFile;
    [HideInInspector] public string executeFunction;

    [HideInInspector] public Story inkStory;
    private bool nextStep;
    private bool firstStep;
    [HideInInspector] public bool notFinished;

    [HideInInspector] public StoryScript storyScript;

    [HideInInspector] public string speakerNameForDisplay;

    public TextMeshProUGUI text, nameText;
    public Transform buttons;
    public GameObject button;
    public Button panelSizedButton;

    private void Start()
    {
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
        }
        if (sentences != "")
        {
            text.text = sentences;
            nameText.text = speakerNameForDisplay;
            LogPanel.Instance.AddLog(speakerNameForDisplay, sentences, false);
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

        LogPanel.Instance.AddLog("", "", false);

        UIManager.Instance.CloseTalk(gameObject);
    }
}
