using UnityEngine;
using System.Collections;
/**
 * Restricts the movement of the object to the screen.
 * @author Jorjon
 * http://answers.unity3d.com/questions/174958/keeping-the-player-inside-the-screen.html
 */

using UnityEngine;

public class Boundings2D : MonoBehaviour
{

    void LateUpdate()
    {
        var left = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        var right = Camera.main.ViewportToWorldPoint(Vector3.one).x;
        var top = Camera.main.ViewportToWorldPoint(Vector3.zero).y;
        var bottom = Camera.main.ViewportToWorldPoint(Vector3.one).y;
        float x = transform.position.x, y = transform.position.y;
        if (transform.position.x <= left + renderer.bounds.extents.x)
        {
            x = left + renderer.bounds.extents.x;
        }
        else if (transform.position.x >= right - renderer.bounds.extents.x)
        {
            x = right - renderer.bounds.extents.x;
        }
        if (transform.position.y <= top + renderer.bounds.extents.y)
        {
            y = top + renderer.bounds.extents.y;
        }
        else if (transform.position.y >= bottom - renderer.bounds.extents.y)
        {
            y = bottom - renderer.bounds.extents.y;
        }
        transform.position = new Vector3(x, y, transform.position.z);
    }
}