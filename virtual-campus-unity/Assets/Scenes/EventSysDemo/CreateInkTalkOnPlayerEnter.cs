using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateInkTalkOnPlayerEnter : CreateInkTalk
{
    public List<string> require = new List<string>();
    public List<string> without = new List<string>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (FlagBag.Instance.HasFlags(require) &&
                FlagBag.Instance.WithoutFlags(without))
            {
                Create();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            UIManager.Instance.CloseTalk();
        }
    }
}
