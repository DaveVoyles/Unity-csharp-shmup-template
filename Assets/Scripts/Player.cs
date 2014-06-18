using System;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    private SoundManager _soundManager              = null;        
    private float        _nextFire                  = 0;          
    private float        _playerSpeed               = 18f;
    private enum State                                { Playing, Explosion, Invincible }
    private State        _state                     = State.Playing;
    private Transform    _playerSpawnPoint          = null;        
    private float        _shipInvisibleTime         = 1.3f;
    private SpawnPool    _pool                      = null;
    private ParticleEffectsManager _particleManager = null;
    private const float DEFAULT_PLAYER_SPEED        = 18f;

    [HideInInspector] public Weapons weapons        = null;
    [HideInInspector] public Transform xform        = null;

    //-------------------------------------------------------------
    // Used for power ups
    public float GetPlayerSpeed()
    {
        return _playerSpeed;
    }

    public void SetPlayerSpeed(float playerSpeed)
    {
        _playerSpeed = playerSpeed;
    }


    void Start()
    {
        xform                     = transform;                                     
        _playerSpawnPoint         = GameObject.Find("PlayerSpawnPoint").transform; // set reference to Spawn Point Object
        xform.position            = _playerSpawnPoint.position;                    // Set player pos to spawnPoint pos
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

        //TODO: Debug for triggering the Game start. REMOVE ME
        if (Input.GetButtonDown("Jump"))
        {
            GameEventManager.TriggerGameStart();
        }
    }


    /// <summary>
    /// Checks for inputs from player handles player movement
    /// </summary>
    private void HandlePlayerMovement()
    { 
        var horizontalMove = (_playerSpeed * Input.GetAxis("Horizontal"))    * Time.deltaTime;
        var verticalMove   = (_playerSpeed * Input.GetAxis("Vertical"))      * Time.deltaTime;
        var moveVector     = new Vector3(horizontalMove, verticalMove, 0);
        // prevents the player moving above its max speed on diagonals
        moveVector         = Vector3.ClampMagnitude(moveVector, _playerSpeed * Time.deltaTime);
        moveVector         = Vector3.ClampMagnitude(moveVector, _playerSpeed * Time.deltaTime); 

        // move the player
        xform.Translate(moveVector);
    }


    /// <summary>
    /// Check for user input and switch weapons accordingly 
    /// </summary>
    private void CheckIfSwitchingWeapon()
    {
        if (Input.GetButtonDown("SwitchWeapon")){
            weapons.SwitchToNextWeapon();
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
            _nextFire = Time.time + weapons.GetFireRate();
            weapons.ShootWeapon();

        }
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
        if (_state != State.Playing) return;

        // Call enemy's Explode function for particles / sfx
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().Explode();
        }

        GameManager.lives--;
        StartCoroutine(OnBecameInvisible());
    }


    /// <summary>
    /// Spawns object at SpawnPoint, which is set in the editor
    /// </summary>
    private IEnumerator OnBecameInvisible()
    {
        // Set state and draw particles where player just died
        _state = State.Explosion;
        _particleManager.CreatePlayerExplosionEffects(xform.position);

        // Make player ship invisible 
        gameObject.renderer.enabled = false;
        // Move spawn point and set player location to that point 
        MoveSpawnPoint();

        yield return new WaitForSeconds(_shipInvisibleTime);

        if (GameManager.lives > 0)
        {
            // Remove any power ups, if they were applied
            ResetDefaultValues();


            xform.position = _playerSpawnPoint.position;

            // Set player to invincible while flashing & create particle effect at spawn point
            _state = State.Invincible;
            _particleManager.CreateSpawnEffects(xform.position); //TODO: Why is this called twice??

        
            // Make player ship visible again
            gameObject.renderer.enabled = true;

            // Make ship flash
            StartCoroutine(gameObject.GetComponent<FlashingObject>().Flash());

            yield return new WaitForSeconds(2.2f);
        }
        // Not flashing anymore? Now you can take hits
        _state = State.Playing;
    }


    /// <summary>
    /// Resets all player variables for power ups, upon player death
    /// </summary>
    private void ResetDefaultValues()
    {
        _playerSpeed = DEFAULT_PLAYER_SPEED;
        weapons.SetDefaultFireRate();
        weapons.SetDefaultBulletVel();
        weapons.SetDefaultBulletDmg();
    }


    /// <summary>
    /// Relocates spawn point to random location on the map, relative to the camera's position
    /// </summary>
    private void MoveSpawnPoint()
    {
        // Get relative position script and set X & Y offset values 
        var relativePos        = _playerSpawnPoint.GetComponent<ScreenRelativePosition>();
        relativePos.screenEdge = ScreenRelativePosition.ScreenEdge.RIGHT;
        relativePos.xOffset    = Random.Range(-25f, -10f);
        relativePos.yOffset    = Random.Range(-7f, 7f);

        // Move the camera, now that we have offsets
        relativePos.CalculatePosition();
    }

}





