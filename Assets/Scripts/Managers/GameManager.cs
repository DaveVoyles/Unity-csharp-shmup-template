/* The Game Manager is an invisible Game Object which manages generic stuff like 
 * keeping track of bullets, spawning enemies, scoring, the GUI etc...
 */

using UnityEngine;
using PathologicalGames;

public class GameManager : MonoBehaviour
{ 
    public Player                        player          = null;
    public AudioClip                     backgroundMusic = null;
    [HideInInspector] public static int  score           = 0;
    [HideInInspector] public static int  lives           = 4;          
    /// <summary> Globally accessible BulletPool for all game objects to reference </summary>
    [HideInInspector] public SpawnPool   BulletPool;
    /// <summary> Globally accessible Particle for all game objects to reference   </summary>
    [HideInInspector] public SpawnPool   ParticlePool;


    private string            _bulletPoolString           = "BulletPool";
    private string            _ParticlePoolString         = "ParticlePool";
    private int               _respawnTime                = 3;
    private SoundManager      _soundManager;
    private static GameObject _GMGameObj;

    /// <summary> Sets the game difficulty </summary>
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    public Difficulty difficulty                          = Difficulty.Medium;

    public enum CurrentGameState
    {
        PauseScreen

    };





    /// <summary>
    /// Sets up a static singleton instance of GameManager, which is accessible to everything
    /// </summary>
    /// <returns>GameMananger object</returns>
    public static GameManager GetSingleton()
    {
        // If a Game Manager exists, then return that
        if (_GMGameObj != null) return (GameManager) _GMGameObj.GetComponent(typeof (GameManager));

        // If one doesn't exist, then create a new GameManager
        _GMGameObj = new GameObject();
        return (GameManager)_GMGameObj.AddComponent(typeof(GameManager));
    }   


    /// <summary>
    /// Set up all of the global properties: Player, bullet & particle pools & set game difficulty
    /// </summary>
    private void Start()
    {
        // Grab a sound manager and play a track
        _soundManager = SoundManager.GetSingleton();    
        _soundManager.PlayClip(backgroundMusic, false); 

        BulletPool   = PoolManager.Pools[_bulletPoolString];
        ParticlePool = PoolManager.Pools[_ParticlePoolString]; 

        SetDifficulty();
       
    }


    /// <summary>
    /// Sets the game difficulty accordingly 
    /// </summary>
    public void SetDifficulty()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                EasyDifficultySettings();
                break;
            case Difficulty.Medium:
                MediumDifficultySettings();
                break;
            case Difficulty.Hard:
                HardDifficultySettings();
                break;
            default:
                DebugUtils.Assert(false);
                break;
        }
    }

    private void EasyDifficultySettings()
    {
        // Insert logic
    }

    private void MediumDifficultySettings()
    {
        // Insert logic
    }

    private void HardDifficultySettings()
    {
        // Insert logic
    }






}