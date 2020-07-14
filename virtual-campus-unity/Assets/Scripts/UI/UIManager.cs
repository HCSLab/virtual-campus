using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum PanelType
    {
        BadgeBag,
        ItemBag,
        SkinBag
    };

    public static UIManager Instance;

    public GameObject mainCanvas;

    public GameObject pressToTalk;
    public GameObject badgeBagPanel;
    public GameObject itemBagPanel;
    public GameObject skinBagPanel;
    public GameObject painterHub;
    public List<GameObject> painters;

    [HideInInspector]
    public GameObject currentTalk;

    private void Awake()
    {
        Instance = this;
    }

	private void Start()
	{
        GetPaintersFromHub();
        DisableAllOpenedPanel();
	}

    public void GetPaintersFromHub()
    {
        //PainterHub ph = painterHub.GetComponent<PainterHub>();
        //painters.Add(ph.entirePainter);
        //painters.Add(ph.headPainter);
        //painters.Add(ph.upperPainter);
        //painters.Add(ph.lowerPainter);
        //painters.Add(ph.hatPainter);
        //painters.Add(ph.armPainter);
    }
	public void DisableAllOpenedPanel()
    {
        itemBagPanel.SetActive(false);
        skinBagPanel.SetActive(false);
        badgeBagPanel.SetActive(false);
        painterHub.SetActive(false);
        foreach(GameObject painter in painters)
        {
            painter.SetActive(false);
        }
    }

    public void OpenTalk(GameObject talk)
    {
        DisableAllOpenedPanel();
        if (currentTalk != null)
        {
            Destroy(currentTalk);
        }
        currentTalk = talk;
    }

    public void CloseTalk(GameObject talk = null)
    {
        if (talk == null)
        {
            if (currentTalk)
            {
                Destroy(currentTalk);
                currentTalk = null;
            }
        }
        else
        {
            if (currentTalk == talk)
            {
                Destroy(currentTalk);
                currentTalk = null;
            }
        }
    }

    /// <summary>If the panel is enabled, close it,
    /// otherwise, close all the enabled panels and enable it.</summary>
    public void TryEnableOrDisablePanel(GameObject panel)
	{
        if (panel.activeSelf)
            panel.SetActive(false);
		else
		{
            DisableAllOpenedPanel();
            panel.SetActive(true);
		}
	}

    public void DisablePlayerController()
    {
        PlayerController pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        ScriptedFirstPersonAIO sfp = GameObject.FindGameObjectWithTag("Player").GetComponent<ScriptedFirstPersonAIO>();
        pc.enabled = false;
        sfp.enabled = false;
    }

    public void EnablePlayerController()
    {
        PlayerController pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        ScriptedFirstPersonAIO sfp = GameObject.FindGameObjectWithTag("Player").GetComponent<ScriptedFirstPersonAIO>();
        pc.enabled = true;
        sfp.enabled = true;
    }
}
