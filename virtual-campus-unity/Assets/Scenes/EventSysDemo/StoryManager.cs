using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

public class StoryManager : MonoBehaviour
{
    public TextAsset inkFile;
    public Story inkStroy;

    public GameObject talkPrefab;
    private GameObject buttonPrefab;

    private List<string> localFlags = new List<string>();

    private List<string> inkFunctions = new List<string>();

    private void Start()
    {
        buttonPrefab = talkPrefab.transform.Find("Panel/Buttons/Button").gameObject;

        inkStroy = new Story(inkFile.text);

        GetAllInkFunctions();

        foreach (var func in inkFunctions)
        {
            if (!inkStroy.HasFunction(func))
            {
                continue;
            }

            List<Ink.Runtime.Object> oldStream = null;
            inkStroy.CheckInFunction(func, out oldStream);
            while (inkStroy.canContinue)
            {
                inkStroy.Continue();
                var tags = inkStroy.currentTags;
                foreach (var tag in tags)
                {
                    PreprocessdTag(tag, func);
                }
            }
            inkStroy.CheckOutFunction(oldStream);
        }
    }

    private void GetAllInkFunctions()
    {
        var text = inkFile.text;

        int pos = 0;
        while (true)
        {
            pos = text.IndexOf("func_", pos);
            if (pos == -1)
            {
                break;
            }

            var epos = text.IndexOf('"', pos);
            var fname = text.Substring(pos, epos - pos);
            inkFunctions.Add(fname);
            pos = epos;
        }
    }

    public void InProcessTag(string tag, InkTalk talk)
    {
        tag = tag.Replace(" ", "");
        var sep = tag.IndexOf(':');
        var op = tag.Substring(0, sep);
        var data = tag.Substring(sep + 1);

        if (op == "addflag")
        {
            AddFlag(data);
        }
        else if (op == "delfalg")
        {
            DelFlag(data);
        }
        else if (op == "enable")
        {
            var obj = transform.Find(data);
            if (obj)
            {
                obj.gameObject.SetActive(true);
            }
        }
        else if (op == "disable")
        {
            var obj = transform.Find(data);
            if (obj)
            {
                obj.gameObject.SetActive(false);
            }
        }
        else if (op == "not_finished")
        {
            talk.notFinished = true;
        }
    }

    private void PreprocessdTag(string tag, string funcName)
    {
        tag = tag.Replace(" ", "");
        var sep = tag.IndexOf(':');
        var op = tag.Substring(0, sep);
        var data = tag.Substring(sep + 1);

        List<string> attachTags = new List<string>();
        List<string> collideTriggerTags = new List<string>();
        List<string> requireTags = new List<string>();
        List<string> withoutTags = new List<string>();

        if (op == "attach")
        {
            attachTags.Add(data);
        }
        else if (op == "collide_triger")
        {
            collideTriggerTags.Add(data);
        }
        else if (op == "require")
        {
            requireTags.Add(data);
        }
        else if (op == "after")
        {
            requireTags.Add(data + "_done");
        }
        else if (op == "without")
        {
            withoutTags.Add(data);
        }

        withoutTags.Add(funcName + "_done");

        foreach (var who in attachTags)
        {
            AttachToSpeaker(who, funcName, requireTags, withoutTags);
        }

        foreach (var who in collideTriggerTags)
        {
            AddCollideTrigger(who, funcName, requireTags, withoutTags);
        }
    }

    private void AttachToSpeaker(string who, string funcName, List<string> require, List<string> without)
    {   
        foreach (var choice in inkStroy.currentChoices)
        {
            var button = Instantiate(buttonPrefab).GetComponent<Button>();
            button.transform.parent = transform;

            button.gameObject.AddComponent<AttachToTalk>();
            var attach = button.GetComponent<AttachToTalk>();
            attach.speakerName = who;
            attach.require.AddRange(require);
            attach.without.AddRange(without);

            button.gameObject.AddComponent<CreateInkTalk>();
            var creater = button.GetComponent<CreateInkTalk>();
            creater.inkFile = inkFile;
            creater.executeFunction = funcName;
            creater.talkPrefab = talkPrefab;
            button.onClick.AddListener(creater.Create);

            var text = button.transform.Find("Text").GetComponent<Text>();
            text.text = choice.text;

            button.name = choice.text;
        }
    }

    private void AddCollideTrigger(string who, string funcName, List<string> require, List<string> without)
    {
        var obj = transform.Find(who);
        if (!obj)
        {
            Debug.LogWarning("No specific GameObject found in AddCollideTrigger in " + name + ":" + funcName + ":" + who);
            return;
        }
        
        obj.gameObject.AddComponent<CreateInkTalkOnPlayerEnter>();
        var creater = obj.GetComponent<CreateInkTalkOnPlayerEnter>();
        creater.inkFile = inkFile;
        creater.executeFunction = funcName;
        creater.talkPrefab = talkPrefab;
        creater.require.AddRange(require);
        creater.without.AddRange(without);
    }

    public void AddFlag(string flag)
    {
        localFlags.Add(flag);
        FlagBag.Instance.AddFlag(flag);
    }

    public void DelFlag(string flag)
    {
        localFlags.Remove(flag);
        FlagBag.Instance.DelFlag(flag);
    }

    private void OnDestroy()
    {
        foreach (var flag in localFlags)
        {
            FlagBag.Instance.DelFlag(flag);
        }
    }
}
