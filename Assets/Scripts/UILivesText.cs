using UnityEngine;
using System.Collections;

public class UILivesText : MonoBehaviour {

    private UILabel _lbl             = null;
    private string  _livesLeftString = "x";


    /// <summary>
    /// Grab lives from GameManager and set default lives on screen
    /// </summary>
    void Start()
    {
        _lbl      = GetComponent<UILabel>();
        _lbl.text = _livesLeftString + "0";
    }


    /// <summary>
    /// Keeps checking the GameManager to see if the lives have been updated
    /// </summary>
	void Update ()
	{
        // Appears as: "X4"
        _lbl.text = _livesLeftString + GameManager.lives; 
	}
}
