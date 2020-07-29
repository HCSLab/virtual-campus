using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
	public static string GetAchievementProgressName(GameObject achievementItem)
	{
		return "AchievementProgress." + achievementItem.name;
	}

	// Corresponding int key in PlayerPerfs
	//	   0: not finished
	//	   1: finished
	public static string GetAchievementStateName(GameObject achievementItem)
	{
		return "AchievementState." + achievementItem.name;
	}

	public static string GetMasterVolumeName()
	{
		return "MasterVolume";
	}

	// Corresponding int key in PlayerPerfs
	//	   0: not visited
	//	   1: visited
	public static string GetRegionName(GameObject region)
	{
		return "RegionState." + region.name;
	}
}
