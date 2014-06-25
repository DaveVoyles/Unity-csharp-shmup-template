/* Handles all of the behavior for enemies. Uses delegates as event listeners, which 
 * tell the enemy how to perform, based on the given situation.
 * 
 * DO NOT apply this script to enemies that follow a pre-set path OR swarm enemies
 * 
 * @Author: Dave Voyles - May 2014  
 */

using System.IO;
using PathologicalGames;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Targetable))]
public class EnemyMovementBehavior : MonoBehaviour
{
    [SerializeField]
    private Enemy       _enemy             = null;
    private Transform   _playerXform       = null;
    private const float FAST_MOVE_SPEED    = 5f;
    private const float MAX_ANGLE_OFFSET   = 20.0f; // Maximum angle offset for new point on paths
    private const float RUN_PATH_SPEED     = 1.5f;  // Speed to run the randomly generated paths
    private const int   PATH_LENGTH        = 10;
    private const float SEG_LENGTH         = 2.0f;
    private float       _pos               = 0.0f;
    private bool        _canSetNewPath     = true;
    private Vector3[]   _path              = null;
    private Transform   _xform             = null;
    private enum CurrentState
    {
        StopThenMoveUpdatePlayer,
        HeadTowardsPlayer,
        MoveToPlayerThenPath,
        CreatePath,
        WaitForPlayer,
        Stopped
    };
    private CurrentState _currentState     = CurrentState.StopThenMoveUpdatePlayer;

    // Used for constant movement toward player
    private float movementSpeed      = 4.0f;
    private float rotationSpeed      = 10.0f;
    private float _sawPlayerDistance = 8f;
    private Transform target         = null;
    private CharacterController controller;


    /// <summary>
    /// Initializes variables and set references to objects used by the script 
    /// </summary>
    private void Awake()
    {
        // MUST keep this reference here. W/ out it, enemies only chance player's starting pos 
        _playerXform     = GameObject.Find("Player").transform;
        _xform           = transform;
        controller       = GetComponent<CharacterController>();

        // Figure out which enemy type we are during initialization
        _enemy = GetComponent<Enemy>();

        // Enemies will move according to the type they were initialized as 
        SetMovementsBasedOnEnemyType();
    }


    //------------------------------------------------------------------------------------------------------
    // ------------------ Functions for event delegates via TargetTracker script ---------------------------


    /// <summary>
    /// Movement behaviors are determined by the type of enemy it is.
    /// Enemy type is set by spawner during initialization
    /// TODO: Set this in Awake()?
    /// </summary>
    private void SetMovementsBasedOnEnemyType()
    {
        switch (_enemy.enemyType)
        {
            case Enemy.EnemyType.Drone:
                // Ignore all functionality
                break;
            case Enemy.EnemyType.Path:
                // Ignore all functionality
                break;
            case Enemy.EnemyType.StationaryShooter:
                // Ignore all functionality
                break;
            case Enemy.EnemyType.PathCreator:
                _currentState = CurrentState.CreatePath;
                break;
            case Enemy.EnemyType.Seeker:
                _currentState = CurrentState.HeadTowardsPlayer;
                break;
            case Enemy.EnemyType.WaitForPlayer:
                _currentState = CurrentState.WaitForPlayer;
                break;
            default:
                DebugUtils.Assert(false);
                break;
        }
    }


    /// <summary>
    /// Enemy movement is based on the current state. 
    /// </summary>
    private void HandleMovementStates()
    {
        switch (_currentState)
        {
            case CurrentState.StopThenMoveUpdatePlayer:
                StartCoroutine(StopThenMoveUpdatePlayerFunc());
                break;
            case CurrentState.CreatePath:
                CreatePathFunc();
                break;
            case CurrentState.HeadTowardsPlayer:
                HeadTowardsPlayerFunc();
                break;
            case CurrentState.MoveToPlayerThenPath:
                StartCoroutine(MoveTolayerThenPathFunc()); 
                break;
            case CurrentState.WaitForPlayer:
                WaitForPlayerFunc();
                break;
            case CurrentState.Stopped:
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                break;
            default:
                DebugUtils.Assert(false);
                break;
        }
    }


