using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [HideInInspector] public GameObject currentTalk; 

    private void Awake()
    {
        Instance = this;
    }

    public void OpenTalk(GameObject talk)
    {
        CloseTalk();
        currentTalk = talk;
    }

    public void CloseTalk()
    {
        if (currentTalk)
        {
            Destroy(currentTalk);
        }
    }
}
