using System;
using System.Collections;
using PathologicalGames;
using UnityEngine;

public class Bullet : MonoBehaviour
{


    private Transform _xform; // cached transform for performance
    public Vector3 velocity; // the velocity for this bullet, defined when it's shot

    private int     _dmg       = 1; // How much dmg does it do to objects it hits?
    private String _bulletPool = "BulletPool";

    private void Start()
    {
        _xform = transform;
    }


    /// <summary>
    /// De-spawn bullet if it is out of range of camera
    /// </summary>
    private void Update()
    {
        if (renderer.isVisible == false){
            PoolManager.Pools["BulletPool"].Despawn(_xform);
        }
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