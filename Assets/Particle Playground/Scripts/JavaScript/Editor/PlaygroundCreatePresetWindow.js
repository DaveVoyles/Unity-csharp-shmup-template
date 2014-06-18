#pragma strict

import System.IO;
import System.Collections.Generic;

class PlaygroundCreatePresetWindow extends EditorWindow {

	static var particleSystemObject : GameObject;
	static var particleSystemIcon : Texture2D;
	static var particleSystemName : String;
	
	static var window : EditorWindow;
	static var scrollPosition : Vector2;
	
	var presetOrPublish : int = 0;
	var selectedPreset : int = 0;
	var createPackage : boolean = true;
	
	var showError1 : boolean = false;
	
	static function ShowWindow () {
		window = EditorWindow.GetWindow(PlaygroundCreatePresetWindow, true, "Preset Wizard");
        window.Show();
	}
	
	function OnEnable () {
		Initialize();
	}
	
	function Initialize () {
		if (Selection.activeGameObject && Selection.activeGameObject.GetComponent(PlaygroundParticles)) {
			particleSystemObject = Selection.activeGameObject;
			particleSystemName = Selection.activeGameObject.name;
		} else if (!particleSystemName) {
			particleSystemName = "";
		}
	}
	
	function OnGUI () {
		EditorGUILayout.BeginVertical();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Playground Preset Wizard", EditorStyles.largeLabel, GUILayout.Height(20));
		EditorGUILayout.Separator();
		
		GUILayout.BeginVertical("box");
		var tmpPresetOrPublish : int = presetOrPublish;
		presetOrPublish = GUILayout.Toolbar (presetOrPublish, ["Preset","Publish"], EditorStyles.toolbarButton);
		if (presetOrPublish==0)
			EditorGUILayout.HelpBox("Create a Particle Playground Preset by selecting a Particle Playground System and an icon (optional). The icon must be in png-format and preferably 32x32 pixels. All connected objects will be childed to the Particle Playground System.", MessageType.Info);
		else
			EditorGUILayout.HelpBox("Prepare a preset for packaging or publication to the Unity Asset Store by selecting the Particle Playground Preset in the list. The icon must be in png-format and preferably 32x32 pixels. All connected meshes, images and/or cached values will be distributed along with your Particle Playground System. Please check all dependencies before you upload your preset.", MessageType.Info);
		if (tmpPresetOrPublish!=presetOrPublish && presetOrPublish==1)
			RefreshFromPresetList();
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
		
		if (presetOrPublish==0) {
			EditorGUILayout.PrefixLabel("Particle System");
		
			// Particle System to become a preset
			var selectedObj : GameObject = particleSystemObject;
			particleSystemObject = EditorGUILayout.ObjectField(particleSystemObject, GameObject, true) as GameObject;
			if (particleSystemObject!=selectedObj) {
				
				// Check if this is a Particle Playground System
				if (particleSystemObject && particleSystemObject.GetComponent(PlaygroundParticles)) {
				
					// Set new name if user hasn't specified one
					if (particleSystemName=="")
						particleSystemName = particleSystemObject.name;
						
					showError1 = false;
				} else {
					showError1 = true;
				}
			}
		} else {
			EditorGUILayout.PrefixLabel("Preset");
		
			// Popup of presets
			var previousSelectedObj : int = selectedPreset;
			selectedPreset = EditorGUILayout.Popup(selectedPreset, PlaygroundParticleWindow.presetNames);
			if (previousSelectedObj!=selectedPreset) {
				RefreshFromPresetList();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Icon");
		particleSystemIcon = EditorGUILayout.ObjectField(particleSystemIcon, Texture2D, false) as Texture2D;
		GUILayout.EndHorizontal();
		particleSystemName = EditorGUILayout.TextField("Name", particleSystemName);
		if (presetOrPublish==1) {
			createPackage = EditorGUILayout.Toggle("Create Package", createPackage);
			if (!createPackage)
				EditorGUILayout.HelpBox("The raw assets connected to this preset will be moved to \"Assets/Playground Preset - "+particleSystemName+"/\""+".", MessageType.Warning);
		}
		
		EditorGUILayout.Separator();
		
		if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
			particleSystemName = particleSystemName.Trim();
			if (presetOrPublish==0 && particleSystemObject && particleSystemObject.GetComponent(PlaygroundParticles) && particleSystemName!="") {
				var tmpName : String = particleSystemObject.name;
				particleSystemObject.name = particleSystemName;
				if (AssetDatabase.LoadAssetAtPath(PlaygroundParticleWindow.playgroundPath+PlaygroundParticleWindow.presetPath+particleSystemName+".prefab", GameObject)) {
					if (EditorUtility.DisplayDialog("Preset with same name found!", 
						"The preset "+particleSystemName+" already exists. Do you want to overwrite it?", 
						"Yes", 
						"No"))
							CreatePreset();
				} else CreatePreset();
				particleSystemObject.name = tmpName;
			} else
			if (presetOrPublish==1 && particleSystemName!="") {
				if (AssetDatabase.LoadAssetAtPath("Assets/"+PlaygroundParticleWindow.presetNames[selectedPreset]+"/Resources/Presets/"+PlaygroundParticleWindow.presetNames[selectedPreset]+".prefab", GameObject)) {
					if (EditorUtility.DisplayDialog("Preset with same name found!", 
						"The preset "+PlaygroundParticleWindow.presetNames[selectedPreset]+" already exists at Assets/"+PlaygroundParticleWindow.presetNames[selectedPreset]+"/Resources/Presets/"+PlaygroundParticleWindow.presetNames[selectedPreset]+". Do you want to overwrite it?", 
						"Yes", 
						"No"))
							CreatePublicPreset(true);
				} else CreatePublicPreset(false);
			}
		}
		GUILayout.EndVertical();
		
		// Space for error messages
		if (showError1 && particleSystemObject)
			EditorGUILayout.HelpBox("GameObject is not a Particle Playground System.", MessageType.Error);
		
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
	}
	
	function CreatePreset () {
		
		// Try to child all connected objects to the particle system
		var ppScript : PlaygroundParticles = particleSystemObject.GetComponent(PlaygroundParticles);
		
		for (var i = 0; i<ppScript.manipulators.Count; i++)
			if (ppScript.manipulators[i].transform)
				ppScript.manipulators[i].transform.parent = particleSystemObject.transform;
		for (i = 0; i<ppScript.paint.paintPositions.Count; i++)
			if (ppScript.paint.paintPositions[i].parent)
				ppScript.paint.paintPositions[i].parent.parent = particleSystemObject.transform;
		for (i = 0; i<ppScript.states.Count; i++)
			if (ppScript.states[i].stateTransform)
				ppScript.states[i].stateTransform.parent = particleSystemObject.transform;
		if (ppScript.sourceTransform)
			ppScript.sourceTransform.parent = particleSystemObject.transform;
		if (ppScript.worldObject.transform)
			ppScript.worldObject.transform.parent = particleSystemObject.transform;
		if (ppScript.skinnedWorldObject.transform)
			ppScript.skinnedWorldObject.transform.parent = particleSystemObject.transform;
		
		// Save it as prefab in presetPath and import
		var particleSystemPrefab : GameObject = PrefabUtility.CreatePrefab(PlaygroundParticleWindow.playgroundPath+PlaygroundParticleWindow.presetPath+particleSystemObject.name+".prefab", particleSystemObject, ReplacePrefabOptions.Default);
		AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(particleSystemIcon as UnityEngine.Object), PlaygroundParticleWindow.playgroundPath+PlaygroundParticleWindow.iconPath+particleSystemPrefab.name+".png");
		AssetDatabase.ImportAsset(PlaygroundParticleWindow.playgroundPath+PlaygroundParticleWindow.iconPath+particleSystemPrefab.name+".png");
		
		// Close window
		window.Close();
	}
	
