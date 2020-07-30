using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	[Header("UI")]
	public GameObject mainMenu;
	public GameObject overwriteConfirmation;
	public GameObject credits;
	public GameObject continueButton;

	bool isThereAnySaveFiles;

	private void Start()
	{
		mainMenu.SetActive(true);
		overwriteConfirmation.SetActive(false);
		credits.SetActive(false);

		isThereAnySaveFiles = PlayerPrefs.GetInt(SaveSystem.GetIsThereAnySaveFileName(), 0) > 0;
		continueButton.SetActive(isThereAnySaveFiles);

	}

	void LoadGame()
	{
		PlayerPrefs.SetInt(SaveSystem.GetIsThereAnySaveFileName(), 1);

		SceneLoadingManager.Instance.LoadGame();

		mainMenu.SetActive(false);
		overwriteConfirmation.SetActive(false);
		credits.SetActive(false);
	}

	public void ContinueGame()
	{
		LoadGame();
	}

	public void NewGame()
	{
		if (!isThereAnySaveFiles)
		{
			LoadGame();
			return;
		}

		mainMenu.SetActive(false);
		overwriteConfirmation.SetActive(true);
	}

	public void ConfirmOverwrite()
	{
		PlayerPrefs.DeleteAll();
		LoadGame();
	}

	public void RefuseOverwrite()
	{
		mainMenu.SetActive(true);
		overwriteConfirmation.SetActive(false);
	}

	public void OpenCredits()
	{
		mainMenu.SetActive(false);
		credits.SetActive(true);
	}

	public void CloseCredits()
	{
		mainMenu.SetActive(true);
		credits.SetActive(false);
	}

	public void Quit()
	{
		Application.Quit();
	}
}
