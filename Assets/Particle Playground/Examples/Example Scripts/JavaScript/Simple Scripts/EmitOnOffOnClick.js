#pragma strict

 var particles : PlaygroundParticles;	// Set the Particle Playground System through Inspector

function Update () {
	if (Input.GetMouseButtonDown (0))
		particles.Emit(!particles.emit);
}