//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

using System;
using Tobii.GameIntegration.Net;
using UnityEngine;

namespace Tobii.Gaming.Internal
{
    internal class TobiiHost : MonoBehaviour, ITobiiHost
    {
        private static TobiiHost _instance;
        private static bool _isShuttingDown;

        private GameViewBoundsProvider _gameViewBoundsProvider;
        private GameViewInfo _gameViewInfo = GameViewInfo.DefaultGameViewInfo;

        private GazePointDataProvider _gazePointDataProvider;
        private HeadPoseDataProvider _headPoseDataProvider;
        private GazeFocus _gazeFocus;
        private int _lastUpdatedFrame;

        private static bool _hasDisplayedEulaError;

        //--------------------------------------------------------------------
        // Public Function and Properties
        //--------------------------------------------------------------------

        public static ITobiiHost GetInstance()
        {
            if (_isShuttingDown)
            {
                return new Stubs.TobiiHostStub();
            }

#if UNITY_EDITOR
            if (!(UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.StandaloneWindows64
                || UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.StandaloneWindows))
            {
                return new Stubs.TobiiHostStub();
            }
#endif

			if (!TobiiEulaFile.IsEulaAccepted())
			{
				if (!_hasDisplayedEulaError)
				{
					Debug.LogWarning("You need to accept EULA to be able to use Tobii Unity SDK.");
					_hasDisplayedEulaError = true;
				}
				return new Stubs.TobiiHostStub();
			}

			if (_instance != null) return _instance;

            var newGameObject = new GameObject("TobiiHost");
            DontDestroyOnLoad(newGameObject);
            _instance = newGameObject.AddComponent<TobiiHost>();
            return _instance;
        }


        public void Shutdown()
        {
            _isShuttingDown = true;

            if (IsInitialized)
            {
#if !UNITY_EDITOR
				TobiiGameIntegrationApi.Shutdown();
#endif
                IsInitialized = false;
            }
        }

        public DisplayInfo DisplayInfo
        {
            get
            {
                var trackerInfo = TobiiGameIntegrationApi.GetTrackerInfo();
                var displaySize = trackerInfo.DisplaySizeMm;
                return new DisplayInfo(displaySize.Width, displaySize.Height);
            }
        }

        public GameViewInfo GameViewInfo
        {
            get { return _gameViewInfo; }
        }

        public UserPresence UserPresence
        {
            get
            {
                return TobiiGameIntegrationApi.IsTrackerConnected()
                    ? (TobiiGameIntegrationApi.IsPresent() ? UserPresence.Present : UserPresence.NotPresent)
                    : UserPresence.Unknown;
            }
        }

        public bool IsInitialized { get; private set; }

        public bool IsConnected { get { return TobiiGameIntegrationApi.IsTrackerConnected(); } }

        public IGazeFocus GazeFocus
        {
            get { return _gazeFocus; }
        }

        public IDataProvider<GazePoint> GetGazePointDataProvider()
        {
            Update();
            _gazePointDataProvider.Update();
            return _gazePointDataProvider;
        }

        public IDataProvider<HeadPose> GetHeadPoseDataProvider()
        {
            Update();
            _headPoseDataProvider.Update();
            return _headPoseDataProvider;
        }

        //--------------------------------------------------------------------
        // MonoBehaviour Messages
        //--------------------------------------------------------------------

        void Awake()
        {
#if UNITY_EDITOR
            _gameViewBoundsProvider = CreateEditorScreenHelper();
#else
			_gameViewBoundsProvider = new UnityPlayerGameViewBoundsProvider();
#endif
            _gazeFocus = new GazeFocus();

            _gazePointDataProvider = new GazePointDataProvider(this);
            _headPoseDataProvider = new HeadPoseDataProvider();

            TrackWindow();
        }

        private void TrackWindow()
        {
            if (_gameViewBoundsProvider.Hwnd != IntPtr.Zero)
            {
                TobiiGameIntegrationApi.TrackWindow(_gameViewBoundsProvider.Hwnd);
                IsInitialized = true;
            }
        }

        void Update()
        {
            if (Time.frameCount == _lastUpdatedFrame) return;

            _lastUpdatedFrame = Time.frameCount;

            TrackWindow();

            var gameViewBounds = _gameViewBoundsProvider.GetGameViewClientAreaNormalizedBounds();
            _gameViewInfo = new GameViewInfo(gameViewBounds);

            TobiiGameIntegrationApi.Update();

            _gazeFocus.UpdateGazeFocus();
        }

        void OnDestroy()
        {
            Shutdown();
        }

        void OnApplicationQuit()
        {
            Shutdown();
        }

#if UNITY_EDITOR
        private static GameViewBoundsProvider CreateEditorScreenHelper()
        {
#if UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1
			return new LegacyEditorGameViewBoundsProvider();
#else
            return new EditorGameViewBoundsProvider();
#endif
        }
#endif
    }
}

#else
using Tobii.Gaming.Stubs;

namespace Tobii.Gaming.Internal
{
    internal partial class TobiiHost : TobiiHostStub
    {
        // all implementation in the stub
    }
}
#endif