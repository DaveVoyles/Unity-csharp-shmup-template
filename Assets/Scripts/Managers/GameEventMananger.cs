/* Tutorial on how to use this: http://catlikecoding.com/unity/tutorials/runner/
 * 
 *  Note that the manager isn't a MonoBehaviour and won't be attached to any Unity object.
 *  Now other scripts can subscribe to these events by assigning methods to them, 
 *  which will be called when the events are triggered.
 *  
 *  Only call an event if an object is subscribed to it, otherwise it will be null and the call will result in an error.
 *  
 * -------------------- USAGE: -------------------
 * 
 * SUBSCRIBING TO EVENTS:
 * In the Start() function for any class, use:  GameEventManager.GameOver += NameOfFunctionYouWantToListenToEvent;
 * 
 * Example:
 * 
 * Player.cs
 * 
 * 	void Start () {
		GameEventManager.GameOver += GameOver;
		gameOverText.enabled      = false;
	}

	private void GameOver () {
		gameOverText.    enabled = true;
		instructionsText.enabled = true;
		enabled                  = true;
	}
 * 
 * TRIGGERING EVENTS:
 * From another class, call: GameEventManager.TriggerGameStart();
 *   
 *  Dave Voyles - June 2014
 */

public static class GameEventManager
{
    public delegate void GameEvent();
    public static event  GameEvent GameStart, GameOver, PauseScreen, StartScreen, UpdateScore, UpdateLives;

    public static void TriggerGameStart()
    {
        if (GameStart == null) return;
        GameStart();
    }

    public static void TriggerGameOver()
    {
        if (GameOver == null) return;
        GameOver();
    }

    public static void TriggerUpdateScore()
    {
        if (UpdateScore == null) return;
        UpdateScore();
    }

    public static void TriggerUpdateLives()
    {
        if (UpdateLives == null) return;
        UpdateLives();
    }

    public static void TriggerPauseScreen()
    {
        if (PauseScreen == null) return;
        PauseScreen();
    }

    public static void TriggerStartScreen()
    {
        if (StartScreen == null) return;
        PauseScreen();
    }


}
