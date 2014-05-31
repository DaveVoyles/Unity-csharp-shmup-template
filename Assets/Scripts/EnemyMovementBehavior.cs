/* Handles all of the behavior for the enemies. Uses delegates as event listeners, which 
 * tell the enemy how to perform, based on the given situation.
 * Uses iTween Events in the editor.
 * Use different delegates depending on the type of enemy this is.
 */
using PathologicalGames;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Targetable))]
public class EnemyMovementBehavior : MonoBehaviour {

    private Transform _playerXform = null;
    private float _fastMoveSpeed   = 2.5f;
    private float _slowMoveSpeed   = 4f;
    private enum CurrentState
    {
        Stopped,
        MoveToPlayerFast,
        MoveToPlayerSlow,
        Wander,
        Stalk
    };
    private CurrentState _currentState = CurrentState.Stopped;

    private void Awake()
    {
        _playerXform     = GameObject.Find("Player").transform;
        var targetable   = GetComponent<Targetable>();

        // Triggered once when this target is first detected by a TargetTracker 
        targetable.AddOnDetectedDelegate(MoveToPlayerQuickly);

        // Triggered once when this target is no longer detected by a TargetTracker
        targetable.AddOnNotDetectedDelegate(WanderAround);
    }
    

    /* Functions for event handlers 
     ************************************************/
    private void StopMovement(TargetTracker source){
        _currentState = CurrentState.Stopped;
    }
    private void WanderAround(TargetTracker source){
        _currentState = CurrentState.Wander;
    }
    private void MoveToPlayerSlowly(TargetTracker source){
        _currentState = CurrentState.MoveToPlayerSlow;
    }
    private void MoveToPlayerQuickly(TargetTracker source){
        _currentState = CurrentState.MoveToPlayerFast;
    }
    private void Stalk(TargetTracker source){
        _currentState = CurrentState.Stalk;
    }


    private void Update()
    {
        HandleMovementStates();
    }


    /// <summary>
    /// Enemy movement is based on the current state, using delegates. ie - stopped, MoveToPlayerFast, MoveToPlayerSlow
    /// </summary>
    private void HandleMovementStates()
    {

        if (_currentState == CurrentState.Stopped){
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
        if (_currentState == CurrentState.Wander){
            //TODO: How do I create wandering logic?
            print("Wandering");
        }
        if (_currentState == CurrentState.MoveToPlayerSlow){
            iTween.MoveUpdate(gameObject, _playerXform.position, _slowMoveSpeed);
        }
        if (_currentState == CurrentState.MoveToPlayerFast)
        {
            //   iTween.PunchScale(gameObject, new Vector3(2,2,2), _slowMoveSpeed);
            iTween.PunchScale(gameObject, iTween.Hash(
                "time", _slowMoveSpeed,
                "amount", new Vector3(2, 2, 2),
                "oncomplete", "DashToPlayer"
                ));

        }
        if (_currentState == CurrentState.Stalk){
            //TODO: Implement functionality 
        }
    }

    private void DashToPlayer()
    {
        iTween.MoveUpdate(gameObject, _playerXform.position, _fastMoveSpeed);
    }
}

