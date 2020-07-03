//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Tobii.Gaming.Internal;
using UnityEngine;

namespace Tobii.Gaming.Stubs
{
	public class GazeFocusStub : IGazeFocus, IRegisterGazeFocusable, IGazeFocusInternal
	{
		//---------------------------------------------------------------------
		// Public static properties and methods
		//---------------------------------------------------------------------
		public static bool IsInitialized { get { return false; } }
		public static void SettingsUpdated() { /** no implementation **/ }
		public static bool IsFocusableObject(GameObject gameObject) { return false; }

		//---------------------------------------------------------------------
		// Implementing IGazeFocus
		//---------------------------------------------------------------------
		public Camera Camera { get; set; }
		public FocusedObject FocusedObject { get { return FocusedObject.Invalid; } }
		public IEnumerable<GameObject> ObjectsInGaze { get { return new List<GameObject>(); } }

		//---------------------------------------------------------------------
		// Implementing IRegisterGazeFocusable
		//---------------------------------------------------------------------
		public void RegisterFocusableComponent(IGazeFocusable gazeFocusableComponent) { /** no implementation **/ }
		public void UnregisterFocusableComponent(IGazeFocusable gazeFocusableComponent) { /** no implementation **/ }

		//---------------------------------------------------------------------
		// Implementing IGazeFocusInternal
		//--------------------------------------------------------------------
		public void UpdateGazeFocus() { /** no implementation **/ }

		//---------------------------------------------------------------------
		// Internal static properties and methods
		//---------------------------------------------------------------------
		internal static IScorer Scorer { get; set; }
		internal static LayerMask LayerMask { get { return -1; } }
		internal static float MaximumDistance { get { return float.PositiveInfinity; } }
	}
}
