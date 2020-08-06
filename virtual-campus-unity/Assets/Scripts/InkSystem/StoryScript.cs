using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Experimental.Rendering.Universal;

public class StoryScript : MonoBehaviour
{
    public TextAsset inkFile;
    public Story inkStory;

    [HideInInspector] public int talkCount = 0;
    [HideInInspector] public bool endStory = false;

    [HideInInspector] public string nameForDisplay;

    [HideInInspector] public string description; 
    [HideInInspector] public bool updated = false;

    public GameObject talkPrefab;
   
    private GameObject buttonPrefab;

    private List<string> inkFunctions = new List<string>();

    private List<string> require = new List<string>();
    private List<string> without = new List<string>();
    private Dictionary<string, string> strCond = new Dictionary<string, string>();
    private Dictionary<string, int>    numCond = new Dictionary<string, int>();

    private List<UnityEngine.Object> dynamicallyGenerated = new List<UnityEngine.Object>();
    private List<string> npcOverrided = new List<string>();

    private List<string> headerTags = new List<string>();

    public void GetStartConditions()
    {
        var tempStory = new Story(inkFile.text);
        PlayerInfo.WriteToInkStory(tempStory);

        while (tempStory.canContinue)
        {
            tempStory.Continue();
            headerTags.AddRange(tempStory.currentTags);
        }
        
        bool allowMultiTry = false;
        require.Clear();
        without.Clear();
        foreach (var tag in headerTags)
        {
            string op, data;
            StandardizationTag(tag, out op, out data);

            if (op == "after")
            {
                require.Add(data);
            }
            else if (op == "require")
            {
                require.Add(data);
            }
            else if (op == "without")
            {
                without.Add(data);
            }
            else if (op == "num_condition")   // eg. #num_condition: likeness_50
            {
                string variable;
                int condition;
                var sep = data.IndexOf("_");
                
                variable = data.Substring(0, sep);
                condition = (int)Convert.ToDouble(data.Substring(sep + 1));
                if (!numCond.ContainsKey(variable))
                    numCond.Add(variable, condition);
            }
            else if (op == "str_condition")   // eg. #str_condition: school_SME
            {
                string variable;
                string condition;
                var sep = data.IndexOf("_");
                
                variable = data.Substring(0, sep);
                condition = data.Substring(sep + 1);
                strCond.Add(variable, condition);
            }
            else if (op == "allow_multi_try")
            {
                allowMultiTry = true;
            }
            else if (op == "name")
            {
                nameForDisplay = data;
            }
            else if (op == "description")
            {
                description = data;
            }
        }

        if (!allowMultiTry)
        {
            without.Add(inkFile.name);
        }

        var tmp = FlagBag.Instance.GetFlagsWithPrefix("description_" + inkFile.name + ":");
        if (tmp.Count > 0)
        {
            StandardizationTag(tmp[0], out var op, out var data);
            description = data;
        }
    }

    public bool CheckStartConditions()
    {
        foreach (var i in numCond)
        {
            if (PlayerInfo.digit.ContainsKey(i.Key))
            {
                if (PlayerInfo.digit[i.Key] < i.Value)
                { // Debug.Log("Failure Detected"); 
                    return false; }
                // else Debug.Log("Success in Initiating");
            }
            else Debug.Log("The num conditions is not found! Please check your ink file tags.");
        }
        if (!FlagBag.Instance.HasFlags(require))
        {
            // Debug.Log("Flags Failure Detected");
            // Debug.Log(require[require.Count - 1]);
            return false;
        }
        if (!FlagBag.Instance.WithoutFlags(without))
        {
            return false;
        }

        foreach (var i in strCond)
        {
            if (PlayerInfo.info.ContainsKey(i.Key))
            {
                if (PlayerInfo.info[i.Key] != i.Value) return false;
            }
            else Debug.Log("The string conditions is not found! Please check your ink file tags.");
        }
        return true;
    }

    private void Start()
    {
        GetStartConditions();

        buttonPrefab = talkPrefab.GetComponent<InkTalk>().button;
        inkStory = new Story(inkFile.text);

        ProcessObjectEnableDisableWhenStart();
        ProcessInitializeTags();

        inkFunctions = GetAllInkFunctions(inkFile);
        ProcessFunctionHeaderTags();
    }

