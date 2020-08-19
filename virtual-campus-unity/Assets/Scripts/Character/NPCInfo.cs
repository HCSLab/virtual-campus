using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Voice { Male, Female, Cat, Young, Murmuring};

public class NPCInfo : MonoBehaviour
{
    public string npcName { get; private set; }
    public GameObject npcCamera;
    public Transform playerDialoguePos;

    public static GameObject mainCam;
    
    public Voice voice;

    private void Awake()
    {
        var textmesh = transform.Find("Canvas/Name").GetComponent<TextMeshProUGUI>();
        textmesh.enableCulling = true;
        npcName = textmesh.text;

        if (!mainCam)
            mainCam = Camera.main.gameObject;
    }

    public void StartTalkMode()
    {
        npcCamera.SetActive(true);
        if (!mainCam) mainCam = Camera.main.gameObject;
        // mainCam.SetActive(false);

        var player = GameObject.FindGameObjectWithTag("Player");
        var controller = player.GetComponent<PlayerController>();
        controller.FreezeUnfreezePlayer(true);
        player.transform.position = playerDialoguePos.position;
        controller.model.transform.forward = playerDialoguePos.forward;
    }

    public void EndTalkMode()
    {
        npcCamera.SetActive(false);
        // mainCam.SetActive(true);

        var player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().FreezeUnfreezePlayer(false);
    }
}
