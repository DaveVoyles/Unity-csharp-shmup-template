using UnityEngine;
using System.Collections;

public class UILives : MonoBehaviour
{

    private UISprite[] _livesUiSprites;

	// Use this for initialization
	void Start () {

        var sprite = GetComponent<UISprite>();
        sprite.spriteName = "livesSprite";
	
	}
	
	// Update is called once per frame
	void Update () {


        for (var i = 0; i < GameManager.lives; i++)
        {
            // do stuff 
        }
	
	}
}
