using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitalTwinCutScene2 : MonoBehaviour
{
    public Animator[] npcAnimators;
	public ParticleSystem[] particleSystems;
    public void OnNPCWalk()
	{
		foreach (var animator in npcAnimators)
		{
			animator.SetBool("Walk", true);
			animator.SetBool("Run", false);
		}
	}

	public void OnNPCIdle()
	{
		foreach (var animator in npcAnimators)
		{
			animator.SetBool("Walk", false);
			animator.SetBool("Run", false);
		}
	}

	public void OnNPCRun()
	{
		foreach (var animator in npcAnimators)
		{
			animator.SetBool("Walk", true);
			animator.SetBool("Run", true);
		}
	}

	public void OnFire()
	{
		foreach (var fire in particleSystems)
			fire.Play();
	}

	public void StopFire()
	{
		foreach (var fire in particleSystems)
			fire.Stop();
	}
}
