using System;
using PathologicalGames;
using UnityEngine;
using System.Collections;

public class ParticleEffectsManager : MonoBehaviour {

    public Transform particleXform;                      // particle prefab
    public Transform playerExplosionXform;
    public AudioClip sfxSpawning;

    private String _particlePool  = "ParticlePool";
    private SpawnPool _pool       = null;

    private void Start()
    {
        _pool = PoolManager.Pools[_particlePool];
    }


    /// <summary>
    /// Creates particles, de-spawns particles, and plays SFX for spawning
    /// </summary>
    /// <param name="spawnLocation">Pass in the Vec3 loc where particles should begin</param>
    public void CreateSpawnEffects(Vector3 spawnLocation)
    {
        var _particleInst = PoolManager.Pools[_particlePool].Spawn(particleXform, spawnLocation, Quaternion.identity);
        PoolManager.Pools[_particlePool].Despawn(_particleInst, 2);

        //TODO: _soundManager.PlayClip(sfxSpawning, false);                      
    }

    /// <summary>
    /// Creates particles, de-spawns particles, play SFX, and shake cam on player on death
    /// </summary>
    /// <param name="spawnLocation">Pass in the Vec3 loc where particles should begin</param>
    public void CreatePlayerExplosionEffects(Vector3 spawnLocation)
    {
        var _particleInst = PoolManager.Pools[_particlePool].Spawn(playerExplosionXform, spawnLocation, Quaternion.identity);
        PoolManager.Pools[_particlePool].Despawn(_particleInst, 2);
        Camera.main.GetComponent<CameraShake>().Shake();

        //TODO: _soundManager.PlayClip(sfxSpawning, false);                      
    }


	
}
