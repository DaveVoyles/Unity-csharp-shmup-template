/* Scrolling background animation
 * Adjust the X,Y values in the editor to get a better look at feel
 */
using UnityEngine;

public class ScrollingUVs : MonoBehaviour
{

    private Vector2 _uvOffset;                // used for UV scrolling 
    private Vector2 _uvAnimationRate;         // define the rate in the editor (0, -0.1f) looks good

    private void Start()
    {
        _uvOffset        = Vector2.zero;
        _uvAnimationRate = new Vector2(0, -1);
    }

    void LateUpdate()
    {
        // increment the UV offset
        _uvOffset += (_uvAnimationRate * Time.deltaTime);

        // apply the UV offset to material 0
        renderer.materials[0].SetTextureOffset("_MainTex", _uvOffset);
    }
}