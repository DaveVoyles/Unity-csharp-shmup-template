#pragma strict

var go : GameObject;	// Set the GameObject you want to enable/disable through Inspector

function Update () {
	if (Input.GetMouseButtonDown (0))
		go.SetActive(!go.activeSelf);
}