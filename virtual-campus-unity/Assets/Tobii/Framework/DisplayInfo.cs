//-----------------------------------------------------------------------
// Copyright 2017 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

namespace Tobii.Gaming
{
	/// <summary>
	/// DisplayInfo contains information about the eye-tracked display monitor.
	/// </summary>
	public struct DisplayInfo
	{
		/// <summary>
		/// Creates a DisplayInfo instance.
		/// </summary>
		/// <param name="displayWidthMm"></param>
		/// <param name="displayHeightMm"></param>
		internal DisplayInfo(float displayWidthMm, float displayHeightMm) : this()
		{
			DisplayWidthMm = displayWidthMm;
			DisplayHeightMm = displayHeightMm;
		}

		/// <summary>
		/// Creates a DisplayInfo instance representing an invalid state.
		/// </summary>
		public static DisplayInfo Invalid
		{
			get { return new DisplayInfo(float.NaN, float.NaN); }
		}

		/// <summary>
		/// Gets the validity of this DisplayInfo instance.
		/// </summary>
		public bool IsValid
		{
			get { return !float.IsNaN(DisplayWidthMm) && !float.IsNaN(DisplayHeightMm); }
		}

		/// <summary>
		/// Gets the width in millimeters of the eye tracked display monitor.
		/// </summary>
		public float DisplayWidthMm { get; private set; }

		/// <summary>
		/// Gets the height in millimeters of the eye tracked display monitor.
		/// </summary>
		public float DisplayHeightMm { get; private set; }
	}
}