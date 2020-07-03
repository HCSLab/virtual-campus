//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Rounds the positions of an particlesystem
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class SetBackground : MonoBehaviour
{
	private ParticleSystem _particleEmitter;

	void Start()
	{
		_particleEmitter = GetComponent<ParticleSystem>();
	}

	void Update()
	{
		var particles = new ParticleSystem.Particle[_particleEmitter.particleCount];
		int particleCount = _particleEmitter.GetParticles(particles);

		for (int i = 0; i < particleCount; i++)
		{
			particles[i].position = new Vector3(
				 Mathf.Round(particles[i].position.x),
				 Mathf.Round(particles[i].position.y),
				 Mathf.Round(particles[i].position.z));
		}

		_particleEmitter.SetParticles(particles, particles.Length);
	}
}
