using System;
using System.Collections;
using PathologicalGames;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3    velocity; // the velocity for this bullet, defined when it's shot

    private int       _dmg       = 1; 
    private String    _bulletPool = "BulletPool";
    private Transform _xform; 

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
            PoolManager.Pools[_bulletPool].Despawn(_xform);
        }
    }

}