	function CreatePublicPreset (isOverwrite : boolean) {
		
		// Create folders
		if (!isOverwrite && !createPackage) {
			AssetDatabase.CreateFolder("Assets", "Playground Preset - "+particleSystemName);
			AssetDatabase.CreateFolder("Assets/Playground Preset - "+particleSystemName, "Resources");
			AssetDatabase.CreateFolder("Assets/Playground Preset - "+particleSystemName+"/Resources", "JavaScript");
			AssetDatabase.CreateFolder("Assets/Playground Preset - "+particleSystemName+"/Resources/JavaScript", "Presets");
		}
		
		// Path to the new resources/presets folder
		var publicPresetPath : String = "Assets/Playground Preset - "+particleSystemName+"/Resources/JavaScript/Presets/";
				
		
		
		// Get dependencies
		var presetDependencies : List.<String> = new List.<String>();
		var tmpPresetDependencies : String[] = AssetDatabase.GetDependencies([AssetDatabase.GetAssetPath(PlaygroundParticleWindow.presetObjects[selectedPreset].presetObject as UnityEngine.Object)]);
		
		// Copy the icon file
		if (!createPackage)
			AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(particleSystemIcon as UnityEngine.Object), publicPresetPath+particleSystemName+".png");
		else
			presetDependencies.Add(AssetDatabase.GetAssetPath(particleSystemIcon as UnityEngine.Object));
		for (var i = 0; i<tmpPresetDependencies.Length; i++) {
			
			
			// Check that the operation won't disturb any of the unnecessary files from the framework
			if (!tmpPresetDependencies[i].Contains("PlaygroundBrushPresetInspector.js") && 
				!tmpPresetDependencies[i].Contains("PlaygroundCreateBrushWindow.js") && 
				!tmpPresetDependencies[i].Contains("PlaygroundCreatePresetWindow.js") && 
				!tmpPresetDependencies[i].Contains("PlaygroundInspector.js") && 
				!tmpPresetDependencies[i].Contains("PlaygroundParticleSystemInspector.js") && 
				!tmpPresetDependencies[i].Contains("PlaygroundParticleWindow.js") && 
				!tmpPresetDependencies[i].Contains("PlaygroundBrushPreset.js")
			) {
				
				// It's the preset, rename if user specified another name
				if (Path.GetFileName(tmpPresetDependencies[i]) == PlaygroundParticleWindow.presetNames[selectedPreset]+".prefab" && particleSystemName!=Path.GetFileName(tmpPresetDependencies[i])) {
					AssetDatabase.Refresh();
					AssetDatabase.RenameAsset(publicPresetPath+Path.GetFileName(tmpPresetDependencies[i]), particleSystemName);
					tmpPresetDependencies[i] = Path.GetDirectoryName(tmpPresetDependencies[i])+"/"+particleSystemName+".prefab";
				}
				
				// Add to preset dependencies list
				presetDependencies.Add(tmpPresetDependencies[i]);
			}
		}
		
