using UnityEngine;
using System.Collections;

public class GUIGameplay : MonoBehaviour {

    // GAMEPLAY
    private string  _scoreString     = "score";
    private string  _livesLeftString = "x";
    private UILabel _scoreUILabel    = null;
    private UILabel _livesUILabel    = null;
    private UILabel _insertCoinLabel = null;
    private float   _flashInterval   = 0.9f;


    private void Awake()
    {
        _scoreUILabel    = GameObject.Find("Label_Score").     GetComponent<UILabel>();
        _livesUILabel    = GameObject.Find("Label_LivesText"). GetComponent<UILabel>();
        _insertCoinLabel = GameObject.Find("Label_InsertCoin").GetComponent<UILabel>();

        GameEventManager.UpdateScore += UpdateScoreString;
        GameEventManager.UpdateLives += UpdateLivesString;

        StartCoroutine(BlinkCaratChar());
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
        for (var n = 0; n < 1; n++)
        {
            yield return new WaitForSeconds(_flashInterval);
            _insertCoinLabel.text = "Insert Coin";

            yield return new WaitForSeconds(_flashInterval);
            _insertCoinLabel.text = "";
            n--;
        }
    }


}
