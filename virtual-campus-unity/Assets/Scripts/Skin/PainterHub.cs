using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainterHub : MonoBehaviour
{
    public GameObject skinBag;
    public GameObject entirePainter;
    public GameObject headPainter;
    public GameObject upperPainter;
    public GameObject lowerPainter;
    public GameObject hatPainter;
    public GameObject armPainter;
    public GameObject uiManager;
    

    public void OnEntireSkinButtonClicked()
    {
        uiManager.GetComponent<UIManager>().openPanel(entirePainter);
        uiManager.GetComponent<UIManager>().closePanel(gameObject);
        entirePainter.GetComponent<PaintView>().Reload();
    }
    public void OnHeadButtonClicked()
    {
        uiManager.GetComponent<UIManager>().openPanel(headPainter);
        uiManager.GetComponent<UIManager>().closePanel(gameObject);
        headPainter.GetComponent<PaintView>().Reload();
    }

    public void OnUpperButtonClicked()
    {
        uiManager.GetComponent<UIManager>().openPanel(upperPainter);
        uiManager.GetComponent<UIManager>().closePanel(gameObject);
        upperPainter.GetComponent<PaintView>().Reload();
    }
    public void OnLowerButtonClicked()
    {
        uiManager.GetComponent<UIManager>().openPanel(lowerPainter);
        uiManager.GetComponent<UIManager>().closePanel(gameObject);
        lowerPainter.GetComponent<PaintView>().Reload();
    }

    public void OnHatButtonClicked()
    {
        uiManager.GetComponent<UIManager>().openPanel(hatPainter);
        uiManager.GetComponent<UIManager>().closePanel(gameObject);
        hatPainter.GetComponent<PaintView>().Reload();
    }

    public void OnArmButtonClicked()
    {
        uiManager.GetComponent<UIManager>().openPanel(armPainter);
        uiManager.GetComponent<UIManager>().closePanel(gameObject);
        armPainter.GetComponent<PaintView>().Reload();
    }
}
