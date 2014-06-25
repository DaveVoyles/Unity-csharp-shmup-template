using UnityEngine;

public class SpawnTimings : MonoBehaviour
{
    [HideInInspector] public int maxNumberOfEnemiesToSpawn = 10;
    [HideInInspector] public float spawnFrequency          = 3f;
    [HideInInspector] public int numberOfEnemiesToSpawn    = 3;

    [SerializeField]
    public SpawnManager _spawnManager                      = null;

    //TODO: Should I pass in different enemy Xform types as vars?


	
	void Start () {
        // Reference to SpawnMananger script
        SpawnWave_1();
	}

    // Update is called once per frame
	void Update () {
	
	}

    private void SpawnWave_1()
    {
        StartCoroutine(_spawnManager.SpawnEnemiesIncrementally(null, 2));
        StartCoroutine(_spawnManager.SpawnPathOne());
         StartCoroutine(_spawnManager.SpawnStationaryEnemy());
        _spawnManager.SpawnGroup(null, 3);


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
