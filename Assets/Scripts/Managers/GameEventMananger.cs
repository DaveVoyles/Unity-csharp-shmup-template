public static class GameEventManager
{

    public delegate void GameEvent();

    public static event GameEvent GameStart, GameOver, UpdateScore, UpdateLives;

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
}
