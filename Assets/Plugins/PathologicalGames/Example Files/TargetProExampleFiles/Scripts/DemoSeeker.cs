using UnityEngine;
using System.Collections;
using PathologicalGames;


/// <summary>
///	Uses a Rigidbody to make this projectile seek a target.
/// </summary>
[RequireComponent(typeof(Projectile))]
[RequireComponent(typeof(SmoothLookAtConstraint))]
public class DemoSeeker : MonoBehaviour
{
    public float maxVelocity = 500;
    public float acceleration = 75;

    // Private Cache...
    private Transform xform;
    private Rigidbody rbd;
    private SmoothLookAtConstraint lookConstraint;
    private Projectile projectile;

    private float minDrag = 10;
    private float drag = 40;

    private void Awake()
    {
        this.xform = this.transform;
        this.rbd = this.rigidbody;
        this.lookConstraint = this.GetComponent<SmoothLookAtConstraint>();

        this.projectile = this.GetComponent<Projectile>();
        this.projectile.AddOnLaunchedDelegate(this.OnLaunched);
        this.projectile.AddOnLaunchedUpdateDelegate(this.OnLaunchedUpdate);
        this.projectile.AddOnDetonationDelegate(this.OnDetonateProjectile);
    }

    /// <summary>
    /// Runs when launched.
    /// </summary>
    private void OnLaunched()
    {
        // Reset the rigidbody state when launced. This is only needed when pooling.
        this.rbd.drag = this.drag;

        // This is a great place to start a fire trail effect. Try a UnityConstraint
        //  Transform constraint to attach it by making this.transform its target.
    }

    /// <summary>
    /// Runs each frame while the projectile is live
    /// </summary>
    private void OnLaunchedUpdate()
    {
        // If the target is active, fly to it, otherwise, continue straight 
        //   The constraint should be set to do nothing when there is no target
        // If there is no target, hit anything in the target layers.
        if (!this.projectile.target.isSpawned)  // Despawned
        {
            this.lookConstraint.target = null;
            this.projectile.detonationMode = Projectile.DETONATION_MODES.HitLayers;
        }
        else
        {
            this.lookConstraint.target = this.projectile.target.transform;
            this.projectile.detonationMode = Projectile.DETONATION_MODES.TargetOnly;
        }

        // Simulate acceleration by starting with a high drag and reducing it 
        //		until it reaches the target drag. Init drag in start().
        if (this.rbd.drag > this.minDrag)
            this.rbd.drag -= (this.acceleration * 0.01f);

        // Fly straight, constraint will handle rotation
        this.rbd.AddForce(this.xform.forward * this.maxVelocity);
    }


    /// <summary>
    /// A delegate run by the projectile component when this projectile detonates
    /// </summary>
    private void OnDetonateProjectile(TargetList targets)
    {
       // A great place for an explosion effect to be triggered!
    }
}