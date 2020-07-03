//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine.UI;

/*
 * Since we are essentially taking control over opacity for all child elements of game objects with a <see cref="CleanUI"/> component, 
 * if you want to programmatically change the opacity for any component governed by this script, you cannot do this directly.
 * Instead, please add this component and change the opacity through the OpacityOverride property.
 */

public class CleanUIOpacityOverrideForCanvas : CleanUIOpacityOverride
{
	protected void Start()
	{
		var graphic = GetComponent<Graphic>();
		if (graphic != null)
		{
			OpacityOverride = graphic.color.a;
		}
	}
}