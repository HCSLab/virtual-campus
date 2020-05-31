using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;

public class InkTalk : MonoBehaviour
{
    public TextAsset inkFile;
    public string executeFunction;

    private Story inkStroy;
    private bool nextStep;
    private bool firstStep;

    public GameObject talk;

    private Text text;
    private Transform buttons;
    private GameObject button;

    public EventOperator afterStoryOperator;

    private void Start()
    {
        text = talk.transform.Find("Panel/Text").GetComponent<Text>();
        buttons = talk.transform.Find("Panel/Buttons");
        button = buttons.Find("Button").gameObject;
        button.SetActive(false);
        button.transform.parent = talk.transform;

        nextStep = true;
        firstStep = true;

        inkStroy = new Story(inkFile.text);

        if (inkStroy.HasFunction(executeFunction))
        {
            inkStroy.EvaluateFunction(executeFunction, text,text);
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
        }

        for (var i = 0; i < inkStroy.currentChoices.Count; i++)
        {
            var btn = Instantiate(button).GetComponent<Button>();
            btn.gameObject.SetActive(true);
            btn.transform.parent = buttons;

            var choice = inkStroy.currentChoices[i];
            var btnText = btn.transform.Find("Text").GetComponent<Text>();
            btnText.text = choice.text;

            var path = choice.pathStringOnChoice;
            btn.onClick.AddListener(delegate { ChoicePathSelected(path); });
        }

        nextStep = false;
        firstStep = false;

        if (!inkStroy.canContinue && inkStroy.currentChoices.Count == 0)
        {
            EndOfStroy();
        }
    }

    private void ChoicePathSelected(string path)
    {
        inkStroy.ChoosePathString(path);
        inkStroy.Continue();
        nextStep = true;
    }

    private void EndOfStroy()
    {
        var eventOp = GetComponent<EventOperator>();
        if (eventOp)
        {
            eventOp.ExecuteOnConditions();
        }

        UIManager.Instance.CloseTalk();
    }
}
