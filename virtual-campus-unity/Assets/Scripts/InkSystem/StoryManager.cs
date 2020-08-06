using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    public List<StoryScript> stories = new List<StoryScript>();

    private Dictionary<string, string> storyStatus  = new Dictionary<string, string>();
    private Dictionary<string, bool>   storyUpdated = new Dictionary<string, bool>();
    private Dictionary<string, string> storyDescript = new Dictionary<string, string>();

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
                MissionPanel.Instance.AddMission(s.nameForDisplay, s.description, false);
                InstantiateStory(s);
            }
            else
            {
                storyStatus.Add(name, "unavailable");
            }

            storyUpdated.Add(name, false);
            storyDescript.Add(name, s.description);
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
            if (storyUpdated[name] == true)
            {
                Debug.Log("Used2");
                MissionPanel.Instance.UpdateMissionDescription(s.nameForDisplay, storyDescript[name]);
                MissionPreviewPanel.Instance.UpdateMission(s.nameForDisplay, storyDescript[name]);
                storyUpdated[name] = false;
            }
            if (storyStatus[name] == "unavailable")
            {
                if (s.CheckStartConditions())
                {
                    storyStatus[name] = "avaliable";
                    MissionPanel.Instance.AddMission(s.nameForDisplay, s.description, false);
                    InstantiateStory(s);
                }
            }
            // else Debug.Log("Warning: Something may be wrong with the story status.");
        }
    }

    private void InstantiateStory(StoryScript s)
    {
        var obj = Instantiate(s.gameObject);
        obj.transform.parent = transform;
        obj.GetComponent<StoryScript>().GetStartConditions();
    }

    public void StartStory(StoryScript story)
    {
        var name = story.inkFile.name;
        if (storyStatus[name] == "avaliable")
        {
            storyStatus[name] = "running";
            FlagBag.Instance.AddFlag(name + "_running");
        }
    }

    public void UpdateStory(StoryScript story)
    {
        var name = story.inkFile.name;
        storyUpdated[name] = true;
        storyDescript[name] = story.description;
        Refresh();
    }

    public void EndStory(StoryScript story)
    {
        var name = story.inkFile.name;
        if (storyStatus[name] == "avaliable")
        {
            StartStory(story);
        }
        if (storyStatus[name] == "running")
        {
            storyStatus[name] = "finished";
            FlagBag.Instance.DelFlag(name + "_running");
            FlagBag.Instance.AddFlag(name);
            MissionPanel.Instance.FinishMission(story.nameForDisplay);
            Destroy(story.gameObject);

            // 清除任务内flag（任务结束就失效了）
            FlagBag.Instance.DelFlagsWithPrefix(name + "_");

            // 成就
            if (name.StartsWith("greetings of "))
            {
                EventCenter.Broadcast(EventCenter.AchievementEvent.OneWelcomeMissionFinished, null);
            }
        }
    }
}
