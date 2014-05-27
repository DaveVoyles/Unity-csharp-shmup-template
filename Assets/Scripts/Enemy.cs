using System;
using PathologicalGames;
using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(Transform))]
public class Enemy : MonoBehaviour
{
    public int              hitPoints;						// assigned when the enemy spawns
    public Vector3          motion;							// assigned when the enemy spawns
    public Transform        deathExplosionPrefab;                 // particle prefab

    private Transform      _enemyTransform;                 // current transform of enemy, cached for perf during init
    private GameObject     _gameManager;                    // Need instance (NOT static) of GM to grab bullet from pool
    private float          _enemyBulletSpeed;
    private SoundManager   _soundManager;
    private Transform      _enemyBulletPrefab;
    private String         _particlePool = "ParticlePool";
    private String         _bulletPool   = "BulletPool";
    private SpawnPool     _spawnPool;

    void Start()
    {
        _enemyTransform    = transform;				         // cached for performance
        _enemyBulletSpeed  = 6;                              // How fast enemy bullets fly
        _gameManager       = GameObject.Find("GameManager"); // store the game manager for accessing its functions
        _enemyBulletPrefab = PoolManager.Pools[_bulletPool].prefabs["EnemyBulletPrefab"];
    }

    void Update()
    {
        _enemyTransform.position += (motion * Time.deltaTime); // move
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))			    // hit by a bullet
        {
            this.TakeDamage(1);								    // take away 1 hit point

            // put the bullet back on the stack for later re-use
            PoolManager.Pools[_bulletPool].Despawn(other.transform);
        }
    }

    /// <summary>
    /// subtract damage and check if it's dead
    /// </summary>
    /// <param name="damage">How much damage should be deducted from health?</param>
    void TakeDamage(int damage)
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
        // put enemy back on the stack for later re-use
        PoolManager.Pools[_bulletPool].Despawn(this.transform);
        // increment the score
        GameManager.score++;
        // Remove reference to particle effect, otherwise it stays in the scene
       
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
        Transform _enemyBulletInstance = PoolManager.Pools["BulletPool"].Spawn(_enemyBulletPrefab);

        //TODO: Do I even need this here? At least, the SetActive part?
        _enemyBulletInstance.gameObject.transform.position = _enemyTransform.position;
        _enemyBulletInstance.gameObject.SetActive(true);

        // calculate the direction to the player
        var shootVector = _gameManager.GetComponent<GameManager>().player.transform.position - _enemyTransform.position;

        // normalize this vector (make its length 1)
        shootVector.Normalize();

        // scale it up to the correct speed
        shootVector *= _enemyBulletSpeed;
        _enemyBulletInstance.gameObject.GetComponent<Bullet>().velocity = shootVector;
    }
}