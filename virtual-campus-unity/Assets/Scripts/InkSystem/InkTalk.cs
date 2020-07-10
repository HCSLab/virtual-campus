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

    [HideInInspector] public Story inkStroy;
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
