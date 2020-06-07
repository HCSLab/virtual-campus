using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateInkTalkOnPlayerEnter : CreateInkTalk
{
    public List<string> require = new List<string>();
    public List<string> without = new List<string>();

    private bool playerInRange = false;
    private bool talkOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = true;
        }
    }

    private void Update()
    {
        if (playerInRange &&
            !talkOpened &&
            FlagBag.Instance.HasFlags(require) &&
            FlagBag.Instance.WithoutFlags(without))
        {
            UIManager.Instance.ShowPressT();

            if (Input.GetKeyDown(KeyCode.T))
            {
                Create();
                talkOpened = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = false;
            talkOpened = false;
            UIManager.Instance.CloseTalk(talk);
            UIManager.Instance.HidePressT();
        }
    }
}
