using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionPreview : MonoBehaviour
{
    public TextMeshProUGUI missionNameText, missionDescriptionText;
    
    public void Initialize(string missionName, string missionDescription, int indexInMissionPanel)
	{
        missionNameText.text = missionName;
        missionDescriptionText.text = missionDescription;
        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(
            () => { UIManager.Instance.OpenTab(MissionPanel.Instance.gameObject); }
            );
        button.onClick.AddListener(
            () => { MissionPanel.Instance.SelectMission(indexInMissionPanel); }
            );
	}
}
