using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : SavableMonoBehavior
{
	public Toggle fullscreenToggle;
	public TMP_Dropdown resolutionDropdownMenu;
	public Slider masterVolumeSlider;

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

	protected override void Start()
	{
		base.Start();

		Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
		attemptAutoResetCoroutineState = CoroutineState.NotStartedYet;

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
		resolutionDropdownMenu.value = resolutions.Length - currentResolutionIndex - 1;
		masterVolumeSlider.value = PlayerPrefs.GetFloat(SaveSystem.GetMasterVolumeName(), 1f);
		OnMasterVolumeChanged(masterVolumeSlider.value);

		fullscreenToggle.onValueChanged.AddListener(OnFullscreenModeChanged);
		resolutionDropdownMenu.onValueChanged.AddListener(OnResolutionChanged);
		masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
	}

	protected override void Save(object data)
	{
		base.Save(data);

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
