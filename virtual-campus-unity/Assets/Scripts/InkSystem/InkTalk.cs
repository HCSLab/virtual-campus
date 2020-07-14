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

    public TextMeshProUGUI text;
    public Transform buttons;
    public GameObject button;
    public Button panelSizedButton;

    private void Start()
    {
        panelSizedButton.gameObject.SetActive(false);

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
            panelSizedButton.gameObject.SetActive(false);
        }

        text.text = "";
        while (inkStory.canContinue)
        {
            text.text += inkStory.Continue();
            var tags = inkStory.currentTags;
            if (storyScript)
            {
                foreach (var tag in tags)
                {
                    storyScript.InProcessTag(tag, this, inkStory);
                }
            }
        }

        foreach (var choice in inkStory.currentChoices)
        {
            if (choice.text == "n")
            {
                panelSizedButton.gameObject.SetActive(true);
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
                btn.onClick.AddListener(() => { ChoicePathSelected(path); });
            }
        }

        nextStep = false;
        firstStep = false;

        if (text.text == "" && inkStory.currentChoices.Count == 1)
        {
            ChoicePathSelected(inkStory.currentChoices[0].pathStringOnChoice);
        }

        if (!inkStory.canContinue && inkStory.currentChoices.Count == 0)
        {
            EndTalk();
        }
    }

    private void ChoicePathSelected(string path)
    {
        inkStory.ChoosePathString(path);
        inkStory.Continue();
        nextStep = true;
    }

    private void EndTalk()
    {
        if (storyScript)
        {
            storyScript.ProcessFunctionHeaderTags();

            if (!notFinished)
                storyScript.AddFlag(executeFunction);
        }

        UIManager.Instance.CloseTalk(gameObject);
    }
}
