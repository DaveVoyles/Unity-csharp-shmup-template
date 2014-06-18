/* Handles all of the GUI and menu logic for the game
 * Can update the score and player lives, based on events it listens for in the GameEventManager
 * Dave Voyles - June 2014
 */

using UnityEngine;

public class GUIManager : MonoBehaviour
{
    private string    _scoreString     = "score";
    private string    _livesLeftString = "x";
    private UILabel   _scoreUILabel    = null;
    private UILabel   _livesUILabel    = null;

    /// <summary>
    /// Listen for the following events, and call these functions when the event is triggered
    /// </summary>
	private void Start ()
    {      
	    GameEventManager.GameStart   += SetScoreStringDuringInit;
	    GameEventManager.GameStart   += SetLivesStringDuringInit;
	    GameEventManager.UpdateScore += UpdateScoreString;
	    GameEventManager.UpdateLives += UpdateLivesString;
    }

    /// <summary>
    /// Locate the GUI elements, which we will set text to
    /// </summary>
    private void Awake()
    {
        _scoreUILabel = GameObject.Find("Label_Score").    GetComponent<UILabel>();
        _livesUILabel = GameObject.Find("Label_LivesText").GetComponent<UILabel>();
    }


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
}
