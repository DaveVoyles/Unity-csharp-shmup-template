#pragma strict

class PlaygroundCreateBrushWindow extends EditorWindow {

	static var brushTexture : Texture2D;
	static var brushName : String = "";
	static var brushScale : float = 1.0;
	static var brushDetail : BRUSHDETAIL;
	static var distance : float = 10000;
	static var spacing : float = .1;
	static var exceedMaxStopsPaint : boolean = false;
	
	static var window : EditorWindow;
	private var scrollPosition : Vector2;
	
	static function ShowWindow () {
		window = EditorWindow.GetWindow(PlaygroundCreateBrushWindow, true, "Brush Wizard");
        window.Show();
	}
	
	function OnEnable () {
		if (!brushName) brushName= "";
	}
	
	function OnGUI () {
		EditorGUILayout.BeginVertical();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Playground Brush Wizard", EditorStyles.largeLabel, GUILayout.Height(20));
		EditorGUILayout.Separator();
		
		GUILayout.BeginVertical("box");
		EditorGUILayout.HelpBox("Create a Particle Playground Brush by selecting a texture and edit its settings. The texture must have Read/Write Enabled and use True Color (non-compressed) in its import settings .", MessageType.Info);
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Brush Texture");
		var selectedBrushTexture : Texture2D = brushTexture;
		brushTexture = EditorGUILayout.ObjectField(brushTexture, Texture2D, false) as Texture2D;
		GUILayout.EndHorizontal();
		if (selectedBrushTexture!=brushTexture)
			brushName = brushTexture.name;
		brushName = EditorGUILayout.TextField("Name", brushName);
		brushScale = EditorGUILayout.FloatField("Scale", brushScale);
		brushDetail = System.Convert.ToInt32(EditorGUILayout.EnumPopup("Detail", brushDetail));
		distance = EditorGUILayout.FloatField("Distance", distance);
		spacing = EditorGUILayout.FloatField("Spacing", spacing);
		exceedMaxStopsPaint = EditorGUILayout.Toggle("Exceeding Max Stops Paint", exceedMaxStopsPaint);
		
		if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
			brushName = brushName.Trim();
			if (brushTexture && brushName!="") {
				if (AssetDatabase.LoadAssetAtPath(PlaygroundParticleWindow.playgroundPath+PlaygroundParticleWindow.brushPath+brushName+".prefab", GameObject)) {
					if (EditorUtility.DisplayDialog("Brush with same name found!", 
						"The brush "+brushName+" already exists. Do you want to overwrite it?", 
						"Yes", 
						"No"))
							CreateBrush();
				} else CreateBrush();
			}
		}
		
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}
	
	function CreateBrush () {
		var brushObject = new GameObject(brushName);
		var brushPreset : PlaygroundBrushPreset = brushObject.AddComponent(PlaygroundBrushPreset);
		brushPreset.presetName = brushName;
		brushPreset.texture = brushTexture;
		brushPreset.scale = brushScale;
		brushPreset.detail = brushDetail;
		brushPreset.distance = distance;
		brushPreset.spacing = spacing;
		brushPreset.exceedMaxStopsPaint = exceedMaxStopsPaint;
		
		var brushPrefab : GameObject = PrefabUtility.CreatePrefab(PlaygroundParticleWindow.playgroundPath+PlaygroundParticleWindow.brushPath+brushName+".prefab", brushObject, ReplacePrefabOptions.Default);
		DestroyImmediate(brushObject);
		
		PlaygroundParticleSystemInspector.LoadBrushes();
		
		window.Close();
	}
}