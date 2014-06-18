using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

class PlaygroundParticleWindowC : EditorWindow {
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PlaygroundParticleWindow variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// Paths
	public static string playgroundPath = "Assets/Particle Playground/";
	public static string presetPath = "Resources/Csharp/Presets/";
	public static string iconPath = "Graphics/Editor/Icons/";
	public static string brushPath = "Resources/Csharp/Brushes/";
	public static string scriptPath = "Scripts/Csharp/";
	
	// Presets
	public static List<PresetObjectC> presetObjects;
	public static bool particlePresetFoldout = true;
	public static int presetListStyle = 0;
	public static string[] presetNames;
	
	// Editor Window specific
	public static Vector2 scrollPosition;
	public static GUIStyle presetButtonStyle;
	public static string searchString = "";
	public static GUIStyle toolbarSearchSkin;
	public static GUIStyle toolbarSearchButtonSkin;
	
	[MenuItem ("Window/Particle Playground/Playground Wizard (C#)")]
	public static void ShowWindow () {
		PlaygroundParticleWindowC window = GetWindow<PlaygroundParticleWindowC>();
		window.title = "Playground (C#)";
        window.Show();
	}
	
	public void OnEnable () {
		Initialize();
	}
	
	public void OnProjectChange () {
		Initialize();
	}
	
	public void OnFocus () {
		Initialize();
	}
	
	public void Initialize () {
		presetButtonStyle = new GUIStyle();
		presetButtonStyle.stretchWidth = true;
		presetButtonStyle.stretchHeight = true;
		Object[] particlePrefabs = Resources.LoadAll("Csharp/Presets", typeof(PlaygroundParticlesC)) as Object[];
		Texture2D particleImageDefault = Resources.LoadAssetAtPath(playgroundPath+iconPath+"Default.png", typeof(Texture2D)) as Texture2D;
		Texture2D particleImage;
		
		presetObjects = new List<PresetObjectC>();
		int i = 0;
		for (; i<particlePrefabs.Length; i++) {
			presetObjects.Add(new PresetObjectC());
			presetObjects[i].presetObject = particlePrefabs[i];
			presetObjects[i].presetScript = (PlaygroundParticlesC)particlePrefabs[i];
			particleImage = Resources.LoadAssetAtPath(playgroundPath+iconPath+presetObjects[i].presetObject.name+".png", typeof(Texture2D)) as Texture2D;
			
			// Try the asset location if we didn't find it in regular editor folder
			if (particleImage==null) {
				particleImage = Resources.LoadAssetAtPath(Path.GetDirectoryName(AssetDatabase.GetAssetPath(presetObjects[i].presetObject as UnityEngine.Object))+"/"+presetObjects[i].presetObject.name+".png", typeof(Texture2D)) as Texture2D;
			}
			
			// Finally use the specified icon (or the default)
			if (particleImage!=null)
				presetObjects[i].presetImage = particleImage;
			else if (particleImageDefault!=null)
				presetObjects[i].presetImage = particleImageDefault;
		}
		presetNames = new string[presetObjects.Count];
		for (i = 0; i<presetNames.Length; i++) {
			presetNames[i] = presetObjects[i].presetObject.name;
			
			// Filter on previous search
			presetObjects[i].unfiltered = (searchString==""?true:presetNames[i].ToLower().Contains(searchString.ToLower()));
		}
	}
	
