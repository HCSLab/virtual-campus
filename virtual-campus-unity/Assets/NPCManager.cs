using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform npc in transform)
        {
            if (FlagBag.Instance.HasFlag("enableNPC:" + npc.name))
            {
                npc.gameObject.SetActive(true);
            }
            if (FlagBag.Instance.HasFlag("disableNPC:" + npc.name))
            {
                npc.gameObject.SetActive(false);
            }
        }
    }
}