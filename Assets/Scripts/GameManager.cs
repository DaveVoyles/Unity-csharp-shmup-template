/* The Game Manager is an invisible Game Object which manages generic stuff like 
 * keeping track of bullets, spawning enemies, scoring, the GUI etc...
 */

using System.Collections;
using UnityEngine;
using PathologicalGames;


public class GameManager : MonoBehaviour {

    public Enemy         enemy;	    	  // PREFAB: Enemy
    public Player        player;		  // SCRIPT: Player
    private bool         _isSpawning;     // Continue to spawn until told otherwise
    public static int    score = 0;	      // counts how many enemies have been killed
    public static int    lives = 5;		  // how many lives the player has left
    private SoundManager _soundManager;
    public AudioClip     backgroundMusic;

   void  Start ()
    {
        _isSpawning       = true;
        StartCoroutine(SpawnEnemy());

        _soundManager = SoundManager.GetSingleton();        // Grab SoundManange
        _soundManager.PlayClip(backgroundMusic, false);     // Play track
    }

    void  OnGUI ()
    {
        GUI.Box( new Rect(10, 10, 80, 20), "Score: " + score);
        GUI.Box( new Rect(10, 40, 80, 20), "Lives: " + lives);
    }

    IEnumerator SpawnEnemy()  // the IEnumerator here allows this function to call itself
    {
        while (_isSpawning)
        {
            // spawn an enemy off screen at a random X position and set hit points
            var randomY  = Random.Range(-4.0f, 4.0f);
            var newEnemy = (Enemy)Instantiate(enemy, new Vector3(10, 0, randomY), Quaternion.identity);
            newEnemy.hitPoints = 4;

            // move it and tell it to shoot at a random time
            newEnemy.motion = new Vector3(-3, 0, 0);
            var shootDelay  = Random.Range(0.5f, 2.0f);
            StartCoroutine(newEnemy.Shoot(shootDelay)); // waits a few seconds then shoots

            // destroy it in 7 seconds (it will be off-screen by then if the player hasn't killed it)
            Destroy(newEnemy.gameObject, 7);
        
            // Wait 3 seconds, then call this function again 
            yield return new WaitForSeconds(3);
        }
    }
}

