using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [HideInInspector] public GameObject currentTalk;
    public GameObject pressT;
    public GameObject itemBag;
    public GameObject badgeBag;
    public GameObject skinBag;
    public List<GameObject> painters;

    private void Awake()
    {
        Instance = this;
    }

    public void CloseAllOpenedPanel()
    {
        ClosePanel(itemBag);
        ClosePanel(skinBag);
        ClosePanel(badgeBag);
        foreach(GameObject painter in painters)
        {
            DeactivatePanel(painter);
        }
    }

    public void OpenTalk(GameObject talk)
    {
        CloseAllOpenedPanel();
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

    public void ShowPressT()
    {
        pressT.SetActive(true);
    }
    public void HidePressT()
    {
        pressT.SetActive(false);
    }

    public void ClosePanel(GameObject panel)
    {
        panel.GetComponent<CanvasGroup>().alpha = 0;
        panel.GetComponent<CanvasGroup>().interactable = false;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OpenPanel(GameObject panel)
    {
        panel.GetComponent<CanvasScaler>().enabled = true; 
        panel.GetComponent<CanvasGroup>().alpha = 1;
        panel.GetComponent<CanvasGroup>().interactable = true;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void ActivatePanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void DeactivatePanel(GameObject panel)
    {
        panel.SetActive(false);
        panel.GetComponent<CanvasScaler>().enabled = true;
    }

    public void ItemBagButtonClicked()
    {
        if (itemBag.GetComponent<CanvasGroup>().alpha == 1)
        {
            ClosePanel(itemBag);
        }
        else
        {
            CloseAllOpenedPanel();
            OpenPanel(itemBag);
        }
    }
    public void BadgeBagButtonClicked()
    {
        if (badgeBag.GetComponent<CanvasGroup>().alpha == 1)
        {
            ClosePanel(badgeBag);
        }
        else
        {
            CloseAllOpenedPanel();
            OpenPanel(badgeBag);
        }
    }

    public void SkinBagButtonClicked()
    {
        if (skinBag.GetComponent<CanvasGroup>().alpha == 1)
        {
            ClosePanel(skinBag);
        }
        else
        {
            CloseAllOpenedPanel();
            OpenPanel(skinBag);
        }
    }
/*
    public void painterButtonClicked()
    {
        if (painter.GetComponent<CanvasGroup>().alpha == 1)
        {
            closePanel(painter);
        }
        else
        {
            CloseAllOpenedPanel();
            openPanel(painter);
        }
    }
    */
}
