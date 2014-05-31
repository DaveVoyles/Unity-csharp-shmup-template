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

    private void Start()
    {
        _xform = transform;
    }

    private void Update()
    {
        if (renderer.isVisible == false){
            PoolManager.Pools["BulletPool"].Despawn(this.transform);
        }
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
    }
}
