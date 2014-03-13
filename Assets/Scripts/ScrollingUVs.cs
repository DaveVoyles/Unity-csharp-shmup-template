using UnityEngine;

public class ScrollingUVs : MonoBehaviour
{

    private Vector2 uvOffset = Vector2.zero; // used for UV scrolling 
    public  Vector2 uvAnimationRate;         // define the rate in the editor (0, -0.1f) looks good

    void LateUpdate()
    {
        // increment the UV offset
        uvOffset += (uvAnimationRate * Time.deltaTime);

        // apply the UV offset to material 0
        renderer.materials[0].SetTextureOffset("_MainTex", uvOffset);
        Debug.Log("late update is called");
    }
}