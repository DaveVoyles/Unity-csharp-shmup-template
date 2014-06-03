/* http://robu3.tumblr.com/post/23187408273/flocking-algorithm-in-unity */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneBehavior : MonoBehaviour
{
    // the overall speed of the simulation
    public float speed             = 4f;
    // max speed any particular drone can move at
    public float maxSpeed          = 8f;
    // maximum steering power
    public float maxSteer          = 0.01f;

    // weights: used to modify the drone's movement
    public float separationWeight  = 1f;
    public float alignmentWeight   = 1f;
    public float cohesionWeight    = 1f;
    public float boundsWeight      = 8f;

    public float neighborRadius    = 4f;
    public float desiredSeparation = 1f;

    // velocity influences
    public Vector3 separation;
    public Vector3 alignment;
    public Vector3 cohesion;
    public Vector3 _bounds;

    // other members of my swarm
    public List<GameObject> drones;
    public SwarmBehavior    swarm;

    // Keep drones locked to player's Z position    
    private Transform _playerXform;


    private void Start()
    {
        _playerXform = GameObject.Find("Player").transform;
        StartCoroutine(StopMovementWhileSpawning());
    }

    /// <summary>
    /// Prevent drones from running after the player as soon as they spawn
    /// Give the player a moment to see the drones, then react
    /// </summary>
    private IEnumerator StopMovementWhileSpawning()
    {
        rigidbody.isKinematic = true;
        yield return new WaitForSeconds(0.7f);
        rigidbody.isKinematic = false;
    }

    void FixedUpdate()
    {
        if (rigidbody.isKinematic == false){
            // we should always apply physics forces in FixedUpdate
            Flock();
        }
    }

    public virtual void Flock()
    {
        var newVelocity = Vector3.zero;

        CalculateVelocities();

        newVelocity   += separation         * separationWeight;
        newVelocity   += alignment          * alignmentWeight;
        newVelocity   += cohesion           * cohesionWeight;
        newVelocity   += _bounds            * boundsWeight;
        newVelocity   =  newVelocity        * speed;
        newVelocity   =  rigidbody.velocity + newVelocity;
        newVelocity.z = 0f;

        rigidbody.velocity = Limit(newVelocity, maxSpeed);
    }

    /// <summary>
    /// Calculates the influence velocities for the drone. We do this in one big loop for efficiency.
    /// </summary>
    protected virtual void CalculateVelocities()
    {
        // the general procedure is that we add up velocities based on the neighbors in our radius for a particular influence (cohesion, separation, etc.) 
        // and divide the sum by the total number of drones in our neighbor radius
        // this produces an evened-out velocity that is aligned with its neighbors to apply to the target drone		
        var separationSum = Vector3.zero;
        var alignmentSum  = Vector3.zero;
        var cohesionSum   = Vector3.zero;
        var boundsSum     = Vector3.zero;

        int separationCount = 0;
        int alignmentCount  = 0;
        int cohesionCount   = 0;
        int boundsCount     = 0;

        for (int i = 0; i < this.drones.Count; i++)
        {
            if (drones[i] == null) continue;

            float distance = Vector3.Distance(transform.position, drones[i].transform.position);

            // separation
            // calculate separation influence velocity for this drone, based on its preference to keep distance between itself and neighboring drones
            // the desire of the drone to keep a minimum distance between itself and other drones
            if (distance > 0 && distance < desiredSeparation)
            {
                // calculate vector headed away from myself
                var direction = transform.position - drones[i].transform.position;
                direction.Normalize();
                direction         = direction / distance; // weight by distance
                separationSum     += direction;
                separationCount++;
            }

            // alignment & cohesion
            // Cohesion: the desire of the drone to be close to neighboring drones
            // Alignment: the desire of the drone to be facing the same direction (be aligned with) neighboring drones
            if (distance > 0 && distance < neighborRadius)
            {
                alignmentSum += drones[i].rigidbody.velocity;
                alignmentCount++;

                cohesionSum += drones[i].transform.position;
                cohesionCount++;
            }

            // bounds
            // calculate the bounds influence vector for this drone, based on whether or not neighboring drones are in bounds
            // the desire of the drone to stay within a particular area (added by me in my implementation, since I’m not doing screen wrapping)
            var bounds = new Bounds(swarm.transform.position, new Vector3(swarm.swarmBounds.x, 10000f, swarm.swarmBounds.y));
            if (distance > 0 && distance < neighborRadius && !bounds.Contains(drones[i].transform.position))
            {
                Vector3 diff = transform.position - swarm.transform.position;
                if (diff.magnitude > 0)
                {
                    boundsSum += swarm.transform.position;
                    boundsCount++;
                }
            }
        }

        // end
        separation = separationCount > 0 ? separationSum      / separationCount           : separationSum;
        alignment  = alignmentCount  > 0 ? Limit(alignmentSum / alignmentCount, maxSteer) : alignmentSum;
        cohesion   = cohesionCount   > 0 ? Steer(cohesionSum  / cohesionCount,  false   ) : cohesionSum;
        _bounds    = boundsCount     > 0 ? Steer(boundsSum    / boundsCount,    false   ) : boundsSum;
    }

    /// <summary>
    /// Returns a steering vector to move the drone towards the target
    /// </summary>
    /// <param type="Vector3" name="target"></param>
    /// <param type="bool" name="slowDown"></param>
    protected virtual Vector3 Steer(Vector3 target, bool slowDown)
    {
        // the steering vector
        var steer           = Vector3.zero;
        var targetDirection = target - transform.position;
        var targetDistance  = targetDirection.magnitude;

        transform.LookAt(target);

        if (targetDistance > 0)
        {
            // move towards the target
            targetDirection.Normalize();

            // we have two options for speed
            if (slowDown && targetDistance < 100f * speed)
            {
                targetDirection *= (maxSpeed * targetDistance / (100f * speed));
                targetDirection *= speed;
            }
            else
            {
                targetDirection *= maxSpeed;
            }

            // set steering vector
            steer = targetDirection - rigidbody.velocity;
            steer = Limit(steer, maxSteer);
        }
        return steer;
    }

    /// <summary>
    /// Limit the magnitude of a vector to the specified max
    /// </summary>
    /// <param type="Vector3" name="v"></param>
    /// <param type="float" name="max"></param>
    protected virtual Vector3 Limit(Vector3 v, float max)
    {
        if (v.magnitude > max){
            return v.normalized * max;
        }
        return v;
    }

    /// <summary>
    /// Show some gizmos to provide a visual indication of what is happening: white => alignment, magenta => separation, blue => cohesion
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, neighborRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + alignment.normalized  * neighborRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + separation.normalized * neighborRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + cohesion.normalized   * neighborRadius);
    }
}