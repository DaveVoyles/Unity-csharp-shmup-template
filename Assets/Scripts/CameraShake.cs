/* Shakes the camera.
 * Found here: http://www.mikedoesweb.com/2012/camera-shake-in-unity/
 */

using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public float        shakeDecay;
    public float        shakeIntensity;

    private float       _shakeDecay;
    private float       _shakeIntensity;
    private Vector3    _originPosition;
    private Quaternion _originRotation;
    private bool       _shaking;
    private Transform  _transform;


    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 40, 80, 20), "Shake"))
        {
            Shake();
        }
    }

    void OnEnable()
    {
        _transform = transform;
    }

    void Update()
    {
        if (!_shaking)
        {
            return;
        }
        if (_shakeIntensity > 0f)
        {
            _transform.localPosition = _originPosition + Random.insideUnitSphere * _shakeIntensity;
            _transform.localRotation = new Quaternion(
            _originRotation.x + Random.Range(-_shakeIntensity, _shakeIntensity) * .2f,
            _originRotation.y + Random.Range(-_shakeIntensity, _shakeIntensity) * .2f,
            _originRotation.z + Random.Range(-_shakeIntensity, _shakeIntensity) * .2f,
            _originRotation.w + Random.Range(-_shakeIntensity, _shakeIntensity) * .2f);
            _shakeIntensity -= _shakeDecay;
        }
        else
        {
            _shaking = false;
            _transform.localPosition = _originPosition;
            _transform.localRotation = _originRotation;
        }
    }

    public void Shake()
    {
        if (!_shaking)
        {
            _originPosition = _transform.localPosition;
            _originRotation = _transform.localRotation;
        }
        // Reset shake values to defaults
        _shaking = true;
        _shakeIntensity = shakeIntensity;
        _shakeDecay     = shakeDecay;
    }

}