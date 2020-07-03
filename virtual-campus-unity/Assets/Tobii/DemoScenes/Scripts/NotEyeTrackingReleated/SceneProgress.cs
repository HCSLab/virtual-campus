using UnityEngine;
using UnityEngine.UI;

public class SceneProgress : MonoBehaviour
{
	private Slider _slider;

	// Use this for initialization
	void Start()
	{
		_slider = GetComponent<Slider>();
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
		_slider.value = (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1) / (float)UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
#else
		_slider.value = (UnityEngine.Application.loadedLevel + 1) / (float)UnityEngine.Application.levelCount;
#endif
	}
}
