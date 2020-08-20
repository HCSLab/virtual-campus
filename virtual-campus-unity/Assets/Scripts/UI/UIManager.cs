using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	struct TabButton
	{
		public TextMeshProUGUI text;
		public Button button;
		public Image image;
	};

	[Header("SFX")]
	public AudioClip buttonPointerEnterClip;
	public AudioClip buttonPointerClickClip;
    public AudioClip missionFinishedSFX;
    public AudioClip achievementSFX;
    public AudioSource missionFinishedSource;
    public AudioSource achievementSource;
	public AudioSource sfxSource;
	public AudioSource textSFXSource;

   [Header("HUD")]
	public GameObject hudCanvas;
	public GameObject pressToTalk;
	public Transform talkContainer;
    public GameObject fullScreenMapCanvas;

	[Header("Tab Menu")]
	public GameObject tabMenuCanvas;
	public GameObject[] tabs;
	public GameObject[] tabButtons;
	public Color openedTabButtonImageColor, closedTabButtonImageColor;
	public Color openedTabButtonTextColor, closedTabButtonTextColor;

	[Header("Photography")]
	public GameObject photographyCanvas;
	public RectTransform splashWhenTakingPhoto;
	public GameObject photographyHint;

    [Header("Others")]
    public Scrollbar settingsVerticalScrollbar;
    public Scrollbar logVerticalScrollbar;
	public GameObject cameraButton;

    [Header("Parkour")]
    public GameObject parkourCanvas;
    public Timer timer;
    public GameObject successText;
    public GameObject failureText;
    public GameObject promptText;
    public AudioClip pathPointSFX;
    public AudioClip countdownSFX;
    public AudioSource pathPointSFXSource;
    public AudioSource counddownSFXSource;
    public AudioClip successSFX;
    public AudioClip failureSFX;
    

    // [Header("Painter")]
    // public GameObject painterHub;
    // public List<GameObject> painters;

    [HideInInspector]
	public GameObject currentTalk;

	// Cache
	TabButton[] tabButtonCaches;

	int currentTabIndex = 0;

	private void Awake()
	{
		Instance = this;
		AddSFXToButtons();
	}

	private void Start()
	{
		// GetPaintersFromHub();

		// Initialize tab buttons.
		tabButtonCaches = new TabButton[tabs.Length];
		for (int i = 0; i < tabs.Length; i++)
		{
			tabButtonCaches[i].text = tabButtons[i].GetComponentInChildren<TextMeshProUGUI>();
			tabButtonCaches[i].button = tabButtons[i].GetComponent<Button>();
			tabButtonCaches[i].image = tabButtons[i].GetComponent<Image>();

			var currentIndex = i;
			tabButtonCaches[i].button.onClick.RemoveAllListeners();
			tabButtonCaches[i].button.onClick.AddListener(() => { OpenTab(currentIndex); });
		}

		// Delay the disabling to ensure the Start() of every panel is called.
		StartCoroutine(DisableTabCanvas());

		// StartCoroutine(TestTask1());
		// StartCoroutine(TestTask2());
		// StartCoroutine(TestTask3());
		// StartCoroutine(TestTask4());
	}

	#region Test Tasks
	[Header("Test")]
	public ItemScriptableObject testItem;
	public RealWorldPhotoScriptableObject[] testPhotos;
	IEnumerator TestTask1()
	{
		yield return new WaitForSeconds(1f);

		MissionPanel.Instance.AddMission("1", "23", true);
		MissionPanel.Instance.AddMission("2", "21", false);
		MissionPanel.Instance.AddMission("校长的问候", "刚刚入学，你还对学校不太熟悉吧，去小广场处找校长，他会给你介绍整个学校的大致分布的。", false);
		MissionPanel.Instance.AddMission("3", "21", false);
		MissionPanel.Instance.AddMission("4", "23", false);

		yield return new WaitForSeconds(5f);
		MissionPanel.Instance.FinishMission("4");
	}


	IEnumerator TestTask2()
	{
		yield return new WaitForSeconds(1f);

		LogPanel.Instance.AddLog("你好", "世界！");

		LogPanel.Instance.AddLog("测试", "这是一个非常长的句子，他真的很长，特别长，长到要换行，怎么还没换行，终于换行了！");
	}

	IEnumerator TestTask3()
	{
		yield return new WaitForSeconds(1f);

		ItemPanel.Instance.AddItem(testItem);
	}

	IEnumerator TestTask4()
	{
		yield return new WaitForSeconds(1f);

		foreach (var photo in testPhotos)
			ItemPanel.Instance.AddPhoto(photo);
	}
	#endregion

	#region Painter
	//void GetPaintersFromHub()
	//{
	//	//PainterHub ph = painterHub.GetComponent<PainterHub>();
	//	//painters.Add(ph.entirePainter);
	//	//painters.Add(ph.headPainter);
	//	//painters.Add(ph.upperPainter);
	//	//painters.Add(ph.lowerPainter);
	//	//painters.Add(ph.hatPainter);
	//	//painters.Add(ph.armPainter);
	//}

	//void DisableAllOpenedPanel()
	//{
	//	painterHub.SetActive(false);
	//	foreach (GameObject painter in painters)
	//	{
	//		painter.SetActive(false);
	//	}
	//}
	#endregion

	void AddSFXToButtons()
	{
		var buttons = GameObject.FindObjectsOfType<Button>();
		foreach(var button in buttons)
		{
			var buttonSound = button.gameObject.AddComponent<ButtonSound>();
			buttonSound.enterClip = buttonPointerEnterClip;
			buttonSound.clickClip = buttonPointerClickClip;
		}
	}

    public void PlayAchievementSFX()
    {
        StartCoroutine(DelayedAchievementSFX());
    }
    public IEnumerator DelayedAchievementSFX()
    {
        yield return new WaitForSeconds(0.5f);
        achievementSource.PlayOneShot(UIManager.Instance.achievementSFX);
    }

    IEnumerator DisableTabCanvas()
	{
		yield return null;
		yield return null;
		for(int i = 0; i < tabs.Length; i++)
			CloseTab(i);
		tabMenuCanvas.SetActive(false);
	}

    public void OpenTalk(GameObject talk)
    {
        if (currentTalk != null)
        {
            Destroy(currentTalk);
        }
        currentTalk = talk;
        pressToTalk.SetActive(false);
		hudCanvas.SetActive(false);
    }

	public void CloseTalk(GameObject talk = null)
	{
		if (!currentTalk) return;
		if (talk == null || talk == currentTalk)
		{
			Destroy(currentTalk);
			currentTalk = null;
			hudCanvas.SetActive(true);
		}
	}

	public void OpenTab(int tabIndex)
	{
		if (!tabMenuCanvas.activeSelf)
        {
            tabMenuCanvas.SetActive(true);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ThirdPersonCamera.FreeForm>().enabled = false;
        }

			

		CloseTab(currentTabIndex);

		currentTabIndex = tabIndex;
		tabs[tabIndex].SetActive(true);
        if (tabIndex == 0)
        {
            resetLogPanel();
        }
        tabButtonCaches[tabIndex].text.color = openedTabButtonTextColor;
		tabButtonCaches[tabIndex].button.interactable = false;
		tabButtonCaches[tabIndex].image.color = openedTabButtonImageColor;
	}

	public void OpenTab(GameObject panel)
	{
		for (int i = 0; i < tabs.Length; i++)
			if (tabs[i] == panel)
			{
				OpenTab(i);
				return;
			}
	}

	public void CloseTab(int tabIndex)
	{
		tabButtonCaches[tabIndex].text.color = closedTabButtonTextColor;
		tabButtonCaches[tabIndex].button.interactable = true;
		tabButtonCaches[tabIndex].image.color = closedTabButtonImageColor;

		tabs[tabIndex].SetActive(false);
	}

    public void CloseCanvas()
    {
        tabMenuCanvas.SetActive(false);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ThirdPersonCamera.FreeForm>().enabled = true;
    }

    public void resetSettingsPanel()
    {
        settingsVerticalScrollbar.value = 1f;
    }

    public void resetLogPanel()
    {
        logVerticalScrollbar.value = 0f;
    }
}
