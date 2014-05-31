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
    // Keep globals here, so that other classes (enemy, etc.) can have one point of reference
    public Player        player;
    public static int    score = 0;          // counts how many enemies have been killed
    public static int    lives = 5;          // how many lives the player has left
    public AudioClip     backgroundMusic;

    private string       _nameOfPool  = "BulletPool";
    private int          _respawnTime = 3;
    private SoundManager _soundManager;

    static GameObject GMGameObj;

    /// <summary>
    /// Sets up a static singleton instance of GameManager, which is accessible to everything
    /// </summary>
    /// <returns>GameMananger object</returns>
    public static GameManager GetSingleton()
    {
        if (GMGameObj == null){
            GMGameObj = new GameObject();
            return (GameManager)GMGameObj.AddComponent(typeof(GameManager));
        }
        return (GameManager)GMGameObj.GetComponent(typeof(GameManager));
    }

    private void Start()
    {
        if (player == null) {
            print("One of your prefabs in" + " " + this.name + " " + "are null");
        }
        _soundManager = SoundManager.GetSingleton();    // Grab SoundMananger
        _soundManager.PlayClip(backgroundMusic, false); // Play track
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 80, 20), "Score: " + score);
        GUI.Box(new Rect(10, 40, 80, 20), "Lives: " + lives);
    }

}; 