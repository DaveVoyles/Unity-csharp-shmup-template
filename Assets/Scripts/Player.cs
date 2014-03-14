using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform  _playerTransform;         // for caching
    public float       PlayerSpeed;
    public float       FireRate;                 // time between shots
    public float       PlayerBulletSpeed;
    private float      _nextFire;                // used to time the next shot
    private float      _horizontalMovementLimit; // stops the player leaving the view
    private float      _verticalMovementLimit;

    void Start()
    {
        _playerTransform         = transform;    // caching the transform is faster than accessing 'transform' directly
        FireRate                 = (float) 0.025;
        _horizontalMovementLimit = 8;
        _verticalMovementLimit   = 6;
        _nextFire                = 0;
        PlayerBulletSpeed        = 25;
        PlayerSpeed              = 18;
    }

    void Update()
    {
        // read movement inputs
        var horizontalMove = (PlayerSpeed * Input.GetAxis("Horizontal"))    * Time.deltaTime;
        var verticalMove   = (PlayerSpeed * Input.GetAxis("Vertical"))      * Time.deltaTime;
        var moveVector     = new Vector3(horizontalMove, 0, verticalMove);
            moveVector     = Vector3.ClampMagnitude(moveVector, PlayerSpeed * Time.deltaTime); // prevents the player moving above its max speed on diagonals

        // move the player
        _playerTransform.Translate(moveVector);

        KeepPlayerInBounds();
        CheckIfShooting();
    }

    private void KeepPlayerInBounds()
    {
        // restrict the position to inside the player's movement limits
        _playerTransform.position = new Vector3(Mathf.Clamp(_playerTransform.position.x, -_horizontalMovementLimit, _horizontalMovementLimit), 0,
                                                Mathf.Clamp(_playerTransform.position.z, -_verticalMovementLimit,   _verticalMovementLimit));
    }

    private void CheckIfShooting()
    {
        // shooting
        if (Input.GetButton("Fire1") && Time.time > _nextFire)
        {
            // delay the next shot by the firing rate
            _nextFire = Time.time + FireRate;
            
            // get a bullet from the stack
            Bullet newBullet = GameManager.PlayerBulletStack.Pop();

            // position and enable it
            newBullet.gameObject.transform.position = _playerTransform.position;
            newBullet.gameObject.SetActive(true); 

            // set its speed (it moves in its own onUpdate function)
            newBullet.Velocity = new Vector3(0, 0, PlayerBulletSpeed);
        }

    }
    void OnTriggerEnter(Collider other) // must have hit an enemy or enemy bullet
    {
        GameManager.Lives--;                                         // lose a life            

        // check if it was a bullet we hit, if so put it back on its stack
        if (other.CompareTag("EnemyBullet"))
        {
            GameManager.EnemyBulletStack.Push(other.GetComponent<Bullet>());
            other.gameObject.SetActive(false);                       // deactivate the bullet
        }
        else if (other.CompareTag("Enemy"))                          // if it was an enemy, just destroy it
        {
            other.GetComponent<Enemy>().Explode();
        }
    }
}