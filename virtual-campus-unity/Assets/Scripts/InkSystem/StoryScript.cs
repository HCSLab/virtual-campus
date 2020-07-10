using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using TMPro;

public class StoryScript : MonoBehaviour
{
    public TextAsset inkFile;
    public Story inkStroy;

    public GameObject talkPrefab;
    private GameObject buttonPrefab;

    private List<string> localFlags = new List<string>();

    private List<string> inkFunctions = new List<string>();

    private List<string> require = new List<string>();
    private List<string> without = new List<string>();
    private bool gotStartConditions = false;

    private void GetStartConditions()
    {
        if (gotStartConditions)
        {
            return;
        }

        if (inkStroy == null)
        {
            inkStroy = new Story(inkFile.text);
        }
        var tags = new List<string>();
        while (inkStroy.canContinue)
        {
            inkStroy.Continue();
            tags.AddRange(inkStroy.currentTags);
        }
        
        bool allowMultiTry = false;
        foreach (var tag in tags)
        {
            string op, data;
            StandardizationTag(tag, out op, out data);

            if (op == "after")
            {
                require.Add(data + "_story_done");
            }
            else if (op == "require")
            {
                require.Add(data);
            }
            else if (op == "without")
            {
                without.Add(data);
            }
            else if (op == "allow_multi_try")
            {
                allowMultiTry = true;
            }
        }

        if (!allowMultiTry)
        {
            without.Add(inkFile.name + "_story_done");
        }

        gotStartConditions = true;
    }

    public bool CheckStartConditions()
    {
        if (!gotStartConditions)
        {
            GetStartConditions();
        }
        if (!FlagBag.Instance.HasFlags(require))
        {
            return false;
        }
        if (!FlagBag.Instance.WithoutFlags(without))
        {
            return false;
        }
        return true;
    }

    private void Start()
    {
        buttonPrefab = talkPrefab.GetComponent<InkTalk>().button;

        inkStroy = new Story(inkFile.text);

        GetAllInkFunctions();
        foreach (var func in inkFunctions)
        {
            //List<Ink.Runtime.Object> oldStream = null;
            inkStroy.CheckInFunction(func);
            List<string> tags = new List<string>();
            while (inkStroy.canContinue)
            {
                inkStroy.Continue();
                tags.AddRange(inkStroy.currentTags);
            }
            PreprocessdTags(tags, func);
            inkStroy.ResetCallstack();
            // inkStroy.CheckOutFunction(oldStream);
        }
    }

    private void GetAllInkFunctions()
    {
        var text = inkFile.text;

        int pos = 0;
        List<string> tp = new List<string>();
        while (true)
        {
            pos = text.IndexOf("func_", pos);
            if (pos == -1)
            {
                break;
            }

            var epos = text.IndexOf('"', pos);
            var fname = text.Substring(pos, epos - pos);
            tp.Add(fname);
            pos = epos;
        }

        inkFunctions.Clear();
        foreach (var func in tp)
        {
            if (inkStroy.HasFunction(func) && !inkFunctions.Contains(func))
            {
                inkFunctions.Add(func);
            }
        }
    }

    private void StandardizationTag(string tag, out string op, out string data)
    {
        var sep = tag.IndexOf(':');
        if (sep != -1)
        {
            op = tag.Substring(0, sep);
            data = tag.Substring(sep + 1);
        }
        else
        {
            op = tag;
            data = "";
        }
        op = op.Trim();
        data = data.Trim();
    }

    public void InProcessTag(string tag, InkTalk talk)
    {
        string op, data;
        StandardizationTag(tag, out op, out data);

        if (op == "addflag")
        {
            AddFlag(data);
        }
        else if (op == "delfalg")
        {
            DelFlag(data);
        }
        else if (op == "add_global_flag")
        {
            FlagBag.Instance.AddFlag(data);
        }
        else if (op == "del_global_flag")
        {
            FlagBag.Instance.DelFlag(data);
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
        else if (op == "notfinished")
        {
            talk.notFinished = true;
        }
        else if (op == "endstory")
        {
            EndStory();
        }
    }

    private void PreprocessdTags(List<string> tags, string funcName)
    {
        List<string> attachTags = new List<string>();
        List<string> collideTriggerTags = new List<string>();
        List<string> requireTags = new List<string>();
        List<string> withoutTags = new List<string>();

        foreach (var tag in tags)
        {
            string op, data;
            StandardizationTag(tag, out op, out data);

            if (op == "attach")
            {
                attachTags.Add(data);
            }
            else if (op == "collidetrigger")
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
            button.transform.SetParent(transform);
            button.transform.localScale = Vector3.one;

            var attach = button.gameObject.AddComponent<AttachToTalk>();
            attach.speakerName = who;
            attach.require.AddRange(require);
            attach.without.AddRange(without);

            var creater = button.gameObject.AddComponent<CreateInkTalk>();
            creater.inkFile = inkFile;
            creater.executeFunction = funcName;
            creater.talkPrefab = talkPrefab;
            creater.storyScript = this;
            button.onClick.AddListener(creater.Create);

            var text = button.transform.Find("Text").GetComponent<TextMeshProUGUI>();
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
        
        var creater = obj.gameObject.AddComponent<CreateInkTalkOnPlayerEnter>();
        creater.inkFile = inkFile;
        creater.executeFunction = funcName;
        creater.talkPrefab = talkPrefab;
        creater.storyScript = this;
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

    public void EndStory()
    {
        FlagBag.Instance.AddFlag(inkFile.name + "_story_done");
        StoryManager.Instance.EndStory(this);
    }

    private void OnDestroy()
    {
        foreach (var flag in localFlags)
        {
            FlagBag.Instance.DelFlag(flag);
        }
    }
}
