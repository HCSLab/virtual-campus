using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneIndexes
{
	PersistentScene,
	MainMenu,
	MainGame
}

public class SceneLoadingManager : MonoBehaviour
{
	static public SceneLoadingManager Instance;

	[Header("Skybox Animation")]
	public GameObject skyboxAnimationContainer;
	public AudioSource sfxSource;
	public Skybox skybox;
	public float cycle;

	[Header("Loading Screen")]
	public GameObject loadingCanvas;
	public Image progressBar;

	List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

	public void LoadGame()
	{
		loadingCanvas.SetActive(true);

		scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.MainMenu));
		scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.MainGame, LoadSceneMode.Additive));

		StartCoroutine(GetSceneLoadProgress());
	}

    public void LoadMenu()
    {
        loadingCanvas.SetActive(true);

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.MainGame));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.MainMenu, LoadSceneMode.Additive));

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

        loadingCanvas.SetActive(false);
		skyboxAnimationContainer.SetActive(false);
		gameObject.LeanCancel();

		SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MainGame));
	}

	void Start()
	{
		if (Instance)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		SceneManager.LoadScene((int)SceneIndexes.MainMenu, LoadSceneMode.Additive);

		loadingCanvas.SetActive(false);
		skyboxAnimationContainer.SetActive(true);

		LeanTween.value(
			gameObject,
			(x) => { skybox.material.SetFloat("_Rotation", x); },
			0f,
			360f,
			cycle
			).setEaseLinear().setRepeat(-1);
	}
}
