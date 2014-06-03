/* Handles all spawning routines for all enemies. Transforms are grabbed from the object pool, and returned after destruction
 * Author: Dave Voyles
 * Date: May 2014
 */
using PathologicalGames;
using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
    
    public Transform pathEnemyXform;
    public Transform enemyXform;
    public Transform droneXform;     
    public int pathOneSpawnAmount             = 0;
    public int pathTwoSpawnAmount             = 0;
    public int pathThreeSpawnAmount           = 0;  
    public float pathOneSpawnInterval         = 0f;
    public float pathTwoSpawnInterval         = 0f;
    public float pathThreeSpawnInterval       = 0f;
    public float stationaryEnemyInterval      = 0f;
    public float timeToRunPathOne             = 0;
    public float timeToRunPathTwo             = 0;
    public float timeToRunPathThree           = 0;
    public iTween.EaseType pathOneEaseType;
    public iTween.EaseType pathTwoEaseType;
    public iTween.EaseType pathThreeEaseType;
    public bool canSpawnPathOne              = false;
    public bool canSpawnPathTwo              = false;
    public bool canSpawnPathThree            = false;
    public bool canSpawnRandomPath           = false;
    public bool canSpawningStationary        = true;
    public bool canSpawningDrones            = true;
    public bool canSpawnImmediately          = true;
    public int spawnAmount                   = 5;
    public float IncrementSpawnInterval      = 1.1f;


    private string _nameOfPool = "BulletPool";  
    private SpawnPool _pool            = null;

    public int numberOfEnemiesToSpawn = 10;
    private int _spawnSphereRadius = 3;
    private Transform _playerXform;
    private Transform _enemySpawnPointXform;

    /// <summary>
    /// Make the pool's group a child of this transform for demo purposes
    /// Set the pool group's local position & rotation
    /// </summary>
	void Start ()
	{
        _pool                     = PoolManager.Pools[_nameOfPool];
        _pool.group.parent        = gameObject.transform;
        _pool.group.localPosition = new Vector3(1.5f, 0, 0);
        _pool.group.localRotation = Quaternion.identity;
        _playerXform              = GameObject.Find("Player").transform;
        _enemySpawnPointXform     = GameObject.Find("EnemySpawnPoint").transform;


        if (canSpawnPathOne){
            StartCoroutine(SpawnPathOne());
        }
        if (canSpawnPathTwo){
            StartCoroutine(SpawnPathTwo());
        }
        if (canSpawnPathThree){
            StartCoroutine(SpawnPathThree());
        }
        if (canSpawnRandomPath){
            SpawnEnemyOnRandomPath();
        }
        if (canSpawningStationary){
            StartCoroutine(SpawnStationaryEnemy());
        }
        if (canSpawnImmediately){
            SpawnEnemiesImmediately();
        }
	}
	
    /// <summary>
    /// Spawn an (this.pathOneSpawnAmount) instances of  every this.pathOneSpawnInterval
    /// </summary>
    private IEnumerator SpawnPathOne()
    {
        // How many enemies should we spawn?
        var count = this.pathOneSpawnAmount;
        while (count >= 0)
        {
            // Grab an instance of the enemy transform
            var _pathEnemyInstance = this._pool.Spawn(pathEnemyXform);
            var _iTweenMoveToPath  = _pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

            // Enemy follows this path
            _iTweenMoveToPath.FollowPathOne(this.timeToRunPathOne, this.pathOneEaseType);
            count--;
            
            // call this function, every (x) seconds
            yield return new WaitForSeconds(this.pathOneSpawnInterval);
        }
    }

    /// <summary>
    /// Spawn an (this.pathOneSpawnAmount) instances of  every this.pathTwoSpawnInterval
    /// </summary>
    private IEnumerator SpawnPathTwo()
    {
        // How many enemies should we spawn?
        var count = this.pathTwoSpawnAmount;
        while (count >= 0)
        {
            // Grab an instance of the enemy transform
            var _pathEnemyInstance = this._pool.Spawn(pathEnemyXform);
            var _iTweenMoveToPath  = _pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

            // Enemy follows this path
            _iTweenMoveToPath.FollowPathTwo(this.timeToRunPathTwo, this.pathTwoEaseType);
            count--;

            // call this function, every (x) seconds
            yield return new WaitForSeconds(this.pathTwoSpawnInterval);
        }
    }

    /// <summary>
    /// Spawn an (this.pathOneSpawnAmount) instances of  every this.pathThreeSpawnInterval
    /// </summary>
    private IEnumerator SpawnPathThree()
    {
        // How many enemies should we spawn?
        var count = this.pathThreeSpawnAmount;
        while (count >= 0)
        {
            // Grab an instance of the enemy transform
            var _pathEnemyInstance = this._pool.Spawn(pathEnemyXform);
            var _iTweenMoveToPath  = _pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

            // Enemy follows this path
            _iTweenMoveToPath.FollowPathThree(this.timeToRunPathThree, this.pathThreeEaseType);
            count--;

            // call this function, every (x) seconds
            yield return new WaitForSeconds(this.pathThreeSpawnInterval);
        }
    }

    /// <summary>
    /// Spawns a stationary enemy
    /// </summary>
    private IEnumerator SpawnStationaryEnemy()
    {
        while (canSpawningStationary)
        {
            // How many enemies should we spawn?
            int count = this.spawnAmount;
            while (count >= 0)
            {
                // spawn an enemy off screen at a random X position and set hit points
                var randomY = Random.Range(-4.0f, 4.0f);

                // Grabs current instance of enemy, by retrieving enemy prefab from spawn pool
                Transform _enemyInstance = this._pool.Spawn(enemyXform);
                ;

                // position, enable, then set velocity
                _enemyInstance.gameObject.transform.position = new Vector3(10, randomY, 0);
                _enemyInstance.gameObject.SetActive(true);

                // Grab the "enemy" script, so that we can access the variables exposed
                var scriptRef = _enemyInstance.GetComponent<Enemy>();

                // waits a few seconds then shoots
                var shootDelay = Random.Range(0.5f, 2.0f);
                StartCoroutine(scriptRef.ShootTowardPlayer(shootDelay));

                // Wait 3 seconds, then call this function again 
                yield return new WaitForSeconds(stationaryEnemyInterval);
                count--;

            }
        }
    }

    /// <summary>
    /// Spawns enemies on a random path at random intervals
    /// </summary>
    private void SpawnEnemyOnRandomPath()
    {
        var _pathEnemyInstance = PoolManager.Pools[_nameOfPool].Spawn(pathEnemyXform);
        var _iTweenMoveToPath  = _pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

        _iTweenMoveToPath.FollowRandomPath();
    }

    /// <summary>
    /// Spawns waves of enemies using enemyTypeXform at a set interval
    /// </summary>
    private IEnumerator SpawnEnemiesIncrementally()
    {
        // How many enemies should we spawn?
        var count = numberOfEnemiesToSpawn;
        while (count >= 0)
        {
            // Spawn an enemy & set spawn location within spawn radius
            SpawnEnemies();
            count--;

            yield return new WaitForSeconds(IncrementSpawnInterval);
        }
    }

    /// <summary>
    /// Immediately Spawn _number ofEnemies within the size of _spawnSphereRadius
    /// </summary>
    private void SpawnEnemiesImmediately()
    {
        for (var i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            SpawnEnemies();
        }
    }

    /// <summary>
    /// Spawns enemies at a given position 
    /// </summary>
    private void SpawnEnemies()
    {
        var enemyInstance = _pool.Spawn(enemyXform);
        var newPos        = Random.insideUnitSphere * _spawnSphereRadius;
        newPos.z          = _playerXform.position.z;
        enemyInstance.transform.position = newPos;
    }



    
}
