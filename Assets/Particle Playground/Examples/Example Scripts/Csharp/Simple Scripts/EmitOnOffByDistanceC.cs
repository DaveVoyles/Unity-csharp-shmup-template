using UnityEngine;
using System.Collections;

public class EmitOnOffByDistanceC : MonoBehaviour {

	public PlaygroundParticlesC particles;	// The particles you want to enable / disable by distance to target
	public Transform target;				// The target that should enable / disable the particles
	public float distance = 10f;			// The distance that should trigger enable / disable
	
	void Update () {

		// Trigger emission when target is within distance
		particles.emit = (Vector3.Distance (target.position, particles.particleSystemTransform.position)<=distance);
	}
}
