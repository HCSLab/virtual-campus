using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ClearPlayerPrefs
{
    [MenuItem("Profile/DelAll", false, 23)]
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
