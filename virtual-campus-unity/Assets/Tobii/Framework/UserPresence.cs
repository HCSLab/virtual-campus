//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

namespace Tobii.Gaming
{
	/// <summary>
	/// Represents different user presence states.
	/// </summary>
	public enum UserPresence
	{
		/// <summary>
		/// User presence is unknown.
		/// This might be due to an error such as the eye tracker not tracking.
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// The user is present.
		/// </summary>
		Present = 1,
		/// <summary>
		/// The user is not present.
		/// </summary>
		NotPresent = 2,
	}

	public static class UserPresenceStatusExtensions
	{
		public static bool IsUserPresent(this UserPresence userPresence)
		{
			return userPresence == UserPresence.Present;
		}
	}
}
