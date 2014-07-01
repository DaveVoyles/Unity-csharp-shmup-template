using System.Collections;
using UnityEngine;

public class SpawnTimings : MonoBehaviour
{
    [HideInInspector] public int          maxNumToSpawn          = 10;
    [HideInInspector] public float        spawnFrequency         = 3f;
    [HideInInspector] public int          numberOfEnemiesToSpawn = 3;
                      public SpawnManager _spawnManager          = null;

    [SerializeField] private Transform    _enemyPrefabOne        = null;
    [SerializeField] private Transform    _enemyPathCreatorPF    = null;
    [SerializeField] private Transform    _enemySeekerPF         = null;
    [SerializeField] private Transform    _pathEnemyOne          = null;
    [SerializeField] private Transform    _pathEnemyTwo          = null;
    [SerializeField] private Transform    _pathEnemyThree        = null;
                     private string       _pathOne               = "path1";
                     private string       _pathTwo               = "path2";
                     private string       _pathThree             = "path3";
                     private float        _bulletSpeed           = -16f;  // neg, so that it goes from right to left

	
	void Start () {

        StartCoroutine(SpawnWave_1());
	}   

    private IEnumerator SpawnWave_1()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(_spawnManager.SpawnEnemiesIncrementally(_enemySeekerPF, 3, 2));
        yield return new WaitForSeconds(3f);
        StartCoroutine(_spawnManager.SpawnOnAPath(_pathEnemyThree, 3, 4, 1, _pathOne));
        //yield return new WaitForSeconds(4.5f);
        //StartCoroutine(_spawnManager.SpawnStationaryEnemy(1f, 3, _bulletSpeed));
        //yield return new WaitForSeconds(4.5f);
        //_spawnManager.SpawnGroup(_enemyPathCreatorPF, 3);
        //yield return new WaitForSeconds(0.5f);
        //StartCoroutine(_spawnManager.swarmBehavior.InstantiateDrones(8));
    }

}
