//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Tobii.Gaming.Internal
{
	/// <summary>
	/// Provides access to some important Win32 API functions. See MSDN for more information.
	/// </summary>
	internal static class Win32Helpers
	{
		public const int MONITOR_DEFAULTTONULL = 0;
		public const int MONITOR_DEFAULTTOPRIMARY = 1;
		public const int MONITOR_DEFAULTTONEAREST = 2;

		[DllImport("user32.dll")]
		public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

		[DllImport("user32.dll")]
		public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

		[StructLayout(LayoutKind.Sequential)]
		public struct MONITORINFO
		{
			public int cbSize;
			public RECT rcMonitor;
			public RECT rcWork;
			public uint dwFlags;
		}

		public const int SW_SHOWDEFAULT = 10;
		public const int GW_OWNER = 4;

		public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

		[DllImport("user32.dll")]
		public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ShowWindowAsync(IntPtr windowHandle, int nCmdShow);

		[DllImport("user32.dll")]
		public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr _hwnd, ref RECT windowClientRect);

		[DllImport("user32.dll")]
		public static extern bool LogicalToPhysicalPoint(IntPtr hWnd, ref POINT lpPoint);

		[DllImport("user32.dll")]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
	}
}
#endif
