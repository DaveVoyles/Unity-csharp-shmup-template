using System;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    private SoundManager _soundManager;                           // reference to global sound manager  
    private float        _nextFire                = 0;            // used to time the next shot
    private Transform    _xform;                                  // for caching
    private float        _playerSpeed             = 18;
    private float        _fireRate                = 0.035f;       // time between shots
    private float        _playerBulletSpeed       = 25;
    private enum state  { Playing, Explosion, Invincible }
    private state        _state                   = state.Playing;
    private const float _bulletVelX               = 40f;
    private const int   _spreadWeaponYoffset      = 10;
    private Transform   _playerSpawnPoint;                        // Finds spawn point in editor
    private float       _shipInvisibleTime        = 1.3f;
    private SpawnPool _bulletPool                 = null;

    public Transform playerBulletPrefab; 
    public Transform playerMissilePrefab;
    public AudioClip sfxShoot;
    public Transform deathExplosionPrefab;                        // particle prefab

    void Start()
    {
        _xform                    = transform;                                     // caching the transform is faster than accessing 'transform' directly
        _soundManager             = SoundManager.GetSingleton();
        _playerSpawnPoint         = GameObject.Find("PlayerSpawnPoint").transform; // set reference to Spawn Point (which is a child of Main Camera)
        _xform.position           = _playerSpawnPoint.position;                    // Set player pos to spawnPoint pos
        _bulletPool               = GameObject.Find("GameManager").transform.gameObject.GetComponent<GameManager>().BulletPool;
        gameObject.GetComponent<ParticleEffectsManager>().CreateSpawnEffects(_xform.position);
    }

    void Update()
    {   
        // Is the player isn't alive, return
        if (_state == state.Explosion) return;

        HandlePlayerMovement();
        CheckIfShooting();
    }

    /// <summary>
    /// Checks for inputs from player handles player movement
    /// </summary>
    private void HandlePlayerMovement()
    { 
        var horizontalMove = (_playerSpeed * Input.GetAxis("Horizontal"))    * Time.deltaTime;
        var verticalMove   = (_playerSpeed * Input.GetAxis("Vertical"))      * Time.deltaTime;
        var moveVector     = new Vector3(horizontalMove, 0, verticalMove);
        // prevents the player moving above its max speed on diagonals
        moveVector         = Vector3.ClampMagnitude(moveVector, _playerSpeed * Time.deltaTime);
        moveVector         = Vector3.ClampMagnitude(moveVector, _playerSpeed * Time.deltaTime); 

        // move the player
        _xform.Translate(moveVector);
    }

    /// <summary>
    /// Is the player shooting? Left-click for bullets, right-click for missiles
    /// </summary>
    private void CheckIfShooting()
    {
        if (Input.GetButton("Fire1") && Time.time > _nextFire && _state == state.Playing)
        {
            // delay the next shot by the firing rate
            _nextFire = Time.time + _fireRate;

          //  ShootBullets();
            ShootSpreadWeapon();
        }
    }

    /// <summary>
    /// Shoots one row of bullets in a straight line
    /// Grabs current instance of bullet, by retrieving bullet prefab from spawn pool
    /// </summary>
    private void ShootBullets()
    {
        Transform _bulletInst = _bulletPool.Spawn(playerBulletPrefab, _xform.position, Quaternion.identity);
        _bulletInst.rigidbody.velocity = new Vector3(_bulletVelX, transform.position.y, transform.position.z);   

        // _soundManager.PlayClip(sfxShoot, false);                      // play shooting SFX
    }

    /// <summary>
    /// Shoots three bullets at once, like the spread weapon in Contra.
    /// Grabs current instance of bullet, by retrieving bullet prefab from spawn pool
    /// </summary>
    private void ShootSpreadWeapon()
    {
        var bulletInst                = _bulletPool.Spawn(playerBulletPrefab, _xform.position, Quaternion.identity);
        bulletInst.rigidbody.velocity = new Vector3(_bulletVelX, _xform.position.y - _spreadWeaponYoffset, _xform.position.z);

        var bulletInst2                = _bulletPool.Spawn(playerBulletPrefab, _xform.position, Quaternion.identity);
        bulletInst2.rigidbody.velocity = new Vector3(_bulletVelX, _xform.position.y, _xform.position.z);

        var bulletInst3                = _bulletPool.Spawn(playerBulletPrefab, _xform.position, Quaternion.identity);
        bulletInst3.rigidbody.velocity = new Vector3(_bulletVelX, _xform.position.y + _spreadWeaponYoffset, _xform.position.z);

        // _soundManager.PlayClip(sfxShoot, false);                      // play shooting SFX
    }


    private void ShootMissiles()
    {
        // Grabs current instance of bullet, by retrieving missile prefab from spawn pool
//        Transform __playerMissileInstance = PoolManager.Pools[_bulletPool].Spawn(playerMissilePrefab);

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
        // Is the player doing anything but playing? Get out of here.
        if (_state != state.Playing) return;    

        // check if it was a bullet we hit, if so put it back on its stack
        if (other.CompareTag("EnemyBullet"))
        {
            // put the bullet back on the stack for later re-use
//            PoolManager.Pools[_bulletPool].Despawn(other.transform);
            KillPlayer(other);
        }
        // If it is a pickup, then spawn the correct option
        //if (other.CompareTag("Pickup"))
        //{
        //    other.gameObject.GetComponent<Pickup>().SpawnMainOption();
        //    // TODO: Add this back to the spawning pool, not deactivate it
        //    other.gameObject.SetActive(false);
        //}
        // if it was an enemy, just destroy it and kill the player
        if (other.CompareTag("Enemy"))
        {
            KillPlayer(other);
        }
    }

    /// <summary>
    /// Kill player, make invisible & invisible, spawn at spawn point, and create particles
    /// </summary>
    /// <param name="other"> Who are we colliding with?</param>
    public void KillPlayer(Collider other)
    {
        // If player isn't alive, then return
        if (_state == state.Playing)
        {
            // Call enemy's Explode function for particles / sfx
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<Enemy>().Explode();
            }
            GameManager.lives--;
            StartCoroutine(OnBecameInvisible());
        }
    }

    /// <summary>
    /// Spawns object at SpawnPoint, which is set in the editor.
    /// Main Camera -> SpawnPoint (child of Main Camera)
    /// </summary>
    private IEnumerator OnBecameInvisible()
    {
        _state = state.Explosion;

        gameObject.GetComponent<ParticleEffectsManager>().CreatePlayerExplosionEffects(_xform.position);
        // Make player ship invisible & move player to PlayerSpawnPoint
        gameObject.renderer.enabled = false;
        transform.position          = new Vector3(_playerSpawnPoint.position.x, _playerSpawnPoint.position.y, _xform.position.z);
        yield return new WaitForSeconds(_shipInvisibleTime);

        if (GameManager.lives > 0)
        {
            // Set player to invincible while flashing & create particle effect at spawn point
            _state = state.Invincible;;
            gameObject.GetComponent<ParticleEffectsManager>().CreateSpawnEffects(_xform.position);

            // Make player ship visible again
            gameObject.renderer.enabled = true;

            // Make ship flash
            StartCoroutine(gameObject.GetComponent<FlashingObject>().Flash());

            yield return new WaitForSeconds(2.2f);
        }
        // Not flashing anymore? Now you can take hits
        _state = state.Playing;
    }

}





