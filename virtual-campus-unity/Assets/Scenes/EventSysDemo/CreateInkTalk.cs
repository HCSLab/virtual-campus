using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateInkTalk : MonoBehaviour
{
    public TextAsset inkFile;
    public string executeFunction;

    public GameObject talkPrefab;

    protected GameObject talk;

    [HideInInspector] public StoryScript storyScript;

    public void Create()
    {
        talk = Instantiate(talkPrefab);

        var ink = talk.GetComponent<InkTalk>();
        ink.inkFile = inkFile;
        ink.talk = talk;
        ink.executeFunction = executeFunction;
        ink.storyScript = storyScript;

        EventCenter.Broadcast("create_" + gameObject.name + "_talk", talk);

        UIManager.Instance.OpenTalk(talk);
    }
}
