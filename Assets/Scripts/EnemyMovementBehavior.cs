/* Handles all of the behavior for the enemies. Uses delegates as event listeners, which 
 * tell the enemy how to perform, based on the given situation.
 * Uses iTween Events in the editor.
 * Use different delegates depending on the type of enemy this is.
 */
using PathologicalGames;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Targetable))]
[RequireComponent(typeof(iTweenEvent))]
public class EnemyMovementBehavior : MonoBehaviour {

    private Transform _playerTransform = null;  
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
        _playerTransform = GameObject.Find("Player").transform;
        var targetable   = this.GetComponent<Targetable>();

        // Triggered once when this target is first detected by a TargetTracker .
        targetable.AddOnDetectedDelegate(this.MoveToPlayerQuickly);

        // Triggered once when this target is no longer detected by a TargetTracker.
        targetable.AddOnNotDetectedDelegate(this.WanderAround);
    }
    

    /* Functions for event handlers 
     ************************************************/
    private void StopMovement(TargetTracker source)
    {
        _currentState = CurrentState.Stopped;
    }
    private void WanderAround(TargetTracker source)
    {
        _currentState = CurrentState.Wander;
    }
    private void MoveToPlayerSlowly(TargetTracker source)
    {
        _currentState = CurrentState.MoveToPlayerSlow;
    }
    private void MoveToPlayerQuickly(TargetTracker source)
    {
        _currentState = CurrentState.MoveToPlayerFast;
    }
    private void Stalk(TargetTracker source)
    {
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

        if (_currentState == CurrentState.Stopped)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        }
        if (_currentState == CurrentState.Wander)
        {
            //TODO: How do I create wandering logic?
            print("Wandering");
        }
        if (_currentState == CurrentState.MoveToPlayerSlow)
        {
            
            // Moves after the player quickly, using the "MoveToPlayerSlow" iTween event (attached)
            iTweenEvent.GetEvent(gameObject, "MoveToPlayerSlow").Play();
            print("Move To player Slow");
        }
        if (_currentState == CurrentState.MoveToPlayerFast)
        {
            // Moves after the player quickly, using the "MoveToPlayer" iTween event (attached)
            iTweenEvent.GetEvent(gameObject, "MoveToPlayerFast").Play();
            print("Move To Player Fast");
        }
        if (_currentState == CurrentState.Stalk)
        {

        }
    }
}

