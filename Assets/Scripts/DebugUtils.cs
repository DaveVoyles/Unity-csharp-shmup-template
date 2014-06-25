/* http://answers.unity3d.com/questions/19122/assert-function.html 
 * Use this to catch errors before they become an issue. Assert will throw an exception and say "Hey, something is wrong here!"
 * It's a sanity check to ensure that all variables are as they should be
 */

using System;
using System.Diagnostics;
using UnityEngine;
using System.Collections;
 
public class DebugUtils
{
    /// <summary>
    /// Used to catch errors during runtime 
    /// </summary>
    //[Conditional("DEBUG")]
    public static void Assert(bool condition)
    {
        if (!condition) throw new Exception();
    }
    

    /// <summary>
    /// Draws text above an object. Currently used to draw the enemyType, but can draw anything you pass in as second param.
    /// Create a new function to draw different objects above enemes
    /// </summary>
    /// <param name="gameObject">Target object we are drawing on top of </param>
    /// <param name="enemyType">should consider moving this to a different object, instead of enemyType</param>
    //[Conditional("DEBUG")]
    public static void DrawEnemyTypeAboveObject(GameObject gameObject, Enemy.EnemyType enemyType, int offsetX = 40, int offsetY = 40, int rectWidth = 90, int rectHeight = 40)
    {
        // Converts 3d space to 2d, to create a plane to draw text to
        var objectPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        // BeginArea positioned in relation to my character
        GUILayout.BeginArea(new Rect((objectPos.x - offsetX), (Screen.height - objectPos.y) - offsetY, Screen.width, Screen.height));

        // Draw the text above the enemy
        GUI.Label(new Rect(gameObject.transform.position.x, gameObject.transform.position.y, rectWidth, rectHeight), enemyType.ToString());

        GUILayout.EndArea();
    }

}
