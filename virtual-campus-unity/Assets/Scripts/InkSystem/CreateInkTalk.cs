using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateInkTalk : MonoBehaviour
{
    public TextAsset inkFile;
    public string executeFunction;

    public GameObject talkPrefab;

    protected GameObject talk;

    [HideInInspector] public NPCInfo speaker;

    [HideInInspector] public StoryScript storyScript;

    public void Create()
    {
        talk = Instantiate(talkPrefab);
        talk.transform.SetParent(UIManager.Instance.talkContainer);
        talk.transform.localScale = Vector3.one;

        var ink = talk.GetComponent<InkTalk>();
        ink.inkFile = inkFile;
        ink.executeFunction = executeFunction;
        ink.storyScript = storyScript;
        ink.speaker = speaker;

        EventCenter.Broadcast("create_" + gameObject.name + "_talk", talk);

        UIManager.Instance.OpenTalk(talk);

        if (speaker)
        {
            speaker.StartTalkMode();
        }
    }
}
