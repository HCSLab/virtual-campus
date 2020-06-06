using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [HideInInspector] public GameObject currentTalk;
    public GameObject pressT;
    public GameObject itemBag;
    public GameObject badgeBag;

    private void Awake()
    {
        Instance = this;
    }

    public void CloseAllOpenedPanel()
    {
        CloseTalk();
        HideItemBag();
        HideBadgeBag();
        HidePressT();
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

    public void ShowItemBag()
    {
        badgeBag.SetActive(false);
        itemBag.SetActive(true);
    }
    public void HideItemBag()
    {
        itemBag.SetActive(false);
    }
    public void ItemBagButtonPressed()
    {
        if (itemBag.activeSelf)
        {
            HideItemBag();
        }
        else
        {
            ShowItemBag();
        }
    }

    public void ShowBadgeBag()
    {
        itemBag.SetActive(false);
        badgeBag.SetActive(true);
    }
    public void HideBadgeBag()
    {
        badgeBag.SetActive(false);
    }
    public void BadgeBagButtonPressed()
    {
        if (badgeBag.activeSelf)
        {
            HideBadgeBag();
        }
        else
        {
            ShowBadgeBag();
        }
    }
}
