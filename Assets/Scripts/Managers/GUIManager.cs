/* Handles all of the GUI and menu logic for the game
 * Can update the score and player lives, based on events it listens for in the GameEventManager
 * Dave Voyles - June 2014
 */

using System.Collections;
using UnityEngine;

public class GUIManager : MonoBehaviour
{

    // GAMEPLAY
    private string  _scoreString      = "score";
    private string  _livesLeftString  = "x";
    private UILabel _scoreUILabel     = null;
    private UILabel _livesUILabel     = null;
    private UILabel _insertCoinLabel  = null;
    private float   _flashInterval    = 0.9f;

    // START SCREEN
    private string _easyString        = "Easy";
    private string _normalString      = "Normal";
    private string _difficultString   = "Difficult";
    private string _optionsString     = "Options";
    private string _creditsString     = "Credits";

    private UILabel _easyUILabel      = null;
    private UILabel _normalUiLabel    = null;
    private UILabel _difficultUiLabel = null;

    // OPTIONS SCREEN
    private UILabel _audioUiLabel     = null;
    private float   _volume           = 1.0f;

    [SerializeField] 
    private GameManager _gm           = null;
    private static GameObject _GMGameObj = null;


    /// <summary>
    /// Sets up a static singleton instance of HUIManager, which is accessible to everything
    /// </summary>
    /// <returns>GUI Manager object</returns>
    public static GameManager GetSingleton()
    {
        // If a Game Manager exists, then return that
        if (_GMGameObj != null) return (GameManager)_GMGameObj.GetComponent(typeof(GameManager));

        // If one doesn't exist, create a new GameManager
        _GMGameObj = new GameObject();
        return (GameManager)_GMGameObj.AddComponent(typeof(GameManager));
    }   


    /// <summary>
    /// Listen for the following events, and call these functions when the event is triggered
    /// </summary>
	private void Awake ()
    {
        GameEventManager.StartScreen += SetStartScreenDuringInit;

        //GameEventManager.GameStart += SetScoreStringDuringInit;
        //GameEventManager.GameStart += SetLivesStringDuringInit;
        //GameEventManager.GameStart   += SetGameUI;
        //GameEventManager.UpdateScore += UpdateScoreString;
        //GameEventManager.UpdateLives += UpdateLivesString;

        // // TODO: Move this so that it only displays during gameplay
        //_scoreUILabel    = GameObject.Find("Label_Score").     GetComponent<UILabel>();
        //_livesUILabel    = GameObject.Find("Label_LivesText"). GetComponent<UILabel>();
        //_insertCoinLabel = GameObject.Find("Label_InsertCoin").GetComponent<UILabel>();

        // TODO: Move this so that it only displays during gameplay
      // StartCoroutine(BlinkCaratChar());
    }



    private void SetGameUI()
    {
        _scoreUILabel    = GameObject.Find("Label_Score").     GetComponent<UILabel>();
        _livesUILabel    = GameObject.Find("Label_LivesText"). GetComponent<UILabel>();
        _insertCoinLabel = GameObject.Find("Label_InsertCoin").GetComponent<UILabel>();
    }


    //-----------------------------------------------------------------------------------------------
    //------------------------------------- Start Screen --------------------------------------------


    /// <summary>
    /// Sets the start screen text
    /// </summary>
    private void SetStartScreenDuringInit()
    {
        _easyUILabel.     text = _easyString;
        _normalUiLabel.   text = _normalString;
        _difficultUiLabel.text = _difficultString;
    }


    //----------------------------------------------------------------------------------------------
    //--------------------------------------- Gameplay ---------------------------------------------


    /// <summary>
    /// Updates the player's lives on the UI
    /// </summary>
    private void UpdateLivesString()
    {
        _livesUILabel.text = _livesLeftString + GameManager.lives;
    }


    /// <summary>
    /// Sets the number of player's lives during initialization
    /// </summary>
    private void SetLivesStringDuringInit()
    {
       _livesUILabel.text = _livesLeftString + "0";
    }


    /// <summary>
    /// Updates the player's score on the UI
    /// </summary>
    private void UpdateScoreString()
    {
        _scoreUILabel.text = _scoreString + " " + GameManager.score;
    }


    /// <summary>
    /// Sets the string for the score when the game loads
    /// </summary>
    private void SetScoreStringDuringInit()
    {
        _scoreUILabel.text = _scoreString + " " + "0";
    }


    /// <summary>
    /// Flashes the "Insert Coin" text 
    /// </summary>
    private IEnumerator BlinkCaratChar()
    {
        // The number keeps incrementing and decrementing, so it never ends
        for(var n = 0; n < 1; n++)
        {
            yield return new WaitForSeconds(_flashInterval);
            _insertCoinLabel.text = "Insert Coin" ;

            yield return new WaitForSeconds(_flashInterval);
            _insertCoinLabel.text = "";
            n--;
        }
    }
}
