using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsPanel : SavableMonoBehavior
{
	static public SettingsPanel Instance;

	public Toggle fullscreenToggle;
	public Toggle antiAliasingToggle;
	public TMP_Dropdown resolutionDropdownMenu;
	public Slider masterVolumeSlider;

	[Header("Anti-aliasing")]
	public UniversalAdditionalCameraData urpCameraData;
	public UniversalRenderPipelineAsset urpAsset;

	[Header("Auto Reset")]
	public int timeBeforeAutoReset;
	public GameObject autoResetPanel;
	public TextMeshProUGUI refuseButtonText;

	Resolution[] resolutions;
	bool originalFullscreenMode;
	int originalResolutionIndex, currentResolutionIndex;

	enum CoroutineState
	{
		NotStartedYet,
		Running,
		Killed
	};

	CoroutineState attemptAutoResetCoroutineState;

	private void Awake()
	{
		Instance = this;
	}

	protected override void Start()
	{
		base.Start();

		attemptAutoResetCoroutineState = CoroutineState.NotStartedYet;

		//urpAsset = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;

		resolutions = Screen.resolutions;
		resolutionDropdownMenu.ClearOptions();
		List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
		for (int i = resolutions.Length - 1; i >= 0; i--)
			options.Add(new TMP_Dropdown.OptionData(resolutions[i].width.ToString() + " x " + resolutions[i].height.ToString()));
		resolutionDropdownMenu.AddOptions(options);

		for (int i = 0; i < resolutions.Length; i++)
		{
			if (Screen.currentResolution.width == resolutions[i].width
					&& Screen.currentResolution.height == resolutions[i].height)
			{
				currentResolutionIndex = originalResolutionIndex = i;
				break;
			}
		}

		fullscreenToggle.isOn = Screen.fullScreen;
		antiAliasingToggle.isOn = PlayerPrefs.GetInt(SaveSystem.GetAntiAliasingModeName(), 1) == 1;
		resolutionDropdownMenu.value = resolutions.Length - currentResolutionIndex - 1;
		masterVolumeSlider.value = PlayerPrefs.GetFloat(SaveSystem.GetMasterVolumeName(), 1f);

		OnAntiAliasingChanged(antiAliasingToggle.isOn);
		OnMasterVolumeChanged(masterVolumeSlider.value);

		fullscreenToggle.onValueChanged.AddListener(OnFullscreenModeChanged);
		antiAliasingToggle.onValueChanged.AddListener(OnAntiAliasingChanged);
		resolutionDropdownMenu.onValueChanged.AddListener(OnResolutionChanged);
		masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

		autoResetPanel.SetActive(false);
	}

	protected override void Save(object data)
	{
		base.Save(data);

		PlayerPrefs.SetInt(SaveSystem.GetAntiAliasingModeName(), antiAliasingToggle.isOn ? 1 : 0);
		PlayerPrefs.SetFloat(SaveSystem.GetMasterVolumeName(), masterVolumeSlider.value);
	}

	/// <param name="newMode">true -> Fullscreen; false -> windowed</param>
	public void OnFullscreenModeChanged(bool newMode)
	{
		if (attemptAutoResetCoroutineState == CoroutineState.Running)
			return;

		print("Fullscreen: " + newMode);

		originalFullscreenMode = !newMode;
		Screen.fullScreen = newMode;

		StartCoroutine(AttemptAutoReset());
	}

	public void OnAntiAliasingChanged(bool newMode)
	{
		if (newMode)
		{
			urpCameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
			urpCameraData.antialiasingQuality = AntialiasingQuality.High;
			urpAsset.msaaSampleCount = 8;
		}
		else
		{
			urpCameraData.antialiasing = AntialiasingMode.None;
			urpAsset.msaaSampleCount = 1;
		}
	}

	public void OnResolutionChanged(int newResolutionIndex)
	{
		if (attemptAutoResetCoroutineState == CoroutineState.Running)
			return;

		currentResolutionIndex = newResolutionIndex = resolutions.Length - newResolutionIndex - 1;

		print("Resolution: " + resolutions[newResolutionIndex].width + " x " + resolutions[newResolutionIndex].height);


		Screen.SetResolution(resolutions[newResolutionIndex].width, resolutions[newResolutionIndex].height, Screen.fullScreen);

		StartCoroutine(AttemptAutoReset());
	}

	public void OnMasterVolumeChanged(float newVolume)
	{
		AudioListener.volume = newVolume;
	}

	public void RefuseAutoReset()
	{
		originalFullscreenMode = Screen.fullScreen;
		originalResolutionIndex = currentResolutionIndex;

		autoResetPanel.SetActive(false);
		attemptAutoResetCoroutineState = CoroutineState.Killed;
	}

	public void AcceptAutoReset()
	{
		fullscreenToggle.isOn = originalFullscreenMode;
		resolutionDropdownMenu.value = resolutions.Length - originalResolutionIndex - 1;

		Screen.SetResolution(resolutions[originalResolutionIndex].width, resolutions[originalResolutionIndex].height, originalFullscreenMode);

		autoResetPanel.SetActive(false);
		attemptAutoResetCoroutineState = CoroutineState.Killed;
	}

	IEnumerator AttemptAutoReset()
	{
		attemptAutoResetCoroutineState = CoroutineState.Running;

		autoResetPanel.SetActive(true);

		string left = "否 (", right = ")";
		int time = timeBeforeAutoReset;
		float totalTime = 0f;

		refuseButtonText.text = left + time + right;

		while (time > 0)
		{
			yield return null;
			totalTime += Time.deltaTime;
			if (timeBeforeAutoReset - time + 1 < totalTime)
				time--;

			refuseButtonText.text = left + time + right;
			if (attemptAutoResetCoroutineState == CoroutineState.Killed)
				yield break;
		}

		AcceptAutoReset();
	}

	public void Save()
	{
		EventCenter.Broadcast(EventCenter.GlobalEvent.Save, null);
	}

	public void Quit()
	{
		Application.Quit();
	}
}
