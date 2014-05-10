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
    private enum      CurrentState
    {
        Attached,
        NotAttached
    };
    private CurrentState _currentState;
    private bool         _isAttachedToFront   = true;
     

	void Start ()
	{
        // Reference to player's transform
	    _playerTransform = GameObject.Find("Player").transform;
        // Set the option's Z pos to that of the player's ship
        this.transform.position = _playerTransform.position;
	}
	
	void Update ()
	{
        // Constantly rotate
        this.transform.Rotate(new Vector3(0, 50, 0) * Time.deltaTime);

	    KeepWithinScreenBounds();
	    HandleInput();

	    if (_isAttachedToFront)
	    {
            AttachToFront();
        }
        else if (_isAttachedToFront == false)
        {
            // Set the option's Z pos to that of the player's ship
            this.transform.position = new Vector3(0, 0, _playerTransform.position.z);
        }

        // Set the option's Z pos to that of the player's ship
        this.transform.position = _playerTransform.position;


	}

    /// <summary>
    /// Changes current state of option, when user hits "FireOption_Main"
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetButton("FireOption_Main") && Time.time > _nextFire)
        {
            // delay the next button press by the firing rate
            _nextFire = Time.time + _fireRate;

            if (_currentState == CurrentState.Attached)
            {
                StartCoroutine(ShootOption());
            }
            if (_currentState == CurrentState.NotAttached)
            {
                StartCoroutine(AttachToFront());

            }
        }
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
    IEnumerator  ShootOption()
    {
        _isAttachedToFront    = false;
        const float pushForce = 700f;
        rigidbody.AddForce(Vector3.right * pushForce);
        Debug.Log("Shoot Option");

        yield return new WaitForSeconds(1f);
        _currentState = CurrentState.NotAttached;
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

        // Find the position just in front of the player
        Vector3 _inFrontOfPlayerPos = _playerTransform.TransformPoint(Vector3.right*_distanceFromPlayer);

        // If option is behind player, set it to pos in front of player
        if (this.transform.position.x < _inFrontOfPlayerPos.x)
        {
            this.transform.position = _inFrontOfPlayerPos;
            this._currentState      = CurrentState.Attached;
        }
        Debug.Log("Returning Option");
    }

    /// <summary>
    /// Keeps the option locked to the player's position
    /// </summary>
    IEnumerator AttachToFront()
    {
        // Keep a slight distance in front of player on X-Axis, but same on Y-Axis
        _isAttachedToFront = true;
        //const float pushForce = 100f;
        //rigidbody.AddForce(Vector3.left * pushForce);
        const float speed = 200;
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, step);
     //   Vector3 _inFrontOfPlayerPos = _playerTransform.TransformPoint(Vector3.right * _distanceFromPlayer);
     //   this.transform.position     = _inFrontOfPlayerPos;

        yield return new WaitForSeconds(1f);
        _currentState = CurrentState.Attached;
        Debug.Log("Attached to front of player");
    }

    /// <summary>
    /// Keeps option within the bounds of the screen
    /// TODO: Change this so it uses the barrier or camera collision, and not this hacked solution
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
            Debug.Log("I hit something -- Stopping movement");
        }

        // TODO: Apply damage to enemy if touching
        if (other.CompareTag("Enemy"))
        {
        }
    }

}
