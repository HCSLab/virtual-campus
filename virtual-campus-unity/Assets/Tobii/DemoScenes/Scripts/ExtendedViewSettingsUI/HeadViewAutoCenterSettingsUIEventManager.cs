using System;
using System.Globalization;
using Tobii.GameIntegration.Net;
using UnityEngine;
using UnityEngine.UI;

public class HeadViewAutoCenterSettingsUIEventManager : MonoBehaviour {

    private HeadViewAutoCenterSettingsForInspector _targetHeadViewAutoCenterSettings;
    private ExtendedViewSettingsUIEventsManager _extendedViewSettingsUiEventsManager;

    private Toggle _isEnabledToggle;

    private InputField _normalizeFasterGazeDeadZoneNormalizedInputField;
    private Slider _normalizeFasterGazeDeadZoneNormalizedSlider;

    private InputField _extendedViewAngleFasterDeadZoneDegreesInputField;
    private Slider _extendedViewAngleFasterDeadZoneDegreesSlider;

    private InputField _maxDistanceFromMasterCmInputField;
    private Slider _maxDistanceFromMasterCmSlider;

    private InputField _maxAngularDistanceDegreesInputField;
    private Slider _maxAngularDistanceDegreesSlider;

    private InputField _fasterNormalizationFactorInputField;
    private Slider _fasterNormalizationFactorSlider;

    private InputField _positionCompensationSpeedInputField;
    private Slider _positionCompensationSpeedSlider;

    private InputField _rotationCompensationSpeedInputField;
    private Slider _rotationCompensationSpeedSlider;

    private bool _uiInitialized;

    void Awake()
    {
        _uiInitialized = false;
    }

    // Use this for initialization
    void Start()
    {
        _extendedViewSettingsUiEventsManager = transform.parent.GetComponent<ExtendedViewSettingsUIEventsManager>();

        if (!_uiInitialized)
        {
            CacheAllUiComponents();
            _uiInitialized = true;
        }

        FillUiWithCurrentSettings();
    }

    private void UpdateExtendedViewSettingsWithHeadAutoCenterSettings()
    {
        _extendedViewSettingsUiEventsManager.UpdateHeadAutoCenterSettings(_targetHeadViewAutoCenterSettings);
    }

    public void FillUiWithCurrentSettings()
    {
        if (!_uiInitialized) return;

        _isEnabledToggle.isOn = GetTargetAutoCenterSettings().IsEnabled;

        _normalizeFasterGazeDeadZoneNormalizedSlider.value = GetTargetAutoCenterSettings().NormalizeFasterGazeDeadZoneNormalized;
        _extendedViewAngleFasterDeadZoneDegreesSlider.value = GetTargetAutoCenterSettings().ExtendedViewAngleFasterDeadZoneDegrees;
        _maxDistanceFromMasterCmSlider.value = GetTargetAutoCenterSettings().MaxDistanceFromMasterCm;
        _maxAngularDistanceDegreesSlider.value = GetTargetAutoCenterSettings().MaxAngularDistanceDegrees;
        _fasterNormalizationFactorSlider.value = GetTargetAutoCenterSettings().FasterNormalizationFactor;
        _positionCompensationSpeedSlider.value = GetTargetAutoCenterSettings().PositionCompensationSpeed;
        _rotationCompensationSpeedSlider.value = GetTargetAutoCenterSettings().RotationCompensationSpeed;

    }

