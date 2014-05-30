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
        this._startingColor = this.renderer.material.color;
        this._mat = GetComponent<MeshRenderer>().material;
    }

    /// <summary>
    /// Object quickly flashes between material colors, over a set period of time
    /// </summary>
    /// <param name="timeToFlash">How long should this flash?</param>
    /// <param name="intervalTime">How quickly should it flash?</param>
    /// <returns></returns>
    public IEnumerator Flash(float timeToFlash = _lengthOfTimeToFlash, float intervalTime = _flashSpeed)
    {
        float elapsedTime = 0f;
        int index         = 0;
        while (elapsedTime < timeToFlash)
        {
            _mat.color  = _colors[index % 2];

            elapsedTime += Time.deltaTime;
            index++;
            yield return new WaitForSeconds(intervalTime);
        }
        // Return to starting color
        this.renderer.material.color = this._startingColor;
    }

}