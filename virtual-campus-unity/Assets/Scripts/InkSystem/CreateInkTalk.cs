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

    [HideInInspector] public StoryScript storyScript;

    public void Create()
    {
        talk = Instantiate(talkPrefab);
        talk.transform.SetParent(UIManager.Instance.mainCanvas.transform);
        
        // Set the proper size of the talk panel.
        talk.transform.localScale = Vector3.one;
        var prefabRT = talkPrefab.GetComponent<RectTransform>();
        var RT = talk.GetComponent<RectTransform>();
        RT.localScale = prefabRT.localScale;
        RT.offsetMin = new Vector2(prefabRT.offsetMin.x, RT.offsetMin.y);
        RT.offsetMax = new Vector2(prefabRT.offsetMax.x, RT.offsetMax.y);
        RT.anchoredPosition = prefabRT.anchoredPosition;

        var ink = talk.GetComponent<InkTalk>();
        ink.inkFile = inkFile;
        ink.executeFunction = executeFunction;
        ink.storyScript = storyScript;

        EventCenter.Broadcast("create_" + gameObject.name + "_talk", talk);

        UIManager.Instance.OpenTalk(talk);
    }
}
