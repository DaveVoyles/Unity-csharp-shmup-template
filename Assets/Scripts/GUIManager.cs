/* Handles all of the GUI and menu logic for the game
 * Can update the score and player lives, based on events it listens for in the GameEventManager
 * Dave Voyles - June 2014
 */

using UnityEngine;

public class GUIManager : MonoBehaviour
{

    public UIScoreText uiScoreScript = null;
    public UILivesText uiLivesScript = null;

    private string    _scoreString = "score";

	void Start ()
    {      
        // Listen for the following events, and call these functions when the event is triggered
	    GameEventManager.GameStart   += SetScoreString;
	    GameEventManager.UpdateScore += UpdateScoreString;
    }

    /// <summary>
    /// Updates the player's score on the UI
    /// </summary>
    private void UpdateScoreString()
    {
        uiScoreScript._lbl.text = _scoreString + "" + GameManager.score;
    }

    /// <summary>
    /// Sets the string for the score when the game loads
    /// </summary>
    private void SetScoreString()
    {
        uiScoreScript._lbl.text = _scoreString + "" + "0";
    }
}
