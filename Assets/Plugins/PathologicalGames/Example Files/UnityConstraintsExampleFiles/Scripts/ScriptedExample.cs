using UnityEngine;
using System.Collections;
using PathologicalGames;


/// <description>
///	Some examples using the API.
/// </description> 
public class ScriptedExample : MonoBehaviour
{
    public float moveSpeed = 1;
    public float turnSpeed = 1;
    public float newDirectionInterval = 3;

    // Cache
    private SmoothLookAtConstraint lookCns;
    private TransformConstraint xformCns;
    private Transform xform;
    
    private void Awake()
    {
        this.xform = this.transform;  // Cache
    
        // Add a transform constraint
        this.xformCns = this.gameObject.AddComponent<TransformConstraint>();
        this.xformCns.noTargetMode = UnityConstraints.NO_TARGET_OPTIONS.SetByScript;
        this.xformCns.constrainRotation = false;

        // Add a smooth lookAt constraint
        this.lookCns = this.gameObject.AddComponent<SmoothLookAtConstraint>();
        this.lookCns.noTargetMode = UnityConstraints.NO_TARGET_OPTIONS.SetByScript;
        this.lookCns.pointAxis = Vector3.up;
        this.lookCns.upAxis = Vector3.forward;
        this.lookCns.speed = this.turnSpeed;

        // Start some co-routines to illustrate SetByScript
        this.StartCoroutine(this.LookAtRandom());
        this.StartCoroutine(this.MoveRandom());
    }

    private IEnumerator MoveRandom()
    {
        // Wait one time for the other o-routine to start up
        yield return new WaitForSeconds(this.newDirectionInterval + 0.001f);

        while (true)
        {
            yield return null;

            // Lets do something a little tricky and move towards the position
            //   set in the lookat constraint. This will change when the other
            //   co-routine does.
            // Note this doesn't create a smooth-follow where the object takes a
            //   nice rounded path to the target position. That would be better
            //   accomplished by moving straght forward and letting the 
            //   SmoothLookAt constraint change the orientation over time.

            // Get a vector from here to the target position
            Vector3 targetDirection = this.lookCns.position - this.xform.position;
            Vector3 moveVect = targetDirection.normalized * this.moveSpeed * 0.1f;
            this.xformCns.position = this.xform.position + moveVect;

            Debug.DrawRay(this.xform.position, this.xform.up*2, Color.grey);
            Debug.DrawRay(this.xform.position, targetDirection.normalized * 2, Color.green);
        }
    }

    // Look in a different (random) direction every X seconds...
    private IEnumerator LookAtRandom()
    {
        while (true)
        {
            yield return new WaitForSeconds(this.newDirectionInterval);

            // Get a random position in a sphere volume
            //   *100 will set the result farther away for the other co-routine's use
            Vector3 randomPosition = Random.insideUnitSphere * 100;

            // Set the constraints internal target position
            //   Move the random result so it is based around this object
            this.lookCns.position = randomPosition + this.xform.position;
        }
    }
}


