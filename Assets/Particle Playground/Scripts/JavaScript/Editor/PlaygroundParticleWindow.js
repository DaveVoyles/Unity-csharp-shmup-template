import System.IO;
import System.Collections.Generic;

class PlaygroundParticleWindow extends EditorWindow {
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PlaygroundParticleWindow variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// Paths
	static var playgroundPath : String = "Assets/Particle Playground/";
	static var presetPath : String = "Resources/JavaScript/Presets/";
	static var iconPath : String = "Graphics/Editor/Icons/";
	static var brushPath : String = "Resources/JavaScript/Brushes/";
	static var scriptPath : String = "Scripts/JavaScript";
	
	// Presets
	static var particlePrefabs : Object[];
	static var presetObjects : List.<PresetObject>;
	static var particlePresetFoldout : boolean = true;
	static var presetListStyle : int = 0;
	static var presetNames : String[];
	
	// Editor Window specific
	static var scrollPosition : Vector2;
	static var presetButtonStyle : GUIStyle;
	static var searchString : String = "";
	static var toolbarSearchSkin : GUIStyle;
	static var toolbarSearchButtonSkin : GUIStyle;
	
	@MenuItem ("Window/Particle Playground/Playground Wizard (JS)")
	static function ShowWindow () {
		var window = GetWindow.<PlaygroundParticleWindow>();
		window.title = "Playground (JS)";
        window.Show();
	}
	
	function OnEnable () {
		Initialize();
	}
	
	function OnProjectChange () {
		Initialize();
	}
	
	function OnFocus () {
		Initialize();
	}
	
	function Initialize () {
		presetButtonStyle = new GUIStyle();
		presetButtonStyle.stretchWidth = true;
		presetButtonStyle.stretchHeight = true;
		var particlePrefabs : Object[] = Resources.LoadAll("JavaScript/Presets", PlaygroundParticles);
		var particleImageDefault : Texture2D = Resources.LoadAssetAtPath(playgroundPath+iconPath+"Default.png", Texture2D) as Texture2D;
		var particleImage : Texture2D;
		
		presetObjects = new List.<PresetObject>();
		for (var i = 0; i<particlePrefabs.Length; i++) {
			presetObjects.Add(new PresetObject());
			presetObjects[i].presetObject = particlePrefabs[i];
			particleImage = Resources.LoadAssetAtPath(playgroundPath+iconPath+presetObjects[i].presetObject.name+".png", Texture2D) as Texture2D;
			
			// Try the asset location if we didn't find it in regular editor folder
			if (particleImage==null) {
				particleImage = Resources.LoadAssetAtPath(Path.GetDirectoryName(AssetDatabase.GetAssetPath(presetObjects[i].presetObject as UnityEngine.Object))+"/"+presetObjects[i].presetObject.name+".png", Texture2D) as Texture2D;
			}
			
			// Finally use the specified icon (or the default)
			if (particleImage!=null)
				presetObjects[i].presetImage = particleImage;
			else if (particleImageDefault!=null)
				presetObjects[i].presetImage = particleImageDefault;
		}
		presetNames = new String[presetObjects.Count];
		for (i = 0; i<presetNames.Length; i++) {
			presetNames[i] = presetObjects[i].presetObject.name;
			
			// Filter on previous search
			presetObjects[i].unfiltered = (searchString==""?true:presetNames[i].ToLower().Contains(searchString.ToLower()));
		}
	}
	
