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
    private float        _nextFire                  = 0;          // used to time the next shot
    private Transform    _xform;
    private float        _playerSpeed               = 18f;
    private float        _fireRate                  = 0.035f;     // time between shots
    private enum State  { Playing, Explosion, Invincible }
    private State        _state                     = State.Playing;
    private float        _bulletVelX                = 40f;
    private int          _bulletDmg                 = 1;
    private const int    _spreadWeaponYoffset       = 10;
    private Transform    _playerSpawnPoint;                        // Finds spawn point in editor
    private float        _shipInvisibleTime         = 1.3f;
    private SpawnPool    _pool                      = null;
    private ParticleEffectsManager _particleManager = null;
    private const float DEFAULT_FIRE_RATE           = 0.035f;
    private const float DEFAULT_PLAYER_SPEED        = 18f;
    private const float DEFAULT_BULLET_VEL_X        = 40f;
    private const int   DEFAULT_BULLET_DMG          = 1;
    public Transform playerBulletPrefab; 
    public Transform playerMissilePrefab;
    public AudioClip sfxShoot;
    [HideInInspector]
    public Weapons weapons                          = null;


    void Start()
    {
        _xform                    = transform;                                     
        _playerSpawnPoint         = GameObject.Find("PlayerSpawnPoint").transform; // set reference to Spawn Point Object
        _xform.position           = _playerSpawnPoint.position;                    // Set player pos to spawnPoint pos
        _pool                     = GameObject.Find("GameManager").GetComponent<GameManager>().BulletPool;
        _particleManager          = GameObject.Find("ParticleManager").GetComponent<ParticleEffectsManager>();
        _soundManager             = SoundManager.GetSingleton();
        weapons                   = GetComponent<Weapons>();
    }

    void Update()
    {   
        // Is the player isn't alive, return
        if (_state == State.Explosion) return;

        HandlePlayerMovement();
        CheckIfShooting();
        CheckIfSwitchingWeapon();
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

    private void CheckIfSwitchingWeapon()
    {
        if (Input.GetButtonDown("SwitchWeapon"))
        {
            weapons.SwitchToNextWeapon();
            print("Switching weapon");
        }

    }

    /// <summary>
    /// Is the player shooting? Left-click for bullets, right-click for missiles
    /// </summary>
    private void CheckIfShooting()
    {
        if (Input.GetButton("Fire1") && Time.time > _nextFire && _state == State.Playing)
        {
            // delay the next shot by the firing rate
            _nextFire = Time.time + _fireRate;
           // ShootSpreadWeapon();
            weapons.ShootWeapon();

        }
    }

    /// <summary>
    /// Shoots one row of bullets in a straight line
    /// Grabs current instance of bullet, by retrieving bullet prefab from spawn pool
    /// </summary>
    public void ShootSingleShot()
    {
        var bulletInst                = _pool.Spawn(playerBulletPrefab, _xform.position, Quaternion.identity);
        bulletInst.rigidbody.velocity = new Vector3(_bulletVelX, 0, _xform.position.z);   

        // _soundManager.PlayClip(sfxShoot, false);                      // play shooting SFX
    }

    /// <summary>
    /// Shoots three bullets at once, like the spread weapon in Contra.
    /// Grabs current instance of bullet, by retrieving bullet prefab from spawn pool
    /// </summary>
    public void ShootSpreadWeapon()
    {
        var bulletInst                = _pool.Spawn(playerBulletPrefab, _xform.position, Quaternion.identity);
        bulletInst.rigidbody.velocity = new Vector3(_bulletVelX, 0 - _spreadWeaponYoffset, _xform.position.z);

        var bulletInst2                = _pool.Spawn(playerBulletPrefab, _xform.position, Quaternion.identity);
        bulletInst2.rigidbody.velocity = new Vector3(_bulletVelX, 0, _xform.position.z);

        var bulletInst3                = _pool.Spawn(playerBulletPrefab, _xform.position, Quaternion.identity);
        bulletInst3.rigidbody.velocity = new Vector3(_bulletVelX, 0 + _spreadWeaponYoffset, _xform.position.z);

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
        if (_state != State.Playing) return;    

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
        if (_state == State.Playing)
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
        _state = State.Explosion;

        _particleManager.CreatePlayerExplosionEffects(_xform.position);
        // Make player ship invisible & move player to PlayerSpawnPoint
        gameObject.renderer.enabled = false;
        _xform.position             = new Vector3(_playerSpawnPoint.position.x, _playerSpawnPoint.position.y, _xform.position.z);
        yield return new WaitForSeconds(_shipInvisibleTime);

        if (GameManager.lives > 0)
        {
            ResetDefaultValues();
            // Set player to invincible while flashing & create particle effect at spawn point
            _state = State.Invincible;;
            _particleManager.CreateSpawnEffects(_xform.position);

            // Make player ship visible again
            gameObject.renderer.enabled = true;

            // Make ship flash
            StartCoroutine(gameObject.GetComponent<FlashingObject>().Flash());

            yield return new WaitForSeconds(2.2f);
        }
        // Not flashing anymore? Now you can take hits
        _state = State.Playing;
    }



    //-------------------------------------------------
    // POWER-UPS    
    //------------------------------------------------
    public float GetFireRate()
    {
        return _fireRate;
    }

    public void SetFireRate(float fireRate)
    {
        _fireRate = fireRate;
    }

    public float GetPlayerSpeed()
    {
        return _playerSpeed;
    }

    public void SetPlayerSpeed(float playerSpeed)
    {
        _playerSpeed = playerSpeed;
    }

    public float GetBulletVelocity()
    {
        return _bulletVelX;
    }

    public void SetBulletVelocity(float bulletVelocity)
    {
        _bulletVelX = bulletVelocity;
    }

    public int GetBulletDmg()
    {
        return _bulletDmg;
    }

    public void SetBulletDmg(int bulletDmg)
    {
        _bulletDmg = bulletDmg;
    }

    /// <summary>
    /// Resets all player variables for powerups, upon death
    /// </summary>
    private void ResetDefaultValues()
    {
        _fireRate    = DEFAULT_FIRE_RATE;
        _playerSpeed = DEFAULT_PLAYER_SPEED;
        _bulletVelX  = DEFAULT_BULLET_VEL_X;
        _bulletDmg   = DEFAULT_BULLET_DMG;
    }




}





