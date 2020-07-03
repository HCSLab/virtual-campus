using System;
using System.Globalization;
using UnityEngine;
using Tobii.GameIntegration.Net;
using UnityEngine.UI;

public class ExtendedViewSettingsUIEventsManager : MonoBehaviour
{
    public enum ExtendedViewSettingsType
    {
        GazeOnly,
        HeadOnly,
        GazeAndHead
    }

    public ExtendedView ExtendedView;

    public ExtendedViewSettingsType SettingsType = ExtendedViewSettingsType.GazeAndHead;

	public SensitivityGradientSettingsUIEventManager GazeViewSensitivityGradientSettingsUiEventManager;

	public SensitivityGradientSettingsUIEventManager HeadViewSensitivityGradientSettingsUiEventManager;

	public HeadViewAutoCenterSettingsUIEventManager HeadViewAutoCenterSettingsUiEventManager;

    private Dropdown _headViewTypeDropdown;

    private InputField _aspectRatioCorrectionFactorInputField;
    private Slider _aspectRatioCorrectionFactorSlider;

    private InputField _headViewPitchCorrectionFactorInputField;
    private Slider _headViewPitchCorrectionFactorSlider;

    private InputField _gazeViewResponsivenessInputField;
    private Slider _gazeViewResponsivenessSlider;

    private InputField _normalizedGazeViewMinimumExtensionAngleInputField;
    private Slider _normalizedGazeViewMinimumExtensionAngleSlider;

    private InputField _normalizedGazeViewExtensionAngleInputField;
    private Slider _normalizedGazeViewExtensionAngleSlider;

    private InputField _headViewResponsivenessInputField;
    private Slider _headViewResponsivenessSlider;
    
    private bool _uiInitialized;

    void Awake()
    {
        _uiInitialized = false;
    }

    // Use this for initialization
    void Start () {

        if (!_uiInitialized)
        {
            CacheAllUiComponents();
            _uiInitialized = true;
        }

        FillUiWithCurrentSettings();
    }

    private void CacheAllUiComponents()
    {
        _headViewTypeDropdown = transform.Find("HeadViewTypeHorizontalPanel/HeadViewTypeDropdown").GetComponent<Dropdown>();

        _aspectRatioCorrectionFactorInputField = transform.Find("AspectRatioCorrectFactorHorizontalPanel/AspectRatioCorrectionFactorInputField").GetComponent<InputField>();
        _aspectRatioCorrectionFactorSlider = transform.Find("AspectRatioCorrectFactorHorizontalPanel/AspectRatioCorrectFactorSlider").GetComponent<Slider>();

        _headViewPitchCorrectionFactorInputField = transform.Find("HeadViewPitchCorrectionFactorHorizontalPanel/HeadViewPitchCorrectionFactorInputField").GetComponent<InputField>();
        _headViewPitchCorrectionFactorSlider = transform.Find("HeadViewPitchCorrectionFactorHorizontalPanel/HeadViewPitchCorrectionFactorSlider").GetComponent<Slider>();

        _gazeViewResponsivenessInputField = transform.Find("GazeViewResponsivenessHorizontalPanel/GazeViewResponsivenessInputField").GetComponent<InputField>();
        _gazeViewResponsivenessSlider = transform.Find("GazeViewResponsivenessHorizontalPanel/GazeViewResponsivenessSlider").GetComponent<Slider>();

        _normalizedGazeViewMinimumExtensionAngleInputField = transform.Find("NormalizedGazeViewMinimumExtensionAngleHorizontalPanel/NormalizedGazeViewMinimumExtensionAngleInputField").GetComponent<InputField>();
        _normalizedGazeViewMinimumExtensionAngleSlider = transform.Find("NormalizedGazeViewMinimumExtensionAngleHorizontalPanel/NormalizedGazeViewMinimumExtensionAngleSlider").GetComponent<Slider>();

        _normalizedGazeViewExtensionAngleInputField = transform.Find("NormalizedGazeViewExtensionAngleHorizontalPanel/NormalizedGazeViewExtensionAngleInputField").GetComponent<InputField>();
        _normalizedGazeViewExtensionAngleSlider = transform.Find("NormalizedGazeViewExtensionAngleHorizontalPanel/NormalizedGazeViewExtensionAngleSlider").GetComponent<Slider>();

        _headViewResponsivenessInputField = transform.Find("HeadViewResponsivenessHorizontalPanel/HeadViewResponsivenessInputField").GetComponent<InputField>();
        _headViewResponsivenessSlider = transform.Find("HeadViewResponsivenessHorizontalPanel/HeadViewResponsivenessSlider").GetComponent<Slider>();
    }

