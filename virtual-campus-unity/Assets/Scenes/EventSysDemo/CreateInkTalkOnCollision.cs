using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateInkTalkOnCollision : CreateInkTalk
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
            Destroy(talk);
        }
    }
}
