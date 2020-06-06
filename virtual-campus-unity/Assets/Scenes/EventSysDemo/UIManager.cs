using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [HideInInspector] public GameObject currentTalk;

    public GameObject pressE;
    public GameObject itemBag;
    public GameObject badgeBag;

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

    public void ShowPressE()
    {
        pressE.SetActive(true);
    }
    public void HidePressE()
    {
        pressE.SetActive(false);
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
