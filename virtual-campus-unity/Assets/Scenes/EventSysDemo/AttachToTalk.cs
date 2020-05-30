using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AttachToTalk : MonoBehaviour
{
    [HideInInspector] public GameObject talk;

    public List<string> hasFlag = new List<string>();

    public string talkerName;

    private void Start()
    {
        EventCenter.AddListener("create_" + talkerName + "_talk", OnTalkCreate);
    }

    private void OnTalkCreate(object data)
    {
        talk = (GameObject)data;

        var buttons = talk.transform.Find("Panel/Buttons");
        var btn = Instantiate(gameObject);
        btn.transform.parent = buttons;
        Destroy(btn.GetComponent<AttachToTalk>());
        btn.SetActive(CheckConditions());
    }

    public bool CheckConditions()
    {
        foreach (var flag in hasFlag)
        {
            if (!FlagBag.Instance.HasFlag(flag))
            {
                return false;
            }
        }
        return true;
    }
}
