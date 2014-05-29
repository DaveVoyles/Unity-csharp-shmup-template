using System;
using PathologicalGames;
using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof (Transform))]
public class Enemy : MonoBehaviour
{
    public int       hitPoints; // assigned when the enemy spawns
    public Vector3   motion; // assigned when the enemy spawns
    public Transform deathExplosionPrefab; // particle prefab

    private Transform     _enemyTransform; // current transform of enemy, cached for perf during init
    private GameObject    _gameManager; // Need instance (NOT static) of GM to grab bullet from pool
    private SoundManager  _soundManager;
    private Transform     _enemyBulletPrefab;
    private String        _particlePool = "ParticlePool";
    private String        _bulletPool = "BulletPool";
    private SpawnPool     _spawnPool;
    private float         _bulletSpeed = -20f;  // neg, so that it goes from right to left

    private void Start()
    {
        _enemyTransform = this.transform; // cached for performance
        _gameManager = GameObject.Find("GameManager"); // store the game manager for accessing its functions
        _enemyBulletPrefab = PoolManager.Pools[_bulletPool].prefabs["EnemyBulletPrefab"];
    }

    private void Update()
    {
        _enemyTransform.position += (motion*Time.deltaTime); // move
    }


    /// <summary>
    /// subtract damage and check if it's dead
    /// </summary>
    /// <param name="damage">How much damage should be deducted from health?</param>
    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            this.Explode();
        }
    }

    /// <summary>
    /// Particles and sound effects when object is destroyed
    /// </summary>
    public void Explode()
    {
        // TODO: play sound
        var _particleInstance = PoolManager.Pools[_particlePool].Spawn(this.deathExplosionPrefab,
            this.transform.position, this.transform.rotation);
        // put this back on the stack for later re-use
        PoolManager.Pools[_bulletPool].Despawn(this.transform);
        // increment the score
        GameManager.score++;
        // Remove reference to particle effect after (x) seconds, otherwise it stays in the scene
        PoolManager.Pools[_particlePool].Despawn(_particleInstance, 3);
    }


    /// <summary>
    ///  waits for 'delay' seconds, then shoots directly at the player
    /// </summary>
    /// <param name="delay">Time between shots</param>
    public IEnumerator ShootTowardPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Grabs current instance of bullet
        Transform _bullletPrefab = PoolManager.Pools["BulletPool"].Spawn(this._enemyBulletPrefab,
                                                                         this._enemyTransform.position, Quaternion.identity);
        _bullletPrefab.rigidbody.velocity = new Vector3(_bulletSpeed,  0, 0);
    }
}