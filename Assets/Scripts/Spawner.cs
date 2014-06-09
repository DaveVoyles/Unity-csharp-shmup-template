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
    public int pathSpawnAmount                = 1;
    public int pathOneSpawnAmount             = 1;
    public int pathTwoSpawnAmount             = 1;
    public int pathThreeSpawnAmount           = 1;  
    public float pathOneSpawnInterval         = 1f;
    public float pathTwoSpawnInterval         = 1f;
    public float pathThreeSpawnInterval       = 1f;
    public float pathSpawnInterval            = 1f;
    public float stationaryEnemyInterval      = 1f;
    public float incrementSpawnInterval       = 1.1f;
    public float timeToRunPath                = 4.5f;
    public float timeToRunPathOne             = 4.5f;
    public float timeToRunPathTwo             = 4.5f;
    public float timeToRunPathThree           = 4.5f;
    public iTween.EaseType pathEaseType;
    public iTween.EaseType pathOneEaseType;
    public iTween.EaseType pathTwoEaseType;
    public iTween.EaseType pathThreeEaseType;
    public bool canSpawnPath                 = false;
    public bool canSpawnPathOne              = false;
    public bool canSpawnPathTwo              = false;
    public bool canSpawnPathThree            = false;
    public bool canSpawnRandomPath           = false;
    public bool canSpawnStationary           = false;
    public bool canSpawnDrones               = false;
    public bool canSpawnGroup                = false;
    public bool canSpawnIncrementally        = false;
    public int stationaryEnemyAmount         = 1;
    public int numOfEnemiesToSpawnInGroup    = 4;

    public bool canSpawnPickup               = false;
    public Transform powerup                 = null;
 
    private SpawnPool _pool                         = null;
    private int _spawnSphereRadius                  = 3;
    private ParticleEffectsManager _particleManager = null;
    private Transform _playerXform                  = null;
    private Transform _enemySpawnPointXform         = null;
    private SwarmBehavior _swarmBehavior            = null;
    private Transform _xForm                        = null;
    private string _pathName                        = "path1";

    public enum pathNames
    {

    }

    /// <summary>
    /// Make the pool's group a child of this transform for demo purposes
    /// Set the pool group's local position & rotation
    /// </summary>
	void Start ()
	{
        SetProperties();
        ToggleWhichEnemiesCanSpawn();
 }

    /// <summary>
    /// Instantiates the values for all private properties
    /// </summary>
    private void SetProperties()
    {
        _xForm                    = transform;
        _pool                     = GameObject.Find("GameManager").GetComponent<GameManager>().BulletPool;
        _pool.group.parent        = gameObject.transform;
        _pool.group.localPosition = new Vector3(1.5f, 0, 0);
        _pool.group.localRotation = Quaternion.identity;
        _playerXform              = GameObject.Find("Player").transform;
        _enemySpawnPointXform     = GameObject.Find("EnemySpawnPoint").transform;
        _particleManager          = GameObject.Find("ParticleManager").GetComponent<ParticleEffectsManager>();
        _swarmBehavior            = GameObject.Find("SwarmBehaviorPrefab").GetComponent<SwarmBehavior>();

        // Create a new swarm behavior if it is null
        if (_swarmBehavior != null) return;
        var swarmObject = new GameObject();
        swarmObject.AddComponent<SwarmBehavior>();
        swarmObject.AddComponent<ParticleEffectsManager>();
        _swarmBehavior = swarmObject.GetComponent<SwarmBehavior>();
    }

    /// <summary>
    /// A boolean controls which enemies / groups can and can't spawn
    /// </summary>
    private void ToggleWhichEnemiesCanSpawn()
    {
        if (canSpawnPath){
            StartCoroutine(SpawnOnAPath());
        }
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
        if (canSpawnStationary){
            StartCoroutine(SpawnStationaryEnemy());
        }
        if (canSpawnGroup){
            SpawnGroup();
        }
        if (canSpawnIncrementally){
            StartCoroutine(SpawnEnemiesIncrementally());
        }
        if (canSpawnDrones){
            StartCoroutine(_swarmBehavior.InstantiateDrones());
        }
        if (canSpawnPickup){
            _pool.Spawn(powerup);
        }
    }


    /// <summary>
    /// Spawn an (pathOneSpawnAmount) instances of  every pathOneSpawnInterval
    /// </summary>
    private IEnumerator SpawnPathOne()
    {
        // How many enemies should we spawn?
        var count = pathOneSpawnAmount;
        while (count > 0)
        {
            // Grab an instance of the enemy transform
            var pathEnemyInstance = _pool.Spawn(pathEnemyXform);
            var iTweenMoveToPath  = pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

            // Enemy follows this path
            iTweenMoveToPath.FollowPathOne(timeToRunPathOne, pathOneEaseType);
            count--;
            
            // call this function, every (x) seconds
            yield return new WaitForSeconds(pathOneSpawnInterval);
        }
    }

    /// <summary>
    /// Spawn an (pathOneSpawnAmount) instances of  every pathTwoSpawnInterval
    /// </summary>
    private IEnumerator SpawnPathTwo()
    {
        // How many enemies should we spawn?
        var count = pathTwoSpawnAmount;
        while (count > 0)
        {
            // Grab an instance of the enemy transform
            var pathEnemyInstance = _pool.Spawn(pathEnemyXform);
            var iTweenMoveToPath  = pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

            // Enemy follows this path
            iTweenMoveToPath.FollowPathTwo(timeToRunPathTwo, pathTwoEaseType);
            count--;

            // call this function, every (x) seconds
            yield return new WaitForSeconds(pathTwoSpawnInterval);
        }
    }

    /// <summary>
    /// Spawn an (pathOneSpawnAmount) instances of  every pathThreeSpawnInterval
    /// </summary>
    private IEnumerator SpawnPathThree()
    {
        // How many enemies should we spawn?
        var count = pathThreeSpawnAmount;
        while (count > 0)
        {
            // Grab an instance of the enemy transform
            var pathEnemyInstance = _pool.Spawn(pathEnemyXform);
            var iTweenMoveToPath  = pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

            // Enemy follows this path
            iTweenMoveToPath.FollowPathThree(timeToRunPathThree, pathThreeEaseType);
            count--;

            // call this function, every (x) seconds
            yield return new WaitForSeconds(pathThreeSpawnInterval);
        }
    }

    private IEnumerator SpawnOnAPath()
    {
        // How many enemies should we spawn?
        var count = pathSpawnAmount;
        while (count > 0)
        {
            // Grab an instance of the enemy transform
            var pathEnemyInstance = _pool.Spawn(pathEnemyXform);
            var iTweenMoveToPath  = pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

            // Enemy follows this path
            iTweenMoveToPath.FollowPath(_pathName, timeToRunPath, pathEaseType);
            count--;

            // call this function, every (x) seconds
            yield return new WaitForSeconds(pathSpawnInterval);
        }
    }

    /// <summary>
    /// Spawns a stationary enemy
    /// </summary>
    private IEnumerator SpawnStationaryEnemy()
    {
        MoveSpawnPoint();
        // How many enemies should we spawn?
        var count = stationaryEnemyAmount;
        while (count > 0)
        {
            // spawn an enemy off screen at a random X position and set hit points
            var randomY = Random.Range(-4.0f, 4.0f);

            // Grabs current instance of enemy, by retrieving enemy prefab from spawn pool
            var enemyInstance = _pool.Spawn(enemyXform);

            // position then set velocity
            enemyInstance.gameObject.transform.position = new Vector3(10, randomY, 0);

            // Grab the "enemy" script, so that we can access the variables exposed
            var enemyScript = enemyInstance.GetComponent<Enemy>();

            // waits a few seconds then shoots
            var shootDelay = Random.Range(0.5f, 2.0f);
            StartCoroutine(enemyScript.ShootTowardPlayer(shootDelay));

            // Wait 3 seconds, then call this function again 
            yield return new WaitForSeconds(stationaryEnemyInterval);
            count--;
        }
    }

    /// <summary>
    /// Spawns enemies on a random path at random intervals
    /// </summary>
    private void SpawnEnemyOnRandomPath()
    {
        var pathEnemyInstance = _pool.Spawn(pathEnemyXform);
        var iTweenMoveToPath  = pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

        iTweenMoveToPath.FollowRandomPath();
    }

    /// <summary>
    /// Spawns waves of enemies using enemyTypeXform at a set interval
    /// </summary>
    private IEnumerator SpawnEnemiesIncrementally()
    {
        MoveSpawnPoint();
        _particleManager.CreateSpawnEffects(_xForm.position);
        // How many enemies should we spawn?
        var count = numOfEnemiesToSpawnInGroup;
        while (count > 0)
        {
            // Spawn an enemy & set spawn location within spawn radius
            SpawnEnemiesWithinSphere();
            count--;

            yield return new WaitForSeconds(incrementSpawnInterval);
        }
    }

    /// <summary>
    /// Immediately Spawn _number ofEnemies within the size of _spawnSphereRadius
    /// </summary>
    private void SpawnGroup()
    {
        MoveSpawnPoint();
        _particleManager.CreateSpawnEffects(_xForm.position);
        for (var i = 0; i < numOfEnemiesToSpawnInGroup; i++){
            SpawnEnemiesWithinSphere();
        }
    }

    /// <summary>
    /// Spawns enemies within a sphere radius, & locked to player-Z pos  
    /// </summary>
    private void SpawnEnemiesWithinSphere()
    {
        var enemyInstance                = _pool.Spawn(enemyXform);
        var newPos                       = Random.insideUnitSphere * _spawnSphereRadius;
        newPos.z                         = _playerXform.position.z;
        enemyInstance.transform.position = newPos;
    }


    /// <summary>
    /// Relocates enemy spawn point to random location on the map, relative to the camera's position
    /// </summary>
    private void MoveSpawnPoint()
    {
        // Get relative position script and set X & Y offset values 
       var relativePos     = _enemySpawnPointXform.gameObject.GetComponent<ScreenRelativePosition>();
       relativePos.xOffset = Random.Range(-25f, -10f);
       relativePos.yOffset = Random.Range(-7f, 7f);

        // Move the camera, now that we have offsets
        relativePos.CalculatePosition();
    }



}
