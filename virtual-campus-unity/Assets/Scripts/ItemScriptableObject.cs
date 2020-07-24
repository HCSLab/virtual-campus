using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemScriptableObject : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite icon;
}