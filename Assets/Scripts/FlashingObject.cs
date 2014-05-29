using UnityEngine;
using System.Collections;

public class FlashingObject : MonoBehaviour
{

    private Material _mat;
    private Color[]  _colors              = { Color.yellow, Color.red };
    private float    _flashSpeed          = 0.1f;
    private float    _lengthOfTimeToFlash = 1f;
    private Color    _startingColor;

    public void Awake()
    {
        this._mat = GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        this._startingColor = this.renderer.material.color;
        StartCoroutine(Flash(this._lengthOfTimeToFlash, this._flashSpeed));
    }

    /// <summary>
    /// Object quickly flashes between material colors, over a set period of time
    /// </summary>
    /// <param name="timeToFlash">How long should this flash?</param>
    /// <param name="intervalTime">How quickly should it flash?</param>
    /// <returns></returns>
    IEnumerator Flash(float timeToFlash, float intervalTime)
    {
        float elapsedTime = 0f;
        int index         = 0;
        while (elapsedTime < timeToFlash)
        {
            _mat.color = _colors[index % 2];

            elapsedTime += Time.deltaTime;
            index++;
            yield return new WaitForSeconds(intervalTime);
        }
        // Return to starting color
        this.renderer.material.color = this._startingColor;
    }

}