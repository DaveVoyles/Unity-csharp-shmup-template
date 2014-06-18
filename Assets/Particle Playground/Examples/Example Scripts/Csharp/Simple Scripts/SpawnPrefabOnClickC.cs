using UnityEngine;
using System.Collections;

public class SpawnPrefabOnClickC : MonoBehaviour {

	public GameObject prefab;	// Prefab you wish to instantiate
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100)) {
				Instantiate (prefab, hit.point, Quaternion.identity);
			}
		}
	}
}
