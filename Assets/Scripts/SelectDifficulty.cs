using UnityEngine;
using System.Collections;

public class SelectDifficulty : MonoBehaviour {


    void OnPress(bool isPressed)
    {
        Application.LoadLevel("Level1");
        GameEventManager.TriggerGameStart();
    }
}