	void OnGUI () {
		if (toolbarSearchSkin==null) {
			toolbarSearchSkin = GUI.skin.FindStyle("ToolbarSeachTextField");
			if (toolbarSearchButtonSkin==null)
				toolbarSearchButtonSkin = GUI.skin.FindStyle("ToolbarSeachCancelButton");
		}
		EditorGUILayout.BeginVertical();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Playground Wizard (C#)", EditorStyles.largeLabel, GUILayout.Height(20));
		EditorGUILayout.Separator();
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.LabelField("Particle Playground v"+PlaygroundC.version.ToString());
		EditorGUILayout.Separator();
		
		// Create New-button
		if(GUILayout.Button("Create New Particle Playground System", EditorStyles.toolbarButton)){
			if (PlaygroundC.reference==null)
				CreateManager();
			PlaygroundParticlesC newParticlePlayground = PlaygroundC.Particle();
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
			string prevSearchString = searchString;
			searchString = GUILayout.TextField(searchString, toolbarSearchSkin, new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Width(Mathf.FloorToInt(Screen.width)-170), GUILayout.MinWidth(170)});
			if (GUILayout.Button("", toolbarSearchButtonSkin)) {
				searchString = "";
				GUI.FocusControl(null);
			}

			if (prevSearchString!=searchString) {
				for (int p = 0; p<presetNames.Length; p++)
					presetObjects[p].unfiltered = (searchString==""?true:presetNames[p].ToLower().Contains(searchString.ToLower()));
			}
			
			EditorGUILayout.Separator();
			presetListStyle = GUILayout.Toolbar (presetListStyle, new string[]{"Icons","List"}, EditorStyles.toolbarButton, GUILayout.MaxWidth(120));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginVertical("box");
			
			if (presetObjects.Count>0) {
				int col1 = Mathf.FloorToInt(Screen.width/2.2f)-46;
				if (presetListStyle==0) EditorGUILayout.BeginHorizontal();
				int iconwidths = 0;
				for (int i = 0; i<presetObjects.Count; i++) {
					
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
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Source:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));			EditorGUILayout.LabelField(presetObjects[i].presetScript.source.ToString(), EditorStyles.miniLabel);					EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("States:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));			EditorGUILayout.LabelField(presetObjects[i].presetScript.states.Count.ToString(), EditorStyles.miniLabel);				EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Particle Count:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));	EditorGUILayout.LabelField(presetObjects[i].presetScript.particleCount.ToString(), EditorStyles.miniLabel);				EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Emission Rate:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));	EditorGUILayout.LabelField(presetObjects[i].presetScript.emissionRate.ToString(), EditorStyles.miniLabel);				EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Lifetime:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));			EditorGUILayout.LabelField(presetObjects[i].presetScript.lifetime.ToString(), EditorStyles.miniLabel);					EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Sorting:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));			EditorGUILayout.LabelField(presetObjects[i].presetScript.sorting.ToString(), EditorStyles.miniLabel);					EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Delta Movement:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));	EditorGUILayout.LabelField(presetObjects[i].presetScript.calculateDeltaMovement.ToString(), EditorStyles.miniLabel);	EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Lifetime Velocity:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));EditorGUILayout.LabelField(presetObjects[i].presetScript.applyLifetimeVelocity.ToString(), EditorStyles.miniLabel);		EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Initial Velocity:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));	EditorGUILayout.LabelField(presetObjects[i].presetScript.applyInitialVelocity.ToString(), EditorStyles.miniLabel);		EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Local Velocity:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));	EditorGUILayout.LabelField(presetObjects[i].presetScript.applyInitialLocalVelocity.ToString(), EditorStyles.miniLabel);	EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Velocity Bending:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));	EditorGUILayout.LabelField(presetObjects[i].presetScript.applyVelocityBending.ToString(), EditorStyles.miniLabel);		EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Collision:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));		EditorGUILayout.LabelField(presetObjects[i].presetScript.collision.ToString(), EditorStyles.miniLabel);					EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Material:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));			EditorGUILayout.LabelField(presetObjects[i].presetScript.particleSystemRenderer.sharedMaterial.name.ToString(), EditorStyles.miniLabel);EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();	EditorGUILayout.LabelField("Update Rate:", EditorStyles.miniBoldLabel, GUILayout.Width(col1));		EditorGUILayout.LabelField(presetObjects[i].presetScript.updateRate.ToString(), EditorStyles.miniLabel);				EditorGUILayout.EndHorizontal();
						
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
						if(GUILayout.Button(presetObjects[i].presetImage, new GUILayoutOption[]{GUILayout.Width(44), GUILayout.Height(40)})){
							CreatePresetObject(i);
						}
						EditorGUILayout.BeginVertical();
						if (GUILayout.Button(presetObjects[i].presetScript.name.ToString(), EditorStyles.label, new GUILayoutOption[]{GUILayout.Height(18)}))
							CreatePresetObject(i);
						EditorGUILayout.LabelField(presetObjects[i].presetScript.source.ToString()+" ("+presetObjects[i].presetScript.particleCount.ToString()+")", EditorStyles.miniLabel, new GUILayoutOption[]{GUILayout.Height(18), GUILayout.Width(240)});
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
				PlaygroundCreatePresetWindowC.ShowWindow();
			}
			
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();
		
		PlaygroundInspectorC.RenderPlaygroundSettings();
		
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
	
	public void CreatePresetObject (int i) {
		PlaygroundParticlesC instantiatedPreset = PlaygroundC.InstantiatePreset(presetObjects[i].presetObject.name);
		if (instantiatedPreset)
			Selection.activeGameObject = instantiatedPreset.particleSystemGameObject;
	}
	
	public void CreateManager () {
		PlaygroundC.ResourceInstantiate("Playground Manager");
	}
}

public class PresetObjectC {
	public Object presetObject;
	public PlaygroundParticlesC presetScript;
	public Texture2D presetImage;
	public bool foldout = false;
	public bool unfiltered = true;
}