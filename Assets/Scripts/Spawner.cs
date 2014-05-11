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
    public float pathOneSpawnInterval         = 0f;
    public float pathTwoSpawnInterval         = 0f;
    public float pathThreeSpawnInterval       = 0f;
    public float spawnStationaryEnemyInterval = 0f;
    public float timeToRunPathOne             = 0;
    public float timeToRunpathTwo             = 0;
    public float timeToRunPathThree           = 0;
    public iTween.EaseType pathOneEaseType;
    public iTween.EaseType pathTwoEaseType;
    public iTween.EaseType pathThreeEaseType;

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

        this.StartCoroutine(SpawnRoutine());
        this.StartCoroutine(SpawnStationaryEnemy());
        this.SpawnEnemyOnRandomPath();
	}
	
    /// <summary>
    /// Spawn an instance every this.spawnInterval
    /// </summary>
    private IEnumerator SpawnRoutine()
    {
        int count = this.pathOneSpawnAmount;
        while (count >= 0)
        {
            // Grab an instance of the enemy transform
            var _pathEnemyInstance = this._pool.Spawn(pathEnemyTransform);
            var _iTweenMoveToPath  = _pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

            // Enemy follows this path
            _iTweenMoveToPath.FollowPathThree(this.timeToRunPathThree, this.pathOneEaseType);
            count--;
            
            // call this function, every (x) seconds
            yield return new WaitForSeconds(this.pathOneSpawnInterval);
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
            scriptRef.hitPoints = 4;

            // waits a few seconds then shoots
            var shootDelay = Random.Range(0.5f, 2.0f);
            StartCoroutine(scriptRef.ShootTowardPlayer(shootDelay));

            // Wait 3 seconds, then call this function again 
            yield return new WaitForSeconds(spawnStationaryEnemyInterval);
        }
    }

    /// <summary>
    /// Spawns enemies on a random path at random intervals
    /// </summary>
    private void SpawnEnemyOnRandomPath()
    {
        var _pathEnemyInstance = PoolManager.Pools[_nameOfPool].Spawn(pathEnemyTransform);
        var _iTweenMoveToPath = _pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

        _iTweenMoveToPath.FollowRandomPath();
    }   
  




}
