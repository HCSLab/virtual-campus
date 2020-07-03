//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

namespace Tobii.Gaming.Internal
{
	public struct FocusedObject
	{
		private GameObject _gameObject;

		public FocusedObject(GameObject gameObject)
		{
			_gameObject = gameObject;
		}

		public static FocusedObject Invalid
		{
			get { return new FocusedObject(null); }
		}

		public bool IsValid
		{
			get { return null != GameObject; }
		}

		public int Key
		{
			get { return GameObject.GetInstanceID(); }
		}

		public GameObject GameObject
		{
			get { return _gameObject; }
			private set { _gameObject = value; }
		}

		public bool Equals(FocusedObject that)
		{
			if (this.IsValid && that.IsValid &&
				this.GameObject.GetInstanceID() == that.GameObject.GetInstanceID())
			{
				return true;
			}

			if (!this.IsValid && !that.IsValid)
			{
				return true;
			}

			return false;
		}
	}
}