using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInfo : SavableMonoBehavior
{
	static public PlayerInfo Instance;

	public int likeness;
	public string school;
	public int flag;
	public int badge;

	protected override void Start()
	{
		base.Start();
		likeness = digit["likeness"];
		school = info["school"];
	}
	private void Update()
	{
		likeness = digit["likeness"];
		school = info["school"];
		flag = digit["acflag"];
		badge = digit["badge"];
	}
	public enum InfoKeyword
	{
		Name,
		ID,
		Gender,
		School,
		College,
	};
	public readonly string[] InfoKeywordEnumToString =
	{
		"name",
		"id",
		"gender",
		"school",
		"college"
	};

	public enum DigitKeyword
	{
		ACFlag,        // Academic Career Index
		KittenLikeness, // Likeness of the Two Cats
		Badge
	}
	public readonly string[] DigitKeywordEnumToString =
	{
		"acflag",
		"likeness",
		"badge"
	};


	public static Dictionary<string, string> info = new Dictionary<string, string>();
	public static Dictionary<string, int> digit = new Dictionary<string, int>();

	private void Awake()
	{
		Instance = this;

		info[InfoKeywordEnumToString[(int)InfoKeyword.Name]] =
			PlayerPrefs.GetString(
				SaveSystem.GetInfoValueName(InfoKeywordEnumToString[(int)InfoKeyword.Name]),
				"test player"
				);
		info[InfoKeywordEnumToString[(int)InfoKeyword.ID]] =
			PlayerPrefs.GetString(
				SaveSystem.GetInfoValueName(InfoKeywordEnumToString[(int)InfoKeyword.ID]),
				"119010000"
				);
		info[InfoKeywordEnumToString[(int)InfoKeyword.Gender]] =
			PlayerPrefs.GetString(
				SaveSystem.GetInfoValueName(InfoKeywordEnumToString[(int)InfoKeyword.Gender]),
				"female"
				);
		info[InfoKeywordEnumToString[(int)InfoKeyword.School]] =
			PlayerPrefs.GetString(
				SaveSystem.GetInfoValueName(InfoKeywordEnumToString[(int)InfoKeyword.School]),
				"SME"
				);
		info[InfoKeywordEnumToString[(int)InfoKeyword.College]] =
			PlayerPrefs.GetString(
				SaveSystem.GetInfoValueName(InfoKeywordEnumToString[(int)InfoKeyword.College]),
				"Shaw"
				);

		// Digit saving
		digit[DigitKeywordEnumToString[(int)DigitKeyword.ACFlag]] =
			PlayerPrefs.GetInt(
				SaveSystem.GetInfoValueName(DigitKeywordEnumToString[(int)DigitKeyword.ACFlag]),
				0
				);
		digit[DigitKeywordEnumToString[(int)DigitKeyword.KittenLikeness]] =
			PlayerPrefs.GetInt(
				SaveSystem.GetInfoValueName(DigitKeywordEnumToString[(int)DigitKeyword.KittenLikeness]),
				0
				);
		digit[DigitKeywordEnumToString[(int)DigitKeyword.Badge]] =
			PlayerPrefs.GetInt(
				SaveSystem.GetInfoValueName(DigitKeywordEnumToString[(int)DigitKeyword.Badge]),
				0
				);
	}

	protected override void Save(object data)
	{
		base.Save(data);

		PlayerPrefs.SetString(
			SaveSystem.GetInfoValueName(InfoKeywordEnumToString[(int)InfoKeyword.Name]),
			info[InfoKeywordEnumToString[(int)InfoKeyword.Name]]
			);
		PlayerPrefs.SetString(
			SaveSystem.GetInfoValueName(InfoKeywordEnumToString[(int)InfoKeyword.ID]),
			info[InfoKeywordEnumToString[(int)InfoKeyword.ID]]
			);
		PlayerPrefs.SetString(
			SaveSystem.GetInfoValueName(InfoKeywordEnumToString[(int)InfoKeyword.Gender]),
			info[InfoKeywordEnumToString[(int)InfoKeyword.Gender]]
			);
		PlayerPrefs.SetString(
			SaveSystem.GetInfoValueName(InfoKeywordEnumToString[(int)InfoKeyword.School]),
			info[InfoKeywordEnumToString[(int)InfoKeyword.School]]
			);
		PlayerPrefs.SetString(
			SaveSystem.GetInfoValueName(InfoKeywordEnumToString[(int)InfoKeyword.College]),
			info[InfoKeywordEnumToString[(int)InfoKeyword.College]]
			);

		PlayerPrefs.SetInt(
			SaveSystem.GetInfoValueName(DigitKeywordEnumToString[(int)DigitKeyword.ACFlag]),
			digit[DigitKeywordEnumToString[(int)DigitKeyword.ACFlag]]
			);
		PlayerPrefs.SetInt(
			SaveSystem.GetInfoValueName(DigitKeywordEnumToString[(int)DigitKeyword.KittenLikeness]),
			digit[DigitKeywordEnumToString[(int)DigitKeyword.KittenLikeness]]
			);
		PlayerPrefs.SetInt(
			SaveSystem.GetInfoValueName(DigitKeywordEnumToString[(int)DigitKeyword.Badge]),
			digit[DigitKeywordEnumToString[(int)DigitKeyword.Badge]]
			);
	}

	public static void WriteToInkStory(Story story)
	{
		foreach (var i in info)
		{
			if (story.variablesState.Contains(i.Key))
			{
				story.variablesState[i.Key] = i.Value;
			}
		}

		foreach (var i in digit)
		{
			if (story.variablesState.Contains(i.Key))
			{
				story.variablesState[i.Key] = i.Value;
			}
		}
	}

	public static void UpdateFromInkStory(Story story)
	{
		string tempInfo = "";
		foreach (var key in info.Keys)
		{
			if (story.variablesState.Contains(key))
			{
				tempInfo = key;
			}
		}
		if (tempInfo != "") info[tempInfo] = (string)story.variablesState[tempInfo];

		string tempDigit = "";
		foreach (var key in digit.Keys)
		{
			if (story.variablesState.Contains(key))
			{
				tempDigit = key;
			}
		}

		if (tempDigit != "") digit[tempDigit] = (int)story.variablesState[tempDigit];

		if (tempDigit == Instance.DigitKeywordEnumToString[(int)DigitKeyword.KittenLikeness])
			EventCenter.Broadcast(EventCenter.AchievementEvent.LikenessUpdated, digit[tempDigit]);
	}
}