    private void CacheAllUiComponents()
    {
        _isEnabledToggle = transform.Find("IsEnabledToggle").GetComponent<Toggle>();

        _normalizeFasterGazeDeadZoneNormalizedInputField = transform.Find("NormalizeFasterGazeDeadZoneNormalizedHorizontalPanel/NormalizeFasterGazeDeadZoneNormalizedInputField").GetComponent<InputField>();
        _normalizeFasterGazeDeadZoneNormalizedSlider = transform.Find("NormalizeFasterGazeDeadZoneNormalizedHorizontalPanel/NormalizeFasterGazeDeadZoneNormalizedSlider").GetComponent<Slider>();

        _extendedViewAngleFasterDeadZoneDegreesInputField = transform.Find("ExtendedViewAngleFasterDeadZoneDegreesHorizontalPanel/ExtendedViewAngleFasterDeadZoneDegreesInputField").GetComponent<InputField>();
        _extendedViewAngleFasterDeadZoneDegreesSlider = transform.Find("ExtendedViewAngleFasterDeadZoneDegreesHorizontalPanel/ExtendedViewAngleFasterDeadZoneDegreesSlider").GetComponent<Slider>();

        _maxDistanceFromMasterCmInputField = transform.Find("MaxDistanceFromMasterCmHorizontalPanel/MaxDistanceFromMasterCmInputField").GetComponent<InputField>();
        _maxDistanceFromMasterCmSlider = transform.Find("MaxDistanceFromMasterCmHorizontalPanel/MaxDistanceFromMasterCmSlider").GetComponent<Slider>();

        _maxAngularDistanceDegreesInputField = transform.Find("MaxAngularDistanceDegreesHorizontalPanel/MaxAngularDistanceDegreesInputField").GetComponent<InputField>();
        _maxAngularDistanceDegreesSlider = transform.Find("MaxAngularDistanceDegreesHorizontalPanel/MaxAngularDistanceDegreesSlider").GetComponent<Slider>();

        _fasterNormalizationFactorInputField = transform.Find("FasterNormalizationFactorHorizontalPanel/FasterNormalizationFactorInputField").GetComponent<InputField>();
        _fasterNormalizationFactorSlider = transform.Find("FasterNormalizationFactorHorizontalPanel/FasterNormalizationFactorSlider").GetComponent<Slider>();

        _positionCompensationSpeedInputField = transform.Find("PositionCompensationSpeedHorizontalPanel/PositionCompensationSpeedInputField").GetComponent<InputField>();
        _positionCompensationSpeedSlider = transform.Find("PositionCompensationSpeedHorizontalPanel/PositionCompensationSpeedSlider").GetComponent<Slider>();

        _rotationCompensationSpeedInputField = transform.Find("RotationCompensationSpeedHorizontalPanel/RotationCompensationSpeedInputField").GetComponent<InputField>();
        _rotationCompensationSpeedSlider = transform.Find("RotationCompensationSpeedHorizontalPanel/RotationCompensationSpeedSlider").GetComponent<Slider>();
    }

    private HeadViewAutoCenterSettingsForInspector GetTargetAutoCenterSettings()
    {
        return _extendedViewSettingsUiEventsManager.GetExtendedViewSettings().HeadViewAutoCenter;
    }

    //****************************** IsEnabled ******************************
    public void IsEnabledToggleValueChanged(bool value)
    {
        _targetHeadViewAutoCenterSettings = GetTargetAutoCenterSettings();
        _targetHeadViewAutoCenterSettings.IsEnabled = value;
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }
    //****************************** IsEnabled ******************************

    //****************************** NormalizeFasterGazeDeadZoneNormalized ******************************
    public void NormalizeFasterGazeDeadZoneNormalizedSliderValueChanged(float value)
    {
        _normalizeFasterGazeDeadZoneNormalizedInputField.text = value.ToString(CultureInfo.CurrentCulture);
        _targetHeadViewAutoCenterSettings = GetTargetAutoCenterSettings();
        _targetHeadViewAutoCenterSettings.NormalizeFasterGazeDeadZoneNormalized = value;
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }

    public void NormalizeFasterGazeDeadZoneNormalizedInputFieldEndEdit(string text)
    {
        _normalizeFasterGazeDeadZoneNormalizedSlider.value = Convert.ToSingle(text);
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }
    //****************************** NormalizeFasterGazeDeadZoneNormalized ******************************

