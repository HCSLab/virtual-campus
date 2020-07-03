//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

public class SetCarCycleOffset : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
		GetComponent<Animator>().Play("CarDrive1", -1, Random.value);
	}
}
