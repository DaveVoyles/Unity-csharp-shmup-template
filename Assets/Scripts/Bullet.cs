using UnityEngine;

public class Bullet : MonoBehaviour
{


    Transform        myTransform; // cached transform for performance
    public Vector3   motion;      // the velocity for this bullet, defined when it's shot

    void Start()
    {
        myTransform = transform;
    }

    void Update()
    {
        // move the bullet
        myTransform.position += (motion * Time.deltaTime);
    }

}