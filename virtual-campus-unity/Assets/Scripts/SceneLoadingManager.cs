using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneIndexes
{
	LogIn,
	MainGame
}

public class SceneLoadingManager : MonoBehaviour
{
	static public SceneLoadingManager instance;

	public GameObject loadingScreen;
	public Image progressBar;

	List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

	public void LoadGame()
	{
		loadingScreen.SetActive(true);

		scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.LogIn));
		scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.MainGame, LoadSceneMode.Additive));

		StartCoroutine(GetSceneLoadProgress());
	}

	IEnumerator GetSceneLoadProgress()
	{
		foreach (AsyncOperation op in scenesLoading)
		{
			while (!op.isDone)
			{
				var totalProgress = 0f;
				foreach (AsyncOperation op2 in scenesLoading)
					totalProgress += op2.progress;
				totalProgress /= scenesLoading.Count;
				progressBar.fillAmount = totalProgress;

				yield return null;
			}
		}

		loadingScreen.SetActive(false);
		SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MainGame));
	}

	void Start()
	{
		if (instance)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
	}
}
