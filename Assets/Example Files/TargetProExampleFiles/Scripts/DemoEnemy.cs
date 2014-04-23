using UnityEngine;
using PathologicalGames;

public class DemoEnemy : MonoBehaviour
{
    public int life = 100;
    public ParticleSystem explosion;
    
    private Color startingColor;
    private bool isDead = false;

    private void Awake()
    {
        this.startingColor = this.renderer.material.color;

        var targetable = this.GetComponent<Targetable>();

        targetable.AddOnDetectedDelegate(this.MakeMeBig);
        targetable.AddOnDetectedDelegate(this.MakeMeGreen);

        targetable.AddOnNotDetectedDelegate(this.MakeMeNormal);
        targetable.AddOnNotDetectedDelegate(this.ResetColor);

        targetable.AddOnHitColliderDelegate(this.OnHit);
    }

    private void OnHit(HitEffectList effects, Target target, Collider other)
    {
        if (this.isDead) return;

        if (other != null)
            Debug.Log(this.name +  " was hit by collider on " + other.name);

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

    private void MakeMeGreen(TargetTracker source)
    {
        if (this.isDead) return;
        this.renderer.material.color = Color.green;
    }

    private void ResetColor(TargetTracker source)
    {
        if (this.isDead) return;
        this.renderer.material.color = this.startingColor;
    }

    private void MakeMeBig(TargetTracker source)
    {
        if (this.isDead) return;
        this.transform.localScale = new Vector3(2, 2, 2);
    }

    private void MakeMeNormal(TargetTracker source)
    {
        if (this.isDead) return;
        this.transform.localScale = Vector3.one;
    }
}
