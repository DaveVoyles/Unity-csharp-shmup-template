using System;
using PathologicalGames;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SoundManager _soundManager;
    private string _nameOfPool              = "BulletPool";   // References SpawnPool game object
    private float _nextFire                 = 0;              // used to time the next shot
    private Transform _playerTransform;                      // for caching
    private float _playerSpeed              = 18;
    private float _fireRate                 = (float)0.035;  // time between shots
    private float _playerBulletSpeed        = 25;

    public Transform playerBulletPrefab;
    public Transform playerMissilePrefab;
    public AudioClip sfxShoot;

    void Start()
    {
        _playerTransform         = transform;       // caching the transform is faster than accessing 'transform' directly
        _soundManager            = SoundManager.GetSingleton();
       // _playerBulletPrefab      = PoolManager.Pools["BulletPool"].prefabs["PlayerBulletPrefab"];  // Grabs a ref to prefab pool
    }

    void Update()
    {
        // read movement inputs
        var horizontalMove = (_playerSpeed * Input.GetAxis("Horizontal"))    * Time.deltaTime;
        var verticalMove   = (_playerSpeed * Input.GetAxis("Vertical"))      * Time.deltaTime;
        var moveVector = new Vector3(horizontalMove, 0, verticalMove);
        // prevents the player moving above its max speed on diagonals
        moveVector         = Vector3.ClampMagnitude(moveVector, _playerSpeed * Time.deltaTime); 

        // move the player
        _playerTransform.Translate(moveVector);

        // KeepPlayerInBounds();
        CheckIfShooting();
    }

    private void CheckIfShooting()
    {
        if (Input.GetButton("Fire1") && Time.time > _nextFire)
        {
            // delay the next shot by the firing rate
            _nextFire = Time.time + _fireRate;

            ShootBullets();
        }
        if (Input.GetButton("Fire2") && Time.time > _nextFire)
        {
            // delay the next shot by the firing rate
            _nextFire = Time.time + _fireRate;

            ShootMissiles();
        }
    }

    private void ShootBullets()
    {
        // Grabs current instance of bullet, by retrieving bullet prefab from spawn pool
        Transform _playerBulletInstance = PoolManager.Pools[_nameOfPool].Spawn(playerBulletPrefab);

        // position, enable, then set velocity
        _playerBulletInstance.gameObject.transform.position = _playerTransform.position;
        _playerBulletInstance.gameObject.SetActive(true);
        _playerBulletInstance.gameObject.GetComponent<Bullet>().velocity = new Vector3(_playerBulletSpeed, 0, 0);

        // _soundManager.PlayClip(sfxShoot, false);                      // play shooting SFX
    }

    private void ShootMissiles()
    {
        // Grabs current instance of bullet, by retrieving missile prefab from spawn pool
        Transform __playerMissileInstance = PoolManager.Pools[_nameOfPool].Spawn(playerMissilePrefab);

        // position, enable, then set velocity
   //     __playerMissileInstance.gameObject.transform.position = _playerTransform.position;
    //    __playerMissileInstance.gameObject.SetActive(true);
       // __playerMissileInstance.gameObject.GetComponent<Projectile>().velocity = new Vector3(playerBulletSpeed, 0, 0);
    }

    /// <summary>
    /// Check what we are colliding with
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // check if it was a bullet we hit, if so put it back on its stack
        if (other.CompareTag("EnemyBullet"))
        {
            other.gameObject.SetActive(false);       // deactivate the bullet
            GameManager.lives--;                     // lose a life      
        }
        // If it is a pickup, then spawn the correct option
        if (other.CompareTag("Pickup"))
        {
             other.gameObject.GetComponent<Pickup>().SpawnMainOption();
            // TODO: Add this back to the spawning pool, not deactivate it
            other.gameObject.SetActive(false);
        }
        // if it was an enemy, just destroy it
        if (other.CompareTag("Enemy")) 
        {
            other.GetComponent<Enemy>().Explode();  // Blow up enemy
            GameManager.lives--;                    // lose a life   
        }
    }
}