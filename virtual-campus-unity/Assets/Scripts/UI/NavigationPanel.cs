using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NavigationPanel : MonoBehaviour
{
	static public NavigationPanel Instance;

    public TextMeshProUGUI areaName;

	private void Awake()
	{
		Instance = this;
	}

	public void UpdateAreaName(string newAreaName)
	{
		areaName.text = newAreaName;
	}
}
