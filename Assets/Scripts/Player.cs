using System;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    private SoundManager _soundManager;                       // reference to global sound manager  
    private float _nextFire                 = 0;              // used to time the next shot
    private Transform _playerTransform;                       // for caching
    private float _playerSpeed              = 18;
    private float _fireRate                 = (float)0.035;   // time between shots
    private float _playerBulletSpeed        = 25;
    private String _particlePool            = "ParticlePool";
    private String _bulletPool              = "BulletPool";
    private float _shipInvisibleTime        = 1.5f;
    private float _blinkRate                = .4f;
    private int _numberOfTimesToBlink       = 10;
    private int _blinkCount                 = 0;
    private enum state{ Playing, Explosion, Invincible }
    private state _state                    = state.Playing;

    private Transform _playerSpawnPoint;

    public Transform playerBulletPrefab;
    public Transform playerMissilePrefab;
    public AudioClip sfxShoot;
    public Transform deathExplosionPrefab;                     // particle prefab
    public Transform spawnParticlePrefab;                      // particle prefab

    void Start()
    {
        _playerTransform         = transform;       // caching the transform is faster than accessing 'transform' directly
        _soundManager            = SoundManager.GetSingleton();
        // set reference to Spawn Point (which is a child of Main Camera)
        _playerSpawnPoint = GameObject.Find("PlayerSpawnPoint").transform;
    }

    void Update()
    {
        // Is the player alive?
        if (_state != state.Explosion)
        {
            // read movement inputs
            var horizontalMove = (_playerSpeed*Input.GetAxis("Horizontal"))*Time.deltaTime;
            var verticalMove   = (_playerSpeed*Input.GetAxis("Vertical"  ))*Time.deltaTime;
            var moveVector     = new Vector3(horizontalMove, 0, verticalMove);
            // prevents the player moving above its max speed on diagonals
            moveVector         = Vector3.ClampMagnitude(moveVector, _playerSpeed*Time.deltaTime);

            // move the player
            _playerTransform.Translate(moveVector);

            CheckIfShooting();
        }
    }

    /// <summary>
    /// Is the player shooting? Left-click for bullets, right-click for missiles
    /// </summary>
    private void CheckIfShooting()
    {
        if (Input.GetButton("Fire1") && Time.time > _nextFire)
        {
            // delay the next shot by the firing rate
            _nextFire = Time.time + _fireRate;

            ShootBullets();
        }
    }

    private void ShootBullets()
    {
        // Grabs current instance of bullet, by retrieving bullet prefab from spawn pool
        Transform _playerBulletInstance = PoolManager.Pools[_bulletPool].Spawn(playerBulletPrefab);

        // position, enable, then set velocity
        _playerBulletInstance.gameObject.transform.position = _playerTransform.position;
        _playerBulletInstance.gameObject.SetActive(true);
        _playerBulletInstance.gameObject.GetComponent<Bullet>().velocity = new Vector3(_playerBulletSpeed, 0, 0);

        // _soundManager.PlayClip(sfxShoot, false);                      // play shooting SFX
    }

    private void ShootMissiles()
    {
        // Grabs current instance of bullet, by retrieving missile prefab from spawn pool
        Transform __playerMissileInstance = PoolManager.Pools[_bulletPool].Spawn(playerMissilePrefab);

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
        if (_state != state.Playing) return;

        // check if it was a bullet we hit, if so put it back on its stack
        if (other.CompareTag("EnemyBullet"))
        {
            // put the bullet back on the stack for later re-use
            PoolManager.Pools[_bulletPool].Despawn(other.transform);
            this.KillPlayer(other);
        }
        // If it is a pickup, then spawn the correct option
        if (other.CompareTag("Pickup"))
        {
            other.gameObject.GetComponent<Pickup>().SpawnMainOption();
            // TODO: Add this back to the spawning pool, not deactivate it
            other.gameObject.SetActive(false);
        }
        // if it was an enemy, just destroy it and kill the player
        if (other.CompareTag("Enemy"))
        {
            this.KillPlayer(other);
        }
    }

    /// <summary>
    /// Kill player, make invisible & invisible, spawn at spawn point, and create particles
    /// </summary>
    /// <param name="other"> Who are we colliding with?</param>
    private void KillPlayer(Collider other)
    {
        // Call enemy's Explode function for particles / sfx
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().Explode();
        }
            GameManager.lives--;                   // lose a life   
            StartCoroutine(this.OnBecameInvisible());
    }

    /// <summary>
    /// Particles and sound effects when object is destroyed
    /// </summary>
    public void Explode()
    {
        // TODO: play sound
       var _explosionInstance = PoolManager.Pools[_particlePool].Spawn(this.deathExplosionPrefab, this.transform.position, this.transform.rotation);
        // Shake Camera
        Camera.main.GetComponent<CameraShake>().Shake();
        // Despawn particle instance after 2 seconds
        PoolManager.Pools[_particlePool].Despawn(_explosionInstance, 2);
    }

    /// <summary>
    /// Spawns object at SpawnPoint, which is set in the editor.
    /// Main Camera -> SpawnPoint (child of Main Camera)
    /// </summary>
    private IEnumerator OnBecameInvisible()
    {
        _state = state.Explosion;
        this.Explode();
        this.gameObject.renderer.enabled = false;
        // Spawn player at spawn point
        this.transform.position = new Vector3(_playerSpawnPoint.position.x, _playerSpawnPoint.position.y,
                                               this.transform.position.z);
        yield return new WaitForSeconds(_shipInvisibleTime);

        if (GameManager.lives > 0)
        {
            print("Lives are > 0");
            // Create particle effect at spawn point
           var _particleInstance = PoolManager.Pools[_particlePool].Spawn(this.spawnParticlePrefab, this.transform.position, this.transform.rotation);
            // Start off with the player being displayed & make it invincible
            this.gameObject.renderer.enabled = true;
            this._state = state.Invincible;

            while (_blinkCount < _numberOfTimesToBlink)
            {
                // Flip back and forth between visible / invisible
                gameObject.renderer.enabled = !this.gameObject.renderer.enabled;

                if (this.gameObject.renderer.enabled == false)
                {
                    _blinkCount++;
                    print("Blink count is:" + "" + _blinkCount);
                    yield return new WaitForSeconds(_blinkRate);
                }

            }
            //this._blinkCount = 0;
            //this._state      = state.Playing;
            // Place particle back in pool after 2 seconds
            PoolManager.Pools[_particlePool].Despawn(_particleInstance, 2);
        }

    }


}

