/// <summary>
/// Builds a random path for the game object to traverse, over a random period of time
/// </summary>
using PathologicalGames;
using UnityEngine;
using System.Collections;

public class iTweenMoveToPath : MonoBehaviour
{
    public int numberOfEnemiesToSpawn     = 3;      // How many enemies should run along the path?
    public float frequency                = 2;      // How often should this path be used?

    private string _iTweenPathName        = null;   // What is the name of the path?
    private int    _timeToRunPath         = 0;      // How long should it take to go over the path?


    /// <summary>
    /// Uses a random time and random path name, then builds a path and has the enemy run along it
    /// </summary>
    public void FollowRandomPath()
    {
        GeneratePathName();
        GenerateTimeToRunPath();
        RandomPathConstructor(_iTweenPathName, _timeToRunPath);
    }

    /// <summary>
    /// Follows a preset path
    /// </summary>
    /// <param name="timeToRunPath"></param>
    /// <param name="easeType"></param>
    public void FollowPathOne(float timeToRunPath, iTween.EaseType easeType)
    {
        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("path1"), "time", timeToRunPath,
            "easetype", easeType, "movetopath", false, "oncomplete", "deactivate"));
    }

    /// <summary>
    /// Follows a preset path
    /// </summary>
    /// <param name="timeToRunPath"></param>
    /// <param name="easeType"></param>
    public void FollowPathTwo(float timeToRunPath, iTween.EaseType easeType)
    {
        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("path2"), "time", timeToRunPath,
            "easetype", easeType, "movetopath", false, "oncomplete", "deactivate"));
    }

    /// <summary>
    /// Follows a preset path
    /// </summary>
    /// <param name="timeToRunPath"></param>
    /// <param name="easeType"></param>
    public void FollowPathThree(float timeToRunPath, iTween.EaseType easeType)
    {
        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("path3"), "time", timeToRunPath,
            "easetype", easeType, "movetopath", false, "oncomplete", "deactivate"));
    }

    /// <summary>
    /// Builds a random path for the object to traverse, passing in the random path name and random length to traverse
    /// </summary>
    /// <param name="_iTweenPathName"></param>
    /// <param name="_timeToRunPath"></param>
    private void RandomPathConstructor(string _iTweenPathName, int _timeToRunPath)
    {
        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath(_iTweenPathName),
            "time", _timeToRunPath, "oncomplete", "deactivate"));
    }

    /// <summary>
    /// If object gets to the end of path and isn't killed, then deactivate and return to pool.
    /// Used as a callback in the BuildRandomPath function
    /// </summary>
    private void deactivate()
    {
        PoolManager.Pools["BulletPool"].Despawn(gameObject.transform);
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


}
