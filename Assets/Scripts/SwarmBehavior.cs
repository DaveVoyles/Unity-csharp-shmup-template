/* An implementation of the flocking algorithm: http://www.red3d.com/cwr/boids/
 * Additional resources: http://harry.me/2011/02/17/neat-algorithms---flocking/ 
 */
using PathologicalGames;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwarmBehavior : MonoBehaviour
{
    /// <summary>
    /// the number of drones we want in this swarm
    /// </summary>
    public int              droneCount        = 1;
    /// <summary>
    /// Size of area that drones can spawn within. Ideally you want to keep this small (~4f)
    /// </summary>
    public float            spawnRadius       = 3f;
    /// <summary>
    /// Drones will try to stay within this boundary when swarming
    /// </summary>
  	public Vector2          swarmBounds       = new Vector2(30f, 30f);
    /// <summary>
    /// Attach the drone prefab
    /// </summary>
    public Transform        droneXform;
    public List<GameObject> drones;

    [SerializeField]
    private SpawnManager    _spawnMananger    = null;
    private Transform       _spawnPointXform;
    private const string    _nameOfPool       = "BulletPool";
    private SpawnPool       _pool             = null;
    private Transform       _playerXform      = null;
    private bool            _isActive         = false;


    protected virtual void Start()
    {
        //if (droneXform == null){
        //    Debug.Log("Please assign a drone prefab.");
        //    return;
        //}

        //_spawnPointXform = GameObject.Find("EnemySpawnPoint").transform;
        //_playerXform     = GameObject.Find("Player").transform;
        //_pool            = PoolManager.Pools[_nameOfPool];
        GameEventManager.GameStart += GameStart;

    }

    private void GameStart()
    {
        if (droneXform == null)
        {
            Debug.Log("Please assign a drone prefab.");
            return;
        }

        _spawnPointXform = GameObject.Find("EnemySpawnPoint").transform;
        _playerXform     = GameObject.Find("Player").transform;
        _pool            = PoolManager.Pools[_nameOfPool];
        _isActive        = true;
    }

    /// <summary>
    /// Keep the drone swarm  target constantly attached to the player
    /// </summary>
    private void Update()
    {
        if (_playerXform == null)
        {
            _playerXform = GameObject.Find("Player").transform;
        }
        transform.position = _playerXform.position;
    }

    /// <summary>
    /// Generates a random spawn point by passing in current location of EnemySpawnPoint
    /// game object in the scene, and a random number within 20 pixels x & y of the original
    /// </summary>
    /// <returns> Vector 3 of a random spawn point, within 20 pixels x & y of the original</returns>
    private Vector3 GeneratedSpawnPoint()
    {   
        var position               = GameObject.Find("EnemySpawnPoint").transform.position;
        const int randomRangeValue = 10;
        var randomSpawnPoint       = new Vector3(position.x + Random.Range(0, randomRangeValue),
                                                 position.y + Random.Range(-randomRangeValue, randomRangeValue), position.y);
        return randomSpawnPoint;
    }

    /// <summary>
    /// Generate the same random, spawn location for all enemies and draw particles
    /// Also, set a brief delay before enemies appear on screen
    /// </summary>
    /// <param name="numToSpawn">How many drones should we spawn?</param>
    public IEnumerator InstantiateDrones(int numToSpawn = 3)
    {
        var randomSpawnLocation = GeneratedSpawnPoint();
        gameObject.GetComponent<ParticleEffectsManager>().CreateSpawnEffects(randomSpawnLocation);
        yield return new WaitForSeconds(.5f);

        SpawnDrones(randomSpawnLocation, numToSpawn);
    }

    /// <summary>
    /// Creates a series of drones 
    /// </summary>
    /// <param name="randomSpawnLocation">Where should they spawn?</param>
    /// <param name="numToSpawn">Passed in from Instantiate drone. How many should we spawn?</param>
    private void SpawnDrones(Vector3 randomSpawnLocation, int numToSpawn = 3)
    {
        // How many drones should we spawn?
        droneCount = numToSpawn;

        for (var i = 0; i < droneCount; i++)
        {
            // Add enemy to the list, so that we can track how many are on screen at once
           _spawnMananger.numOfEnemiesInScene++;

            var droneTemp       = _pool.Spawn(droneXform);
            var db              = droneTemp.gameObject.GetComponent<DroneBehavior>();
            db.drones           = drones;
            db.swarm            = this;

            // spawn at random spawn points throughout the map, then
            // Convert the transform to game object and add it to the list of drones
            droneTemp.transform.position = randomSpawnLocation;            
            var droneTempToGameObject    = droneTemp.gameObject;
            drones.Add(droneTempToGameObject);
        }
    }


    /// <summary>
    /// Draw Gizmos for debugging
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(  transform.position, swarmBounds);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}