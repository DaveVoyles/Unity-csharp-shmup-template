using UnityEngine;

public class Bullet : MonoBehaviour
{


    private Transform _bulletTransform; // cached transform for performance
    public Vector3    Velocity;         // the velocity for this bullet, defined when it's shot

    void Start()
    {
        _bulletTransform = transform;
    }

    void Update()
    {
        // move the bullet
        _bulletTransform.position += (Velocity * Time.deltaTime);
    }

}