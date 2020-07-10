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
    public List<GameObject> painters;

    [HideInInspector]
    public GameObject currentTalk;

    private void Awake()
    {
        Instance = this;
    }

	private void Start()
	{
        DisableAllOpenedPanel();
	}

	public void DisableAllOpenedPanel()
    {
        itemBagPanel.SetActive(false);
        skinBagPanel.SetActive(false);
        badgeBagPanel.SetActive(false);
        foreach(GameObject painter in painters)
        {
            painter.SetActive(false);
        }
    }

    public void OpenTalk(GameObject talk)
    {
        DisableAllOpenedPanel();
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
}