    //------------------------------------------------------------------------------------------------------
    // -------------------------------------------- Movements ----------------------------------------------


    /// <summary>
    /// Handles all movement and path updates
    /// </summary>
    private void FixedUpdate()
    {
        HandleMovementStates();
        HandlePathMovement();
    }


    /// <summary>
    /// Rotates toward the current target (generally the player)
    /// </summary>
    private void LookAtTarget()
    {
        if (target == null) { return; }

        Vector3 direction     = target.position - transform.position;
        transform.rotation    = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }


    /// <summary>
    /// Moves directly toward the currently selected target
    /// </summary>
    private void MoveToTarget()
    {
        if (target == null) { return; }

        var dir = (target.position - transform.position).normalized;
        controller.Move(dir * movementSpeed * Time.deltaTime);
    }


    /// <summary>
    /// Sets target and rotation towards player, then moves to player
    /// </summary>
    private void HeadTowardsPlayerFunc()
    {
        target = _playerXform;
        LookAtTarget();
        MoveToTarget();
    }


    /// <summary>
    /// Wait for player to get close, then attack.
    /// Change the movement behavior of the enemy, based on the distance from player
    /// </summary>
    private void WaitForPlayerFunc()
    {
        // Get current distance from player 
        var distanceToPlayer = Vector3.Distance(_xform.position, _playerXform.position);

        if (distanceToPlayer < _sawPlayerDistance)
        {
            HeadTowardsPlayerFunc();
        }
        else if (distanceToPlayer >= _sawPlayerDistance)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }


    /// <summary>
    /// Stop & wait for a few moments, then go after the player 
    /// </summary>
    private IEnumerator StopThenMoveUpdatePlayerFunc()
    {
        // Stop and wait for a few moments
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        yield return new WaitForSeconds(1.5f);

        _currentState     = CurrentState.HeadTowardsPlayer;
    }


    /// <summary>
    /// Moves to the player, then when it arrives, creates a new path and follows that
    /// </summary>
    private IEnumerator MoveTolayerThenPathFunc()
    {
        iTween.MoveTo(gameObject, _playerXform.position, FAST_MOVE_SPEED);

        yield return new WaitForSeconds(2.5f);
        _currentState = CurrentState.CreatePath;
    }


    /// <summary>
    /// Debug code for drawing paths, if a path exists
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        if (_path != null){
            iTween.DrawPath(_path);
        }
    }


    //------------------------------------------------------------------------------------------------------
    // ----------------------------------------- Path Functions  -------------------------------------------


    /// <summary>
    /// Only used once, sets up the length of the first path for wandering enemies to follow
    /// </summary>
    private void CreatePathFunc()
    {
        // Prevents us from constantly generating new paths
        if (_canSetNewPath != true) return;

        // Set the path starting point
        _path = new Vector3[PATH_LENGTH];
        _path[_path.Length - 2] = Vector3.right;
        _path[_path.Length - 1] = new Vector3(-SEG_LENGTH, 0, 0);

        // Create a new path to follow
        CalculatePath();
    }


    /// <summary>
    /// Once a path has been created, attach the enemy to it, and then move it along the path
    /// </summary>
    private void HandlePathMovement()
    {
        // Don't call this function if we don't have a path to follow 
        if (_path == null) return;

        iTween.PutOnPath(gameObject, _path, _pos);
        var vector3 = iTween.PointOnPath(_path, Mathf.Clamp01(_pos + .01f));

        // Set rotation to the new path
        transform.LookAt(vector3);

        // Set movement speed of the path runner
        _pos = _pos + Time.deltaTime * RUN_PATH_SPEED / _path.Length;

        // If we've reached the end of the path, calculate a new one to follow
        if (!(_pos >= 1.0)) return;
        _canSetNewPath = true;
        _pos           -= 1.0f;
        CalculatePath();
    }


    /// <summary>
    /// Helper function to recalculate the paths for wandering enemies
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

        // We just made a new path, so don't create a new one until we reach the end of this one
        _canSetNewPath = false;
    }

}

