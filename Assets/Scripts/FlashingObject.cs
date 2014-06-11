using UnityEngine;
using System.Collections;

public class FlashingObject : MonoBehaviour
{

    private Material    _mat;
    private Color[]     _colors              = { Color.yellow, Color.red };
    private const float _flashSpeed          = 0.2f;
    private const float _lengthOfTimeToFlash = 0.2f;
    private Color       _startingColor;

    void Start()
    {
        // Set starting color so that we can return to it when done flashing
        _startingColor = renderer.material.color; 
        _mat           = GetComponent<MeshRenderer>().material;
    }

    /// <summary>
    /// Object quickly flashes between material colors, over a set period of time
    /// </summary>
    /// <param name="timeToFlash">How long should this flash?</param>
    /// <param name="intervalTime">How quickly should it flash?</param>
    /// <returns></returns>
    public IEnumerator Flash(float timeToFlash = _lengthOfTimeToFlash, float intervalTime = _flashSpeed)
    {
        var elapsedTime   = 0f;
        var index         = 0;
        
        // Flash back and forth over a set period of time
        while (elapsedTime < timeToFlash)
        {
            _mat.color  = _colors[index % 2];
            elapsedTime += Time.deltaTime;
            index++;

            // Wait a moment before switching colors 
            yield return new WaitForSeconds(intervalTime);
        }

        // Return to starting color
        renderer.material.color = _startingColor;
    }

}