/* Positions the object relative to one of the edges of the camera’s view.
 * only work if your camera has an orthographic projection.
 * Found here: http://www.raywenderlich.com/70344/unity-2d-tutorial-physics-and-screen-sizes
 */
using UnityEngine;
using System.Collections;

public class ScreenRelativePosition : MonoBehaviour {

    public enum ScreenEdge { LEFT, RIGHT, TOP, BOTTOM };
    public ScreenEdge screenEdge;
    public float      yOffset;
    public float      xOffset;

    /// <summary>
    /// Returns current location for spawning particles 
    /// </summary>
    public Vector3 GetCurrentLocation()
    {
      return transform.position;
    }

    void Start ()
    {
        CalculatePosition();
	}


    /// <summary>
    /// Let SpawnManager access this, so that we can set a new spawning location for groups of enemies
    /// </summary>
    public void CalculatePosition()
    {

        Vector3 newPosition = transform.position;
        Camera camera       = Camera.main;

        switch (screenEdge)
        {
            case ScreenEdge.RIGHT:
                newPosition.x = camera.aspect * camera.orthographicSize + xOffset;
                newPosition.y = yOffset;
                newPosition.z = 0;
                break;
            case ScreenEdge.TOP:
                newPosition.y = camera.orthographicSize + yOffset;
                newPosition.x = xOffset;
                newPosition.z = 0;
                break;
            case ScreenEdge.LEFT:
                newPosition.x = -camera.aspect * camera.orthographicSize + xOffset;
                newPosition.y = yOffset;
                newPosition.z = 0;
                break;
            case ScreenEdge.BOTTOM:
                newPosition.y = -camera.orthographicSize + yOffset;
                newPosition.x = xOffset;
                newPosition.z = 0;
                break;
        }
        transform.position = newPosition;
    }


}
