using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;

    public Dictionary<string, CreateInkTalkOnPlayerEnter> npcCreaters
        = new Dictionary<string, CreateInkTalkOnPlayerEnter>();

    private void Awake()
    {
        Instance = this;

        foreach (Transform npc in transform)
        {
            npcCreaters[npc.name] = npc.GetComponent<CreateInkTalkOnPlayerEnter>();
        }
    }

    private void Start()
    {
        RefreshEnable();
        RefreshTalk();
    }

    public void EnableDisableNPCOrigTalk(string who, bool state)
    {
        if (npcCreaters.ContainsKey(who) && npcCreaters[who])
        {
            npcCreaters[who].enabled = state;
        }
    }

    public void RefreshEnable()
    {
        foreach (Transform npc in transform)
        {
            if (FlagBag.Instance.HasFlag("enableNPC:" + npc.name))
            {
                npc.gameObject.SetActive(true);
            }
            if (FlagBag.Instance.HasFlag("disableNPC:" + npc.name))
            {
                npc.gameObject.SetActive(false);
            }
        }
    }

    public void RefreshTalk()
    {
        foreach (Transform npc in transform)
        {
            var creater = npcCreaters[npc.name];
            if (!creater) continue;
            var inkFunctions = StoryScript.GetAllInkFunctions(creater.inkFile);
            var tempStory = new Story(creater.inkFile.text);
            PlayerInfo.WriteToInkStory(tempStory);

            foreach (var func in inkFunctions)
            {
                //List<Ink.Runtime.Object> oldStream = null;
                tempStory.CheckInFunction(func);
                List<string> tags = new List<string>();
                while (tempStory.canContinue)
                {
                    tempStory.Continue();
                    tags.AddRange(tempStory.currentTags);
                }
                tempStory.ResetCallstack();
                // inkStroy.CheckOutFunction(oldStream);

                if (CheckEnterConditionsByTags(tags))
                {
                    creater.executeFunction = func;
                }
            }
        }
    }

    private bool CheckEnterConditionsByTags(List<string> tags)
    {
        List<string> requireTags = new List<string>();
        List<string> withoutTags = new List<string>();

        foreach (var tag in tags)
        {
            string op, data;
            StoryScript.StandardizationTag(tag, out op, out data);

            if (op == "require")
            {
                requireTags.Add(data);
            }
            else if (op == "after")
            {
                requireTags.Add(data);
            }
            else if (op == "without")
            {
                withoutTags.Add(data);
            }
        }

        return FlagBag.Instance.HasFlags(requireTags) &&
               FlagBag.Instance.WithoutFlags(withoutTags);
    }
}