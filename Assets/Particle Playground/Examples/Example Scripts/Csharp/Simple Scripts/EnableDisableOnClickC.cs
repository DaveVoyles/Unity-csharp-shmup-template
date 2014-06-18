using UnityEngine;
using System.Collections;

public class EnableDisableOnClickC : MonoBehaviour {

	public GameObject go;	// Set the GameObject you want to enable/disable through Inspector
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0))
			go.SetActive(!go.activeSelf);
	}
}
