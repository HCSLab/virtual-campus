using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentRegion : MonoBehaviour
{
	public static CurrentRegion Instance;

	public TMPro.TextMeshProUGUI tm;

	[HideInInspector]
	public int regionCount;

	Transform player;
	RegionQuad[] regions;

	private void Awake()
	{
		Instance = this;

		regions = GameObject.FindObjectsOfType<RegionQuad>();
		regionCount = regions.Length;
	}

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	int frameCount = 0;
	void Update()
	{
		frameCount = (frameCount + 1) % 15;
		if (frameCount != 0)
			return;

		RaycastHit hit;
		bool inRegion = Physics.Raycast(player.position, Vector3.down, out hit, 100.0f, 1 << LayerMask.NameToLayer("CurrentRegion"));
		if (inRegion)
		{
			RegionQuad regionQuad = hit.collider.gameObject.GetComponent<RegionQuad>();
			if (!regionQuad.isVisited)
			{
				regionQuad.isVisited = true;
				EventCenter.Broadcast(EventCenter.AchievementEvent.NewAreaExplored, null);
			}
			string regionName = regionQuad.name;
			tm.text = regionName;
		}
		else
		{
			tm.text = "下园";
		}
	}
}
