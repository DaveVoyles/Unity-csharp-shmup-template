using UnityEngine;
using System.Collections;

public class UIScoreText : MonoBehaviour {

    public UILabel _lbl        = null;
    public string _scoreString  = "Score";


    /// <summary>
    /// Grab score from the GameManager & set the default score on the screen
    /// </summary>
    void Start()
    {
        _lbl      = GetComponent<UILabel>();
        //_lbl.text = _scoreString + "" + "0";
    }


    /// <summary>
    /// Keeps checking the GameManager to see if the score has been updated
    /// </summary>
    void Update()
    {
        // Appears as: "Score 0"
   //     _lbl.text = _scoreString + " " + GameManager.score;
    }
}
