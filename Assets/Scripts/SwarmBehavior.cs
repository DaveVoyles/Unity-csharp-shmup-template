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
    public int droneCount      = 10;
    /// <summary>
    /// Size of area that drones can spawn within. Ideally you want to keep this small (~4f)
    /// </summary>
    public float spawnRadius   = 3f;
    /// <summary>
    /// Drones will try to stay within this boundary when swarming
    /// </summary>
  	public Vector2 swarmBounds = new Vector2(30f, 30f);

    /// <summary>
    /// Attach the drone prefab
    /// </summary>
    public Transform prefab;
    public List<GameObject> drones;

    private Transform _spawnPointXform;
    private string _nameOfPool = "BulletPool";
    private SpawnPool _pool    = null;
    private Transform _playerXform;

    protected virtual void Start()
    {
        if (prefab == null){
            Debug.Log("Please assign a drone prefab, and assign the drone script to it.");
            return;
        }

        _spawnPointXform = GameObject.Find("EnemySpawnPoint").transform;
        _pool            = PoolManager.Pools[_nameOfPool];
        _playerXform     = GameObject.Find("Player").transform;

        InstantiateDrones();
    }

    /// <summary>
    /// Generates a random spawn point by passing in current location of EnemySpawnPoint
    /// game object in the scene, and a random number within 20 pixels x & y of the original
    /// </summary>
    /// <returns> Vector 3 of a random spawn point, within 20 pixels x & y of the original</returns>
    private Vector3 GeneratedSpawnPoint()
    {
        var position               = _spawnPointXform.position;
        const int randomRangeValue = 10;
        var randomSpawnPoint = new Vector3(position.x + Random.Range(-randomRangeValue, randomRangeValue),
                                           position.y + Random.Range(-randomRangeValue, randomRangeValue), position.y);
        return randomSpawnPoint;
    }

    /// <summary>
    /// instantiate the drones at a random spawn location
    /// </summary>
    private void InstantiateDrones()
    {
        // Same spawn location for all enemies
        var randomSpawnLocation = GeneratedSpawnPoint();

        for (int i = 0; i < droneCount; i++)
        {
            Transform droneTemp = _pool.Spawn(prefab);
            var db              = droneTemp.gameObject.GetComponent<DroneBehavior>();
            db.drones           = drones;
            db.swarm            = this;

            // spawn at random spawn points throughout the map
            droneTemp.transform.position = randomSpawnLocation;
            // Convert the transform to game object and add it to the list of drones
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