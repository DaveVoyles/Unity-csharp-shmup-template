#pragma strict

var particles : PlaygroundParticles;	// The particles you want to enable / disable by distance to target
var target : Transform;					// The target that should enable / disable the particles
var distance : float = 10.0;			// The distance that should trigger enable / disable
	
function Update () {

	// Trigger emission when target is within distance
	particles.emit = (Vector3.Distance (target.position, particles.particleSystemTransform.position)<=distance);
}