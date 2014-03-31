using System;
using PathologicalGames;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform    _playerTransform;         // for caching
    public float         PlayerSpeed;
    public float         FireRate;                 // time between shots
    public float         PlayerBulletSpeed;
    private float        _nextFire;                // used to time the next shot
    private float        _horizontalMovementLimit; // stops the player leaving the view
    private float        _verticalMovementLimit;
    public AudioClip     SFX_shoot;
    private SoundManager _soundManager;

    private Transform _playerBulletPrefab;
    private Transform _playerBulletInstance;

    void Start()
    {
        _playerTransform         = transform;       // caching the transform is faster than accessing 'transform' directly
        FireRate                 = (float) 0.035;
        _horizontalMovementLimit = 8;
        _verticalMovementLimit   = 6;
        _nextFire                = 0;
        PlayerBulletSpeed        = 25;
        PlayerSpeed              = 18;
        _soundManager            = SoundManager.GetSingleton();
        _playerBulletPrefab      = PoolManager.Pools["BulletPool"].prefabs["PlayerBulletPrefab"];  // Grabs a ref to prefab pool
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
                                                Mathf.Clamp(_playerTransform.position.z, -_verticalMovementLimit,   _verticalMovementLimit  ));
    }

    private void CheckIfShooting()
    {
        if (Input.GetButton("Fire1") && Time.time > _nextFire)
        {
            // delay the next shot by the firing rate
            _nextFire = Time.time + FireRate;

            ShootBullets();
        }
    }

    private void ShootBullets()
    {
        // Grabs current instance of bullet
        _playerBulletInstance = PoolManager.Pools["BulletPool"].Spawn(_playerBulletPrefab);

        // position, enable, then set velocity
        _playerBulletInstance.gameObject.transform.position = _playerTransform.position;
        _playerBulletInstance.gameObject.SetActive(true);
        _playerBulletInstance.gameObject.GetComponent<Bullet>().Velocity = new Vector3(PlayerBulletSpeed, 0, 0);

        _soundManager.PlayClip(SFX_shoot, false);    // play shooting SFX
    }

    void OnTriggerEnter(Collider other) // must have hit an enemy or enemy bullet
    {
        // check if it was a bullet we hit, if so put it back on its stack
        if (other.CompareTag("EnemyBullet"))
        {
     //       GameManager.EnemyBulletStack.Push(other.GetComponent<Bullet>());
            other.gameObject.SetActive(false);                       // deactivate the bullet
            GameManager.Lives--;                                     // lose a life      
        }
        else if (other.CompareTag("Enemy"))                          // if it was an enemy, just destroy it
        {
            other.GetComponent<Enemy>().Explode();
            GameManager.Lives--;                                     // lose a life   
        }
    }
}