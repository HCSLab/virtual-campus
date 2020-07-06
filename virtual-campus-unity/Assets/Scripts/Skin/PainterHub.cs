using System.Collections.Generic;
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
    public GameObject uiManager;
    public GameObject saveAs;
    public InputField nameInput;
    public InputField descriptionInput;

    private GameObject currentPainter;
    private Texture2D currentTex;
    private Sprite currentSprite;


    public void OnEntireSkinButtonClicked()
    {
        uiManager.GetComponent<UIManager>().ActivatePanel(entirePainter);
        uiManager.GetComponent<UIManager>().ClosePanel(gameObject);
        entirePainter.GetComponent<PaintView>().Reload();
        currentPainter = entirePainter;
    }
    public void OnHeadButtonClicked()
    {
        uiManager.GetComponent<UIManager>().ActivatePanel(headPainter);
        uiManager.GetComponent<UIManager>().ClosePanel(gameObject);
        headPainter.GetComponent<PaintView>().Reload();
        currentPainter = headPainter;
    }

    public void OnUpperButtonClicked()
    {
        uiManager.GetComponent<UIManager>().ActivatePanel(upperPainter);
        uiManager.GetComponent<UIManager>().ClosePanel(gameObject);
        upperPainter.GetComponent<PaintView>().Reload();
        currentPainter = upperPainter;
    }
    public void OnLowerButtonClicked()
    {
        uiManager.GetComponent<UIManager>().ActivatePanel(lowerPainter);
        uiManager.GetComponent<UIManager>().ClosePanel(gameObject);
        lowerPainter.GetComponent<PaintView>().Reload();
        currentPainter = lowerPainter;
    }

    public void OnHatButtonClicked()
    {
        uiManager.GetComponent<UIManager>().ActivatePanel(hatPainter);
        uiManager.GetComponent<UIManager>().ClosePanel(gameObject);
        hatPainter.GetComponent<PaintView>().Reload();
        currentPainter = hatPainter;
    }

    public void OnArmButtonClicked()
    {
        uiManager.GetComponent<UIManager>().ActivatePanel(armPainter);
        uiManager.GetComponent<UIManager>().ClosePanel(gameObject);
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
        string description = descriptionInput.text;
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
        //skinBag.Reload();
        uiManager.GetComponent<UIManager>().DeactivatePanel(currentPainter);
        nameInput.text = "";
        descriptionInput.text = "";
        saveAs.SetActive(false);
    }
    public void OnCancelButtonClicked()
    {
        nameInput.text = "";
        descriptionInput.text = "";
        saveAs.SetActive(false);
    }
}
