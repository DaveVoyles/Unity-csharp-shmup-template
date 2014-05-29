/* Barriers catch objects which have left the screen (such as bullets or enemies)
 * and puts them back in the correct stack for later re-use
 */

using PathologicalGames;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    // a bullet has entered this barrier -- put it back on the correct stack
    //void OnTriggerEnter(Collider other)
    //{
    //    print("something hit me");
    //    print(other.tag);
    //    if (other.gameObject.name == "PlayerBulletPrefab")
    //    {
    //        // put the bullet back on the stack for later re-use
    //        PoolManager.Pools["BulletPool"].Despawn(other.transform);
    //        print("Barrier: Bullet hit me");
    //    }

    //    // was this a bullet?
    //    if (other.CompareTag("PlayerBullet") || other.CompareTag("EnemyBullet"))                                 
    //    {
    //        // put the bullet back on the stack for later re-use
    //         PoolManager.Pools["BulletPool"].Despawn(other.transform);
    //        print("Barrier: Bullet hit me");
    //    }
    //}

}