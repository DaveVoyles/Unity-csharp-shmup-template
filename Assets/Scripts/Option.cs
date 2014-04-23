using System;
using UnityEngine;
using System.Collections;

public class Option : MonoBehaviour
{

    private Transform _playerTransform;
    private float     _distanceFromPlayer      = 2;
    private float     _horizontalMovementLimit = 17f;
    private float     _verticalMovementLimit   = 9.5f;
    private bool      _isFiringForward         = false;   
    private bool      _isReturningFromForward  = false;
    private Transform _currentTransform;
    private float     _nextFire                = 0;
    private float     _fireRate                = 2;
    private enum CurrentState
    {
        AttachedToFront,
        AttachedToBack,
        Idle,
        FiringForward,
        FiringBackward,
        ReturningFromForward,
        ReturningFromBackward,
    }
    private CurrentState _currentState;
     

	void Start ()
	{
        // Reference to player's position
	    _playerTransform = GameObject.Find("Player").transform;
        AttachedToFront();
	}
	
	void Update ()
	{
        // Constantly rotate
        this.transform.Rotate(new Vector3(0, 50, 0) * Time.deltaTime);
        KeepWithinScreenBounds();

	    if (Input.GetButton("FireOption_Main") && Time.time > _nextFire)
	    {
	        // delay the next button press by the firing rate
	        _nextFire = Time.time + _fireRate;

	        if (_currentState == CurrentState.AttachedToFront     )
	        {
                ShootOption();
	        }
	        if (_currentState == CurrentState.FiringForward       )
	        {
                ReturnFromForwrad();
	        }
	        if (_currentState == CurrentState.ReturningFromForward)
	        {
                AttachedToFront();
	        }
	        if (_currentState == CurrentState.Idle                )
	        {
                ReturnFromForwrad();
	        }
	    }

	    // What is the option currently doing?
        //switch (_currentState)
        //{
        //    case CurrentState.AttachedToFront:
        //        AttachedToFront();
        //        break;
        //    case CurrentState.AttachedToBack:
        //        break;
        //    case CurrentState.FiringBackward:
        //        break;
        //    case CurrentState.FiringForward:
        //        ShootOption();
        //        break;
        //    case CurrentState.ReturningFromBackward:
        //        break;
        //    case CurrentState.ReturningFromForward:
        //        ReturnFromForwrad();
        //        break;
        //    default:
        //        _currentState = CurrentState.AttachedToFront;
        //        AttachedToFront();
        //        break;
        //}
	}

    /// <summary>
    /// Used for physics operations
    /// </summary>
    void FixedUpdate()
    {
    }

    /// <summary>
    /// Shoots the option in front of the player
    /// </summary>
    void ShootOption()
    {
        const float pushForce = 250f;
        rigidbody.AddForce(Vector3.right * pushForce);
        Debug.Log("Shoot Option");
    }

    /// <summary>
    /// Returns option to the player
    /// </summary>
    void ReturnFromForwrad()
    {
        const float pushForce = 30f;
      //  rigidbody.AddForce(Vector3.left * pushForce);
        // Return to the player over a 3 second period
        iTween.MoveTo(this.gameObject, _playerTransform.position, 3);

        // If option is behind player, actually set it to pos in front of player
        Vector3 _inFrontOfPlayerPos     = _playerTransform.TransformPoint(Vector3.right * _distanceFromPlayer);
        if (this.transform.position.x < _inFrontOfPlayerPos.x)
        {
            this.transform.position = _inFrontOfPlayerPos;
            _currentState = CurrentState.AttachedToFront;
        }
        Debug.Log("Returning Option");
    }

    /// <summary>
    /// Keeps the option locked to the player's position
    /// </summary>
    void AttachedToFront()
    {
        Vector3 _inFrontOfPlayerPos = _playerTransform.TransformPoint(Vector3.right * _distanceFromPlayer);
        this.transform.position     = _inFrontOfPlayerPos;
        Debug.Log("Attached to front of player");
    }

    /// <summary>
    /// Keeps option within the bounds of the screen
    /// </summary>
    void KeepWithinScreenBounds()
    {
        this.transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, -_horizontalMovementLimit, _horizontalMovementLimit), 0,
                                              Mathf.Clamp(this.transform.position.z, -_verticalMovementLimit,   _verticalMovementLimit  ));
    }

    /// <summary>
    /// Events triggered when option hits another object
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // Stop applying force to the option
        if (other.CompareTag("BulletCollectors"))
        {
            rigidbody.velocity = Vector3.zero;
            _currentState = CurrentState.Idle;
            Debug.Log("I hit something -- Stopping movement");
        }

        // TODO: Apply damage to enemy if touching
        if (other.CompareTag("Enemy"))
        {
        }

    }

}
