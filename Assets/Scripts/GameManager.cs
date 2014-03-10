// The Game Manager is an invisible Game Object which manages generic stuff like 
// keeping track of bullets, spawning enemies, scoring, the GUI etc...

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour {

    public Bullet                playerBullet;		 // link to the bullet prefab
    public Bullet                enemyBullet;		 // prefab link
    public Enemy                 enemy;			     // enemy prefab
    public Player                player;			 // link to the player
    public static Stack<Bullet>  playerBulletStack;
    public static Stack<Bullet>  enemyBulletStack;

    public int score = 0;				     // counts how many enemies have been killed
    public int lives = 5;					 // how many lives the player has left

    void  Start ()
    {
        playerBulletStack = new Stack<Bullet>();
        enemyBulletStack  = new Stack<Bullet>();
        CreatePlayerBulletStack();
        CreateEnemyBulletStack();
      //  SpawnEnemy();
        SpawnEnemyNew();
    }

    void  CreatePlayerBulletStack (){
        // create bullets for the player and store them in a stack
        // this is faster than instantiating them when the player shoots
        for (var i= 0; i < 50; i++)
        {
            Bullet newBullet = Instantiate (playerBullet, Vector3.zero, Quaternion.identity) as Bullet; // create a bullet
            newBullet.gameObject.active = false;                                                        // disable it until it's needed
            playerBulletStack.Push(newBullet);	                                                        // put it on the stack
			Debug.Log("creating a bullet");
        }    
}

void  CreateEnemyBulletStack (){
    // create bullets for enemies
    // creating way more than needed, because in a game like this it's good to have lots of enemy bullets
    for (var j= 0; j < 40; j++)
    {
		Bullet newEnemyBullet = (Bullet) Instantiate (enemyBullet, Vector3.zero, Quaternion.identity); // create a bullet
		newEnemyBullet.gameObject.active = false;                                                      // disable it until it's needed
		enemyBulletStack.Push(newEnemyBullet);                                                         // put it on the stack
    }
}

void  OnGUI (){
    GUI.Box( new Rect(10, 10, 80, 20), "Score: " + score);
    GUI.Box( new Rect(10, 40, 80, 20), "Lives: " + lives);
}

 //IEnumerator SpawnEnemy()  // the IEnumerator here allows this function to call itself
 //   {
 //       // spawn an enemy off screen at a random X position and set hit points
 //       var randomX        = Random.Range(-4.0f, 4.0f);
 //       var newEnemy       = (Enemy) Instantiate(enemy, new Vector3(randomX, 0, 10), Quaternion.identity);
 //       newEnemy.hitPoints = 4;
	
 //       // move it and tell it to shoot in a random time
 //       newEnemy.motion = new Vector3(0, 0, -3);
 //       var shootDelay  = Random.Range(0.5f, 2.0f);
 //       newEnemy.Shoot(shootDelay); // waits a few seconds then shoots
	
 //       // destroy it in 7 seconds (it will be off-screen by then if the player hasn't killed it)
 //       Destroy(newEnemy.gameObject, 7);
	
 //       // wait 3 seconds and spawn another enemy
 //       yield return new WaitForSeconds(3);
 //       yield return SpawnEnemy();
 //       Debug.Log("spawning enemy");
 //   }

 void SpawnEnemyNew()  // the IEnumerator here allows this function to call itself
 {
     // spawn an enemy off screen at a random X position and set hit points
     var randomX  = Random.Range(-4.0f, 4.0f);
     var newEnemy = (Enemy)Instantiate(enemy, new Vector3(randomX, 0, 10), Quaternion.identity);
     newEnemy.HitPoints = 4;

     // move it and tell it to shoot in a random time
     newEnemy.Motion = new Vector3(0, 0, -3);
     var shootDelay = Random.Range(0.5f, 2.0f);
     newEnemy.Shoot(shootDelay); // waits a few seconds then shoots


     Debug.Log("spawning enemy");
 }




}