	function OnGUI () {
		if (!toolbarSearchSkin) {
			toolbarSearchSkin = GUI.skin.FindStyle("ToolbarSeachTextField");
			if (!toolbarSearchButtonSkin)
				toolbarSearchButtonSkin = GUI.skin.FindStyle("ToolbarSeachCancelButton");
		}
		EditorGUILayout.BeginVertical();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Playground Wizard (JS)", EditorStyles.largeLabel, GUILayout.Height(20));
		EditorGUILayout.Separator();
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.LabelField("Particle Playground v"+Playground.version.ToString());
		EditorGUILayout.Separator();
		
		// Create New-button
		if(GUILayout.Button("Create New Particle Playground System", EditorStyles.toolbarButton)){
			if (Playground.reference==null)
				CreateManager();
			var newParticlePlayground : PlaygroundParticles = Playground.Particle();
			Selection.activeGameObject = newParticlePlayground.particleSystemGameObject;
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndVertical();
		
		// Presets
		EditorGUILayout.BeginVertical("box");
		particlePresetFoldout = GUILayout.Toggle(particlePresetFoldout, "Presets", EditorStyles.foldout);
		if (particlePresetFoldout) {
			EditorGUILayout.BeginHorizontal("Toolbar");
			
			// Search
			var prevSearchString : String = searchString;
			searchString = GUILayout.TextField(searchString, toolbarSearchSkin, [GUILayout.ExpandWidth(false), GUILayout.Width(Mathf.FloorToInt(Screen.width)-170), GUILayout.MinWidth(170)]);
			if (GUILayout.Button("", toolbarSearchButtonSkin)) {
				searchString = "";
				GUI.FocusControl(null);
			}

			if (prevSearchString!=searchString) {
				for (var p = 0; p<presetNames.Length; p++)
					presetObjects[p].unfiltered = (searchString==""?true:presetNames[p].ToLower().Contains(searchString.ToLower()));
			}
			
			EditorGUILayout.Separator();
			presetListStyle = GUILayout.Toolbar (presetListStyle, ["Icons","List"], EditorStyles.toolbarButton, GUILayout.MaxWidth(120));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginVertical("box");
			
			if (presetObjects.Count>0) {
				var col1 : int = Mathf.FloorToInt(Screen.width/2.2)-46;
				if (presetListStyle==0) EditorGUILayout.BeginHorizontal();
				var iconwidths : int;
				for (var i = 0; i<presetObjects.Count; i++) {
					
					// Filter out by search
					if (!presetObjects[i].unfiltered) continue;
					
					// Preset Object were destroyed
					if (!presetObjects[i].presetObject) {
						Initialize();
						return;
					}
					
					// List
					if (presetListStyle==1) {
						EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(22));
						EditorGUILayout.BeginHorizontal();
						presetObjects[i].foldout = GUILayout.Toggle(presetObjects[i].foldout, presetObjects[i].presetObject.name, EditorStyles.foldout);
						EditorGUILayout.Separator();
						if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
							CreatePresetObject(i);
						}
						EditorGUILayout.EndHorizontal();
						
						if (presetObjects[i].foldout) {
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Source:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));			EditorGUILayout.LabelField(presetObjects[i].presetObject.source.ToString(), EditorStyles.miniLabel);					EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("States:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));			EditorGUILayout.LabelField(presetObjects[i].presetObject.states.Count.ToString(), EditorStyles.miniLabel);				EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Particle Count:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));	EditorGUILayout.LabelField(presetObjects[i].presetObject.particleCount.ToString(), EditorStyles.miniLabel);				EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Emission Rate:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));	EditorGUILayout.LabelField(presetObjects[i].presetObject.emissionRate.ToString(), EditorStyles.miniLabel);				EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Lifetime:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));			EditorGUILayout.LabelField(presetObjects[i].presetObject.lifetime.ToString(), EditorStyles.miniLabel);					EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Sorting:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));			EditorGUILayout.LabelField(presetObjects[i].presetObject.sorting.ToString(), EditorStyles.miniLabel);					EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Delta Movement:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));	EditorGUILayout.LabelField(presetObjects[i].presetObject.calculateDeltaMovement.ToString(), EditorStyles.miniLabel);	EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Lifetime Velocity:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));EditorGUILayout.LabelField(presetObjects[i].presetObject.applyLifetimeVelocity.ToString(), EditorStyles.miniLabel);		EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Initial Velocity:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));	EditorGUILayout.LabelField(presetObjects[i].presetObject.applyInitialVelocity.ToString(), EditorStyles.miniLabel);		EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Local Velocity:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));	EditorGUILayout.LabelField(presetObjects[i].presetObject.applyInitialLocalVelocity.ToString(), EditorStyles.miniLabel);	EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Velocity Bending:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));	EditorGUILayout.LabelField(presetObjects[i].presetObject.applyVelocityBending.ToString(), EditorStyles.miniLabel);	EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Collision:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));		EditorGUILayout.LabelField(presetObjects[i].presetObject.collision.ToString(), EditorStyles.miniLabel);					EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Material:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));			EditorGUILayout.LabelField(presetObjects[i].presetObject.particleSystemRenderer.sharedMaterial.name.ToString(), EditorStyles.miniLabel);					EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Update Rate:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));		EditorGUILayout.LabelField(presetObjects[i].presetObject.updateRate.ToString(), EditorStyles.miniLabel);				EditorGUILayout.EndHorizontal();
						
							EditorGUILayout.Separator();
							if(GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(50))){
								if (EditorUtility.DisplayDialog("Permanently delete this preset?", 
									"The preset "+presetObjects[i].presetObject.name+" will be removed, are you sure?", 
									"Yes", 
									"No")) {
										AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(presetObjects[i].presetObject));
									}
							}
						}
						
						EditorGUILayout.EndVertical();
					}
					
					
					if (presetListStyle==0) {
					
						// Break row for icons
						iconwidths+=318;
						if (iconwidths>=Screen.width && i>0) {
							iconwidths=318;
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();
						}
						
						if (Screen.width>636)
							EditorGUILayout.BeginVertical("box", GUILayout.Width(300));
						else
							EditorGUILayout.BeginVertical("box");
						EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(42));
						if(GUILayout.Button(presetObjects[i].presetImage, [GUILayout.Width(44), GUILayout.Height(40)])){
							CreatePresetObject(i);
						}
						EditorGUILayout.BeginVertical();
						if (GUILayout.Button(presetObjects[i].presetObject.name.ToString(), EditorStyles.label, [GUILayout.Height(18)]))
							CreatePresetObject(i);
						EditorGUILayout.LabelField(presetObjects[i].presetObject.source.ToString()+" ("+presetObjects[i].presetObject.particleCount.ToString()+")", EditorStyles.miniLabel, [GUILayout.Height(18), GUILayout.Width(240)]);
						EditorGUILayout.EndVertical();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndVertical();
						
					}
				}
				if (presetListStyle==0) EditorGUILayout.EndHorizontal();
			} else {
				EditorGUILayout.HelpBox("No presets found. Make sure that the path to the presets are set to: \""+playgroundPath+presetPath+"\"", MessageType.Info);
			}
			
			if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
				PlaygroundCreatePresetWindow.ShowWindow();
			}
			
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();
		
		PlaygroundInspector.RenderPlaygroundSettings();
		
		GUILayout.FlexibleSpace();
		
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Official Site", EditorStyles.toolbarButton)) {
				Application.OpenURL ("http://playground.polyfied.com/");
			}
			if (GUILayout.Button("Asset Store", EditorStyles.toolbarButton)) {
				Application.OpenURL ("http://u3d.as/5ZJ");
			}
			if (GUILayout.Button("Support Forum", EditorStyles.toolbarButton)) {
				Application.OpenURL ("http://forum.unity3d.com/threads/215154-Particle-Playground");
			}
			if (GUILayout.Button("Mail Support", EditorStyles.toolbarButton)) {
				Application.OpenURL ("mailto:support@polyfied.com");
			}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		
		GUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}
	
	function CreatePresetObject (i : int) {
		var instantiatedPreset : PlaygroundParticles = Playground.InstantiatePreset(presetObjects[i].presetObject.name);
		if (instantiatedPreset)
			Selection.activeGameObject = instantiatedPreset.particleSystemGameObject;
	}
	
	function CreateManager () {
		Playground.ResourceInstantiate("Playground Manager");
	}
}

class PresetObject {
	var presetObject : Object;
	var presetImage : Texture2D;
	var foldout : boolean = false;
	var unfiltered : boolean = true;
}