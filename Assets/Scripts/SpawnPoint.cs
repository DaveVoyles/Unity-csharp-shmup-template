using System;
using PathologicalGames;
using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{

    private Transform _spawnPointTransform;
    public Transform spawnParticlePrefab;                      // particle prefab
    public AudioClip sfxShoot;
    private String _particlePool = "ParticlePool";
   

	// Use this for initialization
	void Start () {
        _spawnPointTransform = GameObject.Find("EnemySpawnPoint").transform; 
	}

    private void Awake()
    {
        // Create particle effect at spawn point
        var _particleInstance = PoolManager.Pools[_particlePool].Spawn(this.spawnParticlePrefab,
            this.transform.position, this.transform.rotation);

        // put particles back in stack
        PoolManager.Pools[_particlePool].Despawn(_particleInstance, 2);

        // _soundManager.PlayClip(sfxShoot, false);                      // play shooting SFX
    }

    // Update is called once per frame
	void Update () {
	
	}
}
