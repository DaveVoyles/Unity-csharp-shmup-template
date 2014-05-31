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
    /// Trying to reset velocity on awake, but not working....
    /// </summary>
    //private void Awake()
    //{
    //    rigidbody.velocity = Vector3.zero;
    //    rigidbody.angularVelocity = Vector3.zero;
    //}


    /// <summary>
    /// De-spawn bullet if it is out of range of camera
    /// </summary>
    private void Update()
    {
        if (renderer.isVisible == false){
            PoolManager.Pools["BulletPool"].Despawn(this.transform);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Hit an enemy
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(this._dmg);
            // put the bullet back on the stack for later re-use
            PoolManager.Pools[_bulletPool].Despawn(this.transform);
        }
    }

}