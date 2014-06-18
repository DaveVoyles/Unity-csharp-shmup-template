using UnityEngine;
using System;

[ExecuteInEditMode()]
[RequireComponent(typeof(PlaygroundParticlesC))]
public class PlaygroundPresetLaserC : MonoBehaviour {

	public float laserMaxDistance = 100f;			// How far the laser reaches (in Units)
	public Gradient laserColor;					// Color of laser (similar as lifetimeColor)
	public int particleCount = 1000;				// How many particles in the simulation
	public LayerMask collisionLayer = -1;			// The collision layers raycasting sees

	private PlaygroundParticlesC particles;
	private int previousParticleCount;

	void Start () {
		particles = GetComponent<PlaygroundParticlesC>();
		laserColor = particles.lifetimeColor;
		previousParticleCount = particleCount;
	}

	void Update () {

		// Send a Raycast from particle system's source transform forward
		RaycastHit hit;
		if (Physics.Raycast(particles.sourceTransform.position, particles.sourceTransform.forward, out hit, laserMaxDistance, collisionLayer)) {
			
			// Set overflow offset z to hit distance (divide by particle count which by default is 1000)
			particles.overflowOffset.z = Vector3.Distance(particles.sourceTransform.position, hit.point)/(1+particles.particleCount);
			
		} else {
		
			// Render laser to laserMaxDistance on clear sight
			particles.overflowOffset.z = laserMaxDistance/(1+particles.particleCount);
		}
		
		// Update the amount of particles if particleCount changes
		if (particleCount!=previousParticleCount) {
			PlaygroundC.SetParticleCount(particles, particleCount);
			previousParticleCount = particleCount;
		}
		
		// Update the lifetimeColor if laserColor changes
		if (laserColor != particles.lifetimeColor)
			particles.lifetimeColor = laserColor;
	}
}