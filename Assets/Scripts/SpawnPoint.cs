using System;
using PathologicalGames;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class SpawnPoint : MonoBehaviour
{
    public int numberOfEnemiesToSpawn    = 10;
    public Transform enemyTypeXform;
    public float IncrementSpawnInterval  = 1.1f;
    public bool isSpawning               = false;         // TODO: This is here for Debug -- remove for final build

    private int _spawnSphereRadius       = 5;
    private Transform _xform;                            
    private Transform _playerXform;
    private String _particlePool = "ParticlePool";
    private SpawnPool _pool      = null;


	private void Start ()
    {
	    if (!isSpawning) return;
        gameObject.GetComponent<ParticleEffectsManager>().CreateSpawnEffects(_xform.position);
	    SpawnEnemiesImmediately();
	    StartCoroutine(SpawnEnemiesIncrementally());
    }

    private void Awake()
    {
        _pool        = GameObject.Find("GameManager").transform.gameObject.GetComponent<GameManager>().BulletPool;
        _xform       = GameObject.Find("EnemySpawnPoint").transform;
        _playerXform = GameObject.Find("Player").transform;

    }

    /// <summary>
    /// Spawns waves of enemies using enemyTypeXform at a set interval
    /// </summary>
    private IEnumerator SpawnEnemiesIncrementally()
    {
        // How many enemies should we spawn?
        var count = numberOfEnemiesToSpawn;
        while (count >= 0)
        {
            // Spawn an enemy & set spawn location within spawn radius
            SpawnEnemies();
            count--;

            yield return new WaitForSeconds(IncrementSpawnInterval);
        }
    }

    /// <summary>
    /// Immediately Spawn _number ofEnemies within the size of _spawnSphereRadius
    /// </summary>
    private void SpawnEnemiesImmediately()
    {
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
