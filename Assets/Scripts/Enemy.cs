using PathologicalGames;
using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int             hitPoints;						// assigned when the enemy spawns
    public Vector3         motion;							// assigned when the enemy spawns

    private Transform      _enemyTransform;                 // current transform of enemy, cached for perf during init
    private GameObject     _gameManager;                    // Need instance (NOT static) of GM to grab bullet from pool
    private float          _enemyBulletSpeed;
    private SoundManager   _soundManager;
    private Transform      _enemyBulletPrefab;

    void Start()
    {
        _enemyTransform    = transform;				         // cached for performance
        _enemyBulletSpeed  = 6;                              // How fast enemy bullets fly
        _gameManager       = GameObject.Find("GameManager"); // store the game manager for accessing its functions
        _enemyBulletPrefab = PoolManager.Pools["BulletPool"].prefabs["EnemyBulletPrefab"];
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

            // put the bullet back on the stack for later re-use
            PoolManager.Pools["BulletPool"].Despawn(other.transform);
        }
    }

    void TakeDamage(int damage)
    {
        // subtract damage and check if it's dead
        hitPoints -= damage;
        if (hitPoints <= 0)
            Explode();
    }

    public void Explode() // destroy this enemy
    {
        if (gameObject != null)
        {
            // TODO: play sound & particle effect
            // TODO: Add pooling for enemies as well
            Destroy(gameObject);
        }

        // increment the score
        GameManager.score++;
    }


    public IEnumerator ShootTowardPlayer(float delay) // waits for 'delay' seconds, then shoots directly at the player
    {
        yield return new WaitForSeconds(delay);

        // Grabs current instance of bullet
        Transform _enemyBulletInstance = PoolManager.Pools["BulletPool"].Spawn(_enemyBulletPrefab);

        _enemyBulletInstance.gameObject.transform.position = _enemyTransform.position;
        _enemyBulletInstance.gameObject.SetActive(true);

        // calculate the direction to the player
        var shootVector = _gameManager.GetComponent<GameManager>().player.transform.position - _enemyTransform.position;

        // normalize this vector (make its length 1)
        shootVector.Normalize();

        // scale it up to the correct speed
        shootVector *= _enemyBulletSpeed;
        _enemyBulletInstance.gameObject.GetComponent<Bullet>().velocity = shootVector;
    }
}