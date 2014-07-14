/* Handles all spawning routines for all enemies. Transforms are grabbed from the object pool, and returned after destruction
 * Contains all of the functions and types of spawning that can occur.
 * SpawnTimer manages how frequently enemies spawn, based on difficulty.
 * 
 * @Author: Dave Voyles - May 2014
 */

using PathologicalGames;
using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public int pathSpawnAmount                = 1;
    public float pathSpawnInterval            = 1f;
    public float stationaryEnemyInterval      = 1f;
    public float incrementSpawnInterval       = 1.1f;
    public float timeToRunPath                = 4.5f;
    public iTween.EaseType pathEaseType;
    public bool canSpawnPath                 = false;
    public bool canSpawnRandomPath           = false;
    public bool canSpawnStationary           = false;
    public bool canSpawnDrones               = false;
    public bool canSpawnGroup                = false;
    public bool canSpawnIncrementally        = false;
    public int stationaryEnemyAmount         = 1;
    public int numOfEnemiesToSpawnInGroup    = 4;
    public SwarmBehavior swarmBehavior       = null;
    public bool canSpawnPickup               = false;
    public Transform powerup                 = null;
    [HideInInspector]
    public int numOfEnemiesInScene           = 0;
 
    private SpawnPool _pool                  = null;
    private int _spawnSphereRadius           = 3;
    [SerializeField]
    private Transform _enemySpawnPointXform  = null;
    private Transform _xForm                 = null;
    private string _pathOne                  = "path1";
    private const string _BULLET_POOL_STRING = "BulletPool";
    private float _bulletSpeed               = -16f;  // neg, so that it goes from right to left

    // Expose these to the editor
    [SerializeField]
    private Transform              pathEnemyXform    = null;
    [SerializeField]
    private Transform              enemyXform        = null;
    [SerializeField]
    private Transform              droneXform        = null;
    [SerializeField]
    private ScreenRelativePosition screenRelativePos = null;
    [SerializeField]
    private GameManager            _gm               = null;

    //TODO: Make these private?
    public ParticleEffectsManager  _particleManager  = null;
    [SerializeField]
    public SpawnPool               spawnPool         = null;
    [SerializeField] 
    public Transform               _playerXform      = null;

    

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
        _pool                     = PoolManager.Pools[_BULLET_POOL_STRING];
        swarmBehavior             = GameObject.Find("SwarmBehaviorPrefab").GetComponent<SwarmBehavior>();

        // SWARM BEHAVIORS
        // Create a new swarm behavior if it is null
        if (swarmBehavior != null) return;
        var swarmObject           = new GameObject();
        swarmObject.AddComponent<SwarmBehavior>();
        swarmObject.AddComponent<ParticleEffectsManager>();
        swarmBehavior             = swarmObject.GetComponent<SwarmBehavior>();
    }

    /// <summary>
    /// DEBUG booleans to control which enemies / groups can and can't spawn
    /// </summary>
    private void ToggleWhichEnemiesCanSpawn()
    {
        //if (canSpawnPath){
        //    StartCoroutine(SpawnOnAPath(pathEnemyXform, 1, timeToRunPath, pathSpawnInterval, _pathOne));
        //}
        //if (canSpawnRandomPath){
        //    StartCoroutine(SpawnEnemyOnRandomPath(enemyXform, 3, timeToRunPath, pathSpawnInterval));
        //}
        if (canSpawnStationary){
            StartCoroutine(SpawnStationaryEnemy(3,3,_bulletSpeed));
        }
        if (canSpawnGroup){
            SpawnGroup(enemyXform, numOfEnemiesToSpawnInGroup);
        }
        if (canSpawnIncrementally){
            StartCoroutine(SpawnEnemiesIncrementally(enemyXform, 3, pathSpawnInterval));
        }
        if (canSpawnDrones){
            StartCoroutine(swarmBehavior.InstantiateDrones());
        }
        if (canSpawnPickup){
            _pool.Spawn(powerup);
        }
    }


    //----------------------------------------------------------------------------------------------
    //------------------------------------- Path Enemies--------------------------------------------


    /// <summary>
    /// Spawn enemies on the path, incrementally
    /// </summary>
    /// <param name="enemyTransform">Which type should we spawn?</param>
    /// <param name="numToSpawn">How many?</param>
    /// <param name="speed">How quickly should it run this path?</param>
    /// <param name="interval">How much separation between enemy spawns?</param>
    /// <param name="pathName">which path should we spawn on?</param>
    //public IEnumerator SpawnOnAPath(Transform enemyTransform, int numToSpawn, float speed, float interval, string pathName)
    //{
    //    // How many enemies should we spawn?
    //    var count = numToSpawn;
    //    while (count > 0)
    //    {
    //        // Grab an instance of the enemy transform
    //        var pathEnemyInstance = _pool.Spawn(enemyTransform);
    //        var iTweenMoveToPath  = pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

    //        // Enemy follows this path
    //        iTweenMoveToPath.FollowPath(pathName, speed, pathEaseType);
    //        count--;

    //        // Add enemy to the list, so that we can track how many are on screen at once
    //        numOfEnemiesInScene++;

    //        // call this function, every (x) seconds
    //        yield return new WaitForSeconds(interval);
    //    }
    //}


    /// <summary>
    /// Spawns enemies on a random path 
    /// </summary>
    /// <param name="enemyTransform">Which type should we spawn?</param>
    /// <param name="numToSpawn">How many?</param>
    /// <param name="speed">How quickly should it run this path?</param>
    /// <param name="interval">How much separation between enemy spawns?</param>
    //public IEnumerator SpawnEnemyOnRandomPath(Transform enemyTransform, int numToSpawn, float speed, float interval)
    //{
    //    // How many enemies should we spawn?
    //    var count = numToSpawn;
    //    while (count > 0)
    //    {
    //        // Grab an instance of the enemy transform
    //        var pathEnemyInstance = _pool.Spawn(enemyTransform);
    //        var iTweenMoveToPath  = pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

    //        // Enemy follows this path
    //        iTweenMoveToPath.FollowRandomPath();
    //        count--;

    //        // Add enemy to the list, so that we can track how many are on screen at once
    //        numOfEnemiesInScene++;

    //        // call this function, every (x) seconds
    //        yield return new WaitForSeconds(interval);
    //    }

    //}

    //----------------------------------------------------------------------------------------------
    //---------------------------------- Spawning Functions ----------------------------------------


    /// <summary>
    /// Spawns a stationary enemy
    /// </summary>
    /// <param name="delay">Time between shots</param>
    /// <param name="numOfBullets">How many bullets should we shoot?></param>
    /// <param name="bulletSpeed">How fast should the bullets go?</param>
    public IEnumerator SpawnStationaryEnemy(float delay, int numOfBullets, float bulletSpeed)
    {
        MoveSpawnPoint();

        // How many enemies should we spawn?
        var count = stationaryEnemyAmount;
        while (count > 0)
        {
            // spawn an enemy off screen at a random X position and set hit points
            var randomY       = Random.Range(-4.0f, 4.0f);

            // Grabs current instance of enemy, by retrieving enemy prefab from spawn pool
            var enemyInstance = _pool.Spawn(enemyXform);

            // position then set velocity
            enemyInstance.gameObject.transform.position = new Vector3(10, randomY, 0);

            // Grab the "enemy" script, so that we can access the variables exposed
            var enemyScript   = enemyInstance.GetComponent<Enemy>();

            // waits a few seconds then shoots
            StartCoroutine(enemyScript.ShootTowardPlayer(delay, numOfBullets, bulletSpeed));

            // call this function recursively, every (x) seconds
            yield return new WaitForSeconds(stationaryEnemyInterval);
            count--;

            // Add enemy to the list, so that we can track how many are on screen at once
            numOfEnemiesInScene++;
        }
    }


    /// <summary>
    /// Spawns waves of enemies using enemyTypeXform at a set interval
    /// </summary>
    /// <param name="enemyXform">Type of enemy to spawn. Default is Xform used in SpawnMananger</param>
    /// <param name="numToSpawn">How many enemies do we create?</param>
    /// <param name="interval">How often do they spawn?</param>
    public IEnumerator SpawnEnemiesIncrementally(Transform enemyXform, int numToSpawn, float interval)
    {
        // Resets spawn point to new location
        MoveSpawnPoint();

        // Create particles at spawn location
        _particleManager.CreateSpawnEffects(enemyXform.position);

        // How many enemies should we spawn?
        var count = numToSpawn;
        while (count > 0)
        {
            // Spawn an enemy & set spawn location within spawn radius
            SpawnEnemiesWithinSphere(enemyXform);
            count--;

            // Add enemy to the list, so that we can track how many are on screen at once
            numOfEnemiesInScene++;

            // call this function recursively, every (x) seconds
            yield return new WaitForSeconds(interval);
        }
    }


    /// <summary>
    /// Immediately Spawn _number ofEnemies within the size of _spawnSphereRadius
    /// </summary>
    /// <param name="enemyXform">Type of enemy to spawn. Default is Xform used in SpawnMananger</param>
    /// <param name="numToSpawn">How many enemies do we create?</param>
    public void SpawnGroup(Transform enemyXform, int numToSpawn)
    {
        // Resets spawn point to new location
        MoveSpawnPoint();

        // Create particles at spawn location
        _particleManager.CreateSpawnEffects(enemyXform.position);

        // Spawn some enemies
        for (var i = 0; i < numToSpawn; i++){
            SpawnEnemiesWithinSphere(enemyXform);
        }
    }


    //----------------------------------------------------------------------------------------------
    //---------------------------- Private Support Functions ---------------------------------------

    /// <summary>
    /// Spawns enemies within a sphere radius, & locked to player-Z pos  
    /// </summary>
    /// <param name="enemyXform">Transform type for the enemy</param>
    private void SpawnEnemiesWithinSphere(Transform enemyXform)
    {
        // Spawn enemy
        var enemyInstance = spawnPool.Spawn(enemyXform);

        // Set spawn location
        var newPos                                    = Random.insideUnitSphere * _spawnSphereRadius;
        newPos.z                                      = 0;
        enemyInstance.transform.position              = newPos;
    }


    /// <summary>
    /// Relocates spawn point to random location on the map, relative to the camera's position
    /// </summary>
    private void MoveSpawnPoint()
    {
        // Get relative position script and set X & Y offset values 
       var relativePos     = screenRelativePos;
       relativePos.xOffset = Random.Range(-25f, -10f);
       relativePos.yOffset = Random.Range(-7f, 7f);

        // Move the camera, now that we have offsets
        relativePos.CalculatePosition();
    }



}
