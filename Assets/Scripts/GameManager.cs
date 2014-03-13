/* The Game Manager is an invisible Game Object which manages generic stuff like 
 * keeping track of bullets, spawning enemies, scoring, the GUI etc...
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour {

    public Bullet                playerBullet;		 // PREFAB: Bullet
    public Bullet                enemyBullet;		 // PREFAB: Enemy Bullet
    public Enemy                 enemy;			     // PREFAB: Enemy
    public Player                player;			 // SCRIPT: Player
    public static Stack<Bullet>  playerBulletStack;
    public static Stack<Bullet>  enemyBulletStack; 
    private bool _isSpawning;                        // Continue to spawn until told otherwise

    public static int score = 0;	                 // counts how many enemies have been killed
    public static int lives = 5;		             // how many lives the player has left


    void  Start ()
    {
        _isSpawning       = true;
        playerBulletStack = new Stack<Bullet>();
        enemyBulletStack  = new Stack<Bullet>();
        CreatePlayerBulletStack();
        CreateEnemyBulletStack();
        StartCoroutine(SpawnEnemy());
    }

    void  CreatePlayerBulletStack ()
    {
        // create bullets for the player and store them in a stack
        // this is faster than instantiating them when the player shoots 
        for (var i= 0; i < 50; i++)
        {
            var newBullet = (Bullet) Instantiate(playerBullet, Vector3.zero, Quaternion.identity);          // create a bullet
            newBullet.gameObject.SetActive(false);                                                          // disable it until it's needed
            playerBulletStack.Push(newBullet);	                                                            // put it on the stack
        }    
}

    void  CreateEnemyBulletStack ()
    {
        // create bullets for enemies
        // creating way more than needed, because in a game like this it's good to have lots of enemy bullets
        for (var j= 0; j < 40; j++)
        {
		    var newEnemyBullet = (Bullet) Instantiate (enemyBullet, Vector3.zero, Quaternion.identity);     // create a bullet
		    newEnemyBullet.gameObject.SetActive(false);                                                     // disable it until it's needed
		    enemyBulletStack.Push(newEnemyBullet);                                                          // put it on the stack
        }
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
            var randomX        = Random.Range(-4.0f, 4.0f);
            var newEnemy       = (Enemy)Instantiate(enemy, new Vector3(randomX, 0, 10), Quaternion.identity);
            newEnemy.HitPoints = 4;

            // move it and tell it to shoot at a random time
            newEnemy.Motion = new Vector3(0, 0, -3);
            var shootDelay  = Random.Range(0.5f, 2.0f);
            StartCoroutine(newEnemy.Shoot(shootDelay)); // waits a few seconds then shoots

            // destroy it in 7 seconds (it will be off-screen by then if the player hasn't killed it)
            Destroy(newEnemy.gameObject, 7);

            // Wait 3 seconds, then call this function again 
            yield return new WaitForSeconds(3);
        }
    }
    
}

