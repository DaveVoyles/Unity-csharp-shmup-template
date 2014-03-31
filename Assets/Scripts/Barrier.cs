// Barriers catch objects which have left the screen (such as bullets or enemies)
// and puts them back in the correct stack for later re-use

using PathologicalGames;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private Transform _playerBulletInstance;

    // a bullet has entered this barrier - check what type it is and put it back on the correct stack
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))                                 // was this a player bullet?
        {
            // put the bullet back on the stack for later re-use
             PoolManager.Pools["BulletPool"].Despawn(other.transform);
        }
        else if (other.CompareTag("EnemyBullet"))                             // was this an enemy bullet?
        {

                // CODE FOR ENEMY BULLET
        }
    }
}