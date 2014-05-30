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
    private float _lifespan    = 3;

    private void Start()
    {
        _xform = transform;
    }

    public void OnSpawned()  // Might be able to make this private and still work?
    {
        // Start the timer as soon as this instance is spawned.
        this.StartCoroutine(this.TimedDespawn());
    }

    private IEnumerator TimedDespawn()
    {
        // Wait for 'lifespan' (seconds) then despawn this instance.
        yield return new WaitForSeconds(this._lifespan);

        PoolManager.Pools["BulletPool"].Despawn(this.transform);
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

        // Hit a bullet barrier
        if (other.gameObject.CompareTag("BulletCollectors"))
        {
            // put the bullet back on the stack for later re-use
            PoolManager.Pools[_bulletPool].Despawn(this.transform);
        }
    }

}