using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
	public static string GetAchievementProgressName(GameObject achievementItem)
	{
		return "AchievementProgress." + achievementItem.name;
	}
}
