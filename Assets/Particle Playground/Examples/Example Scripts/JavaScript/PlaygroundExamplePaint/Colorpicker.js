#pragma strict

private var initialColor : Color32;

function Start () {
	initialColor = renderer.sharedMaterial.color;
}

function OnMouseEnter () {
	renderer.sharedMaterial.color = Color.black;
	var fadeTime : float = Time.time+.5;
	var t : float;
	while (Time.time<fadeTime && !Input.GetMouseButtonDown(0)) {
		renderer.sharedMaterial.color = Color.Lerp(renderer.sharedMaterial.color, initialColor, t);
		t+=Time.deltaTime/.5;
		yield;
	}
}

function OnMouseExit () {
	renderer.sharedMaterial.color = initialColor;
}

function OnMouseDown () {
	renderer.sharedMaterial.color = initialColor;
	PlaygroundExamplePaint.selectedColor = renderer.sharedMaterial.color;
	renderer.sharedMaterial.color = Color.white;
	var fadeTime : float = Time.time+.5;
	var t : float;
	while (Time.time<fadeTime) {
		renderer.sharedMaterial.color = Color.Lerp(renderer.sharedMaterial.color, initialColor, t);
		t+=Time.deltaTime/.5;
		yield;
	}
}