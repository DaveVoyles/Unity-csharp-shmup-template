#pragma strict

/* This example is using scripts from the Procedural Examples: https://www.assetstore.unity3d.com/#/content/5141
 * See Example Scripts/External/Procedural/PaintVertices.js to deform a mesh.
*/

function OnGUI () {
	if (GUI.Button(new Rect(10,10,80,20), "Reset"))
		Application.LoadLevel(Application.loadedLevel);
}