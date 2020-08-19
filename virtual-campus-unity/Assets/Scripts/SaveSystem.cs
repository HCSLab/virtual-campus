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

	public static string GetAntiAliasingModeName()
	{
		return "AntiAliasingMode";
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

	public static string GetFlagCountName()
	{
		return "FlagCount";
	}

	public static string GetIthFlagName(int i)
	{
		return "FlagName." + i;
	}

	public static string GetInfoValueName(string keyword)
	{
		return "InfoValue." + keyword;
	}

	public static string GetIsThereAnySaveFileName()
	{
		return "IsAnySaveFileExist";
	}

	public static string GetItemFlagName(ItemScriptableObject i)
	{
		return "ItemPanel.HasItem" + i.name;
	}

	public static string GetSkinFlagName(SkinScriptableObject s)
	{
		return "ItemPanel.HasSkin" + s.name;
	}

	public static string GetRealWorldPhotoFlagName(RealWorldPhotoScriptableObject r)
	{
		return "ItemPanel.HasRealWorldPhoto" + r.name;
	}
}
