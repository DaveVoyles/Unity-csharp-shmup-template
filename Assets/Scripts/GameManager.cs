/* The Game Manager is an invisible Game Object which manages generic stuff like 
 * keeping track of bullets, spawning enemies, scoring, the GUI etc...
 */

using System;
using System.Collections;
using UnityEngine;
using PathologicalGames;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{

    public Enemy enemy; // PREFAB: Enemy
    public Player player; // SCRIPT: Player
    public static int score = 0; // counts how many enemies have been killed
    public static int lives = 5; // how many lives the player has left
    public AudioClip backgroundMusic;
    public Transform enemyTransform;

    private bool _isSpawning = true; // Continue to spawn until told otherwise
    private SoundManager _soundManager;
    private string _nameOfPool = "BulletPool";
    private int _respawnTime = 3;

    private void Start()
    {
     //   StartCoroutine(SpawnMovingEnemy());
        StartCoroutine(SpawnStationaryEnemy());
        _soundManager = SoundManager.GetSingleton(); // Grab SoundManange
        _soundManager.PlayClip(backgroundMusic, false); // Play track
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 80, 20), "Score: " + score);
        GUI.Box(new Rect(10, 40, 80, 20), "Lives: " + lives);
    }


    /// <summary>
    /// Spawns an enemy which scrolls from right to left on the screen
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnMovingEnemy() // the IEnumerator here allows this function to call itself
    {
        while (_isSpawning)
        {
            // spawn an enemy off screen at a random X position and set hit points
            var randomY = Random.Range(-4.0f, 4.0f);

            // Grabs current instance of bullet, by retrieving bullet prefab from spawn pool
            Transform _enemyInstance = PoolManager.Pools[_nameOfPool].Spawn(enemyTransform);

            // position, enable, then set velocity
            _enemyInstance.gameObject.transform.position = new Vector3(10, 0, randomY);
            _enemyInstance.gameObject.SetActive(true);

            // Grab the "enemy" script, so that we can access the variables exposed
            var scriptRef = _enemyInstance.GetComponent<Enemy>();
            scriptRef.hitPoints = 4;

            // move it and tell it to shoot at a random time
            scriptRef.motion = new Vector3(-3, 0, 0);
            var shootDelay   = Random.Range(0.5f, 2.0f);

            // waits a few seconds then shoots
            StartCoroutine(scriptRef.ShootTowardPlayer(shootDelay)); 

            // Wait 3 seconds, then call this function again 
            yield return new WaitForSeconds(_respawnTime);    }
    }

    /// <summary>
    /// Spawns a stationary enemy
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnStationaryEnemy() // the IEnumerator here allows this function to call itself
    {
        while (_isSpawning)
        {
            // spawn an enemy off screen at a random X position and set hit points
            var randomY = Random.Range(-4.0f, 4.0f);

            // Grabs current instance of bullet, by retrieving bullet prefab from spawn pool
            Transform _enemyInstance = PoolManager.Pools[_nameOfPool].Spawn(enemyTransform);

            // position, enable, then set velocity
            _enemyInstance.gameObject.transform.position = new Vector3(10, 0, randomY);
            _enemyInstance.gameObject.SetActive(true);

            // Grab the "enemy" script, so that we can access the variables exposed
            var scriptRef = _enemyInstance.GetComponent<Enemy>();
            scriptRef.hitPoints = 4;

            // waits a few seconds then shoots
            var shootDelay = Random.Range(0.5f, 2.0f);
            StartCoroutine(scriptRef.ShootTowardPlayer(shootDelay)); 

            // Wait 3 seconds, then call this function again 
            yield return new WaitForSeconds(_respawnTime);
        }
    }
};

