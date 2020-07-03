//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

namespace Tobii.Gaming.Internal
{
	public interface IGazeFocusable
	{
		GameObject gameObject { get; }
		void UpdateGazeFocus(bool hasFocus);
	}
}
