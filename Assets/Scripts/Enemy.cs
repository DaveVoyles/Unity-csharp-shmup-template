using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int             HitPoints;						// assigned when the enemy spawns
    public Vector3         motion;							// assigned when the enemy spawns
    private Transform      _enemyTransform;                    // current transform of enemy, cached for perf during init
    private GameObject     _gameManager;                    // Need instance (NOT static) of GM to grab bullet from pool
    private float          _enemyBulletSpeed;

    void Start()
    {
        _enemyTransform   = transform;				        // cached for performance
        _enemyBulletSpeed = 6;                              // How fast enemy bullets fly
        _gameManager      = GameObject.Find("GameManager"); // store the game manager for accessing its functions
    }

    void Update()
    {
        _enemyTransform.position += (motion * Time.deltaTime); // move
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))			    // hit by a bullet
        {
            TakeDamage(1);								    // take away 1 hit point

            // disable the bullet and put it back on its stack
            other.gameObject.SetActive(false); 
            GameManager.PlayerBulletStack.Push(other.GetComponent<Bullet>());
        }
    }

    void TakeDamage(int damage)
    {
        // subtract damage and check if it's dead
        HitPoints -= damage;
        if (HitPoints <= 0)
            Explode();
    }

    public void Explode() // destroy this enemy
    {
        // TODO: play sound & particle effect
        // TODO: Add pooling for enemies as well
        Destroy(gameObject);

        // increment the score
        GameManager.Score++;
    }


    public IEnumerator Shoot(float delay) // waits for 'delay' seconds, then shoots directly at the player
    {
        yield return new WaitForSeconds(delay);

        // get a bullet from the stack
        Bullet newBullet = GameManager.EnemyBulletStack.Pop();

        // position and enable it
        if (newBullet != null)
        {
            newBullet.gameObject.transform.position = _enemyTransform.position;
            newBullet.gameObject.SetActive(true);

            // calculate the direction to the player
            var shootVector = _gameManager.GetComponent<GameManager>().player.transform.position - _enemyTransform.position;

            // normalize this vector (make its length 1)
            shootVector.Normalize();

            // scale it up to the correct speed
            shootVector       *= _enemyBulletSpeed;
            newBullet.Velocity = shootVector;
        }
    }
}