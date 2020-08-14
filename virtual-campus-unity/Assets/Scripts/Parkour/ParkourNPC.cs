using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParkourNPC : MonoBehaviour
{
    public GameObject parkourTalkPanel;

    public List<string> parkours;

    public void LoadLevel(int i)
    {
        Object parkourPrefab = Resources.Load(parkours[i], typeof(GameObject));
        GameObject parkour = Instantiate(parkourPrefab) as GameObject;
        parkourTalkPanel.SetActive(false);
    }

    public void OpenTalkPanel()
    {
        parkourTalkPanel.SetActive(true);
        UIManager.Instance.pressToTalk.SetActive(false); 
    }

    public void CloseTalkPanel()
    {
        parkourTalkPanel.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        if (other.tag == "Player")
        {
            UIManager.Instance.pressToTalk.SetActive(true);
            var btn = UIManager.Instance.pressToTalk.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { OpenTalkPanel(); });
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!enabled) return;

        if (other.tag == "Player")
        {
            if (parkourTalkPanel.activeSelf)
            {
                UIManager.Instance.sfxSource.PlayOneShot(UIManager.Instance.buttonPointerClickClip);
            }
            UIManager.Instance.pressToTalk.SetActive(false);
            CloseTalkPanel();
            
        }
    }
}
