using Ink.Runtime;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AttachToTalk : MonoBehaviour
{
    [HideInInspector] public GameObject talk;

    public string speakerName;

    public List<string> require = new List<string>();
    public List<string> without = new List<string>();

    private void Start()
    {
        EventCenter.AddListener("create_" + speakerName + "_talk", OnTalkCreate);
    }

    private void OnTalkCreate(object data)
    {
        talk = (GameObject)data;

        var buttons = talk.transform.Find("Panel/Buttons");
        var btn = Instantiate(gameObject).GetComponent<Button>();
        btn.transform.SetParent(buttons);
        btn.transform.localScale = Vector3.one;
        Destroy(btn.GetComponent<AttachToTalk>());
        btn.onClick = GetComponent<Button>().onClick;

        bool active = FlagBag.Instance.HasFlags(require) && 
                      FlagBag.Instance.WithoutFlags(without);
        btn.gameObject.SetActive(active);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener("create_" + speakerName + "_talk", OnTalkCreate);
    }
}