    //****************************** ExtendedViewAngleFasterDeadZoneDegrees ******************************
    public void ExtendedViewAngleFasterDeadZoneDegreesSliderValueChanged(float value)
    {
        _extendedViewAngleFasterDeadZoneDegreesInputField.text = value.ToString(CultureInfo.CurrentCulture);
        _targetHeadViewAutoCenterSettings = GetTargetAutoCenterSettings();
        _targetHeadViewAutoCenterSettings.ExtendedViewAngleFasterDeadZoneDegrees = value;
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }

    public void ExtendedViewAngleFasterDeadZoneDegreesInputFieldEndEdit(string text)
    {
        _extendedViewAngleFasterDeadZoneDegreesSlider.value = Convert.ToSingle(text);
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }
    //****************************** ExtendedViewAngleFasterDeadZoneDegrees ******************************

    //****************************** MaxDistanceFromMasterCm ******************************
    public void MaxDistanceFromMasterCmSliderValueChanged(float value)
    {
        _maxDistanceFromMasterCmInputField.text = value.ToString(CultureInfo.CurrentCulture);
        _targetHeadViewAutoCenterSettings = GetTargetAutoCenterSettings();
        _targetHeadViewAutoCenterSettings.MaxDistanceFromMasterCm = value;
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }

    public void MaxDistanceFromMasterCmInputFieldEndEdit(string text)
    {
        _maxDistanceFromMasterCmSlider.value = Convert.ToSingle(text);
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }
    //****************************** MaxDistanceFromMasterCm ******************************

    //****************************** MaxAngularDistanceDegrees ******************************
    public void MaxAngularDistanceDegreesSliderValueChanged(float value)
    {
        _maxAngularDistanceDegreesInputField.text = value.ToString(CultureInfo.CurrentCulture);
        _targetHeadViewAutoCenterSettings = GetTargetAutoCenterSettings();
        _targetHeadViewAutoCenterSettings.MaxAngularDistanceDegrees = value;
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }

    public void MaxAngularDistanceDegreesInputFieldEndEdit(string text)
    {
        _maxAngularDistanceDegreesSlider.value = Convert.ToSingle(text);
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }
    //****************************** MaxAngularDistanceDegrees ******************************

    //****************************** FasterNormalizationFactor ******************************
    public void FasterNormalizationFactorSliderValueChanged(float value)
    {
        _fasterNormalizationFactorInputField.text = value.ToString(CultureInfo.CurrentCulture);
        _targetHeadViewAutoCenterSettings = GetTargetAutoCenterSettings();
        _targetHeadViewAutoCenterSettings.FasterNormalizationFactor = value;
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }

    public void FasterNormalizationFactorInputFieldEndEdit(string text)
    {
        _fasterNormalizationFactorSlider.value = Convert.ToSingle(text);
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }
    //****************************** FasterNormalizationFactor ******************************

    //****************************** PositionCompensationSpeed ******************************
    public void PositionCompensationSpeedSliderValueChanged(float value)
    {
        _positionCompensationSpeedInputField.text = value.ToString(CultureInfo.CurrentCulture);
        _targetHeadViewAutoCenterSettings = GetTargetAutoCenterSettings();
        _targetHeadViewAutoCenterSettings.PositionCompensationSpeed = value;
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }

    public void PositionCompensationSpeedInputFieldEndEdit(string text)
    {
        _positionCompensationSpeedSlider.value = Convert.ToSingle(text);
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }
    //****************************** PositionCompensationSpeed ******************************

    //****************************** RotationCompensationSpeed ******************************
    public void RotationCompensationSpeedSliderValueChanged(float value)
    {
        _rotationCompensationSpeedInputField.text = value.ToString(CultureInfo.CurrentCulture);
        _targetHeadViewAutoCenterSettings = GetTargetAutoCenterSettings();
        _targetHeadViewAutoCenterSettings.RotationCompensationSpeed = value;
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }

    public void RotationCompensationSpeedInputFieldEndEdit(string text)
    {
        _rotationCompensationSpeedSlider.value = Convert.ToSingle(text);
        UpdateExtendedViewSettingsWithHeadAutoCenterSettings();
    }
    //****************************** RotationCompensationSpeed ******************************
}
