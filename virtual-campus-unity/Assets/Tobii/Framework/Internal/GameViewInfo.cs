//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

namespace Tobii.Gaming.Internal
{
	internal struct GameViewInfo
	{
		private readonly Rect _normalizedClientAreaBounds;

		public Rect NormalizedClientAreaBounds
		{
			get { return _normalizedClientAreaBounds; }
		}

		public static GameViewInfo DefaultGameViewInfo
		{
			get { return new GameViewInfo(new Rect(0f, 0f, 1f, 1f)); }
		}

		public GameViewInfo(Rect normalizedClientAreaBounds)
		{
			_normalizedClientAreaBounds = normalizedClientAreaBounds;
		}
	}
}