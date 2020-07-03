using UnityEngine;
using UnityEngine.UI;

public class ExtendedViewSettingsUICollapseExpandManager : MonoBehaviour
{

    public float CollapsExpandSpeed = 1;

    private bool _isCollapsed = true;
    private GameObject _otherSettingsPanel;
    private Text _collapseExpandButtonText;
    private const float CollapsedHeight = 60;
    private const float ExpandedHeight = 2100;
    private float _currentInterpolationValue = 0;

    private RectTransform _panelTransform;

    // Use this for initialization
    void Start ()
	{
	    _otherSettingsPanel = transform.Find("OtherSettingsPanel").gameObject;
	    _collapseExpandButtonText = transform.Find("TitlePanel/CollapseExpandButton/Text").gameObject.GetComponent<Text>();
        _panelTransform = GetComponent<RectTransform>();
	    
        ShowHideSettingsPanel();
	}
	
	// Update is called once per frame
	void Update () {
	    var sizeDelta = _panelTransform.sizeDelta;
	    var direction = _isCollapsed ? -1 : 1;
	    _currentInterpolationValue += (direction * Time.unscaledDeltaTime * CollapsExpandSpeed);
	    _currentInterpolationValue = Mathf.Clamp01(_currentInterpolationValue);

	    sizeDelta.y = Mathf.Lerp(CollapsedHeight, ExpandedHeight, _currentInterpolationValue);
	    _panelTransform.sizeDelta = sizeDelta;
    }

    private void ShowHideSettingsPanel()
    {
        _otherSettingsPanel.SetActive(!_isCollapsed);
        _collapseExpandButtonText.text = _isCollapsed ? "+" : "-";

        UpdateParentHeight();
    }

    private void UpdateParentHeight()
    {
        var parentPanelTransform = transform.parent.GetComponent<RectTransform>();
        var sizeDelta = parentPanelTransform.sizeDelta;
        var sign = _isCollapsed ? -1 : 1;
        sizeDelta.y += sign * ExpandedHeight;
        parentPanelTransform.sizeDelta = sizeDelta;
    }

    public void CollapseExpandButtonClick()
    {
        _isCollapsed = !_isCollapsed;
     
        ShowHideSettingsPanel();
    }
}
