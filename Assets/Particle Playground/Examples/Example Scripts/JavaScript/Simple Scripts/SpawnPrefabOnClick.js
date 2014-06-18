#pragma strict

var prefab : GameObject;	// Prefab you wish to instantiate

function Update () {
	if (Input.GetMouseButtonDown (0)) {
		var ray : Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		var hit : RaycastHit;
		if (Physics.Raycast (ray, hit, 100)) {
			Instantiate (prefab, hit.point, Quaternion.identity);
		}
	}
}