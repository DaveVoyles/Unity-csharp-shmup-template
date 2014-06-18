/* The Game Manager is an invisible Game Object which manages generic stuff like 
 * keeping track of bullets, spawning enemies, scoring, the GUI etc...
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using PathologicalGames;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // Keep globals here, so that other classes (enemy, etc.) can have one point of reference
    public Player        player;
    public AudioClip     backgroundMusic;

    [HideInInspector]
    public static int    score = 0;
    [HideInInspector]
    public static int    lives = 4;          
    /// <summary> Globally accessible BulletPool for all game objects to reference </summary>
    [HideInInspector]
    public SpawnPool     BulletPool;
    /// <summary> Globally accessible Particle for all game objects to reference   </summary>
    [HideInInspector]
    public SpawnPool     ParticlePool;

    private string       _bulletPoolString   = "BulletPool";
    private string       _ParticlePoolString = "ParticlePool";
    private int          _respawnTime        = 3;
    private SoundManager _soundManager;


    static GameObject GMGameObj;

    /// <summary>
    /// Sets up a static singleton instance of GameManager, which is accessible to everything
    /// </summary>
    /// <returns>GameMananger object</returns>
    public static GameManager GetSingleton()
    {
        // If a Game Manager exists, then return that
        if (GMGameObj != null) return (GameManager) GMGameObj.GetComponent(typeof (GameManager));

        // If one doesn't exist, then create a new GameManager
        GMGameObj = new GameObject();
        return (GameManager)GMGameObj.AddComponent(typeof(GameManager));
    }   


    /// <summary>
    /// Set up all of the global properties: Player, bullet & particle pools
    /// </summary>
    private void Start()
    {
                if (player == null)
        {
            print("One of your prefabs in" + " " + this.name + " " + "are null");
        }
        _soundManager = SoundManager.GetSingleton();    // Grab SoundMananger
        _soundManager.PlayClip(backgroundMusic, false); // Play track

        BulletPool   = PoolManager.Pools[_bulletPoolString];
        ParticlePool = PoolManager.Pools[_ParticlePoolString]; //TODO: Not working??
       
    }

}