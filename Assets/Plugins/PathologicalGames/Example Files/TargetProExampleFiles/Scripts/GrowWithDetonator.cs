using UnityEngine;
using System.Collections;

using PathologicalGames;

[RequireComponent(typeof(Detonator))]
public class GrowWithDetonator : MonoBehaviour
{
    public Detonator detonator;
    private Transform xform;

    private void Awake()
    {
        this.xform = this.transform;
    }

    void Update()
    {
        Vector3 scl = this.detonator.range * 2.1f; // Let geo move ahead of real range
        scl.y *= 0.2f; // More cenematic hieght.
        this.xform.localScale = scl;

        // Blend the alpha channel of the color off as the range expands.
        Color col = this.renderer.material.GetColor("_TintColor");
        col.a = Mathf.Lerp(0.7f, 0, this.detonator.range.x / this.detonator.maxRange.x);
        this.renderer.material.SetColor("_TintColor", col);
    }
}