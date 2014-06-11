using System;
using System.Security.Cryptography;
using PathologicalGames;
using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

[RequireComponent(typeof (Transform))]
public class Enemy : MonoBehaviour
{
    public int                    hitPoints = 8; 
    [HideInInspector]
    public Vector3                 motionDir;      // assigned when the enemy spawns
    public Transform               particlePrefab; // particle prefab
    public Transform               powerupXform;

    private Transform              _xform; // current transform of enemy, cached for perf during init
    private SoundManager           _soundManager;
    public Transform               _bulletXform;
    private ParticleEffectsManager _particleManager;
    private SpawnPool              _spawnPool;
    private float                  _bulletSpeed = -20f;  // neg, so that it goes from right to left
    private Color                  _startingColor;

    /// <summary> SpawnManager sets enemy type when spawning enemies </summary>
    public enum EnemyType
    {
        Drone,
        Path,
        PathCreator,
        Seeker,
        Stationary 
    }
    public EnemyType enemyType = EnemyType.Stationary;


    private void Start()
    {
        _xform           = transform; 
        _startingColor   = renderer.material.color; 
        _spawnPool       = GameObject.Find("GameManager").GetComponent<GameManager>().BulletPool;
        _particleManager = GameObject.Find("ParticleManager").GetComponent<ParticleEffectsManager>();
    }

    private void Update()
    {
        _xform.position += (motionDir*Time.deltaTime); 
    }


    /// <summary>
    /// subtract damage and check if it's dead
    /// </summary>
    /// <param name="damage">How much damage should be deducted from health?</param>
    public void TakeDamage(int damage)
    {
        // Make ship flash
        var flashScript = gameObject.GetComponent<FlashWhenHit>();
        StartCoroutine(flashScript.FlashWhite());

        hitPoints -= damage;
        if (hitPoints <= 0){
            Explode();
            CheckIfPowerupCanBeDropped();
        }
    }

    /// <summary>
    /// Particles and sound effects when object is destroyed
    /// </summary>
    public void Explode()
    {
        _particleManager.CreateExplodingEnemyEffects(_xform.position);

        // put this back on the stack for later re-use
        _spawnPool.Despawn(_xform);
        GameManager.score++;

        // Prevents enemy from re-spawning as white (stayed flashing on dead)
        renderer.material.color = _startingColor;
    }

    /// <summary>
    ///  waits for 'delay' seconds, then shoots directly at the player
    /// </summary>
    /// <param name="delay">Time between shots</param>
    public IEnumerator ShootTowardPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Grabs current instance of bullet
        var bullletPrefab                = _spawnPool.Spawn(_bulletXform,_xform.position, Quaternion.identity);
        bullletPrefab.rigidbody.velocity = new Vector3(_bulletSpeed,  0, 0);
    }


    /// <summary>
    /// Roll a random number to determine if power up can be dropped upon death
    /// TODO: Change drop rate based on game difficulty
    /// </summary>
    private void CheckIfPowerupCanBeDropped()
    {
        var randomNum = Random.Range(1, 10);
        print(randomNum);
        if (randomNum == 1)
        {
            SetPowerupType();
        }

    }

    /// <summary>
    /// Roll a random number to determine which powerup type will be dropped upon enemy death
    /// </summary>
    private void SetPowerupType()
    {
        var randomNum = Random.Range(1, 4);
        print(randomNum);

        switch (randomNum)
        {
            case 1:
                powerupXform.gameObject.GetComponent<PowerUp>().pickupType = PowerUp.PickupType.BulletVel;
                break;
            case 2:
                powerupXform.gameObject.GetComponent<PowerUp>().pickupType = PowerUp.PickupType.BulletDmg;
                break;
            case 3:
                powerupXform.gameObject.GetComponent<PowerUp>().pickupType = PowerUp.PickupType.FireRate;
                break;
            case 4:
                powerupXform.gameObject.GetComponent<PowerUp>().pickupType = PowerUp.PickupType.SpeedBoost;
                break;
            default:
                DebugUtils.Assert(false);
                break;
        }
    }
}