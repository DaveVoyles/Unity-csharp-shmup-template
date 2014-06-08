/* De-spawn object if it is out of range of camera 
 * OnBecameInvisible can be a co-routine, simply use the yield statement in the function. When running in the editor, 
 * scene view cameras will also cause this function to be called.*/
using PathologicalGames;
using UnityEngine;
using System.Collections;

public class DespawnOnNotVisible : MonoBehaviour
{
    private SpawnPool _pool = null;

	void Start () {
        _pool = GameObject.Find("GameManager").GetComponent<GameManager>().BulletPool;
	}

    /// <summary>
    /// De-spawn object if it is out of range of camera
    /// </summary>
    private void OnBecameInvisible () {
        PoolManager.Pools["BulletPool"].Despawn(transform);
    }

}
