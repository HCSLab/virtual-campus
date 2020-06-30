﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinBag : Bag
{
    public static SkinBag Instance;
    public GameObject skinPreviewPlayer;

    private bool rotateLeft = false;
    private bool rotateRight = false;

    public Space m_RotateSpace;
    public float m_RotateSpeed = 20f;

    public GameObject painterHub;
    public GameObject uiManager;
    protected virtual void Awake()
    {
        Instance = this;
        foreach (GameObject obj in testItems){
            Texture2D tex = obj.GetComponent<SkinItem>().texture;
            Sprite sprite = Sprite.Create(TextureToTexture2D(tex), new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1000);
            obj.GetComponent<SkinItem>().image = sprite;
        }
        //painterHub = transform.Find("PainterHub").gameObject;
        //Debug.Log(painterHub);
    }

    protected override void Start()
    {
        for (int i = 0; i < 1; i++)
        {
            foreach (var item in testItems)
            {
                Add(item);
            }
        }
    }

    public Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }
    public void OnSaveClicked()
    {
        if (currentItem != null)
        {
            SkinItem spriteItem = (SkinItem) currentItem;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkin>().playerTexture = spriteItem.texture;
            //Debug.Log(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkin>().playerMaterial);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkin>().playerMaterial.mainTexture = spriteItem.texture;
        }
        BagButtonPressed();
    }


    public override void Add(GameObject obj, bool copy = true)
    {
        GetComponent<CanvasScaler>().enabled = false;
        var display = Instantiate(displayPrefab);
        var itemBox = display.GetComponent<SkinBox>();

        Item item;
        if (copy)
        {
            item = Instantiate(obj).GetComponent<Item>();
        }
        else
        {
            item = obj.GetComponent<Item>();
        }
        item.transform.parent = transform;

        itemBox.Init(item);

        itemBoxs.Add(itemBox);
        display.transform.SetParent(layout);
        GetComponent<CanvasScaler>().enabled = true;
    }

    public void OnCancelClicked()
    {
        BagButtonPressed();
    }

    public override void Select(Item item) 
    {
        detailName.text = item.itemName;
        detailDescription.text = item.description;
        currentItem = item;
        SkinItem spriteItem = (SkinItem)currentItem;
        skinPreviewPlayer.GetComponent<PlayerSkin>().playerTexture = spriteItem.texture;
        skinPreviewPlayer.GetComponent<PlayerSkin>().playerMaterial.mainTexture = spriteItem.texture;
    }

    public void RotateLeftDown()
    {
        rotateLeft = true;
    }

    public void RotateLeftUp()
    {
        rotateLeft = false;
    }

    public void RotateRightDown()
    {
        rotateRight = true;
    }

    public void RotateRightUp()
    {
        rotateRight = false;
    }
    public void OnPainterHubButtonClicked()
    {
        if (painterHub.GetComponent<CanvasGroup>().alpha == 1)
        {
            uiManager.GetComponent<UIManager>().ClosePanel(painterHub);
        }
        else
        {
            uiManager.GetComponent<UIManager>().OpenPanel(painterHub);
            uiManager.GetComponent<UIManager>().ClosePanel(gameObject);
        }
    }

    private void Update()
    {
        if (rotateLeft)
        {
            skinPreviewPlayer.transform.Rotate(Vector3.up * m_RotateSpeed * Time.deltaTime, m_RotateSpace);
        }
        else if (rotateRight)
        {
            skinPreviewPlayer.transform.Rotate(Vector3.down * m_RotateSpeed * Time.deltaTime, m_RotateSpace);
        }
    }

    public void ReloadSprites()
    {
        foreach (ItemBox itemBox in itemBoxs)
        {
            itemBox.image.sprite = itemBox.item.image;
        }
    }

    public override void Reload()
    {
        GetComponent<CanvasScaler>().enabled = false;
        foreach (var box in itemBoxs)
        {
            Destroy(box.gameObject);
        }
        itemBoxs.Clear();
        for (int i = 0; i < 1; i++)
        {
            foreach (var item in testItems)
            {
                Add(item);
            }
        }
        GetComponent<CanvasScaler>().enabled = true;
    }
}