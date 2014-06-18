using UnityEngine;
using System.Collections;

public class FlashingInsertCoin : MonoBehaviour
{
    private float _flashInterval = 0.9f;
    private UILabel _lbl         = null;
        
    void Start()
    {
        _lbl      = GetComponent<UILabel>();
        StartCoroutine(BlinkCaratChar());
    }

    /// <summary>
    /// Flashes the "Insert Coin" text 
    /// </summary>
    IEnumerator BlinkCaratChar ()
    {
        for(var n = 0; n < 1; n++)
        {
            yield return new WaitForSeconds(0.9f);
            _lbl.text = "Insert Coin" ;
            yield return new WaitForSeconds(0.9f);
            _lbl.text = "";
            n--;
        }
    }


}