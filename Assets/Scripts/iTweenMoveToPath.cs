///// <summary>
///// Builds a random path for the game object to traverse, over a random period of time
///// </summary>
//using PathologicalGames;
//using UnityEngine;
//using System.Collections;

//public class iTweenMoveToPath : MonoBehaviour
//{
//    public int numberOfEnemiesToSpawn     = 3;      // How many enemies should run along the path?
//    public float frequency                = 2;      // How often should this path be used?

//    private string _iTweenPathName        = null;   // What is the name of the path?
//    private int    _timeToRunPath         = 0;      // How long should it take to go over the path?


//    /// <summary>
//    /// Uses a random time and random path name, then builds a path and has the enemy run along it
//    /// </summary>
//    public void FollowRandomPath()
//    {
//        GeneratePathName();
//        GenerateTimeToRunPath();
//        RandomPathConstructor(_iTweenPathName, _timeToRunPath);
//    }

//    /// <summary>
//    /// Follows the first path from iTween paths
//    /// </summary>
//    /// <param name="timeToRunPath">How long will it take to finish?</param>
//    /// <param name="easeType"> Which type of standard easing will we use?</param>
//    public void FollowPathOne(float timeToRunPath, iTween.EaseType easeType)
//    {
//        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("path1"), "time", timeToRunPath,
//            "easetype", easeType, "movetopath", false, "oncomplete", "deactivate"));
//    }

//    /// <summary>
//    /// Follows a preset path
//    /// </summary>
//    /// <param name="timeToRunPath"></param>
//    /// <param name="easeType"></param>
//    public void FollowPathTwo(float timeToRunPath, iTween.EaseType easeType)
//    {
//        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("path2"), "time", timeToRunPath,
//            "easetype", easeType, "movetopath", false, "oncomplete", "deactivate"));
//    }

//    /// <summary>
//    /// Follows a preset path
//    /// </summary>
//    /// <param name="timeToRunPath"></param>
//    /// <param name="easeType"></param>
//    public void FollowPathThree(float timeToRunPath, iTween.EaseType easeType)
//    {
//        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("path3"), "time", timeToRunPath,
//            "easetype", easeType, "movetopath", false, "oncomplete", "deactivate"));
//    }

//    /// <summary>
//    /// Enemies can spawn on any path, passed in as a string
//    /// </summary>
//    /// <param name="pathName"></param>
//    /// <param name="timeToRunPath"></param>
//    /// <param name="easeType"></param>
//    public void FollowPath(string pathName, float timeToRunPath, iTween.EaseType easeType)
//    {
//        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath(pathName), "time", timeToRunPath,
//            "easetype", easeType, "movetopath", false, "oncomplete", "deactivate"));
//    }
    

//    /// <summary>
//    /// Builds a random path for the object to traverse, passing in the random path name and random length to traverse
//    /// </summary>
//    /// <param name="iTweenPathName"></param>
//    /// <param name="timeToRunPath"></param>
//    private void RandomPathConstructor(string iTweenPathName, int timeToRunPath)
//    {
//        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath(iTweenPathName),
//            "time", timeToRunPath, "oncomplete", "deactivate"));
//    }

//    /// <summary>
//    /// DO NOT DELETE: If object gets to the end of path and isn't killed, then deactivate and return to pool.
//    /// Used as a callback in the BuildRandomPath function
//    /// </summary>
//    private void deactivate()
//    {
//        PoolManager.Pools["BulletPool"].Despawn(gameObject.transform);
//    }

//    /// <summary>
//    /// Generates a random path name for the enemy to follow
//    /// </summary>
//    private string GeneratePathName()
//    {
//        var randomNum    = Random.Range(1, 3);   
//        var intToString  = randomNum.ToString(); 
//        _iTweenPathName  = "path" + intToString; 

//        return _iTweenPathName;                   
//    }

//    /// <summary>
//    /// Returns a random length of time for the enemy to run along the path
//    /// </summary>
//    private int GenerateTimeToRunPath()
//    {
//        _timeToRunPath = Random.Range(5, 10);   

//        return _timeToRunPath;
//    }


//}
