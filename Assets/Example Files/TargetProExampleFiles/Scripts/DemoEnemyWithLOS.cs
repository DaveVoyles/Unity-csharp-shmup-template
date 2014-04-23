using UnityEngine;
using System.Collections;

using PathologicalGames;


/// <summary>
/// This enemy will change size and color if it is detected, factoring in 
/// Line-Of-Sigh (LOS). If the LOS component is set to filter the TargetTracker 
/// this will be norma size and color when not in LOS. If the LOS component is 
/// set to filter the FireController, it is technically detected anywhere in 
/// range, even f the turret can't fire on it.
/// </summary>
public class DemoEnemyWithLOS : MonoBehaviour
{
    public int life = 100;
    public ParticleSystem explosion;
    
    private Color startingColor;
    private bool isDead = false;

    private void Awake()
    {
        this.startingColor = this.renderer.material.color;

        var targetable = this.GetComponent<Targetable>();

        targetable.AddOnDetectedDelegate(this.OnDetected);
        targetable.AddOnNotDetectedDelegate(this.OnNotDetected);

        targetable.AddOnHitColliderDelegate(this.OnHit);
    }


    private void OnHit(HitEffectList effects, Target target, Collider other)
    {
        if (this.isDead) return;

        if (other != null)
            Debug.Log(this.name + " was hit by collider on " + other.name);

        foreach (HitEffect effect in effects)
        {
            if (effect.name == "Damage")
            {
                this.life -= (int)effect.value;
            }
        }

        if (this.life <= 0)
        {
            this.isDead = true;
            Instantiate
            (
                this.explosion.gameObject,
                this.transform.position,
                this.transform.rotation
            );

            this.gameObject.SetActive(false);
        }
    }


    private void OnDetected(TargetTracker source)
    {
        this.StartCoroutine(this.UpdateStartWhileDetected(source));
    }

    private void OnNotDetected(TargetTracker source)
    {
        this.StopAllCoroutines();
        this.ResetStates();
    }

    private void ResetStates()
    {
        this.transform.localScale = Vector3.one;
        this.renderer.material.color = this.startingColor;
    }

    private IEnumerator UpdateStartWhileDetected(TargetTracker source)
    {
        while (true)
        {
            if (this.isDead) yield return null;  // Just in case

            // Note: All of this could be more efficient with some caching
            if (source.targets.Contains(new Target(this.transform, source)))
            {
                this.transform.localScale = new Vector3(2, 2, 2);
                this.renderer.material.color = Color.green;
            }
            else
            {
                ResetStates();
            }

            yield return null;
        }
    }

}
