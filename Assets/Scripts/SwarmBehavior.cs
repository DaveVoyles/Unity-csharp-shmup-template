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
    
    private string _nameOfPool = "BulletPool";
    private SpawnPool _pool    = null;
    private Transform _playerXform;

    protected virtual void Start()
    {
        if (prefab == null){
            Debug.Log("Please assign a drone prefab, and assign the drone script to it.");
            return;
        }

        _pool        = PoolManager.Pools[_nameOfPool];
        _playerXform = GameObject.Find("Player").transform;

        InstantiateDrones();

    }

    /// <summary>
    /// instantiate the drones
    /// </summary>
    private void InstantiateDrones()
    {
        for (int i = 0; i < droneCount; i++)
        {
            Transform droneTemp = _pool.Spawn(prefab);
            var db              = droneTemp.gameObject.GetComponent<DroneBehavior>();
            db.drones           = drones;
            db.swarm            = this;

            // spawn inside circle
            var spawnPos                  = new Vector2(this.transform.position.x + 20f, this.transform.position.y) + Random.insideUnitCircle * spawnRadius /2;
            // Start drone locked w/ player's Z-axis
            droneTemp.transform.position = new Vector3(spawnPos.x, transform.position.y, _playerXform.position.z);
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