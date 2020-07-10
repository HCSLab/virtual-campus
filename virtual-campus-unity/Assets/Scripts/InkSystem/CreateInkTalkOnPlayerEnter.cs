using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateInkTalkOnPlayerEnter : CreateInkTalk
{
	public List<string> require = new List<string>();
	public List<string> without = new List<string>();

	private bool playerInRange = false;
	private bool talkOpened = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			playerInRange = true;

			if (playerInRange &&
				!talkOpened &&
				FlagBag.Instance.HasFlags(require) &&
				FlagBag.Instance.WithoutFlags(without))
			{
				UIManager.Instance.pressToTalk.SetActive(true);
				var btn = UIManager.Instance.pressToTalk.GetComponent<Button>();
				btn.onClick.RemoveAllListeners();
				btn.onClick.AddListener(() => { Create(); talkOpened = true; });
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			playerInRange = false;
			talkOpened = false;
			UIManager.Instance.CloseTalk(talk);
			UIManager.Instance.pressToTalk.SetActive(false);
		}
	}
}
