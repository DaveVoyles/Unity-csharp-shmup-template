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
    public Player        player;
    public static int    score = 0;          // counts how many enemies have been killed
    public static int    lives = 5;          // how many lives the player has left
    public AudioClip     backgroundMusic;

    private string       _nameOfPool  = "BulletPool";
    private int          _respawnTime = 3;
    private SoundManager _soundManager;

    private void Start()
    {   
        _soundManager = SoundManager.GetSingleton();    // Grab SoundManange
        _soundManager.PlayClip(backgroundMusic, false); // Play track
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 80, 20), "Score: " + score);
        GUI.Box(new Rect(10, 40, 80, 20), "Lives: " + lives);
    }

}; 