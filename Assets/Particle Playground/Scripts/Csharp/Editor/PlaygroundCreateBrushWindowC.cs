using UnityEditor;
using UnityEngine;

public class PlaygroundCreateBrushWindowC : EditorWindow {

	public static Texture2D brushTexture;
	public static string brushName = "";
	public static float brushScale = 1f;
	public static BRUSHDETAILC brushDetail;
	public static float distance = 10000f;
	public static float spacing = .1f;
	public static bool exceedMaxStopsPaint = false;
	
	public static EditorWindow window;
	private Vector2 scrollPosition;
	
	public static void ShowWindow () {
		window = EditorWindow.GetWindow<PlaygroundCreateBrushWindowC>(true, "Brush Wizard");
        window.Show();
	}
	
	void OnEnable () {
	}
	
	void OnGUI () {
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
		Texture2D selectedBrushTexture = brushTexture;
		brushTexture = EditorGUILayout.ObjectField(brushTexture, typeof(Texture2D), false) as Texture2D;
		GUILayout.EndHorizontal();
		if (selectedBrushTexture!=brushTexture)
			brushName = brushTexture.name;
		brushName = EditorGUILayout.TextField("Name", brushName);
		brushScale = EditorGUILayout.FloatField("Scale", brushScale);
		brushDetail = (BRUSHDETAILC)EditorGUILayout.EnumPopup("Detail", brushDetail);
		distance = EditorGUILayout.FloatField("Distance", distance);
		spacing = EditorGUILayout.FloatField("Spacing", spacing);
		exceedMaxStopsPaint = EditorGUILayout.Toggle("Exceeding Max Stops Paint", exceedMaxStopsPaint);
		
		if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
			brushName = brushName.Trim();
			if (brushTexture && brushName!="") {
				if (AssetDatabase.LoadAssetAtPath(PlaygroundParticleWindowC.playgroundPath+PlaygroundParticleWindowC.brushPath+brushName+".prefab", typeof(GameObject))) {
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
	
	public void CreateBrush () {
		GameObject brushObject = new GameObject(brushName);
		PlaygroundBrushPresetC brushPreset = brushObject.AddComponent<PlaygroundBrushPresetC>();
		brushPreset.presetName = brushName;
		brushPreset.texture = brushTexture;
		brushPreset.scale = brushScale;
		brushPreset.detail = brushDetail;
		brushPreset.distance = distance;
		brushPreset.spacing = spacing;
		brushPreset.exceedMaxStopsPaint = exceedMaxStopsPaint;
		
		PrefabUtility.CreatePrefab(PlaygroundParticleWindowC.playgroundPath+PlaygroundParticleWindowC.brushPath+brushName+".prefab", brushObject, ReplacePrefabOptions.Default);
		DestroyImmediate(brushObject);
		
		PlaygroundParticleSystemInspectorC.LoadBrushes();
		
		window.Close();
	}
}