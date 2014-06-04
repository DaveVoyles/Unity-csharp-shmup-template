using System;
using PathologicalGames;
using UnityEngine;
using System.Collections;

public class ParticleEffectsManager : MonoBehaviour {

    public Transform particleXform;                      // particle prefab
    public Transform playerExplosionXform;
    public AudioClip sfxSpawning;

    private const String PARTICLEPOOLSTRING = "ParticlePool";

    ///<summary>
    /// Creates particles, de-spawns particles, and plays SFX for spawning
    /// </summary>
    /// <param name="spawnLocation">Pass in the Vec3 loc where particles should begin</param>
    public void CreateSpawnEffects(Vector3 spawnLocation)
    {
        var pool         = PoolManager.Pools[PARTICLEPOOLSTRING];
        var particleInst = pool.Spawn(particleXform, spawnLocation, Quaternion.identity);
        pool.Despawn(particleInst, 2);    

        //TODO: _soundManager.PlayClip(sfxSpawning, false);                      
    }

    /// <summary>
    /// Creates particles, de-spawns particles, play SFX, and shake cam on player on death
    /// </summary>
    /// <param name="spawnLocation">Pass in the Vec3 loc where particles should begin</param>
    public void CreatePlayerExplosionEffects(Vector3 spawnLocation)
    {
        var pool         = PoolManager.Pools[PARTICLEPOOLSTRING];
        var particleInst = pool.Spawn(playerExplosionXform, spawnLocation, Quaternion.identity);
        pool.Despawn(particleInst, 2);
        Camera.main.GetComponent<CameraShake>().Shake();

        //TODO: _soundManager.PlayClip(sfxSpawning, false);                      
    }


    /// <summary>
    /// Creates explosive effects for enemies
    /// </summary>
    /// <param name="spawnLocation">Pass in the Vec3 loc where particles should begin</param>
    public void CreateExplodingEnemyEffects(Vector3 spawnLocation)
    {
        var pool         = PoolManager.Pools[PARTICLEPOOLSTRING];
        var particleInst = pool.Spawn(playerExplosionXform, spawnLocation, Quaternion.identity);
        pool.Despawn(particleInst, 2);

        //TODO: _soundManager.PlayClip(sfxSpawning, false);  
    }



}
