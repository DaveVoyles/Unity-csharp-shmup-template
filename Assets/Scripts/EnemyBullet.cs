using System;
using PathologicalGames;
using UnityEngine;
using System.Collections;

public class EnemyBullet : MonoBehaviour
{
    public Vector3 velocity; // the velocity for this bullet, defined when it's shot

    private Transform _xform; // cached transform for performance
    private int _dmg           = 1; // How much dmg does it do to objects it hits?
    private String _bulletPool = "BulletPool";
    private float _lifespan    = 3;

    private void Start()
    {
        _xform = transform;
    }

    public void OnSpawned()  
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
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<Player>();
            player.KillPlayer(this.collider);
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