    private void ProcessObjectEnableDisableWhenStart()
    {
        foreach (var flag in FlagBag.Instance.bag)
        {
            if (flag.StartsWith(inkFile.name + "_enable:"))
            {
                StandardizationTag(flag, out string op, out string data);
                var obj = transform.Find(data);
                if (obj)
                {
                    obj.gameObject.SetActive(true);
                }
            }
            if (flag.StartsWith(inkFile.name + "_disable:"))
            {
                StandardizationTag(flag, out string op, out string data);
                var obj = transform.Find(data);
                if (obj)
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }
    }

    public void ProcessInitializeTags()
    {
        // initialize只会执行一次，哪怕在任务中间退出，然后重新启动，也不会执行第二次
        if (FlagBag.Instance.HasFlag(inkFile.name + "_initialize"))
        {
            return;
        }
        FlagBag.Instance.AddFlag(inkFile.name + "_initialize");

        foreach (var tag in headerTags)
        {
            string op, data;
            StandardizationTag(tag, out op, out data);

            if (op == "enableNPC")
            {
                ProcessEnableDisableFlag("enableNPC:" + data);
                NPCManager.Instance.RefreshEnable();
            }
            else if (op == "disableNPC")
            {
                ProcessEnableDisableFlag("disableNPC:" + data);
                NPCManager.Instance.RefreshEnable();
            }
        }
    }

    public void ProcessFunctionHeaderTags()
    {
        EliminateEffects();

        var tempStory = new Story(inkFile.text);
        PlayerInfo.WriteToInkStory(tempStory);

        for (int i = inkFunctions.Count - 1; i >= 0; i--)
        {
            var func = inkFunctions[i];
            //List<Ink.Runtime.Object> oldStream = null;
            tempStory.CheckInFunction(func);
            List<string> tags = new List<string>();
            while (tempStory.canContinue)
            {
                tempStory.Continue();
                tags.AddRange(tempStory.currentTags);
            }
            PreprocessdTags(tags, func, tempStory);
            tempStory.ResetCallstack();
            // inkStroy.CheckOutFunction(oldStream);
        }
    }

    public static List<string> GetAllInkFunctions(TextAsset inkFile)
    {
        var funcs = new List<string>();
        var text = inkFile.text;
        var story = new Story(text);

        int pos = 0;
        List<string> tmp = new List<string>();
        while (true)
        {
            pos = text.IndexOf("func_", pos);
            if (pos == -1)
            {
                break;
            }

            var epos = text.IndexOf('"', pos);
            var fname = text.Substring(pos, epos - pos);
            tmp.Add(fname);
            pos = epos;
        }

        foreach (var func in tmp)
        {
            if (story.HasFunction(func) && !funcs.Contains(func))
            {
                funcs.Add(func);
            }
        }
        return funcs;
    }

    public static void StandardizationTag(string tag, out string op, out string data)
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

    public void InProcessTag(string tag, InkTalk talk, Story tempStory)
    {
        string op, data;
        StandardizationTag(tag, out op, out data);
        if (op == "addflag")
        {
            AddLocalFlag(data);
        }
        else if (op == "delfalg")
        {
            DelLocalFlag(data);
        }
        else if (op == "addflag_global")
        {
            FlagBag.Instance.AddFlag(data);
        }
        else if (op == "delflag_global")
        {
            FlagBag.Instance.DelFlag(data);
        }
        else if (op == "enable")
        {
            var obj = transform.Find(data);
            if (obj)
            {
                ProcessEnableDisableFlag(inkFile.name + "_enable:" + data);
                obj.gameObject.SetActive(true);
            }
        }
        else if (op == "disable")
        {
            var obj = transform.Find(data);
            if (obj)
            {
                ProcessEnableDisableFlag(inkFile.name + "_disable:" + data);
                obj.gameObject.SetActive(false);
            }
        }
        else if (op == "notfinished")
        {
            talk.notFinished = true;
        }
        else if (op == "endstory")
        {
            endStory = true;
        }
        else if (op == "upd_info")
        {
            PlayerInfo.UpdateFromInkStory(tempStory);
        }
        else if (op == "upd_description")
        {
            FlagBag.Instance.DelFlagsWithPrefix("description_" + inkFile.name + ":");
            description = data + "\n\n" + "<color=grey>" + description + "</color>";
            FlagBag.Instance.AddFlag("description_" + inkFile.name + ":" + description);
            MissionPanel.Instance.UpdateMissionDescription(nameForDisplay, description);
            MissionPreviewPanel.Instance.UpdateMission(nameForDisplay, description);
        }
        else if (op == "enableNPC")
        {
            ProcessEnableDisableFlag("enableNPC:" + data);
            NPCManager.Instance.RefreshEnable();
        }
        else if (op == "disableNPC")
        {
            ProcessEnableDisableFlag("disableNPC:" + data);
            NPCManager.Instance.RefreshEnable();
        }
        else if (op == "additem")
        {
            ItemPanel.Instance.AddItem(data);
            FlagBag.Instance.AddFlag("item_" + data);
        }
        else if (op == "delitem")
        {
            ItemPanel.Instance.RemoveItem(data);
            FlagBag.Instance.DelFlag("item_" + data);
        }
        else if (op == "addskin")
        {
            ItemPanel.Instance.AddSkin(data);
            FlagBag.Instance.AddFlag("skin_" + data);
        }
        else if (op == "addphoto")
        {
            ItemPanel.Instance.AddPhoto(data);
            FlagBag.Instance.AddFlag("photo_" + data);
        }
    }

    private void ProcessEnableDisableFlag(string toAdd)
    {
        string toDel;
        if (toAdd.StartsWith("enable"))
        {
            toDel = toAdd.Replace("enable", "disable");
        }
        else if (toAdd.StartsWith("disable"))
        {
            toDel = toAdd.Replace("disable", "enable");
        }
        else
        {
            return;
        }

        if (FlagBag.Instance.HasFlag(toDel))
        {
            FlagBag.Instance.DelFlag(toDel);
        }
        if (!FlagBag.Instance.HasFlag(toAdd))
        {
            FlagBag.Instance.AddFlag(toAdd);
        }
    }

    private void PreprocessdTags(List<string> tags, string funcName, Story tempStory)
    {
        List<string> attachTags = new List<string>();
        List<string> collideTriggerTags = new List<string>();
        List<string> requireTags = new List<string>();
        List<string> withoutTags = new List<string>();
        bool isOverride = false;
        bool talkImediatelyAfterCollision = false;

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
                requireTags.Add(inkFile.name + "_" + data);
            }
            else if (op == "after")
            {
                requireTags.Add(inkFile.name + "_" + data);
            }
            else if (op == "without")
            {
                withoutTags.Add(inkFile.name + "_" + data);
            }
            else if (op == "require_global")
            {
                requireTags.Add(data);
            }
            else if (op == "without_global")
            {
                withoutTags.Add(data);
            }
            else if (op == "override")
            {
                isOverride = true;
            }
            else if (op == "require_item")
            {
                requireTags.Add("item_" + data);
            }
            else if (op == "require_skin")
            {
                requireTags.Add("skin_" + data);
            }
            else if (op == "require_photo")
            {
                requireTags.Add("photo_" + data);
            }
            else if (op == "talk_immediately_after_collision")
            {
                talkImediatelyAfterCollision = true;
            }
        }

