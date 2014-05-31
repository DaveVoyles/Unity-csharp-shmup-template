using System;
using PathologicalGames;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class SpawnPoint : MonoBehaviour
{
    public Transform particleXform;                      // particle prefab
    public AudioClip sfxSpawning;
    public int numberOfEnemiesToSpawn = 10;
    public Transform enemyTypeXform;
    public float spawnInterval = 1.1f;

    private Transform _xform;                            // cache spawn point xform
    private String _particlePool = "ParticlePool";
    private string _nameOfPool   = "BulletPool";
    private SpawnPool _pool      = null;
    private int _spawnSphereRadius = 5;
    private Transform _playerXform;

	private void Start ()
    {
        CreateSpawnEffects();
        InstantiateEnemies();
	}

    private void Awake()
    {
        _pool = PoolManager.Pools[_nameOfPool];
        _xform = GameObject.Find("EnemySpawnPoint").transform;
        _playerXform = GameObject.Find("Player").transform;
    }


    /// <summary>
    /// Creates particles, de-spawns particles, and plays SFX for spawning
    /// </summary>
    private void CreateSpawnEffects()
    {
        // Create particle effect at spawn point, then put them back in the stack after (2) seconds
        var _particleInst = PoolManager.Pools[_particlePool].Spawn(particleXform, _xform.position,_xform.rotation);
        PoolManager.Pools[_particlePool].Despawn(_particleInst, 2);

        //TODO: _soundManager.PlayClip(sfxSpawning, false);                      
    }

    /// <summary>
    /// Spawns waves of enemies using enemyTypeXform at a set interval
    /// </summary>
    private IEnumerator SpawnEnemies()
    {
        // How many enemies should we spawn?
        int count = numberOfEnemiesToSpawn;
        while (count >= 0)
        {
            // Spawn an enemy & set spawn location within spawn radius
            var _enemyInstance  = _pool.Spawn(enemyTypeXform);
            var newPos          = Random.insideUnitSphere*_spawnSphereRadius;
            newPos.z            = _playerXform.position.z;  
            _enemyInstance.transform.position = newPos;
            count--;

            // call this function, every (x) seconds
            yield return new WaitForSeconds(spawnInterval);
        }
    }


    /// <summary>
    /// Spawn _number ofEnemies within the size of _spawnSphereRadius
    /// </summary>
    private void InstantiateEnemies()
    {
        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            var _enemyInstance  = _pool.Spawn(enemyTypeXform);
            var newPos          = Random.insideUnitSphere * _spawnSphereRadius;
            newPos.z            = _playerXform.position.z;
            _enemyInstance.transform.position = newPos;
        }
    }

}
