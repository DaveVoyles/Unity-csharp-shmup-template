using System;
using System.Collections;
using PathologicalGames;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3    velocity; // the velocity for this bullet, defined when it's shot

    private int           _dmg       = 1;
    private const String BULLET_POOL = "BulletPool";
    private Transform    _xform; 

    private void Start()
    {
        _xform = transform;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Only check for hits on enemies
        if (!other.gameObject.CompareTag("Enemy")) return;

        // apply dmg to enemy
        var enemy = other.gameObject.GetComponent<Enemy>();
        enemy.TakeDamage(_dmg);

        // put the bullet back on the stack for later re-use
        PoolManager.Pools[BULLET_POOL].Despawn(_xform);
    }

    public void SetDmg(int dmg)
    {
        _dmg = dmg;
    }

}