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
    public GameObject painter;

    private void Awake()
    {
        Instance = this;
    }

    public void CloseAllOpenedPanel()
    {
        closePanel(itemBag);
        closePanel(skinBag);
        //closeBag(badgeBag);
        closePanel(painter);
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

    public void closePanel(GameObject panel)
    {
        panel.GetComponent<CanvasGroup>().alpha = 0;
        panel.GetComponent<CanvasGroup>().interactable = false;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void openPanel(GameObject panel)
    {
        panel.GetComponent<CanvasScaler>().enabled = true; 
        panel.GetComponent<CanvasGroup>().alpha = 1;
        panel.GetComponent<CanvasGroup>().interactable = true;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void itemBagButtonClicked()
    {
        if (itemBag.GetComponent<CanvasGroup>().alpha == 1)
        {
            closePanel(itemBag);
        }
        else
        {
            CloseAllOpenedPanel();
            openPanel(itemBag);
        }
    }
    public void badgeBagButtonClicked()
    {
        if (badgeBag.GetComponent<CanvasGroup>().alpha == 1)
        {
            closePanel(badgeBag);
        }
        else
        {
            CloseAllOpenedPanel();
            openPanel(badgeBag);
        }
    }

    public void skinBagButtonClicked()
    {
        if (skinBag.GetComponent<CanvasGroup>().alpha == 1)
        {
            closePanel(skinBag);
        }
        else
        {
            CloseAllOpenedPanel();
            openPanel(skinBag);
        }
    }

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
}
