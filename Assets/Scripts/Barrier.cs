// Barriers catch objects which have left the screen (such as bullets or enemies)
// and puts them back in the correct stack for later re-use

using UnityEngine;

public class Barrier : MonoBehaviour
{
    // a bullet has entered this barrier - check what type it is and put it back on the correct stack
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))                                 // was this a player bullet?
        {
            // put the bullet back on the stack for later re-use
            GameManager.PlayerBulletStack.Push(other.GetComponent<Bullet>()); // push the Bullet component, not the collider
            other.gameObject.SetActive(false);                                // deactivate the bullet
        }
        else if (other.CompareTag("EnemyBullet"))                             // was this an enemy bullet?
        {
            GameManager.EnemyBulletStack.Push(other.GetComponent<Bullet>());
            other.gameObject.SetActive(false);                                // deactivate the bullet
        }
    }
}