using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ModifiedButton : MonoBehaviour, IPointerClickHandler
{

    public UnityEvent leftClick;
    public UnityEvent middleClick;
    public UnityEvent rightClick;


    private void Start()
    {
        leftClick.AddListener(new UnityAction(ButtonLeftClick));
        middleClick.AddListener(new UnityAction(ButtonMiddleClick));
        rightClick.AddListener(new UnityAction(ButtonRightClick));
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            leftClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Middle)
            middleClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            rightClick.Invoke();
    }


    private void ButtonLeftClick()
    {
        //Debug.Log("Button Left Click");
    }

    private void ButtonMiddleClick()
    {
        //Debug.Log("Button Middle Click");
    }

    private void ButtonRightClick()
    {
        //Debug.Log("Button Right Click");
    }
}