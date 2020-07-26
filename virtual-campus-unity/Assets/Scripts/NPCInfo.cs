using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCInfo : MonoBehaviour
{
    public string npcName { get; private set; }
    public GameObject npcCamera;
    public Transform playerDialoguePos;

    private GameObject mainCam;

    private void Start()
    {
        npcName = transform.Find("Canvas/Name").GetComponent<TextMeshProUGUI>().text;
        mainCam = Camera.main.gameObject;
    }

    public void StartTalkMode()
    {
        npcCamera.SetActive(true);
        mainCam.SetActive(false);

        var player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<ScriptedFirstPersonAIO>().playerCanMove = false;
        var model = player.GetComponent<PlayerController>().model;
        player.transform.position = playerDialoguePos.position;
        model.transform.forward = playerDialoguePos.forward;
    }

    public void EndTalkMode()
    {
        npcCamera.SetActive(false);
        mainCam.SetActive(true);

        var player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().enabled = true;
        player.GetComponent<ScriptedFirstPersonAIO>().playerCanMove = true;
    }
}
