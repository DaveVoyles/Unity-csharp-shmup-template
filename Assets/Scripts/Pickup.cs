using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{

    public Transform  mainOption;
    private Transform _playerTransform;

	void Start () {
        _playerTransform = GameObject.Find("Player").transform;
	}

    void Update () {
        // Constantly rotate
	    this.transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }

    public void SpawnMainOption()
    {
        // Rotate 90 degrees on Z, then create new option
        Quaternion target = Quaternion.Euler(0, 0, 90);
        Instantiate(mainOption, _playerTransform.position, target);
    }
}
