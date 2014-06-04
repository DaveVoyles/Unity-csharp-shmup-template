using System;
using PathologicalGames;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class SpawnPoint : MonoBehaviour
{
    public int numberOfEnemiesToSpawn               = 10;
    public Transform enemyTypeXform;
    public float incrementSpawnInterval             = 1.1f;

    private int _spawnSphereRadius                  = 5;
    private Transform _xform;                            
    private Transform _playerXform;
    private ParticleEffectsManager _particleManager = null;
    private SpawnPool _pool                         = null;


	private void Start ()
    {
	    SpawnEnemiesImmediately();  
	    StartCoroutine(SpawnEnemiesIncrementally());
    }

    /// <summary>
    /// Initialize all private properties
    /// </summary>
    private void Awake()
    {
        _pool            = GameObject.Find("GameManager").GetComponent<GameManager>().BulletPool;
        _xform           = GameObject.Find("EnemySpawnPoint").transform;
        _playerXform     = GameObject.Find("Player").transform;
        _particleManager = GameObject.Find("ParticleManager").GetComponent<ParticleEffectsManager>();
    }

    /// <summary>
    /// Spawns waves of enemies using enemyTypeXform at a set interval
    /// </summary>
    private IEnumerator SpawnEnemiesIncrementally()
    {
        _particleManager.CreateSpawnEffects(_xform.position);
        print(_particleManager);
        // How many enemies should we spawn?
        var count = numberOfEnemiesToSpawn;
        while (count >= 0)
        {
            // Spawn an enemy & set spawn location within spawn radius
            SpawnEnemies();
            count--;

            yield return new WaitForSeconds(incrementSpawnInterval);
        }
    }

    /// <summary>
    /// Immediately Spawn _number ofEnemies within the size of _spawnSphereRadius
    /// </summary>
    private void SpawnEnemiesImmediately()
    {
        _particleManager.CreateSpawnEffects(_xform.position);
        print(_particleManager);
        for (var i = 0; i < numberOfEnemiesToSpawn; i++){
            SpawnEnemies();
        }
    }

    /// <summary>
    /// Spawns enemies at a given position 
    /// </summary>
    private void SpawnEnemies()
    {
        var enemyInstance                = _pool.Spawn(enemyTypeXform);
        var newPos                       = Random.insideUnitSphere * _spawnSphereRadius;
        newPos.z                         = _playerXform.position.z;
        enemyInstance.transform.position = newPos;
    }

}
