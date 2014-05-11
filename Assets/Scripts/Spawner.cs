/* Handles all spawning routines for all enemies. Transforms are grabbed from the object pool, and returned after destruction
 * Author: Dave Voyles
 * Date: May 2014
 */

using PathologicalGames;
using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
    
    public Transform pathEnemyTransform;
    public int pathOneSpawnAmount     = 0;
    public float pathOneSpawnInterval = 0f;

    private string _nameOfPool = "BulletPool";
    private SpawnPool _pool    = null;

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
            _iTweenMoveToPath.FollowPathThree();
            count--;
            
            // call this function, every (x) seconds
            yield return new WaitForSeconds(this.pathOneSpawnInterval);
        }
    }
}
