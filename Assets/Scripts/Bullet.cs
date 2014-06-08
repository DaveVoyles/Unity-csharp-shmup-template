using System;
using System.Collections;
using PathologicalGames;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3    velocity; // the velocity for this bullet, defined when it's shot

    private int           _dmg        = 1;
    private const int    DEFAULT_DMG = 1;
    private const String BULLET_POOL = "BulletPool";
    private Transform    _xform; 

    private void Start()
    {
        _xform = transform;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Hit an enemy
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(_dmg);
            // put the bullet back on the stack for later re-use
            PoolManager.Pools[BULLET_POOL].Despawn(_xform);
        }
    }

    public void SetDmg(int dmg)
    {
        _dmg = dmg;
      //  print("Player bullet dmg is now:" + "" + _dmg);
        Debug.Log("Player bullet dmg is now:" + "" + _dmg);
    }

}