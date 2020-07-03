//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using Tobii.Gaming.Internal;
using UnityEngine;

namespace Tobii.Gaming.Stubs
{
	internal class TobiiHostStub : ITobiiHost
	{
		private static TobiiHostStub _instance;

		public IGazeFocus GazeFocus { get { return new GazeFocusStub(); } }
		public DisplayInfo DisplayInfo { get { return DisplayInfo.Invalid; } }
		public UserPresence UserPresence { get { return UserPresence.Unknown; } }
		public bool IsInitialized { get { return false; } }
		public bool IsConnected { get { return false; } }

		public GameViewInfo GameViewInfo
		{
			get { return new GameViewInfo(new Rect(float.NaN, float.NaN, float.NaN, float.NaN)); }
		}

		public int GetInstanceID() { return 0; }

		public static ITobiiHost GetInstance()
		{
			if (_instance == null)
			{
				_instance = new TobiiHostStub();
			}

			return _instance;
		}

		public void Shutdown() { /** no implementation **/ }

		internal GameViewInfo GetGameViewInfo()
		{
			return new GameViewInfo(new Rect(float.NaN, float.NaN, float.NaN, float.NaN));
		}

	    public IDataProvider<GazePoint> GetGazePointDataProvider() 
		{ 
			return new GazePointDataProviderStub(); 
		} 
	
		public IDataProvider<HeadPose> GetHeadPoseDataProvider() 
		{ 
			return new HeadPoseDataProviderStub(); 
		} 

		public static implicit operator bool(TobiiHostStub exists)
		{
			return null != exists;
		}
	}
}