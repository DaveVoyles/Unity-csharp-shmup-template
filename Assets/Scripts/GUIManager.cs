using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour
{

    public UIScoreText uiScoreScript = null;
    public UILivesText uiLivesScript = null;

    private string    _scoreString = "score";

	// Use this for initialization
	void Start ()
    {      
	    GameEventManager.GameStart   += SetScoreString;
	    GameEventManager.UpdateScore += UpdateScoreString;
    }
	
	// Update is called once per frame
	void Update () 
    {

	}

    private void UpdateScoreString()
    {
        uiScoreScript._lbl.text = _scoreString;
    }

    /// <summary>
    /// Sets the string for the score when the game loads
    /// </summary>
    private void SetScoreString()
    {
        uiScoreScript._lbl.text = _scoreString + "" + "0";
    }
}
