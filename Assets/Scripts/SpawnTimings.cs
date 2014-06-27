using UnityEngine;

public class SpawnTimings : MonoBehaviour
{
    [HideInInspector] public int maxNumberOfEnemiesToSpawn = 10;
    [HideInInspector] public float spawnFrequency          = 3f;
    [HideInInspector] public int numberOfEnemiesToSpawn    = 3;

    public SpawnManager _spawnManager                      = null;

    [SerializeField] private Transform _enemyPrefabOne      = null;
    [SerializeField] private Transform _enemyPrefabTwo      = null;
    [SerializeField] private Transform _enemyPrefabThree    = null;
    [SerializeField] private Transform _pathEnemyOne        = null;
    [SerializeField] private Transform _pathEnemyTwo        = null;
    [SerializeField] private Transform _pathEnemyThree      = null;
                     private string     _pathOne            = "path1";
                     private string     _pathTwo            = "path2";
                     private string     _pathThree          = "path3";

	
	void Start () {
        // Reference to SpawnMananger script
        SpawnWave_1();
	}

    // Update is called once per frame

	void Update () {
	
	}

    private void SpawnWave_1()
    {
        //StartCoroutine(_spawnManager.SpawnEnemiesIncrementally(_enemyPrefabThree, 2));
        //StartCoroutine(_spawnManager.SpawnPathOne(_pathEnemyOne, 3, 4, 1));
        //StartCoroutine(_spawnManager.SpawnPathTwo(_pathEnemyTwo, 3, 4, 1));
        //StartCoroutine(_spawnManager.SpawnPathThree(_pathEnemyThree, 3, 4, 1));
        StartCoroutine(_spawnManager.SpawnOnAPath(_pathEnemyThree, 3, 4, 1, _pathOne));
        //StartCoroutine(_spawnManager.SpawnStationaryEnemy());
        //_spawnManager.SpawnGroup(null, 3);
        StartCoroutine(_spawnManager.swarmBehavior.InstantiateDrones(15));

    }

    //public IEnumerator SpawnEnemies()
    //{
    //    var elapsedTime = 0f;
    //    var index = 0;

    //    // Flash back and forth over a set period of time
    //    while (elapsedTime < timeToFlash)
    //    {

    //        elapsedTime += Time.deltaTime;
    //        index++;

    //        // Wait a moment before switching colors 
    //        yield return new WaitForSeconds(spawnFrequency);
    //    }
    //}


}
