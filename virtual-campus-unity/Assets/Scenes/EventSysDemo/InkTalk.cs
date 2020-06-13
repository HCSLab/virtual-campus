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

    private Text text;
    private Transform buttons;
    private GameObject button;
    private Button panelSizedButton;

    private void Start()
    {
        text = talk.transform.Find("Panel/Text").GetComponent<Text>();
        buttons = talk.transform.Find("Panel/Buttons");
        button = buttons.Find("Button").gameObject;
        button.SetActive(false);
        button.transform.parent = talk.transform;
        panelSizedButton = talk.transform.Find("Panel/BigButton").GetComponent<Button>();
        panelSizedButton.gameObject.SetActive(false);

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
            panelSizedButton.gameObject.SetActive(false);
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
            if (choice.text == "n")
            {
                panelSizedButton.gameObject.SetActive(true);
                var path = choice.pathStringOnChoice;
                panelSizedButton.onClick.RemoveAllListeners();
                panelSizedButton.onClick.AddListener(delegate { ChoicePathSelected(path); });
            }
            else
            {
                var btn = Instantiate(button).GetComponent<Button>();
                btn.gameObject.SetActive(true);
                btn.transform.parent = buttons;

                var btnText = btn.transform.Find("Text").GetComponent<Text>();
                btnText.text = choice.text;

                var path = choice.pathStringOnChoice;
                btn.onClick.AddListener(delegate { ChoicePathSelected(path); });
            }
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
