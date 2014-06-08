using UnityEngine;
using System.Collections;

/// <summary>
/// Creates wandering behaviour for a CharacterController.
/// https://gist.github.com/mminer/1337455
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Wander : MonoBehaviour
{
    public float speed = 5;
    public float directionChangeInterval = 1;
    public float maxHeadingChange = 30;

    CharacterController controller;
    float heading;
    Vector3 targetRotation;

    Vector3 forward
    {
        get { return transform.TransformDirection(Vector3.left); }
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Set random initial rotation
        heading = Random.Range(0, 360);
        transform.eulerAngles = new Vector3(0, heading, 0);

        StartCoroutine(NewHeadingRoutine());
    }

    void Update()
    {
        transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
        controller.SimpleMove(forward * speed);
    }   

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag != "BulletCollectors"){ return; }

        // Bounce off the wall using angle of reflection
        var newDirection = Vector3.Reflect(forward, hit.normal);
        transform.rotation = Quaternion.FromToRotation(Vector3.left, newDirection);
        heading = transform.eulerAngles.y;
        NewHeading();
    }

    /// <summary>
    /// Calculates a new direction to move towards.
    /// </summary>
    void NewHeading()
    {
        var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
        var ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        heading = Random.Range(floor, ceil);
        targetRotation = new Vector3(0, heading, 0);
    }

    /// <summary>
    /// Repeatedly calculates a new direction to move towards.
    /// Use this instead of MonoBehaviour.InvokeRepeating so that the interval can be changed at runtime.
    /// </summary>
    IEnumerator NewHeadingRoutine()
    {
        while (true)
        {
            NewHeading();
            yield return new WaitForSeconds(directionChangeInterval);
        }
    }


    /// <summary>
    /// Show some gizmos to provide a visual indication of what is happening: white => alignment, magenta => separation, blue => cohesion
    /// </summary>
    //protected virtual void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, neighborRadius);

    //    Gizmos.color = Color.white;
    //    Gizmos.DrawLine(transform.position, transform.position + alignment.normalized * neighborRadius);

    //    Gizmos.color = Color.magenta;
    //    Gizmos.DrawLine(transform.position, transform.position + separation.normalized * neighborRadius);

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(transform.position, transform.position + cohesion.normalized * neighborRadius);
    //}
}