        withoutTags.Add(inkFile.name + "_" + funcName);


        foreach (var who in attachTags)
        {
            AttachToSpeaker(who, funcName, tempStory.currentChoices, requireTags, withoutTags);
        }
        
        foreach (var who in collideTriggerTags)
        {
            if (isOverride)
            {
                if (OverrideNPCTalk(who, requireTags, withoutTags))
                {
                    AddCollideTrigger(who, funcName, requireTags, withoutTags, talkImediatelyAfterCollision);
                }
            }
            else
            {
                AddCollideTrigger(who, funcName, requireTags, withoutTags, talkImediatelyAfterCollision);
            }
        }
    }

    private void AttachToSpeaker(string who, string funcName, List<Choice> choices, List<string> require, List<string> without)
    {   
        foreach (var choice in choices)
        {
            var button = Instantiate(buttonPrefab).GetComponent<Button>();
            dynamicallyGenerated.Add(button.gameObject);
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

    private void AddCollideTrigger(string who, string funcName, List<string> require, List<string> without, bool talkImediatelyAfterCollision)
    {
        var obj = transform.Find(who);
        if (!obj)
        {
            obj = NPCManager.Instance.transform.Find(who);
        }
        if (!obj)
        {
            Debug.LogWarning("No specific GameObject found in AddCollideTrigger in " + name + ":" + funcName + ":" + who);
            return;
        }
        
        var creater = obj.gameObject.AddComponent<CreateInkTalkOnPlayerEnter>();
        dynamicallyGenerated.Add(creater);
        creater.inkFile = inkFile;
        creater.executeFunction = funcName;
        creater.talkPrefab = talkPrefab;
        creater.storyScript = this;
        creater.require.AddRange(require);
        creater.without.AddRange(without);
        creater.talkImmediatelyAfterCollision = talkImediatelyAfterCollision;
    }

    private bool OverrideNPCTalk(string who, List<string> require, List<string> without)
    {
        if (!npcOverrided.Contains(who) &&
            FlagBag.Instance.HasFlags(require) &&
            FlagBag.Instance.WithoutFlags(without))
        {
            npcOverrided.Add(who);
            NPCManager.Instance.EnableDisableNPCOrigTalk(who, false);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddLocalFlag(string flag)
    {
        FlagBag.Instance.AddFlag(inkFile.name + "_" + flag);
    }

    public void DelLocalFlag(string flag)
    {
        FlagBag.Instance.DelFlag(inkFile.name + "_" + flag);
    }

    private void EliminateEffects()
    {
        foreach (var obj in dynamicallyGenerated)
        {
            Destroy(obj);
        }
        dynamicallyGenerated.Clear();
        foreach (var npc in npcOverrided)
        {
            NPCManager.Instance.EnableDisableNPCOrigTalk(npc, true);
        }
        npcOverrided.Clear();
    }

    private void OnDestroy()
    {
        EliminateEffects();
    }
}
