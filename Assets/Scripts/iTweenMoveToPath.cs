using PathologicalGames;
using UnityEngine;
using System.Collections;

/// <summary>
/// Author: Dave Voyles
/// Date: May 2014
/// Builds a random path for the game object to traverse, over a random period of time
/// </summary>

public class iTweenMoveToPath : MonoBehaviour
{

    private string _iTweenPathName        = null;      // What is the name of the path?
    private int    _timeToRunPath         = 0;          // How long should it take to go over the path?
    public int     numberOfEnemiesToSpawn = 3;  // How many enemies should run along the path?
    public float   frequency              = 2;               // How often should this path be used?


    public void FollowRandomPath()
    {
        GeneratePathName();
        GenerateTimeToRunPath();
        PathConstructor(_iTweenPathName, _timeToRunPath);
    }

    /// <summary>
    /// Builds a random path for the object to traverse, passing in the random path name and random length to traverse
    /// </summary>
    /// <param name="_iTweenPathName"></param>
    /// <param name="_timeToRunPath"></param>
    private void PathConstructor( string _iTweenPathName, int _timeToRunPath)
    {
        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath(_iTweenPathName), "time", _timeToRunPath, "oncomplete", "deactivate" ));
    }
    public void FollowPathThree()
    {
        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("path3"), "time", 5, "oncomplete", "deactivate"));
    }

    /// <summary>
    /// If object gets to the end of path and isn't killed, then deactivate and return to pool.
    /// Used as a callback in the BuildRandomPath function
    /// </summary>
    private void deactivate()
    {
        gameObject.SetActive(false);
        // TODO: put the bullet back on the stack for later re-use
       // PoolManager.Pools["BulletPool"].Despawn(gameObject.transform);
    }

    /// <summary>
    /// Generates a random path name for the enemy to follow
    /// </summary>
    private string GeneratePathName()
    {
        var _randomNum   = Random.Range(1, 3);   
        var _intToString = _randomNum.ToString(); 
        _iTweenPathName  = "path" + _intToString; 

        return _iTweenPathName;                   
    }

    /// <summary>
    /// Returns a random length of time for the enemy to run along the path
    /// </summary>
    private int GenerateTimeToRunPath()
    {
        _timeToRunPath = Random.Range(5, 10);   

        return _timeToRunPath;
    }


    /// <summary>
    /// Spawns enemies on path one
    /// </summary>
    /// <param name="_frequency">How often the enemy should spawn</param>
    /// <param name="numberOfEnemiesToSpawn">The number of enemies to spawn</param>
    /// <returns></returns>
    public IEnumerator FollowPathOne(float frequency, int numberOfEnemiesToSpawn)
    {
        var numberOfEnemiesSpawned = 0;
        if (numberOfEnemiesSpawned < numberOfEnemiesToSpawn)
        {

            iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("path1"), "time", 5, "oncomplete", "deactivate"));
            numberOfEnemiesSpawned++;
            Debug.Log("We can spawn!");
            yield return new WaitForSeconds(frequency);
        }
    }


}
