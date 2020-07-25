using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    public List<StoryScript> stories = new List<StoryScript>();

    private Dictionary<string, string> storyStatus = new Dictionary<string, string>();

    [HideInInspector] public bool refreshFlag = false;

    private void Awake()
    {
        Instance = this;
    }

	private void Start()
	{
        foreach (var s in stories)
        {
            s.GetStartConditions();
            var name = s.inkFile.name;

            if (FlagBag.Instance.HasFlag(name))
            {
                storyStatus.Add(name, "finished");
                MissionPanel.Instance.AddMission(s.nameForDisplay, s.description, true);
            }
            else if (FlagBag.Instance.HasFlag(name + "_running"))
            {
                storyStatus.Add(name, "running");
                MissionPanel.Instance.AddMission(s.nameForDisplay, s.description, false);
                InstantiateStory(s);
            }
            else if (s.CheckStartConditions())
            {
                storyStatus.Add(name, "avaliable");
                InstantiateStory(s);
            }
            else
            {
                storyStatus.Add(name, "unavailable");
            }
        }
    }

	private void LateUpdate()
    {
        if (refreshFlag)
        {
            Refresh();
            refreshFlag = false;
        }
    }

    private void Refresh()
    {
        foreach (var s in stories)
        {
            var name = s.inkFile.name;
            if (storyStatus[name] == "unavailable")
            {
                if (s.CheckStartConditions())
                {
                    storyStatus[name] = "avaliable";
                    InstantiateStory(s);
                }
            }
        }
    }

    private void InstantiateStory(StoryScript s)
    {
        var obj = Instantiate(s.gameObject);
        obj.transform.parent = transform;
    }

    public void StartStory(StoryScript story)
    {
        var name = story.inkFile.name;
        storyStatus[name] = "running";
        FlagBag.Instance.AddFlag(name + "_running");
        MissionPanel.Instance.AddMission(story.nameForDisplay, story.description, false);
    }

    public void EndStory(StoryScript story)
    {
        var name = story.inkFile.name;
        storyStatus[name] = "finished";
        FlagBag.Instance.DelFlag(name + "_running");
        FlagBag.Instance.AddFlag(name);
        MissionPanel.Instance.FinishMission(story.nameForDisplay);
        Destroy(story.gameObject);
    }
}