	public void FillUiWithCurrentSettings()
	{
	    if (!_uiInitialized) return;

        _headViewTypeDropdown.value = (int)GetExtendedViewSettings().HeadViewType;
        _aspectRatioCorrectionFactorSlider.value = GetExtendedViewSettings().AspectRatioCorrectionFactor;
        _headViewPitchCorrectionFactorSlider.value = GetExtendedViewSettings().HeadViewPitchCorrectionFactor;
        _gazeViewResponsivenessSlider.value = GetExtendedViewSettings().GazeViewResponsiveness;
        _normalizedGazeViewMinimumExtensionAngleSlider.value = GetExtendedViewSettings().NormalizedGazeViewMinimumExtensionAngle;
        _normalizedGazeViewExtensionAngleSlider.value = GetExtendedViewSettings().NormalizedGazeViewExtensionAngle;
        _headViewResponsivenessSlider.value = GetExtendedViewSettings().HeadViewResponsiveness;

		GazeViewSensitivityGradientSettingsUiEventManager.FillUiWithCurrentSettings();
		HeadViewSensitivityGradientSettingsUiEventManager.FillUiWithCurrentSettings();
		HeadViewAutoCenterSettingsUiEventManager.FillUiWithCurrentSettings();
    }

    public ExtendedViewSettingsForInspector GetExtendedViewSettings()
	{
		switch (SettingsType)
		{
		    case ExtendedViewSettingsType.GazeOnly:
		        return ExtendedView.GazeOnlySettings;
		    case ExtendedViewSettingsType.HeadOnly:
		        return ExtendedView.HeadOnlySettings;
		    case ExtendedViewSettingsType.GazeAndHead:
		        break;
		    default:
		        throw new ArgumentOutOfRangeException();
		}

	    return ExtendedView.HeadAndGazeSettings;
	}

    //****************************** HeadViewType ******************************
    public void HeadViewTypeDropDownValueChanged(int value)
    {
        GetExtendedViewSettings().HeadViewType = (HeadViewType) value;
        ExtendedView.UpdateAllExtendedViewSettings();
    }
    //****************************** HeadViewType ******************************


    //****************************** AspectRatioCorrectFactor ******************************
    public void AspectRatioCorrectFactorSliderValueChanged(float value)
    {
        _aspectRatioCorrectionFactorInputField.text = value.ToString(CultureInfo.CurrentCulture);
        GetExtendedViewSettings().AspectRatioCorrectionFactor = value;
        ExtendedView.UpdateAllExtendedViewSettings();
    }

    public void AspectRatioCorrectFactorInputFieldEndEdit(string text)
    {
        _aspectRatioCorrectionFactorSlider.value = Convert.ToSingle(text);
    }
    //****************************** AspectRatioCorrectFactor ******************************

    //****************************** HeadViewPitchCorrectionFactor ******************************
    public void HeadViewPitchCorrectionFactorSliderValueChanged(float value)
    {
        _headViewPitchCorrectionFactorInputField.text = value.ToString(CultureInfo.CurrentCulture);
        GetExtendedViewSettings().HeadViewPitchCorrectionFactor = value;
        ExtendedView.UpdateAllExtendedViewSettings();
    }

    public void HeadViewPitchCorrectionFactorInputFieldEndEdit(string text)
    {
        _headViewPitchCorrectionFactorSlider.value = Convert.ToSingle(text);
    }
    //****************************** HeadViewPitchCorrectionFactor ******************************


