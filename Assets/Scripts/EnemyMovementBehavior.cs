/* Handles all of the behavior for the enemies. Uses delegates as event listeners, which 
 * tell the enemy how to perform, based on the given situation.
 * Uses iTween Events in the editor.
 * Use different delegates depending on the type of enemy this is.
 * Recalc math came from: 
 */
using PathologicalGames;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Targetable))]
public class EnemyMovementBehavior : MonoBehaviour {

    private Transform   _playerXform     = null;
    private const float FAST_MOVE_SPEED  = 2.5f;
    private const float SLOW_MOVE_SPEED  = 8f;
    private const float MAX_ANGLE_OFFSET = 20.0f; // Maximum angle offset for new point on paths
    private const float RUN_PATH_SPEED   = 1.5f;  // Speed to run the path
    private const int   PATH_LENGTH      = 10;
    private const float SEG_LENGTH       = 2.0f;
    private float       _pos             = 0.0f;
    private Vector3[]   _path            = null;
    private enum CurrentState
    {
        Stopped,
        MoveToPlayerFast,
        MoveToPlayerSlow,
        CreatePath,
    };
    private CurrentState _currentState = CurrentState.Stopped;




    private void Awake()
    {
        _playerXform     = GameObject.Find("Player").transform;
        var targetable   = GetComponent<Targetable>();

        // Triggered once when this target is first detected by a TargetTracker 
        targetable.AddOnDetectedDelegate(CreatePathToFollow);

        // Triggered once when this target is no longer detected by a TargetTracker
    //    targetable.AddOnNotDetectedDelegate(CreatePathToFollow);
    }
    


    //---------------------------------------------------------------------
    // Functions for event handlers 
    private void StopMovement(TargetTracker source){
        _currentState = CurrentState.Stopped;
    }
    private void CreatePathToFollow(TargetTracker source){
        _currentState = CurrentState.CreatePath;
    }
    private void MoveToPlayerSlowly(TargetTracker source){
        _currentState = CurrentState.MoveToPlayerSlow;
    }
    private void MoveToPlayerQuickly(TargetTracker source){
        _currentState = CurrentState.MoveToPlayerFast;
    }


    private void Update()
    {
        HandleMovementStates();
        HandlePathMovement();
    }


    /// <summary>
    /// Move enemy along randomly generated paths
    /// </summary>
    private void HandlePathMovement()
    {
        iTween.PutOnPath(gameObject, _path, _pos);
        var vector3 = iTween.PointOnPath(_path, Mathf.Clamp01(_pos + .01f));

        // Set rotation to the new path
        transform.LookAt(vector3);

        // Set movement speed of the path runner
        _pos = _pos + Time.deltaTime * RUN_PATH_SPEED / _path.Length;

        // If we've reached the end of the path, calculate a new one to follow
        if (!(_pos >= 1.0)) return;
        _pos -= 1.0f;
        CalculatePath();
    }


    /// <summary>
    /// Set sup the length of the paths for wandering enemies to follow
    /// </summary>
    private void SetUpPaths()
    {
        _path                   = new Vector3[PATH_LENGTH];
        _path[_path.Length - 2] = Vector3.right;
        _path[_path.Length - 1] = new Vector3(-SEG_LENGTH, 0, 0);

        // Create a new path to follow
        CalculatePath();
    }


    /// <summary>
    /// Recalculates the paths for wandering enemies
    /// </summary>
    private void CalculatePath()
    {
        var vector3  = _path[_path.Length - 1] - _path[_path.Length - 2];
            _path[0] = _path[_path.Length - 1];

        for (var i = 1; i < _path.Length; i++)
        {
            // Create a new angle to follow, based on the min and max offset, & lock enemy on Z axis
            var q    = Quaternion.AngleAxis(Random.Range(-MAX_ANGLE_OFFSET, MAX_ANGLE_OFFSET), new Vector3(0, 0, 1));
            vector3  = q * vector3;
            _path[i] = _path[i - 1] + vector3;
        }
    }


    /// <summary>
    /// Debug code for drawing paths
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        iTween.DrawPath(_path);
    }

    
    /// <summary>
    /// Enemy movement is based on the current state, using delegates. ie - stopped, MoveToPlayerFast, MoveToPlayerSlow
    /// </summary>
    private void HandleMovementStates()
    {
        switch (_currentState)
        {
            case  CurrentState.Stopped:
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                break;
            case CurrentState.CreatePath:
                SetUpPaths();
                break;
            case CurrentState.MoveToPlayerFast:
                iTween.MoveUpdate(gameObject, _playerXform.position, FAST_MOVE_SPEED);
                break;
            case CurrentState.MoveToPlayerSlow:
                iTween.MoveUpdate(gameObject, _playerXform.position, SLOW_MOVE_SPEED);
                break;
            default:
                DebugUtils.Assert(false);
                break;
        }
    }
   

}

