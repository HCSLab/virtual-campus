using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelAnimator : MonoBehaviour {

    public float MoveSpeed = 1;

    private float _currentInterpolationValue = 0;

    private bool _doHide = true;

    private RectTransform _panelTransform;

    private readonly float _maxHorizontalShift = Screen.width / 2.0f;

    private readonly float _halfScreenWidth = Screen.width / 2.0f;

    // Use this for initialization
    void Start ()
	{
	    _panelTransform = GetComponent<RectTransform>();
	    var sizeDelta = _panelTransform.sizeDelta;

	    sizeDelta.x = _halfScreenWidth - 40;
	    sizeDelta.y = Screen.height - 80;
	    _panelTransform.sizeDelta = sizeDelta;
	}
	
	// Update is called once per frame
    void Update()
    {
        LerpPanelPosition();
    }

    private void LerpPanelPosition()
    {
        var panelPosition = _panelTransform.anchoredPosition;
        var direction = _doHide ? 1 : -1;
        _currentInterpolationValue += (direction * Time.unscaledDeltaTime * MoveSpeed);
        _currentInterpolationValue = Mathf.Clamp01(_currentInterpolationValue);

        panelPosition.x = Mathf.Lerp(0, _maxHorizontalShift, _currentInterpolationValue);

        _panelTransform.anchoredPosition = panelPosition;
    }

    public void StartShowOrHide()
    {
        _doHide = !_doHide;
    }
}
