using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
	public Toggle fullscreenToggle;
	public TMP_Dropdown resolutionDropdownMenu;
	public Slider masterVolumeSlider;

	public float timeBeforeAutoRestoreVideoSettings;

	Resolution[] resolutions;
	private void Start()
	{
		resolutions = Screen.resolutions;
		resolutionDropdownMenu.ClearOptions();
		List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
		for (int i = resolutions.Length - 1; i >= 0; i--)
			options.Add(new TMP_Dropdown.OptionData(resolutions[i].width.ToString() + " x " + resolutions[i].height.ToString()));
		resolutionDropdownMenu.AddOptions(options);

		fullscreenToggle.onValueChanged.AddListener(OnFullscreenModeChanged);
		resolutionDropdownMenu.onValueChanged.AddListener(OnResolutionChanged);
		masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
	}

	/// <param name="newMode">true -> Fullscreen; false -> windowed</param>
	public void OnFullscreenModeChanged(bool newMode)
	{
		print("Fullscreen: " + newMode);
	}

	public void OnResolutionChanged(int newResolutionIndex)
	{
		newResolutionIndex = resolutions.Length - newResolutionIndex - 1;

		print("Resolution: " + resolutions[newResolutionIndex].width + " x " + resolutions[newResolutionIndex].height);
	}

	public void OnMasterVolumeChanged(float newVolume)
	{
		print("Volume: " + newVolume);
	}
}
