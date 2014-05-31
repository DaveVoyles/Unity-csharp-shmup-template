using System;
using PathologicalGames;
using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof (Transform))]
public class Enemy : MonoBehaviour
{
    public int       hitPoints = 10; 
    public Vector3   motionDir;               // assigned when the enemy spawns
    public Transform particlePrefab; // particle prefab

    private Transform     _xform; // current transform of enemy, cached for perf during init
    private SoundManager  _soundManager;
    private Transform     _bulletPrefab;
    private String        _particlePool = "ParticlePool";
    private String        _bulletPool = "BulletPool";
    private SpawnPool     _spawnPool;
    private float         _bulletSpeed = -20f;  // neg, so that it goes from right to left
    private Color         _startingColor;

    private void Start()
    {
        if (particlePrefab == null){
            print("One of your prefabs in" + " " + this.name + " " + "are null");
        }
        _xform         = transform; // cached for performance
        _bulletPrefab  = PoolManager.Pools[_bulletPool].prefabs["EnemyBulletPrefab"];
        _startingColor = renderer.material.color; 
    }

    private void Update()
    {
        _xform.position += (motionDir*Time.deltaTime); // move
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
        if (hitPoints <= 0)
        {
            Explode();
        }
    }

    /// <summary>
    /// Particles and sound effects when object is destroyed
    /// </summary>
    public void Explode()
    {
        // TODO: play sound
        var _particleInstance = PoolManager.Pools[_particlePool].Spawn(particlePrefab,
            transform.position, transform.rotation);
        // put this back on the stack for later re-use
        PoolManager.Pools[_bulletPool].Despawn(transform);
        // increment the score
        GameManager.score++;
        // Remove reference to particle effect after (x) seconds, otherwise it stays in the scene
        PoolManager.Pools[_particlePool].Despawn(_particleInstance, 3);
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
        Transform _bullletPrefab = PoolManager.Pools["BulletPool"].Spawn(_bulletPrefab,_xform.position, Quaternion.identity);
        _bullletPrefab.rigidbody.velocity = new Vector3(_bulletSpeed,  0, 0);
    }
}