//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

public class SceneContent : MonoBehaviour
{
	public string Headline;
	[Multiline]
	public string Description;
	[Multiline]
	public string InteractionTip;
	public bool HasSuggestion;
}
