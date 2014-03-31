using UnityEngine;

public class Bullet : MonoBehaviour
{


    private Transform _bulletTransform; // cached transform for performance
    public Vector3    velocity;         // the velocity for this bullet, defined when it's shot

    void Start()
    {
        _bulletTransform = transform;
    }

    void Update()
    {
        // move the bullet
        _bulletTransform.position += (velocity * Time.deltaTime);
    }

}