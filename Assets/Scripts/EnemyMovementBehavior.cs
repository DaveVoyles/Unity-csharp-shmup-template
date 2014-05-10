using PathologicalGames;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Targetable))]
public class EnemyMovementBehavior : MonoBehaviour {

    private Transform _playerTransform = null;  
    private enum CurrentState
    {
        Stopped,
        MoveToPlayerQuickly,
        MoveToPlayerSlowly,
        Wander
    };
    private CurrentState _currentState = CurrentState.Stopped;


    private void Awake()

    {
        _playerTransform = GameObject.Find("Player").transform;
        var targetable   = this.GetComponent<Targetable>();

     //   targetable.AddOnDetectedDelegate(this.MoveToPlayerSlowly);
        targetable.AddOnDetectedDelegate(this.MoveToPlayerQuickly);

        targetable.AddOnNotDetectedDelegate(this.StopMovement);
     //   targetable.AddOnDetectedDelegate(this.Wander);
    }
    
    private void StopMovement(TargetTracker source)
    {
        _currentState = CurrentState.Stopped;
    }

    private void Wander(TargetTracker source)
    {

    }

    private void MoveToPlayerSlowly(TargetTracker source)
    {
        _currentState = CurrentState.MoveToPlayerSlowly;
    }

    private void MoveToPlayerQuickly(TargetTracker source)
    {
        _currentState = CurrentState.MoveToPlayerQuickly;
    }


    private void Update()
    {

        if (_currentState == CurrentState.Stopped)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        }
        if (_currentState == CurrentState.Wander)
        {

        }
        if(_currentState == CurrentState.MoveToPlayerSlowly)
        {
            const float speed = 4f;
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, step);
        }
        if (_currentState == CurrentState.MoveToPlayerQuickly)
        {
            const float speed = 8f;
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, step);
        } 
    }
}

