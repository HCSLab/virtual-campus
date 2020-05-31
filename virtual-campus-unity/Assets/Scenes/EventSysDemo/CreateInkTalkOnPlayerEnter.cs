using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateInkTalkOnPlayerEnter : CreateInkTalk
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Create();
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
