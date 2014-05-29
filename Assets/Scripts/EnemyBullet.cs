using System;
using PathologicalGames;
using UnityEngine;
using System.Collections;

public class EnemyBullet : MonoBehaviour
{

    private Transform _bulletTransform; // cached transform for performance
    public Vector3 velocity; // the velocity for this bullet, defined when it's shot

    private int _dmg = 1; // How much dmg does it do to objects it hits?
    private String _bulletPool = "BulletPool";

    private void Start()
    {
        _bulletTransform = transform;
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
