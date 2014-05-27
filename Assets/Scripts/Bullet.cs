using PathologicalGames;
using UnityEngine;

public class Bullet : MonoBehaviour
{


    private Transform _bulletTransform; // cached transform for performance
    public Vector3    velocity;         // the velocity for this bullet, defined when it's shot

    private float _lifeTime = 2.0f;
    private float _spawnTime = 0.0f;
    
    private void Start()
    {
        _bulletTransform = transform;
    }

    private void OnEnable()
    {
        _spawnTime = Time.time;
    }

    //private void Update()
    //{
    //    // If bullet has been alive for more than (x) seconds, than despawn
    //    if (Time.time > _spawnTime + _lifeTime)
    //    {
    //        PoolManager.Pools["BulletPool"].Despawn(this._bulletTransform);
    //    }
    //}

    ///<summary>
    ///   Check what we are colliding with
    ///   </summary>
    void OnCollisionEnter(Collision other)
    {
        // was this a bullet?
        if (other.gameObject.CompareTag("BulletCollectors"))                                 
        {
            print("hit Bullet Collector");
            // put the bullet back on the stack for later re-use
             PoolManager.Pools["BulletPool"].Despawn(this._bulletTransform);
        }
    }



}