    //****************************** GazeViewResponsiveness ******************************
    public void GazeViewResponsivenessSliderValueChanged(float value)
    {
        _gazeViewResponsivenessInputField.text = value.ToString(CultureInfo.CurrentCulture);
        GetExtendedViewSettings().GazeViewResponsiveness = value;
        ExtendedView.UpdateAllExtendedViewSettings();
    }

    public void GazeViewResponsivenessInputFieldEndEdit(string text)
    {
        _gazeViewResponsivenessSlider.value = Convert.ToSingle(text);
    }
    //****************************** GazeViewResponsiveness ******************************

    //****************************** NormalizedGazeViewMinimumExtensionAngle ******************************
    public void NormalizedGazeViewMinimumExtensionAngleSliderValueChanged(float value)
    {
        _normalizedGazeViewMinimumExtensionAngleInputField.text = value.ToString(CultureInfo.CurrentCulture);
        GetExtendedViewSettings().NormalizedGazeViewMinimumExtensionAngle = value;
        ExtendedView.UpdateAllExtendedViewSettings();
    }

    public void NormalizedGazeViewMinimumExtensionAngleInputFieldEndEdit(string text)
    {
        _normalizedGazeViewMinimumExtensionAngleSlider.value = Convert.ToSingle(text);
    }
    //****************************** NormalizedGazeViewMinimumExtensionAngle ******************************


    //****************************** NormalizedGazeViewExtensionAngle ******************************
    public void NormalizedGazeViewExtensionAngleSliderValueChanged(float value)
    {
        _normalizedGazeViewExtensionAngleInputField.text = value.ToString(CultureInfo.CurrentCulture);
        GetExtendedViewSettings().NormalizedGazeViewExtensionAngle = value;
        ExtendedView.UpdateAllExtendedViewSettings();
    }

    public void NormalizedGazeViewExtensionAngleInputFieldEndEdit(string text)
    {
        _normalizedGazeViewExtensionAngleSlider.value = Convert.ToSingle(text);
    }
    //****************************** NormalizedGazeViewExtensionAngle ******************************

    //****************************** HeadViewResponsiveness ******************************
    public void HeadViewResponsivenessSliderValueChanged(float value)
    {
        _headViewResponsivenessInputField.text = value.ToString(CultureInfo.CurrentCulture);
        GetExtendedViewSettings().HeadViewResponsiveness = value;
        ExtendedView.UpdateAllExtendedViewSettings();
    }

    public void HeadViewResponsivenessInputFieldEndEdit(string text)
    {
        _headViewResponsivenessSlider.value = Convert.ToSingle(text);
    }
    //****************************** HeadViewResponsiveness ******************************

    //****************************** SensitivityGradientSettings ******************************
    public void UpdateGazeViewSensitivityGradientSettings(
        SensitivityGradientSettingsForInspector gazeViewSensitivityGradientSettings)
    {
        GetExtendedViewSettings().GazeViewSensitivityGradientSettings = gazeViewSensitivityGradientSettings;
        ExtendedView.UpdateAllExtendedViewSettings();
    }

    public void UpdateHeadViewSensitivityGradientSettings(
        SensitivityGradientSettingsForInspector headViewSensitivityGradientSettings)
    {
        GetExtendedViewSettings().HeadViewSensitivityGradientSettings = headViewSensitivityGradientSettings;
        ExtendedView.UpdateAllExtendedViewSettings();
    }
    //****************************** SensitivityGradientSettings ******************************

    //****************************** HeadViewAutoCenterSettings ******************************
    public void UpdateHeadAutoCenterSettings(
        HeadViewAutoCenterSettingsForInspector headViewAutoCenterSettings)
    {
        GetExtendedViewSettings().HeadViewAutoCenter = headViewAutoCenterSettings;
        ExtendedView.UpdateAllExtendedViewSettings();
    }
    //****************************** HeadViewAutoCenterSettings ******************************

}
