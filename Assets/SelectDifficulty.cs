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
        GameEventManager.TriggerGameStart();
        Application.LoadLevel("Level1");


    }
}
