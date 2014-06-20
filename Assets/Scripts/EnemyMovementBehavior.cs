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
public class EnemyMovementBehavior : MonoBehaviour
{

    [SerializeField] private Enemy       _enemy           = null;
    private Transform   _playerXform       = null;
    private const float FAST_MOVE_SPEED    = 4f;
    private const float MAX_ANGLE_OFFSET   = 20.0f; // Maximum angle offset for new point on paths
    private const float RUN_PATH_SPEED     = 1.5f;  // Speed to run the path
    private const int   PATH_LENGTH        = 10;
    private const float SEG_LENGTH         = 2.0f;
    private float       _pos               = 0.0f;
    private float       _sawPlayerDistance = 4f;
    private bool        _canSetNewPath     = true;
    private Vector3[]   _path              = null;
    private Transform   _xform             = null;
    private enum CurrentState
    {
        Stopped,
        MoveToPlayer,
        MoveUpdatePlayer,
        CreatePath,
    };
    private CurrentState _currentState     = CurrentState.Stopped;

    // Used for constant movement toward player
    private float movementSpeed = 2.0f;
    private float rotationSpeed = 10.0f;
    private Transform target    = null;
    private CharacterController controller;


    /// <summary>
    /// Initializes variables and set references to objects used by the script 
    /// </summary>
    private void Awake()
    {
        _xform           = transform;
        // MUST keep this reference here. W/ out it, enemies only chance player's starting pos 
        _playerXform     = GameObject.Find("Player").transform;
        var targetable   = GetComponent<Targetable>();
        controller       = GetComponent<CharacterController>();

        // Figure out which enemy type we are during initialization
        _enemy = GetComponent<Enemy>();

            targetable.AddOnDetectedDelegate(CreatePathToFollow);
        //targetable.AddOnNotDetectedDelegate(MoveToPlayerSlowly);

    }

    void LookAtTarget()
    {
        if (target == null) { return; }

        Vector3 direction     = target.position - transform.position;
        transform.rotation    = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    void MoveToTarget()
    {
        if (target == null) { return; }

        var dir = (target.position - transform.position).normalized;
        controller.Move(dir * movementSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Movement behaviors are determined by the type of enemy it is.
    /// Enemy type is set by spawner during initialization
    /// TODO: Set this in Awake()?
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
                _currentState = CurrentState.MoveUpdatePlayer;
                break;
            case Enemy.EnemyType.Stationary:
                break;
            default:
                DebugUtils.Assert(false);
                break;
        }
    }



    //------------------------------------------------------------------------------------------------------
    // ------------------ Functions for event delegates via TargetTracker script ---------------------------

    private void StopMovement(TargetTracker source){
        _currentState = CurrentState.Stopped;
    }
    private void CreatePathToFollow(TargetTracker source){
        _currentState = CurrentState.CreatePath;
    }
    private void MoveUpdatePlayer(TargetTracker source){
        _currentState = CurrentState.MoveUpdatePlayer;
    }
    private void MoveToPlayer(TargetTracker source){
        _currentState = CurrentState.MoveToPlayer;
    }

    /// <summary>
    /// Change the movement behavior of the enemy, based on the distance from player
    /// </summary>
    private void CheckStateChange()
    {
        // Get current distance from player 
        var distanceToPlayer = Vector3.Distance(_xform.position, _playerXform.position);

        if (distanceToPlayer < _sawPlayerDistance)
        {
            _currentState = CurrentState.MoveUpdatePlayer;
        }
        else
        {
            _currentState = CurrentState.CreatePath;
        }
    }


    /// <summary>
    /// Handles all movement and path updates
    /// </summary>
    private void FixedUpdate()
    {
        HandleMovementStates();
        HandlePathMovement();
        CheckStateChange();
    }



    /// <summary>
    /// Only used once, sets up the length of the first path for wandering enemies to follow
    /// </summary>
    private void SetPathStartingPoint()
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
            case CurrentState.Stopped:
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                print("stopping");
                break;
            case CurrentState.CreatePath:
                SetPathStartingPoint();
                gameObject.transform.localScale = (new Vector3(1, 1, 1));
                print("setting up a path");
                break;
            case CurrentState.MoveToPlayer:
                iTween.MoveTo(gameObject, _playerXform.position, FAST_MOVE_SPEED);
                print("Moving to player fast");
                break;
            case CurrentState.MoveUpdatePlayer:
                gameObject.transform.localScale = (new Vector3(3, 3, 3));
                target = _playerXform;
                LookAtTarget();
                MoveToTarget(); 
                print("moving to player slow");
                break;
            default:
                DebugUtils.Assert(false);
                break;
        }
    }
   

}

