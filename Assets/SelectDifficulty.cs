using UnityEngine;
using System.Collections;

public class SelectDifficulty : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnPress(bool isPressed)
    {
          print("button is pressed");
         //   Application.LoadLevel("Level1"); 
            Application.LoadLevel("Level1");
    }
}
