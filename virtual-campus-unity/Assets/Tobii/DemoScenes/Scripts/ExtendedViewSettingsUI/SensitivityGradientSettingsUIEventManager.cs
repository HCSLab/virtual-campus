using System;
using System.Globalization;
using Tobii.GameIntegration.Net;
using UnityEngine;
using UnityEngine.UI;

public class SensitivityGradientSettingsUIEventManager : MonoBehaviour {
    public enum SensitivityGradientSettingsType
    {
        GazeView,
        HeadView
    }

    public SensitivityGradientSettingsType SettingsType;

    private ExtendedViewSettingsUIEventsManager _extendedViewSettingsUiEventsManager;
    private SensitivityGradientSettingsForInspector _targetSensitivityGradientSettings;

    private InputField _scaleInputField;
    private Slider _scaleSlider;

    private InputField _exponentInputField;
    private Slider _exponentSlider;

    private InputField _inflectionPointInputField;
    private Slider _inflectionPointSlider;

    private InputField _startPointInputField;
    private Slider _startPointSlider;

    private InputField _endPointInputField;
    private Slider _endPointSlider;

    private bool _uiInitialized;

    void Awake()
    {
        _uiInitialized = false;
    }

    // Use this for initialization
    void Start ()
    {
        _extendedViewSettingsUiEventsManager = transform.parent.GetComponent<ExtendedViewSettingsUIEventsManager>();

        if (!_uiInitialized)
        {
            CacheAllUiComponents();
            _uiInitialized = true;
        }

        FillUiWithCurrentSettings();
    }

    private void UpdateExtendedViewSettingsWithSensitivityGradientSettings()
    {
        switch (SettingsType)
        {
            case SensitivityGradientSettingsType.GazeView:
                _extendedViewSettingsUiEventsManager.UpdateGazeViewSensitivityGradientSettings(_targetSensitivityGradientSettings);
                break;

            case SensitivityGradientSettingsType.HeadView:
                _extendedViewSettingsUiEventsManager.UpdateHeadViewSensitivityGradientSettings(_targetSensitivityGradientSettings);
                break;

                default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void FillUiWithCurrentSettings()
    {
        if(!_uiInitialized) return;
        
        _scaleSlider.value = GetTargetGradientSettings().Scale;
        _exponentSlider.value = GetTargetGradientSettings().Exponent;
        _inflectionPointSlider.value = GetTargetGradientSettings().InflectionPoint;
        _startPointSlider.value = GetTargetGradientSettings().StartPoint;
        _endPointSlider.value = GetTargetGradientSettings().EndPoint;
    }

    private SensitivityGradientSettingsForInspector GetTargetGradientSettings()
    {
        return SettingsType == SensitivityGradientSettingsType.GazeView ? _extendedViewSettingsUiEventsManager.GetExtendedViewSettings().GazeViewSensitivityGradientSettings : _extendedViewSettingsUiEventsManager.GetExtendedViewSettings().HeadViewSensitivityGradientSettings;
    }

    private void CacheAllUiComponents()
    {
        _scaleInputField = transform.Find("ScaleHorizontalPanel/ScaleInputField").GetComponent<InputField>();
        _scaleSlider = transform.Find("ScaleHorizontalPanel/ScaleSlider").GetComponent<Slider>();

        _exponentInputField = transform.Find("ExponentHorizontalPanel/ExponentInputField").GetComponent<InputField>();
        _exponentSlider = transform.Find("ExponentHorizontalPanel/ExponentSlider").GetComponent<Slider>();

        _inflectionPointInputField = transform.Find("InflectionPointHorizontalPanel/InflectionPointInputField").GetComponent<InputField>();
        _inflectionPointSlider = transform.Find("InflectionPointHorizontalPanel/InflectionPointSlider").GetComponent<Slider>();

        _startPointInputField = transform.Find("StartPointHorizontalPanel/StartPointInputField").GetComponent<InputField>();
        _startPointSlider = transform.Find("StartPointHorizontalPanel/StartPointSlider").GetComponent<Slider>();

        _endPointInputField = transform.Find("EndPointHorizontalPanel/EndPointInputField").GetComponent<InputField>();
        _endPointSlider = transform.Find("EndPointHorizontalPanel/EndPointSlider").GetComponent<Slider>();
    }

    //****************************** Scale ******************************
    public void ScaleSliderValueChanged(float value)
    {
        _scaleInputField.text = value.ToString(CultureInfo.CurrentCulture);
        _targetSensitivityGradientSettings = GetTargetGradientSettings();
        _targetSensitivityGradientSettings.Scale = value;
        UpdateExtendedViewSettingsWithSensitivityGradientSettings();
    }

    public void ScaleInputFieldEndEdit(string text)
    {
        _scaleSlider.value = Convert.ToSingle(text);
        UpdateExtendedViewSettingsWithSensitivityGradientSettings();
    }
    //****************************** Scale ******************************

    //****************************** Exponent ******************************
    public void ExponentSliderValueChanged(float value)
    {
        _exponentInputField.text = value.ToString(CultureInfo.CurrentCulture);
        _targetSensitivityGradientSettings = GetTargetGradientSettings();
        _targetSensitivityGradientSettings.Exponent = value;
        UpdateExtendedViewSettingsWithSensitivityGradientSettings();
    }

    public void ExponentInputFieldEndEdit(string text)
    {
        _exponentSlider.value = Convert.ToSingle(text);
        UpdateExtendedViewSettingsWithSensitivityGradientSettings();
    }
    //****************************** Exponent ******************************

    //****************************** InflectionPoint ******************************
    public void InflectionPointSliderValueChanged(float value)
    {
        _inflectionPointInputField.text = value.ToString(CultureInfo.CurrentCulture);
        _targetSensitivityGradientSettings = GetTargetGradientSettings();
        _targetSensitivityGradientSettings.InflectionPoint = value;
        UpdateExtendedViewSettingsWithSensitivityGradientSettings();
    }

    public void InflectionPointInputFieldEndEdit(string text)
    {
        _inflectionPointSlider.value = Convert.ToSingle(text);
        UpdateExtendedViewSettingsWithSensitivityGradientSettings();
    }
    //****************************** InflectionPoint ******************************

    //****************************** StartPoint ******************************
    public void StartPointSliderValueChanged(float value)
    {
        _startPointInputField.text = value.ToString(CultureInfo.CurrentCulture);
        _targetSensitivityGradientSettings = GetTargetGradientSettings();
        _targetSensitivityGradientSettings.StartPoint = value;
        UpdateExtendedViewSettingsWithSensitivityGradientSettings();
    }

    public void StartPointInputFieldEndEdit(string text)
    {
        _startPointSlider.value = Convert.ToSingle(text);
        UpdateExtendedViewSettingsWithSensitivityGradientSettings();
    }
    //****************************** StartPoint ******************************

    //****************************** EndPoint ******************************
    public void EndPointSliderValueChanged(float value)
    {
        _endPointInputField.text = value.ToString(CultureInfo.CurrentCulture);
        _targetSensitivityGradientSettings = GetTargetGradientSettings();
        _targetSensitivityGradientSettings.EndPoint = value;
        UpdateExtendedViewSettingsWithSensitivityGradientSettings();
    }

    public void EndPointInputFieldEndEdit(string text)
    {
        _endPointSlider.value = Convert.ToSingle(text);
        UpdateExtendedViewSettingsWithSensitivityGradientSettings();
    }
    //****************************** EndPoint ******************************
}