		// Check that necessary files are in
		if (!presetDependencies.Contains("Playground.js"))
			presetDependencies.Add(PlaygroundParticleWindow.playgroundPath+PlaygroundParticleWindow.scriptPath+"Playground.js");
		if (!presetDependencies.Contains("PlaygroundParticles.js"))
			presetDependencies.Add(PlaygroundParticleWindow.playgroundPath+PlaygroundParticleWindow.scriptPath+"PlaygroundParticles.js");
		if (!presetDependencies.Contains("Playground Manager"))
			presetDependencies.Add(PlaygroundParticleWindow.playgroundPath+"Resources/Playground Manager.prefab");
		
		// Refresh the project
		AssetDatabase.Refresh();
		
		// User wants to create a package of all dependency assets
		if (createPackage) {
			AssetDatabase.ExportPackage(presetDependencies.ToArray(), "Playground Preset - "+particleSystemName+".unitypackage", ExportPackageOptions.Interactive);
			
		// All dependency assets will be moved instead	
		} else {
			Undo.RecordObjects(EditorUtility.CollectDeepHierarchy ([PlaygroundParticleWindow.presetObjects[selectedPreset].presetObject as UnityEngine.Object]) as UnityEngine.Object[], "Move all dependencies of preset");
			for (i = 0; i<presetDependencies.Count; i++) {
				AssetDatabase.MoveAsset(presetDependencies[i], publicPresetPath+Path.GetFileName(presetDependencies[i]));
			}
			
			// Select the prefab
			EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(publicPresetPath+PlaygroundParticleWindow.presetNames[selectedPreset]+".prefab", UnityEngine.Object) as UnityEngine.Object);
		
		}
		
		// Refresh the project
		AssetDatabase.Refresh();
		
		// Close window
		window.Close();
	}
	
	function RefreshFromPresetList () {
		particleSystemIcon = Resources.LoadAssetAtPath(PlaygroundParticleWindow.playgroundPath+PlaygroundParticleWindow.iconPath+PlaygroundParticleWindow.presetNames[selectedPreset]+".png", Texture2D) as Texture2D;
		particleSystemName = PlaygroundParticleWindow.presetNames[selectedPreset];
	}
}