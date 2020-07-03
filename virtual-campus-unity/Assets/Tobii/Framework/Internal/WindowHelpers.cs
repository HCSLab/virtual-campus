//-----------------------------------------------------------------------
// Copyright 2015 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

using System;
using System.Text;
using System.Diagnostics;

namespace Tobii.Gaming.Internal
{
	/// <summary>
	/// Contains utility functions for window handling.
	/// </summary>
	internal static class WindowHelpers
	{
		/// <summary>
		/// Shows the current window.
		/// </summary>
		public static void ShowCurrentWindow()
		{
			IntPtr hwnd = FindWindowWithThreadProcessId(Process.GetCurrentProcess().Id);
			Win32Helpers.ShowWindowAsync(hwnd, Win32Helpers.SW_SHOWDEFAULT);
		}

		internal static IntPtr FindWindowWithThreadProcessId(int processId)
		{
			var window = new IntPtr();

			Win32Helpers.EnumWindows(delegate (IntPtr wnd, IntPtr param)
			{
				var windowProcessId = 0;
				Win32Helpers.GetWindowThreadProcessId(wnd, out windowProcessId);
				if (windowProcessId != processId || !IsMainWindow(wnd))
				{
					return true;
				}

				window = wnd;
				return false;
			},
			IntPtr.Zero);

			if (window.Equals(IntPtr.Zero))
			{
				UnityEngine.Debug.LogError("Could not find any window with process id " + processId);
			}

			return window;
		}

		private static bool IsMainWindow(IntPtr hwnd)
		{
			return (Win32Helpers.GetWindow(hwnd, Win32Helpers.GW_OWNER) == IntPtr.Zero) && Win32Helpers.IsWindowVisible(hwnd);
		}

		internal static IntPtr GetGameViewWindowHandle(int processId)
		{
			const string GameViewCaption = "UnityEditor.GameView";
			const string UnityContainerClassName = "UnityContainerWndClass";

			var window = new IntPtr();

			Win32Helpers.EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
			{
				if (!Win32Helpers.IsWindowVisible(hWnd))
				{
					return true;
				}

				var windowProcessId = 0;
				Win32Helpers.GetWindowThreadProcessId(hWnd, out windowProcessId);

				if (windowProcessId == processId)
				{
					StringBuilder builder = new StringBuilder(256);
					Win32Helpers.GetClassName(hWnd, builder, 256);

					if (builder.ToString() == UnityContainerClassName)
					{
						//Ok, we found one of our containers, let's try to find the game view handle among the children
						Win32Helpers.EnumChildWindows(hWnd, delegate (IntPtr childHwnd, IntPtr childParam)
						{
							if (!Win32Helpers.IsWindowVisible(childHwnd))
							{
								return true;
							}

							int childLength = Win32Helpers.GetWindowTextLength(childHwnd);
							if (childLength == 0)
							{
								return true;
							}

							StringBuilder childBuilder = new StringBuilder(childLength);
							Win32Helpers.GetWindowText(childHwnd, childBuilder, childLength + 1);

							if (childBuilder.ToString() == GameViewCaption)
							{
								//Found it!
								window = childHwnd;
								return false;
							}

							return true;
						},
						IntPtr.Zero);
					}
				}

				return true;

			}, IntPtr.Zero);

			if (window.Equals(IntPtr.Zero))
			{
				UnityEngine.Debug.LogError("Could not find game view!");
			}

			return window;
		}
	}

}

#else
using System;
namespace Tobii.EyeTracking
{
    internal static class WindowHelpers
    {
        public static void ShowCurrentWindow()
        {
            throw new InvalidOperationException("Not available on this platform.");
        }
    }
}
#endif
