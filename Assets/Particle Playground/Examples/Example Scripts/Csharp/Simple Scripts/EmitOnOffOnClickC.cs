using UnityEngine;
using System.Collections;

public class EmitOnOffOnClickC : MonoBehaviour {

	public PlaygroundParticlesC particles;	// Set the Particle Playground System through Inspector
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0))
			particles.Emit(!particles.emit);
	}
}
