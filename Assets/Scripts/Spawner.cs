/* Handles all spawning routines for all enemies. Transforms are grabbed from the object pool, and returned after destruction
 * Author: Dave Voyles
 * Date: May 2014
 */
using PathologicalGames;
using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
    
    public Transform pathEnemyTransform;
    public Transform enemyTransform;            
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
    public bool bSpawnPathOne = false;
    public bool bSpawnPathTwo = false;
    public bool bSpawnPathThree = false;
    public bool bSpawnRandomPath = false;

    private string _nameOfPool = "BulletPool";  
    private SpawnPool _pool    = null;
    private bool _isSpawning   = true; // Continue to spawn until told otherwise
 

    /// <summary>
    /// Make the pool's group a child of this transform for demo purposes
    /// Set the pool group's local position & rotation
    /// </summary>
	void Start ()
	{
        this._pool                     = PoolManager.Pools[_nameOfPool];
        this._pool.group.parent        = gameObject.transform;
        this._pool.group.localPosition = new Vector3(1.5f, 0, 0);
        this._pool.group.localRotation = Quaternion.identity;

        if (bSpawnPathOne){
            this.StartCoroutine(SpawnPathOne());
        }
        if (bSpawnPathTwo){
            this.StartCoroutine(SpawnPathTwo());
        }
        if (bSpawnPathThree){
            this.StartCoroutine(SpawnPathThree());
        }
        if (bSpawnRandomPath){
            this.SpawnEnemyOnRandomPath();
        }
        this.StartCoroutine(SpawnStationaryEnemy());
	}
	
    /// <summary>
    /// Spawn an (this.pathOneSpawnAmount) instances of  every this.pathOneSpawnInterval
    /// </summary>
    private IEnumerator SpawnPathOne()
    {
        // How many enemies should we spawn?
        int count = this.pathOneSpawnAmount;
        while (count >= 0)
        {
            // Grab an instance of the enemy transform
            var _pathEnemyInstance = this._pool.Spawn(pathEnemyTransform);
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
        int count = this.pathTwoSpawnAmount;
        while (count >= 0)
        {
            // Grab an instance of the enemy transform
            var _pathEnemyInstance = this._pool.Spawn(pathEnemyTransform);
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
        int count = this.pathThreeSpawnAmount;
        while (count >= 0)
        {
            // Grab an instance of the enemy transform
            var _pathEnemyInstance = this._pool.Spawn(pathEnemyTransform);
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
        while (_isSpawning)
        {
            // spawn an enemy off screen at a random X position and set hit points
            var randomY = Random.Range(-4.0f, 4.0f);

            // Grabs current instance of enemy, by retrieving enemy prefab from spawn pool
            Transform _enemyInstance = this._pool.Spawn(enemyTransform); ;

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
        }
    }

    /// <summary>
    /// Spawns enemies on a random path at random intervals
    /// </summary>
    private void SpawnEnemyOnRandomPath()
    {
        var _pathEnemyInstance = PoolManager.Pools[_nameOfPool].Spawn(pathEnemyTransform);
        var _iTweenMoveToPath  = _pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

        _iTweenMoveToPath.FollowRandomPath();
    }   
  
    
}
