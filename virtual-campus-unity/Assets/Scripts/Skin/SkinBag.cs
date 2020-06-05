using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class SkinBag : Bag
{
    public int index;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

        for (int i = 0; i < 10; i++)
        {
            foreach (var item in testItems)
            {
                Add(item);
            }
        }
    }

    public void OnSaveClicked()
    {
        if (currentItem != null)
        {
            SpriteItem spriteItem = new SpriteItem(currentItem);
            GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Mapping>().ChangeMapping(spriteItem);
        }
        BagButtonPressed();
    }

    public void OnCancelClicked()
    {
        BagButtonPressed();
    }
}