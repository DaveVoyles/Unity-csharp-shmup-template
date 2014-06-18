#pragma strict

// Example of scripted preset emitting in a circle on instantiation.
// To instantiate this during runtime as a preset use: Playground.InstantiatePreset("Playground Circle Shot (Script)");

@script ExecuteInEditMode()
@script RequireComponent (PlaygroundParticles)

var numberOfParticles : int = 30;                   // The number of particles to emit each cycle
var force : float = 10.0;                           // The force to emit in forward direction
var cycles : int = 1;								// The number of cycles to emit
var rotationNormal : Vector3 = Vector3(0,0,1);      // The axis you want to rotate around
var color : Color = Color.white;                    // The color of particle
var parent : Transform;                             // The parent to particle (can be null)
var yieldBeforeEmission : float = .0;				// The seconds to wait before starting emission
var yieldBetweenShots : float = .0;                 // The seconds between shots (if any)
var yieldBetweenCycles : float = .0;				// The seconds between cycles (if any)
var whenDone : WhenDoneCircleShot;					// Should this GameObject inactivate or destroy when emission is done?

private var thisTransform : Transform;
private var particles : PlaygroundParticles;

function Start () {
	particles = GetComponent(PlaygroundParticles);
	thisTransform = transform;
	Shoot();
}

function Shoot () {

	// Set variables
	var rotationSpeed : float = 360/numberOfParticles;
	var timeDone : float;
	
	// Set particle count to match the amount needed
	particles.particleCount = numberOfParticles*cycles;
	
	// Wait before emission starts (if applicable)
	if (yieldBeforeEmission>0) {
		timeDone = Playground.globalTime+yieldBeforeEmission;
		while (Playground.globalTime<timeDone)
			yield;
	}
	
	// Loop through every cycle (c) and particle (p)
	for (var c : int = 0; c<cycles; c++) {
	    for (var p : int = 0; p<numberOfParticles; p++) {
	    
	    	// Emit a particle in rotated direction
	    	particles.Emit(thisTransform.position, thisTransform.right*force, color, parent);
	        
	        // Rotate towards direction
	        thisTransform.Rotate(rotationNormal*rotationSpeed);
	        
	        // Wait for next emission (if applicable)
	        if (yieldBetweenShots>0) {
	        	timeDone = Playground.globalTime+yieldBetweenShots;
	            while (Playground.globalTime<timeDone)
	            	yield;
	        }     
		}
		
		// Wait for next cycle (if applicable)
		if (yieldBetweenCycles>0) {
			timeDone = Playground.globalTime+yieldBetweenCycles;
	    	while (Playground.globalTime<timeDone)
	    		yield;
	    }
	}
	
	// Return if not in Play Mode in Editor
	#if UNITY_EDITOR
		if (!EditorApplication.isPlaying)
			return;
	#endif
	
	// Wait for action when last particle's lifetime is over
	switch (whenDone) {
		case WhenDoneCircleShot.Inactivate:
			yield WaitForSeconds(particles.lifetime);
			gameObject.SetActive(false);
		break;
		case WhenDoneCircleShot.Destroy:
			yield WaitForSeconds(particles.lifetime);
			Destroy(gameObject);
		break;
	}
}

enum WhenDoneCircleShot {
	Nothing,
	Inactivate,
	Destroy,
}