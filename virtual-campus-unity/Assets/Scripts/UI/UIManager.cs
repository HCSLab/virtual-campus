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

	[Header("HUD")]
	public GameObject hudCanvas;
	public GameObject pressToTalk;

	[Header("Tab Menu")]
	public GameObject tabMenuCanvas;
	public GameObject[] tabs;
	public GameObject[] tabButtons;
	public Color openedTabButtonImageColor, closedTabButtonImageColor;
	public Color openedTabButtonTextColor, closedTabButtonTextColor;
	public GameObject itemBagPanel;

	[Header("Photography")]
	public GameObject photographyCanvas;

	[Header("Painter")]
	public GameObject painterHub;
	public List<GameObject> painters;

	[HideInInspector]
	public GameObject currentTalk;

	// Cache
	ScriptedFirstPersonAIO playerSFPAIO;
	TabButton[] tabButtonCaches;

	int currentTabIndex = 0;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		playerSFPAIO = GameObject.FindGameObjectWithTag("Player")
			.GetComponent<ScriptedFirstPersonAIO>();

		GetPaintersFromHub();

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

			CloseTab(i);
		}
	}

	void GetPaintersFromHub()
	{
		//PainterHub ph = painterHub.GetComponent<PainterHub>();
		//painters.Add(ph.entirePainter);
		//painters.Add(ph.headPainter);
		//painters.Add(ph.upperPainter);
		//painters.Add(ph.lowerPainter);
		//painters.Add(ph.hatPainter);
		//painters.Add(ph.armPainter);
	}

	void DisableAllOpenedPanel()
	{

		painterHub.SetActive(false);
		foreach (GameObject painter in painters)
		{
			painter.SetActive(false);
		}
	}

	public void OpenTalk(GameObject talk)
	{
		DisableAllOpenedPanel();
		if (currentTalk != null)
		{
			Destroy(currentTalk);
		}
		currentTalk = talk;
		playerSFPAIO.playerCanMove = false;
	}

	public void CloseTalk(GameObject talk = null)
	{
		if (talk == null)
		{
			if (currentTalk)
			{
				Destroy(currentTalk);
				currentTalk = null;
				playerSFPAIO.playerCanMove = true;
			}
		}
		else
		{
			if (currentTalk == talk)
			{
				Destroy(currentTalk);
				currentTalk = null;
				playerSFPAIO.playerCanMove = true;
			}
		}
	}

	public void OpenTab(int tabIndex)
	{
		if (!tabMenuCanvas.activeSelf)
			tabMenuCanvas.SetActive(true);

		CloseTab(currentTabIndex);

		currentTabIndex = tabIndex;
		tabs[tabIndex].SetActive(true);
		tabButtonCaches[tabIndex].text.color = openedTabButtonTextColor;
		tabButtonCaches[tabIndex].button.interactable = false;
		tabButtonCaches[tabIndex].image.color = openedTabButtonImageColor;

		if (tabs[tabIndex] == itemBagPanel)
		{
			ItemBag.Instance.Reselect();
		}
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
}
