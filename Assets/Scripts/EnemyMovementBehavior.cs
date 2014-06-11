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

    private Enemy _enemy;

    /// <summary>
    /// Initializes variables and set references to objects used by the script 
    /// </summary>
    private void Awake()
    {
        _playerXform     = GameObject.Find("Player").transform;
        var targetable   = GetComponent<Targetable>();

        // Figure out which enemy type we are during initialization
        _enemy = GetComponent<Enemy>();

        // Triggered once when this target is first detected by a TargetTracker 
        targetable.AddOnDetectedDelegate(CreatePathToFollow);

        // Triggered once when this target is no longer detected by a TargetTracker
    //    targetable.AddOnNotDetectedDelegate(CreatePathToFollow);
    }
   

    /// <summary>
    /// Movement behaviors are determined by the type of enemy it is.
    /// Enemy type is set by spawner during initialization
    /// </summary>
    private void MovementsBasedOnEnemyType()
    {
        switch (_enemy.enemyType)
        {
            case Enemy.EnemyType.Drone:
                break;
            case Enemy.EnemyType.Path:
                // TODO: Logic for path followers
                break;
            case Enemy.EnemyType.PathCreator:
                _currentState = CurrentState.CreatePath;
                break;
            case Enemy.EnemyType.Seeker:
                _currentState = CurrentState.MoveToPlayerSlow;
                break;
            case Enemy.EnemyType.Stationary:
                break;
            default:
                DebugUtils.Assert(false);
                break;
        }

    }




    //---------------------------------------------------------------------
    // Functions for event delegates via TargetTracker script

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


    /// <summary>
    /// Handles all movement and path updates
    /// </summary>
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

