using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagBag : SavableMonoBehavior
{
    static public FlagBag Instance;

    public List<string> bag = new List<string>();

    private void Awake()
    {
        Instance = this;
        Load();
    }

    void Load()
    {
        var flagCount = PlayerPrefs.GetInt(SaveSystem.GetFlagCountName(), 0);
        for (int i = 0; i < flagCount; i++)
            bag.Add(PlayerPrefs.GetString(SaveSystem.GetIthFlagName(i)));
    }

    protected override void Save(object data)
	{
		base.Save(data);

        PlayerPrefs.SetInt(SaveSystem.GetFlagCountName(), bag.Count);
        for (int i = 0; i < bag.Count; i++)
            PlayerPrefs.SetString(SaveSystem.GetIthFlagName(i), bag[i]);
	}

	public void AddFlag(string name)
    {
        bag.Add(name);

        StoryManager.Instance.refreshFlag = true;
    }

    public bool HasFlag(string name)
    {
        return bag.Contains(name);
    }

    public bool HasFlags(List<string> list)
    {
        foreach (var flag in list)
        {
            if (!HasFlag(flag))
            {
                return false;
            }
        }
        return true;
    }

    public bool WithoutFlags(List<string> list)
    {
        foreach (var flag in list)
        {
            if (HasFlag(flag))
            {
                return false;
            }
        }
        return true;
    }

    public void DelFlag(string name)
    {
        bag.Remove(name);

        StoryManager.Instance.refreshFlag = true;
    }

    public List<string> GetFlagsWithPrefix(string prefix)
    {
        var res = new List<string>();
        foreach (var flag in bag)
        {
            if (flag.StartsWith(prefix))
            {
                res.Add(flag);
            }
        }
        return res;
    }

    public void DelFlagsWithPrefix(string prefix)
    {
        var tmp = GetFlagsWithPrefix(prefix);
        foreach (var flag in tmp)
        {
            bag.Remove(flag);
        }

        //bag.RemoveAll((string flag) => { return flag.StartsWith(prefix); });

        StoryManager.Instance.refreshFlag = true;
    }
}
