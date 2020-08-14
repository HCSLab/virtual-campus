using System;
using System.Collections.Generic;

public static class EventCenter
{
	private static Dictionary<string, Action<object>> eventDict = new Dictionary<string, Action<object>>();

	public enum GlobalEvent
	{
		Save,
	};

	public enum AchievementEvent
	{
		None, // The event that will never happen.
		TickPerMinute,
		OneMissionFinished,
		NewAreaExplored,
		EmpathyBadgeEarned,
		CatBadgeEarned,
		PhotographyBadgeEarned,
		SchoolBadgeEarned,
		OneWelcomeMissionFinished,
        OneTreasureFound,
        OneParkourFinished,
        ParkourChengdaoGold,
        ParkourShawGold,
        ParkourAdminGold,
		LikenessUpdated
    };

	public static void AddListener<T>(T eventType, Action<object> callback)
	{
		if (!eventDict.ContainsKey(eventType.ToString()))
		{
			eventDict.Add(eventType.ToString(), null);
		}
		eventDict[eventType.ToString()] += callback;
	}

	public static void RemoveListener<T>(T eventType, Action<object> callback)
	{
		if (!eventDict.ContainsKey(eventType.ToString()))
			return;

		if (eventDict[eventType.ToString()] == null)
			return;

		eventDict[eventType.ToString()] -= callback;
	}

	public static void Broadcast<T>(T eventType, object data)
	{
		if (!eventDict.ContainsKey(eventType.ToString()))
			return;

		eventDict[eventType.ToString()]?.Invoke(data);
	}
}