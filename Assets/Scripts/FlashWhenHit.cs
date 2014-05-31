/* Briefly flashes white when hit 
 * http://forum.unity3d.com/threads/224086-Make-a-sprite-flash
 */
using UnityEngine;
using System.Collections;
 
public class FlashWhenHit : MonoBehaviour
{
    private const float _flashSpeed = 0.15f;
    private Color       _startingColor;

    void Start()
    {
        _startingColor = renderer.material.color;
    }
    
    /// <summary>
    /// Flips back and forth between white and the starting color of the object
    /// </summary>
    /// <returns></returns>
    public IEnumerator FlashWhite()
    {
        for (var n = 0; n < 1; n++)
        {
            renderer.material.color = Color.white;
            yield return new WaitForSeconds(_flashSpeed);
            renderer.material.color = _startingColor;
            yield return new WaitForSeconds(_flashSpeed);
        }
    }


}
