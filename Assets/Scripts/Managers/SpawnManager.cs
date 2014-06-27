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
 
    private SpawnPool _pool                  = null;
    private int _spawnSphereRadius           = 3;
    private Transform _enemySpawnPointXform  = null;
    private Transform _xForm                 = null;
    private string _pathOne                  = "path1";

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
        _pool                     = GameObject.Find("GameManager").GetComponent<GameManager>().BulletPool;
        _pool.group.parent        = gameObject.transform;
        _pool.group.localPosition = new Vector3(1.5f, 0, 0);
        _pool.group.localRotation = Quaternion.identity;
        _enemySpawnPointXform     = GameObject.Find("EnemySpawnPoint").transform;
        swarmBehavior             = GameObject.Find("SwarmBehaviorPrefab").GetComponent<SwarmBehavior>();

        // Create a new swarm behavior if it is null
        if (swarmBehavior != null) return;
        var swarmObject = new GameObject();
        swarmObject.AddComponent<SwarmBehavior>();
        swarmObject.AddComponent<ParticleEffectsManager>();
        swarmBehavior   = swarmObject.GetComponent<SwarmBehavior>();
    }

    /// <summary>
    /// DEBUG booleans to control which enemies / groups can and can't spawn
    /// </summary>
    private void ToggleWhichEnemiesCanSpawn()
    {
        if (canSpawnPath){
            StartCoroutine(SpawnOnAPath(pathEnemyXform, 1, timeToRunPath, pathSpawnInterval, _pathOne));
        }
        if (canSpawnRandomPath){
            SpawnEnemyOnRandomPath();
        }
        if (canSpawnStationary){
            StartCoroutine(SpawnStationaryEnemy());
        }
        if (canSpawnGroup){
            SpawnGroup(enemyXform, numOfEnemiesToSpawnInGroup);
        }
        if (canSpawnIncrementally){
            StartCoroutine(SpawnEnemiesIncrementally(enemyXform, numOfEnemiesToSpawnInGroup));
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
    /// <param name="timeBetweenSpawns">How much separation between enemy spawns?</param>
    /// <param name="pathName">which path should we spawn on?</param>
    public IEnumerator SpawnOnAPath(Transform enemyTransform, int numToSpawn, float speed, float timeBetweenSpawns, string pathName)
    {
        // How many enemies should we spawn?
        var count = numToSpawn;
        while (count > 0)
        {
            // Grab an instance of the enemy transform
            var pathEnemyInstance = _pool.Spawn(enemyTransform);
            var iTweenMoveToPath  = pathEnemyInstance.gameObject.GetComponent<iTweenMoveToPath>();

            // Enemy follows this path
            iTweenMoveToPath.FollowPath(pathName, speed, pathEaseType);
            count--;

            // call this function, every (x) seconds
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }


    /// <summary>
    /// Spawns enemies on a random path at random intervals
    /// </summary>
    public void SpawnEnemyOnRandomPath()
    {
        // Spawn enemy and set type
        var enemyInstance = _pool.Spawn(pathEnemyXform);
        enemyInstance.GetComponent<Enemy>().enemyType = Enemy.EnemyType.Path;

        // Move enemy onto the path
        var iTweenMoveToPath = enemyInstance.gameObject.GetComponent<iTweenMoveToPath>();
        iTweenMoveToPath.FollowRandomPath();
    }



    //----------------------------------------------------------------------------------------------
    //------------------------------- Stationary or Moving -----------------------------------------

    /// <summary>
    /// Spawns a stationary enemy
    /// </summary>
    public IEnumerator SpawnStationaryEnemy()
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

            // Set enemy type
            enemyInstance.GetComponent<Enemy>().enemyType = Enemy.EnemyType.WaitForPlayer;

            // position then set velocity
            enemyInstance.gameObject.transform.position = new Vector3(10, randomY, 0);

            // Grab the "enemy" script, so that we can access the variables exposed
            var enemyScript   = enemyInstance.GetComponent<Enemy>();

            // waits a few seconds then shoots
            var shootDelay    = Random.Range(0.5f, 2.0f);
            StartCoroutine(enemyScript.ShootTowardPlayer(shootDelay));

            // call this function recursively, every (x) seconds
            yield return new WaitForSeconds(stationaryEnemyInterval);
            count--;
        }
    }


    /// <summary>
    /// Spawns waves of enemies using enemyTypeXform at a set interval
    /// </summary>
    /// <param name="EnemyXform">Type of enemy to spawn. Default is Xform used in SpawnMananger</param>
    /// <param name="NumOfEnemiesToSpawnInGroup"></param>
    public IEnumerator SpawnEnemiesIncrementally(Transform EnemyXform, int NumOfEnemiesToSpawnInGroup)
    {
        // Resets spawn point to new location
        MoveSpawnPoint();

        // Create particles at spawn location
        _particleManager.CreateSpawnEffects(EnemyXform.position);

        // How many enemies should we spawn?
        var count = NumOfEnemiesToSpawnInGroup;
        while (count > 0)
        {
            // Spawn an enemy & set spawn location within spawn radius
            SpawnEnemiesWithinSphere(EnemyXform);
            count--;

            // call this function recursively, every (x) seconds
            yield return new WaitForSeconds(incrementSpawnInterval);
        }
    }


    /// <summary>
    /// Immediately Spawn _number ofEnemies within the size of _spawnSphereRadius
    /// </summary>
    public void SpawnGroup(Transform EnemyXform, int NumOfEnemiesToSpawnInGroup)
    {
        // Resets spawn point to new location
        MoveSpawnPoint();

        // Create particles at spawn location
        _particleManager.CreateSpawnEffects(_xForm.position);

        // Spawn some enemies
        for (var i = 0; i < NumOfEnemiesToSpawnInGroup; i++){
            SpawnEnemiesWithinSphere(EnemyXform);
        }
    }


    //----------------------------------------------------------------------------------------------
    //---------------------------- Private Support Functions ---------------------------------------


    /// <summary>
    /// Spawns enemies within a sphere radius, & locked to player-Z pos  
    /// </summary>
    private void SpawnEnemiesWithinSphere(Transform EnemyXform)
    {
        // Spawn enemy
        var enemyInstance = spawnPool.Spawn(EnemyXform);

        // Set spawn location
        var newPos                                    = Random.insideUnitSphere * _spawnSphereRadius;
        newPos.z = _playerXform.position.z;
        enemyInstance.transform.position              = newPos;

        // Set enemy type //TODO: What type of enemy should this be?
        enemyInstance.GetComponent<Enemy>().enemyType = Enemy.EnemyType.WaitForPlayer;
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
