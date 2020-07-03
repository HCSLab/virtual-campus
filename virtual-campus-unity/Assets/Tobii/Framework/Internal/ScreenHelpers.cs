//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tobii.Gaming.Internal
{
    /// <summary>
    /// Provides functions related to game view bounds resolution.
    /// </summary>
    internal abstract class GameViewBoundsProvider
    {
        private IntPtr _hwnd = IntPtr.Zero;

        public IntPtr Hwnd
        {
            get
            {
                if (_hwnd == IntPtr.Zero)
                {
                    _hwnd = GetGameViewWindowHandle();
                }
                return _hwnd;
            }
            protected set
            {
                _hwnd = value;
            }
        }

        /// <summary>
        /// Gets the eye tracked monitor's screen bounds on the Virtual Screen.
        /// </summary>
        /// <returns>Rect populated with upper-left corner location (x,y), and
        /// screen size (width, height) in pixels on the Virtual Screen.</returns>
        public Rect GetMonitorScreenBounds()
        {
            var monitorHandle = Win32Helpers.MonitorFromWindow(Hwnd, Win32Helpers.MONITOR_DEFAULTTONEAREST);
            var monitorInfo = new Win32Helpers.MONITORINFO();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);

            Win32Helpers.GetMonitorInfo(monitorHandle, ref monitorInfo);
            var rc = monitorInfo.rcMonitor;
            return new Rect(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
        }

        /// <summary>
        /// Finds the window associated with the current thread and process.
        /// </summary>
        /// <returns>A window handle represented as a <see cref="IntPtr"/>.</returns>
        protected virtual IntPtr GetGameViewWindowHandle()
        {
            var processId = Process.GetCurrentProcess().Id;
            return WindowHelpers.FindWindowWithThreadProcessId(processId);
        }

        /// <summary>
        /// Gets the Game View window's top left Position.
        /// </summary>
        /// <remarks>Overridden in test project. Do not remove without updating tests.</remarks>
        protected virtual Vector2 GetWindowPosition()
        {
            var windowPosition = new Win32Helpers.POINT();
            Win32Helpers.ClientToScreen(Hwnd, ref windowPosition);
            return new Vector2(windowPosition.x, windowPosition.y);
        }

        /// <summary>
        /// Gets the Game View window's bottom right corner Position.
        /// </summary>
        /// <remarks>Overridden in test project. Do not remove without updating tests.</remarks>
        protected virtual Vector2 GetWindowBottomRight()
        {
            var clientRect = new Win32Helpers.RECT();
            Win32Helpers.GetClientRect(Hwnd, ref clientRect);

            var bottomRight = new Win32Helpers.POINT { x = clientRect.right, y = clientRect.bottom };
            Win32Helpers.ClientToScreen(Hwnd, ref bottomRight);

            return new Vector2(bottomRight.x, bottomRight.y);
        }

        /// <summary>
        /// Gets the (Unity) screen size.
        /// </summary>
        /// <returns></returns>
        protected virtual Vector2 GetScreenSize()
        {
            return new Vector2(Screen.width, Screen.height);
        }

        public abstract Rect GetGameViewClientAreaNormalizedBounds();

#if UNITY_EDITOR
        /// <summary>
        /// Gets the Unity toolbar height.
        /// </summary>
        /// <returns></returns>
        protected virtual float GetToolbarHeight()
        {
            try
            {
                return UnityEditor.EditorStyles.toolbar.fixedHeight * 2.0f; // seems only half the physica pixel size is returned by Unity, or that there are two stacked toolbars.
            }
            catch (NullReferenceException)
            {
                return 0f;
            }
        }

        /// <summary>
        /// Gets the Unity game view.
        /// </summary>
        /// <returns></returns>
        protected virtual UnityEditor.EditorWindow GetMainGameView()
        {
            var unityEditorType = Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Diagnostics.Debug.Assert(unityEditorType != null);
            var getMainGameViewMethod = unityEditorType.GetMethod("GetMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
            System.Diagnostics.Debug.Assert(getMainGameViewMethod != null);
            var result = getMainGameViewMethod.Invoke(null, null);
            return (UnityEditor.EditorWindow)result;
        }
#endif
    }

    /// <summary>
    /// Provides utility functions related to screen and window handling within the Unity Player.
    /// </summary>
    internal class UnityPlayerGameViewBoundsProvider : GameViewBoundsProvider
    {
        public override Rect GetGameViewClientAreaNormalizedBounds()
        {
            var topLeft = GetWindowPosition();
            var bottomRight = GetWindowBottomRight();

            var logicalWidth = (int)(bottomRight.x - topLeft.x);
            var logicalHeight = (int)(bottomRight.y - topLeft.y);

            var isTrueFullScreen = (logicalWidth == Screen.width) && (logicalHeight == Screen.height);
            if (Screen.fullScreen
                && !isTrueFullScreen
                && !(logicalHeight < float.Epsilon
                    || logicalWidth < float.Epsilon))
            {
                //Full screen with abnormal settings (aspect ratio or scaling), unity always seems to gazePointVisualisationScale to the physical screen
                float gameAspectRatio = (float)Screen.width / Screen.height;
                float logicalAspectRatio = (float)logicalWidth / logicalHeight;

                if (gameAspectRatio > logicalAspectRatio)
                {
                    //Bars to top and bottom
                    float gameHeightInLogicalPixels = logicalWidth / gameAspectRatio;
                    return new Rect(0, ((logicalHeight - gameHeightInLogicalPixels) / 2.0f) / logicalHeight, 1, gameHeightInLogicalPixels / logicalHeight);
                }
                else
                {
                    //Bars to left and right
                    float gameWidthInLogicalPixels = logicalHeight * gameAspectRatio;
                    return new Rect(((logicalWidth - gameWidthInLogicalPixels) / 2.0f) / logicalWidth, 0, gameWidthInLogicalPixels / logicalWidth, 1);
                }
            }
            else
            {
                //Simple full screen where aspect is equal or windowed
                return new Rect(0, 0, 1, 1);
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// This class is used to resolve the editor game view bounds in 
    /// Unity versions previous to 4.6.
    /// </summary>
    internal class LegacyEditorGameViewBoundsProvider : GameViewBoundsProvider
    {
        private UnityEditor.EditorWindow _gameWindow;
        private bool _initialized;

        private void Initialize()
        {
            _gameWindow = GetMainGameView();
            _initialized = true;
        }

        /// <summary>
        /// Gets the Position of the game view in logical pixels when run from Unity Editor.
        /// </summary>
        /// <returns>The Position of the game view in logical pixels.</returns>
        public override Rect GetGameViewClientAreaNormalizedBounds()
        {
            if (!_initialized)
            {
                Initialize();
            }

            var gameWindowBounds = _gameWindow.position;

            // Adjust for the toolbar
            var toolbarHeight = GetToolbarHeight();
            gameWindowBounds.y += toolbarHeight;
            gameWindowBounds.height -= toolbarHeight;

            // Get the screen size.
            var screenSize = GetScreenSize();

            // Adjust for unused areas caused by fixed aspect ratio or resolution vs game window size mismatch
            var gameViewOffsetX = (gameWindowBounds.width - screenSize.x) / 2.0f;
            var gameViewOffsetY = (gameWindowBounds.height - screenSize.y) / 2.0f;

            if (screenSize.x < float.Epsilon
                || screenSize.y < float.Epsilon)
            {
                return new Rect(0, 0, 1, 1);
            }

            return new Rect(
                gameViewOffsetX / screenSize.x,
                gameViewOffsetY / screenSize.y,
                1,
                1);
        }
    }

    /// <summary>
    /// This class is used to resolve the editor game view bounds for 
    /// Unity version 4.6 and above.
    /// </summary>
    internal class EditorGameViewBoundsProvider : GameViewBoundsProvider
    {
        //[StructLayout(LayoutKind.Sequential)]
        //public struct POINT
        //{
        //    public int X;
        //    public int Y;
        //    public static implicit operator Vector2(POINT p)
        //    {
        //        return new Vector2(p.X, p.Y);
        //    }
        //}
        //[DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool GetCursorPos(out POINT lpPoint);

        private float _newHandleTimer = 2.0f;

        /// <summary>
        /// Gets the Position of the game view in logical pixels when run from Unity Editor.
        /// </summary>
        /// <returns>The Position of the game view in logical pixels.</returns>
        public override Rect GetGameViewClientAreaNormalizedBounds()
        {
            UpdateWindowHandle();

            var topLeft = GetWindowPosition();
            var bottomRight = GetWindowBottomRight();

            if (bottomRight.y - topLeft.y < float.Epsilon
                || bottomRight.y - topLeft.y < float.Epsilon)
            {
                return new Rect(0, 0, 1, 1);
            }


            //Vector2 mousePositionScreen = Input.mousePosition;
            //// flip input Cursor y (as the Reference "0" is the last scanline)
            //mousePositionScreen.y = Screen.height - 1 - mousePositionScreen.y;

            //POINT mousePositionDesktop;
            //GetCursorPos(out mousePositionDesktop);
            //var mousePositionWindow = new Vector2(mousePositionDesktop.X - topLeft.x, mousePositionDesktop.Y - topLeft.y);
            //var mousePositionWindowNormalized = new Vector2(mousePositionWindow.x / (bottomRight.x - topLeft.x),
            //    mousePositionWindow.y / (bottomRight.y - topLeft.y));

            //var mousePositionScreenNormalized = new Vector2(mousePositionScreen.x / Screen.width,
            //    mousePositionScreen.y / Screen.height);


            //var xOffset = (mousePositionWindowNormalized.x - mousePositionScreenNormalized.x) /
            //              (1 - mousePositionScreenNormalized.x);

            //var yOffset = (mousePositionWindowNormalized.y - mousePositionScreenNormalized.y) /
            //              (1 - mousePositionScreenNormalized.y);

            var toolbarHeight = GetToolbarHeight();

            var leftOffset = 0.0f;
            var rightOffset = 0.0f;
            var topOffset = toolbarHeight / (bottomRight.y - topLeft.y);
            var bottomOffset = 0.0f;

            if (Screen.height > 0 
                && Screen.width > 0
                && (1 - leftOffset - rightOffset) * (bottomRight.x - topLeft.x) > 0
                && (1 - topOffset - bottomOffset) * (bottomRight.y - topLeft.y) > 0)
            {
                var aspectScreen = Screen.width / (float)Screen.height;
                var aspectWindow = (1 - leftOffset - rightOffset) * (bottomRight.x - topLeft.x) /
                                   ((1 - topOffset - bottomOffset) * (bottomRight.y - topLeft.y));

                if (aspectScreen < aspectWindow)
                {
                    leftOffset += 0.5f * (1 - aspectScreen / aspectWindow);
                    rightOffset += 0.5f * (1 - aspectScreen / aspectWindow);
                }
                else
                {
                    topOffset += 0.5f * (1 - aspectWindow / aspectScreen);
                    bottomOffset += 0.5f * (1 - aspectWindow / aspectScreen);
                }
            }

            return new Rect(leftOffset, topOffset, 1 - leftOffset - rightOffset, 1 - topOffset - bottomOffset);
        }

        private void UpdateWindowHandle()
        {
            const float handleUpdateIntervalSecs = 2.0f;

            _newHandleTimer += Time.unscaledDeltaTime;
            if (_newHandleTimer > handleUpdateIntervalSecs)
            {
                //This function costs 0.5ms, so we want to do it as seldom as we can get away with. Caching it will not work though since it can change parents etc. etc.
                Hwnd = GetGameViewWindowHandle();
                _newHandleTimer = 0.0f;
            }
        }

        /// <summary>
        /// Gets the Game View window handle.
        /// </summary>
        /// <remarks>Overridden in test project. Do not remove without updating tests.</remarks>
        protected override IntPtr GetGameViewWindowHandle()
        {
            return WindowHelpers.GetGameViewWindowHandle(Process.GetCurrentProcess().Id);
        }
    }
#endif
}

#endif
