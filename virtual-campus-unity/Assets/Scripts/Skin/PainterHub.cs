using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PainterHub : MonoBehaviour
{
    public GameObject skinBagObject;
    public GameObject entirePainter;
    public GameObject headPainter;
    public GameObject upperPainter;
    public GameObject lowerPainter;
    public GameObject hatPainter;
    public GameObject armPainter;
    public GameObject saveAs;
    public GameObject quitButton;
    public TMP_InputField nameInput;

    private GameObject currentPainter;
    private Texture2D currentTex;
    private Sprite currentSprite;


    public void OnEntireSkinButtonClicked()
    {
        entirePainter.SetActive(true);
        gameObject.SetActive(false);
        entirePainter.GetComponent<PaintView>().Reload();
        currentPainter = entirePainter;
    }
    public void OnHeadButtonClicked()
    {
        headPainter.SetActive(true);
        gameObject.SetActive(false);
        headPainter.GetComponent<PaintView>().Reload();
        currentPainter = headPainter;
    }

    public void OnUpperButtonClicked()
    {
        upperPainter.SetActive(true);
        gameObject.SetActive(false);
        upperPainter.GetComponent<PaintView>().Reload();
        currentPainter = upperPainter;
    }
    public void OnLowerButtonClicked()
    {
        lowerPainter.SetActive(true);
        gameObject.SetActive(false);
        lowerPainter.GetComponent<PaintView>().Reload();
        currentPainter = lowerPainter;
    }

    public void OnHatButtonClicked()
    {
        hatPainter.SetActive(true);
        gameObject.SetActive(false);
        hatPainter.GetComponent<PaintView>().Reload();
        currentPainter = hatPainter;
    }

    public void OnArmButtonClicked()
    {
        armPainter.SetActive(true);
        gameObject.SetActive(false);
        armPainter.GetComponent<PaintView>().Reload();
        currentPainter = armPainter;
    }

    public void SaveAs(Texture2D tex, Sprite sprite)
    {
        saveAs.SetActive(true);
        currentTex = tex;
        currentSprite = sprite;
    }

    public void OnSaveButtonClicked()
    {
        string name = nameInput.text;
        string description = string.Empty;
        var skinBag = skinBagObject.GetComponent<SkinBag>();
        GameObject newSkin = new GameObject();
        newSkin.name = "Customized Sprite";
        newSkin.transform.SetParent(skinBag.transform);
        var skinItem = newSkin.AddComponent<SkinItem>();
        skinItem.itemName = name;
        skinItem.description = description;
        skinItem.image = currentSprite;
        skinItem.texture = currentTex;
        skinItem.customized = true;
        skinBag.Add(newSkin);

        currentPainter.SetActive(false);
        nameInput.text = string.Empty;
        saveAs.SetActive(false);
    }
    public void OnCancelButtonClicked()
    {
        nameInput.text = string.Empty;
        saveAs.SetActive(false);
    }

    public void OnQuitButtonClicked()
    {
        gameObject.SetActive(false);
    }
}
