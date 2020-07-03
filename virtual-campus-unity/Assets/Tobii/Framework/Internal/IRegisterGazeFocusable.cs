//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//----------------------------------------------------------------------


using UnityEngine;

namespace Tobii.Gaming.Internal
{
	internal interface IRegisterGazeFocusable
	{
		/// <summary>
		/// Registers the supplied <see cref="IGazeFocusable"/> component so that
		/// the <see cref="GameObject"/> it belongs to can be focused using eye-gaze.
		/// </summary>
		/// <param name="gazeFocusableComponent">The component to register.</param>
		void RegisterFocusableComponent(IGazeFocusable gazeFocusableComponent);

		/// <summary>
		/// Unregisters the supplied <see cref="IGazeFocusable"/> component so
		/// that the <see cref="GameObject"/> it belongs to no longer can be 
		/// focused using eye-gaze.
		/// </summary>
		/// <param name="gazeFocusableComponent">The component to unregister.</param>
		void UnregisterFocusableComponent(IGazeFocusable gazeFocusableComponent);
	}
}