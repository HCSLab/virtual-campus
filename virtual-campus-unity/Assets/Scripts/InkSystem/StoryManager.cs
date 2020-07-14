using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    public List<StoryScript> stories = new List<StoryScript>();

    private Dictionary<string, StoryScript> runningStories = new Dictionary<string, StoryScript>();
    private Dictionary<string, string> storyStatus = new Dictionary<string, string>();

    [HideInInspector] public bool refreshFlag;

    private void Awake()
    {
        Instance = this;

        // story staeus should be load from file or host later.
        foreach (var s in stories)
        {
            storyStatus.Add(s.inkFile.name, "not_started");
            s.GetStartConditions();
        }

        refreshFlag = true;
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
            if (!storyStatus.ContainsKey(name) || storyStatus[name] == "not_started")
            {
                if (s.CheckStartConditions())
                {
                    storyStatus[name] = "running";
                    var obj = Instantiate(s.gameObject);
                    obj.transform.parent = transform;
                    var SS = obj.GetComponent<StoryScript>();
                    runningStories.Add(name, SS);
                }
            }
        }
    }

    public void EndStory(StoryScript story)
    {
        var name = story.inkFile.name;
        storyStatus[name] = "finished";
        runningStories.Remove(name);
        Destroy(story.gameObject);
    }
}
