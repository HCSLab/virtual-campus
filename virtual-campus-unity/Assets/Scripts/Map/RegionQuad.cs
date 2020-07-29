using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionQuad : SavableMonoBehavior
{
	public new string name;
	public new string tag;

	[HideInInspector]
	public bool isVisited;

	protected override void Start()
	{
		base.Start();

		isVisited = PlayerPrefs.GetInt(SaveSystem.GetRegionName(gameObject), 0) > 0;
		if (isVisited)
			EventCenter.Broadcast(EventCenter.AchievementEvent.NewAreaExplored, null);
	}

	protected override void Save(object data)
	{
		base.Save(data);

		PlayerPrefs.SetInt(SaveSystem.GetRegionName(gameObject), isVisited ? 1 : 0);
	}
}
