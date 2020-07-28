using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavableMonoBehavior : MonoBehaviour
{
	protected virtual void Start()
	{
		EventCenter.AddListener(EventCenter.GlobalEvent.Save, Save);
	}

	protected virtual void Save(object data)
	{

	}

	protected virtual void OnDestroy()
	{
		Save(null);
		EventCenter.RemoveListener(EventCenter.GlobalEvent.Save, Save);
	}
}
