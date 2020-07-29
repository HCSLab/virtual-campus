using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInfo : SavableMonoBehavior
{
	public enum InfoKeyword
	{
		Name,
		ID,
		Gender,
		School,
		College
	};

	public readonly string[] KeywordEnumToString =
	{
		"name",
		"id",
		"gender",
		"school",
		"college"
	};


	public static Dictionary<string, string> info = new Dictionary<string, string>();

	private void Awake()
	{
		info[KeywordEnumToString[(int)InfoKeyword.Name]] =
			PlayerPrefs.GetString(SaveSystem.GetInfoValueName(
				KeywordEnumToString[(int)InfoKeyword.Name]),
				"test player"
				);
		info[KeywordEnumToString[(int)InfoKeyword.ID]] =
			PlayerPrefs.GetString(SaveSystem.GetInfoValueName(
				KeywordEnumToString[(int)InfoKeyword.ID]),
				"119010000"
				);
		info[KeywordEnumToString[(int)InfoKeyword.Gender]] =
			PlayerPrefs.GetString(SaveSystem.GetInfoValueName(
				KeywordEnumToString[(int)InfoKeyword.Gender]),
				"female"
				);
		info[KeywordEnumToString[(int)InfoKeyword.School]] =
			PlayerPrefs.GetString(SaveSystem.GetInfoValueName(
				KeywordEnumToString[(int)InfoKeyword.School]),
				"SME"
				);
		info[KeywordEnumToString[(int)InfoKeyword.College]] =
			PlayerPrefs.GetString(SaveSystem.GetInfoValueName(
				KeywordEnumToString[(int)InfoKeyword.College]),
				"Shaw"
				);

		// info["name"] = "test player";
		// info["id"] = "119010000";
		// info["gender"] = "female";
		// info["school"] = "SME";
		// info["collage"] = "Shaw";
	}

	protected override void Save(object data)
	{
		base.Save(data);

		PlayerPrefs.SetString(
			KeywordEnumToString[(int)InfoKeyword.Name],
			info[KeywordEnumToString[(int)InfoKeyword.Name]]
			);
		PlayerPrefs.SetString(
			KeywordEnumToString[(int)InfoKeyword.ID],
			info[KeywordEnumToString[(int)InfoKeyword.ID]]
			);
		PlayerPrefs.SetString(
			KeywordEnumToString[(int)InfoKeyword.Gender],
			info[KeywordEnumToString[(int)InfoKeyword.Gender]]
			);
		PlayerPrefs.SetString(
			KeywordEnumToString[(int)InfoKeyword.School],
			info[KeywordEnumToString[(int)InfoKeyword.School]]
			);
		PlayerPrefs.SetString(
			KeywordEnumToString[(int)InfoKeyword.College],
			info[KeywordEnumToString[(int)InfoKeyword.College]]
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
	}

	public static void UpdateFromInkStory(Story story)
	{
		foreach (var key in info.Keys)
		{
			if (story.variablesState.Contains(key))
			{
				info[key] = (string)story.variablesState[key];
			}
		}
	}
}
