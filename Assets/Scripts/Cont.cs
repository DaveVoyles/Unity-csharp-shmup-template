
using UnityEngine;
using System.Collections;

public class Cont : MonoBehaviour {

    private float movementSpeed = 2.0f;
    private float rotationSpeed = 10.0f;
    private Transform target    = null;
    private CharacterController controller;
    private Transform   _playerXform;
    private GameManager _gameManager;

    void  Start (){
        controller   = GetComponent<CharacterController>();
        _playerXform = GameObject.Find("Player").transform;
    }
 
    void Update(){
        target = _playerXform;
        LookAtTarget();
        MoveToTarget();
    }
 
    void  LookAtTarget (){
        if (Time.timeScale <= 0 || target == null) { return; }

        Vector3 direction = target.position - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
 
    void  MoveToTarget (){
        if (Time.timeScale < 0 || target == null){ return; }

        var dir = (target.position - transform.position).normalized;
        controller.Move(dir * movementSpeed * Time.deltaTime);
    }
}