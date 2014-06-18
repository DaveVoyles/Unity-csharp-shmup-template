#pragma strict

@script ExecuteInEditMode()

var laserMaxDistance : float = 100;			// How far the laser reaches (in Units)
var laserColor : Gradient;					// Color of laser (similar as lifetimeColor)
var particleCount : int = 1000;				// How many particles in the simulation
var collisionLayer : LayerMask = -1;		// The collision layers raycasting sees

private var particles : PlaygroundParticles;
private var previousParticleCount : int;

function Start () {
	particles = GetComponent(PlaygroundParticles);
	laserColor = particles.lifetimeColor;
	previousParticleCount = particleCount;
}

function Update () {

	// Send a Raycast from particle system's source transform forward
	var hit : RaycastHit;
	if (Physics.Raycast(particles.sourceTransform.position, particles.sourceTransform.forward, hit, laserMaxDistance, collisionLayer)) {
		
		// Set overflow offset z to hit distance (divide by particle count which by default is 1000)
		particles.overflowOffset.z = Vector3.Distance(particles.sourceTransform.position, hit.point)/(1+particles.particleCount);
		
	} else {
	
		// Render laser to laserMaxDistance on clear sight
		particles.overflowOffset.z = laserMaxDistance/(1+particles.particleCount);
	}
	
	// Update the amount of particles if particleCount changes
	if (particleCount!=previousParticleCount) {
		Playground.SetParticleCount(particles, particleCount);
		previousParticleCount = particleCount;
	}
	
	// Update the lifetimeColor if laserColor changes
	if (laserColor != particles.lifetimeColor)
		particles.lifetimeColor = laserColor;
}