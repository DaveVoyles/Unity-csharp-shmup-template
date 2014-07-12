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

    private const string      _BULLET_POOL_STRING        = "BulletPool";
    private const string      _PARTICLE_POOL_STRING      = "ParticlePool";
    private int               _respawnTime               = 3;
    private static GameObject _GMGameObj;
    
    //----------------------
    // States and difficulty
    //-----------------------

    /// <summary> Sets the game difficulty </summary>
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    public Difficulty difficulty                          = Difficulty.Medium;

    /// <summary> Manages the current state of the game </summary>
    public enum CurrentState
    {
          GameStart
        , Playing
        , GameOver  
        , PauseScreen
        , StartScreen
        , OptionsScreen
        , CreditsScreen
        , UpdateScore
        , UpdateLives
    };
    public CurrentState currentState = CurrentState.StartScreen;



    /// <summary>
    /// Sets up a static singleton instance of GameManager, which is accessible to everything
    /// </summary>
    /// <returns>GameMananger object</returns>
    public static GameManager GetSingleton()
    {
        // If a Game Manager exists, then return that
        if (_GMGameObj != null) return (GameManager) _GMGameObj.GetComponent(typeof (GameManager));

        // If one doesn't exist, create a new GameManager
        _GMGameObj = new GameObject();
        return (GameManager)_GMGameObj.AddComponent(typeof(GameManager));
    }   


    /// <summary>
    /// Listen for GameStart event, which is triggered by the Start Screen
    /// </summary>
    private void Start()
    {
        GameEventManager.GameStart += GameStart;
    }


    /// <summary>
    /// Set up all of the global properties: Player, bullet & particle pools & set game difficulty
    /// </summary>
    private void GameStart()
    {
        SetObjectPools();
        //TODO Tie this into the menus 
        SetDifficulty();
        print("GameStart is called");
    }


    /// <summary>
    /// Creates a reference to object pools in the game
    /// </summary>
    private void SetObjectPools()
    {
        BulletPool   = PoolManager.Pools[_BULLET_POOL_STRING];
        ParticlePool = PoolManager.Pools[_PARTICLE_POOL_STRING];
    }


    //-----------------------------------------------------------------------------------------------
    //------------------------------------- Difficulty Settings -------------------------------------

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