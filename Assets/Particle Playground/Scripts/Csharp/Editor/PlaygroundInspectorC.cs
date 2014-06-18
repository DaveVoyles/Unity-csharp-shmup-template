using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor (typeof(PlaygroundC))]
public class PlaygroundInspectorC : Editor {
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Playground variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public static PlaygroundC playgroundScriptReference;
	public static SerializedObject playground;						// Playground
	public static SerializedProperty calculate;
	public static SerializedProperty pixelFilterMode;
	public static SerializedProperty garbageCollectOnResize;
	public static SerializedProperty autoGroup;
	public static SerializedProperty buildZeroAlphaPixels;
	public static SerializedProperty drawGizmos;
	public static SerializedProperty paintToolbox;
	public static SerializedProperty showShuriken;
	
	public static SerializedProperty particleSystems;
	public static SerializedProperty manipulators;
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PlaygroundInspector variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public static bool playgroundFoldout = true;
	public static bool particleSystemsFoldout = false;
	public static bool manipulatorsFoldout = false;
	public static bool advancedSettingsFoldout = false;
	public static List<bool> manipulatorListFoldout;
	public static bool targetsFoldout = false;
	public static bool limitsFoldout = false;
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Internal variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public static Vector3 manipulatorHandlePosition;
	public static GUIStyle boxStyle;
	
	public void OnEnable () {
		Initialize(target as PlaygroundC);
	}
	
	public static void Initialize (PlaygroundC targetRef) {
		if (playgroundScriptReference==null) return;
		playgroundScriptReference = targetRef;
		playground = new SerializedObject(playgroundScriptReference);
		particleSystems = playground.FindProperty("particleSystems");
		manipulators = playground.FindProperty("manipulators");
		calculate = playground.FindProperty("calculate");
		pixelFilterMode = playground.FindProperty("pixelFilterMode");
		garbageCollectOnResize = playground.FindProperty("garbageCollectOnResize");
		autoGroup = playground.FindProperty("autoGroup");
		buildZeroAlphaPixels = playground.FindProperty("buildZeroAlphaPixels");
		drawGizmos = playground.FindProperty("drawGizmos");
		paintToolbox = playground.FindProperty("paintToolbox");
		showShuriken = playground.FindProperty("showShuriken");
		
		manipulatorListFoldout = new List<bool>();
		manipulatorListFoldout.AddRange(new bool[playgroundScriptReference.manipulators.Count]);
	}
	
	public override void OnInspectorGUI () {
		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");
		
		EditorGUILayout.Separator();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Particle Playground v"+PlaygroundC.version.ToString(), EditorStyles.largeLabel, GUILayout.Height(20));
		
		EditorGUILayout.Separator();
		
		if(GUILayout.Button("Open Playground Wizard", EditorStyles.toolbarButton, GUILayout.Width(130))) {
			PlaygroundParticleWindowC.ShowWindow();
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		RenderPlaygroundSettings();
		
		if (Event.current.type == EventType.ValidateCommand &&
			Event.current.commandName == "UndoRedoPerformed") {
			foreach (PlaygroundParticlesC p in playgroundScriptReference.particleSystems) {
				p.Start();
			}
		}
		
	}
	
	void OnSceneGUI () {
		if (playgroundScriptReference!=null && playgroundScriptReference.drawGizmos && manipulatorsFoldout)
			for (int i = 0; i<playgroundScriptReference.manipulators.Count; i++)
				RenderManipulatorInScene(playgroundScriptReference.manipulators[i], playgroundScriptReference.manipulators[i].inverseBounds? new Color(1f,.6f,.4f,1f) : new Color(.4f,.6f,1f,1f));
		if (GUI.changed)
            EditorUtility.SetDirty (target);
	}
	
	public static void RenderManipulatorInScene (ManipulatorObjectC thisManipulator, Color manipulatorColor) {
		// Draw Manipulators in Scene View
		if (thisManipulator.transform) {
			Handles.color = new Color(manipulatorColor.r,manipulatorColor.g,manipulatorColor.b,Mathf.Clamp(Mathf.Abs(thisManipulator.strength),.25f,1f));
			Handles.color = thisManipulator.enabled? Handles.color : new Color(manipulatorColor.r,manipulatorColor.g,manipulatorColor.b,.2f);
			
			// Position
			if (Tools.current==Tool.Move)
				thisManipulator.transform.position = Handles.PositionHandle(thisManipulator.transform.position, Tools.pivotRotation==PivotRotation.Global? Quaternion.identity : thisManipulator.transform.rotation);
			// Rotation
			else if (Tools.current==Tool.Rotate)
				thisManipulator.transform.rotation = Handles.RotationHandle(thisManipulator.transform.rotation, thisManipulator.transform.position);
			// Scale
			else if (Tools.current==Tool.Scale)
				thisManipulator.transform.localScale = Handles.ScaleHandle(thisManipulator.transform.localScale, thisManipulator.transform.position, thisManipulator.transform.rotation, HandleUtility.GetHandleSize(thisManipulator.transform.position));

			// Sphere Size
			if (thisManipulator.shape==MANIPULATORSHAPEC.Sphere) {
				thisManipulator.size = Handles.RadiusHandle (Quaternion.identity, thisManipulator.transform.position, thisManipulator.size);
				if (thisManipulator.enabled && GUIUtility.hotControl>0)
					Handles.Label(thisManipulator.transform.position+new Vector3(thisManipulator.size+1f,1f,0f), "Size: "+thisManipulator.size.ToString("f2"));
			
			// Box Bounds
			} else {
				DrawManipulatorBox(thisManipulator);
			}
			
			// Strength
			manipulatorHandlePosition = thisManipulator.transform.position+new Vector3(0f,thisManipulator.strength,0f);
			
			Handles.DrawLine(thisManipulator.transform.position, manipulatorHandlePosition);
			thisManipulator.strength = Handles.ScaleValueHandle(thisManipulator.strength, manipulatorHandlePosition, Quaternion.identity, HandleUtility.GetHandleSize(manipulatorHandlePosition), Handles.SphereCap, 1);      
			if (thisManipulator.enabled && GUIUtility.hotControl>0)
				Handles.Label(manipulatorHandlePosition+new Vector3(1f,1f,0f), "Strength: "+thisManipulator.strength.ToString("f2"));
			
			Handles.color = new Color(.4f,.6f,1f,.025f);
			Handles.DrawSolidDisc(thisManipulator.transform.position, Camera.current.transform.forward, thisManipulator.strength);
			Handles.color = new Color(.4f,.6f,1f,.5f);
			Handles.DrawSolidDisc(thisManipulator.transform.position, Camera.current.transform.forward, HandleUtility.GetHandleSize(thisManipulator.transform.position)*.05f);
		}
		
	}
	
	// Draws a Manipulator bounding box with handles in scene view
	public static void DrawManipulatorBox (ManipulatorObjectC manipulator) {
		Vector3 boxFrontTopLeft;
		Vector3 boxFrontTopRight;
		Vector3 boxFrontBottomLeft;
		Vector3 boxFrontBottomRight;
		Vector3 boxBackTopLeft;
		Vector3 boxBackTopRight;
		Vector3 boxBackBottomLeft;
		Vector3 boxBackBottomRight;
		Vector3 boxFrontDot;
		Vector3 boxLeftDot;
		Vector3 boxUpDot;
		
		// Always set positive values of bounds
		manipulator.bounds.extents = new Vector3(Mathf.Abs(manipulator.bounds.extents.x), Mathf.Abs(manipulator.bounds.extents.y), Mathf.Abs(manipulator.bounds.extents.z));
		
		// Set positions from bounds
		boxFrontTopLeft 		= new Vector3(manipulator.bounds.center.x - manipulator.bounds.extents.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z - manipulator.bounds.extents.z);
		boxFrontTopRight 		= new Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z - manipulator.bounds.extents.z);
		boxFrontBottomLeft 		= new Vector3(manipulator.bounds.center.x - manipulator.bounds.extents.x, manipulator.bounds.center.y - manipulator.bounds.extents.y, manipulator.bounds.center.z - manipulator.bounds.extents.z);
		boxFrontBottomRight 	= new Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y - manipulator.bounds.extents.y, manipulator.bounds.center.z - manipulator.bounds.extents.z);
		boxBackTopLeft 			= new Vector3(manipulator.bounds.center.x - manipulator.bounds.extents.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
		boxBackTopRight 		= new Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
		boxBackBottomLeft 		= new Vector3(manipulator.bounds.center.x - manipulator.bounds.extents.x, manipulator.bounds.center.y - manipulator.bounds.extents.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
		boxBackBottomRight 		= new Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y - manipulator.bounds.extents.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
		
		boxFrontDot				= new Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y, manipulator.bounds.center.z);
		boxUpDot				= new Vector3(manipulator.bounds.center.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z);
		boxLeftDot				= new Vector3(manipulator.bounds.center.x, manipulator.bounds.center.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
				
		// Apply transform positioning
		boxFrontTopLeft			= manipulator.transform.TransformPoint(boxFrontTopLeft);
		boxFrontTopRight		= manipulator.transform.TransformPoint(boxFrontTopRight);
		boxFrontBottomLeft		= manipulator.transform.TransformPoint(boxFrontBottomLeft);
		boxFrontBottomRight		= manipulator.transform.TransformPoint(boxFrontBottomRight);
		boxBackTopLeft			= manipulator.transform.TransformPoint(boxBackTopLeft);
		boxBackTopRight			= manipulator.transform.TransformPoint(boxBackTopRight);
		boxBackBottomLeft		= manipulator.transform.TransformPoint(boxBackBottomLeft);
		boxBackBottomRight		= manipulator.transform.TransformPoint(boxBackBottomRight);
		
		boxFrontDot				= manipulator.transform.TransformPoint(boxFrontDot);
		boxLeftDot				= manipulator.transform.TransformPoint(boxLeftDot);
		boxUpDot				= manipulator.transform.TransformPoint(boxUpDot);
				
		// Draw front lines
		Handles.DrawLine(boxFrontTopLeft, boxFrontTopRight);
		Handles.DrawLine(boxFrontTopRight, boxFrontBottomRight);
		Handles.DrawLine(boxFrontBottomLeft, boxFrontTopLeft);
		Handles.DrawLine(boxFrontBottomRight, boxFrontBottomLeft);
		
		// Draw back lines
		Handles.DrawLine(boxBackTopLeft, boxBackTopRight);
		Handles.DrawLine(boxBackTopRight, boxBackBottomRight);
		Handles.DrawLine(boxBackBottomLeft, boxBackTopLeft);
		Handles.DrawLine(boxBackBottomRight, boxBackBottomLeft);
		
		// Draw front to back lines
		Handles.DrawLine(boxFrontTopLeft, boxBackTopLeft);
		Handles.DrawLine(boxFrontTopRight, boxBackTopRight);
		Handles.DrawLine(boxFrontBottomLeft, boxBackBottomLeft);
		Handles.DrawLine(boxFrontBottomRight, boxBackBottomRight);
		
		// Draw extents handles
		boxFrontDot = Handles.Slider(boxFrontDot, manipulator.transform.right, HandleUtility.GetHandleSize(boxFrontDot)*.03f, Handles.DotCap, 0f);
		boxUpDot = Handles.Slider(boxUpDot, manipulator.transform.up, HandleUtility.GetHandleSize(boxUpDot)*.03f, Handles.DotCap, 0f);
		boxLeftDot = Handles.Slider(boxLeftDot, manipulator.transform.forward, HandleUtility.GetHandleSize(boxLeftDot)*.03f, Handles.DotCap, 0f);
		
		manipulator.bounds.extents = new Vector3(
				manipulator.transform.InverseTransformPoint(boxFrontDot).x-manipulator.bounds.center.x,
		 		manipulator.transform.InverseTransformPoint(boxUpDot).y-manipulator.bounds.center.y,
				manipulator.transform.InverseTransformPoint(boxLeftDot).z-manipulator.bounds.center.z
		);	
	}
	
	public static void RenderPlaygroundSettings () {
		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");
		EditorGUILayout.BeginVertical(boxStyle);

		playgroundFoldout = GUILayout.Toggle(playgroundFoldout, "Playground Manager", EditorStyles.foldout);
		if (playgroundFoldout) {
		
		EditorGUILayout.BeginVertical(boxStyle);
		if (playgroundScriptReference==null) {
			 playgroundScriptReference = GameObject.FindObjectOfType<PlaygroundC>();
			if (playgroundScriptReference)
				Initialize(playgroundScriptReference);
		}
		
		if (playgroundFoldout && playgroundScriptReference!=null) {
			playground.Update();
			
			// Particle System List
			if (GUILayout.Button("Particle Systems ("+playgroundScriptReference.particleSystems.Count+")", EditorStyles.toolbarDropDown)) particleSystemsFoldout=!particleSystemsFoldout;
			if (particleSystemsFoldout) {
				
				EditorGUILayout.Separator();
				
				if (playgroundScriptReference.particleSystems.Count>0) {
					for (int ps = 0; ps<playgroundScriptReference.particleSystems.Count; ps++) {
						
						EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(26));
						EditorGUILayout.BeginHorizontal();
						
						if (playgroundScriptReference.particleSystems[ps].particleSystemGameObject == Selection.activeGameObject) GUILayout.BeginHorizontal(boxStyle);
						
						GUILayout.Label(ps.ToString(), EditorStyles.miniLabel, new GUILayoutOption[]{GUILayout.Width(18)});
						if (GUILayout.Button(playgroundScriptReference.particleSystems[ps].particleSystemGameObject.name, EditorStyles.label)) {
							Selection.activeGameObject = playgroundScriptReference.particleSystems[ps].particleSystemGameObject;
						}
						EditorGUILayout.Separator();
						GUI.enabled = (playgroundScriptReference.particleSystems.Count>1);
						if(GUILayout.Button("U", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							particleSystems.MoveArrayElement(ps, ps==0?playgroundScriptReference.particleSystems.Count-1:ps-1);
						}
						if(GUILayout.Button("D", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							particleSystems.MoveArrayElement(ps, ps<playgroundScriptReference.particleSystems.Count-1?ps+1:0);
						}
						GUI.enabled = true;
						
						// Clone
						if(GUILayout.Button("+", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							GameObject ppsDuplicateGo = Instantiate(playgroundScriptReference.particleSystems[ps].particleSystemGameObject, playgroundScriptReference.particleSystems[ps].particleSystemTransform.position, playgroundScriptReference.particleSystems[ps].particleSystemTransform.rotation) as GameObject;
							PlaygroundParticlesC ppsDuplicate = ppsDuplicateGo.GetComponent<PlaygroundParticlesC>();
							
							// Cache state data
							for (int x = 0; x<ppsDuplicate.states.Count; x++)
								ppsDuplicate.states[x].Initialize();
							
							// Set particle count to initiate all arrays
							PlaygroundC.SetParticleCount(ppsDuplicate, ppsDuplicate.particleCount);
							
							// Add this to Manager
							if (PlaygroundC.reference!=null) {
								PlaygroundC.particlesQuantity++;
								PlaygroundC.reference.particleSystems.Add(ppsDuplicate);
								ppsDuplicate.particleSystemId = PlaygroundC.particlesQuantity;
								if (PlaygroundC.reference.autoGroup && ppsDuplicate.particleSystemTransform.parent==null)
									ppsDuplicate.particleSystemTransform.parent = PlaygroundC.referenceTransform;
							}
						}
						if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							if (EditorUtility.DisplayDialog(
								"Remove "+playgroundScriptReference.particleSystems[ps].particleSystemGameObject.name+"?",
								"Are you sure you want to remove this Particle Playground System?", 
								"Yes", "No")) {
									if (Selection.activeGameObject==playgroundScriptReference.particleSystems[ps].particleSystemGameObject)
										Selection.activeGameObject = PlaygroundC.referenceTransform.gameObject;
									PlaygroundC.Destroy(playgroundScriptReference.particleSystems[ps]);
									playground.ApplyModifiedProperties();
									return;
								}
						}
						
						if (playgroundScriptReference.particleSystems[ps].particleSystemGameObject == Selection.activeGameObject) GUILayout.EndHorizontal();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndVertical();
					}
				} else {
					EditorGUILayout.HelpBox("No particle systems created.", MessageType.Info);
				}
				
				if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
					PlaygroundParticlesC createdParticles = PlaygroundC.Particle();
					Selection.activeGameObject = createdParticles.particleSystemGameObject;
				}
				
				EditorGUILayout.Separator();
			}
			
			// Manipulators
			if (GUILayout.Button("Global Manipulators ("+playgroundScriptReference.manipulators.Count+")", EditorStyles.toolbarDropDown)) manipulatorsFoldout=!manipulatorsFoldout;
			if (manipulatorsFoldout) {
				
				EditorGUILayout.Separator();
				
				if (manipulators.arraySize>0) {
									
					
					for (int i = 0; i<manipulators.arraySize; i++) {
						string mName;
						if (playgroundScriptReference.manipulators[i].transform) {
							mName = playgroundScriptReference.manipulators[i].transform.name;
							if (mName.Length>24)
								mName = mName.Substring(0, 24)+"...";
						} else mName = "(Missing Transform!)";
						
						EditorGUILayout.BeginVertical(boxStyle);
						
						EditorGUILayout.BeginHorizontal();
						
						GUILayout.Label(i.ToString(), EditorStyles.miniLabel, GUILayout.Width(18));
						manipulatorListFoldout[i] = GUILayout.Toggle(manipulatorListFoldout[i], ManipulatorTypeName(playgroundScriptReference.manipulators[i].type), EditorStyles.foldout, GUILayout.Width(Screen.width/4));
						if (playgroundScriptReference.manipulators[i].transform) {
							if (GUILayout.Button(" ("+mName+")", EditorStyles.label)) {
								Selection.activeGameObject = playgroundScriptReference.manipulators[i].transform.gameObject;
							}
						} else {
							GUILayout.Button(ManipulatorTypeName(playgroundScriptReference.manipulators[i].type)+" (Missing Transform!)", EditorStyles.label);
						}
						EditorGUILayout.Separator();
						GUI.enabled = (playgroundScriptReference.manipulators.Count>1);
						if(GUILayout.Button("U", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							manipulators.MoveArrayElement(i, i==0?playgroundScriptReference.manipulators.Count-1:i-1);
						}
						if(GUILayout.Button("D", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							manipulators.MoveArrayElement(i, i<playgroundScriptReference.manipulators.Count-1?i+1:0);
						}
						GUI.enabled = true;
						if(GUILayout.Button("+", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							PlaygroundC.reference.manipulators.Add(playgroundScriptReference.manipulators[i].Clone());
							manipulatorListFoldout.Add(manipulatorListFoldout[i]);
						}
						if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							
							if (EditorUtility.DisplayDialog(
								"Remove "+ManipulatorTypeName(playgroundScriptReference.manipulators[i].type)+" Manipulator "+i+"?",
								"Are you sure you want to remove the Manipulator assigned to "+mName+"? (GameObject in Scene will remain intact)", 
								"Yes", "No")) {
									manipulators.DeleteArrayElementAtIndex(i);
									manipulatorListFoldout.RemoveAt(i);
									playground.ApplyModifiedProperties();
									return;
								}
						}
						
						EditorGUILayout.EndHorizontal();
						
						if (manipulatorListFoldout[i] && i<manipulators.arraySize) {
							RenderManipulatorSettings(playgroundScriptReference.manipulators[i], manipulators.GetArrayElementAtIndex(i), true);
						}
						GUI.enabled = true;
						EditorGUILayout.Separator();
						EditorGUILayout.EndVertical();
					}
					
				} else {
					EditorGUILayout.HelpBox("No manipulators created.", MessageType.Info);
				}
				
				if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
					if (Selection.gameObjects.Length>0 && Selection.activeGameObject.transform && Selection.activeTransform!=null)
						PlaygroundC.ManipulatorObject(Selection.activeGameObject.transform);
					else
						manipulators.InsertArrayElementAtIndex(manipulators.arraySize);
					manipulatorListFoldout.Add(true);
					SceneView.RepaintAll();
				}
					
				
				EditorGUILayout.Separator();
								
			}
			
			// Advanced Settings
			if (GUILayout.Button("Advanced", EditorStyles.toolbarDropDown)) advancedSettingsFoldout=!advancedSettingsFoldout;
			if (advancedSettingsFoldout) {
				
				EditorGUILayout.Separator();
				EditorGUILayout.PropertyField(calculate, new GUIContent("Calculate Particles", "Calculate forces on PlaygroundParticles objects. Disabling this overrides independently set values and halts all PlaygroundParticles objects."));
				EditorGUILayout.PropertyField(garbageCollectOnResize, new GUIContent("Garbage Collection", "Issue a GC.Collect when resizing particle lists."));
				EditorGUILayout.PropertyField(autoGroup, new GUIContent("Group Automatically", "Automatically parent a PlaygroundParticles object to Playground Manager if it has no parent."));
				EditorGUILayout.PropertyField(buildZeroAlphaPixels, new GUIContent("Build Zero Alpha Pixels", "Turn this on if you want to build particles from 0 alpha pixels into states."));
				EditorGUILayout.PropertyField(drawGizmos, new GUIContent("Scene Gizmos", "Show gizmos in Scene View for Playground objects."));
				EditorGUILayout.PropertyField(paintToolbox, new GUIContent("Paint Toolbox", "Show Paint toolbox in Scene View when Source is set to Paint"));
				EditorGUILayout.PropertyField(showShuriken, new GUIContent("Show Shuriken", "Show the Shuriken component in Inspector."));
				EditorGUILayout.PropertyField(pixelFilterMode, new GUIContent("Pixel Filter Mode", "Color filtering mode when creating particles from pixels in an image."));
				EditorGUILayout.Separator();
				
				// Time reset
				GUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Time Simulation");
				if(GUILayout.Button("Reset", EditorStyles.toolbarButton, GUILayout.Width(50))){
					PlaygroundC.TimeReset();
					for (int p = 0; p<playgroundScriptReference.particleSystems.Count; p++)
						playgroundScriptReference.particleSystems[p].Start();
				}
				GUILayout.EndHorizontal();
				
				EditorGUILayout.Separator();
				
				// Limits
				EditorGUILayout.BeginVertical(boxStyle);
				limitsFoldout = GUILayout.Toggle(limitsFoldout, "Editor Limits", EditorStyles.foldout);
				if (limitsFoldout) {
					EditorGUILayout.Separator();
					playgroundScriptReference.maximumAllowedTransitionTime = EditorGUILayout.FloatField("Transition Time", playgroundScriptReference.maximumAllowedTransitionTime);
					EditorGUILayout.Separator();
					playgroundScriptReference.maximumAllowedParticles = EditorGUILayout.IntField("Particle Count", playgroundScriptReference.maximumAllowedParticles);
					playgroundScriptReference.maximumAllowedLifetime = EditorGUILayout.FloatField("Particle Lifetime", playgroundScriptReference.maximumAllowedLifetime);
					playgroundScriptReference.maximumAllowedRotation = EditorGUILayout.FloatField("Particle Rotation", playgroundScriptReference.maximumAllowedRotation);
					playgroundScriptReference.maximumAllowedSize = EditorGUILayout.FloatField("Particle Size", playgroundScriptReference.maximumAllowedSize);
					playgroundScriptReference.maximumAllowedScale = EditorGUILayout.FloatField("Particle Scale", playgroundScriptReference.maximumAllowedScale);
					playgroundScriptReference.maximumAllowedSourceScatter = EditorGUILayout.FloatField("Source Scatter", playgroundScriptReference.maximumAllowedSourceScatter);
					EditorGUILayout.Separator();
					playgroundScriptReference.maximumAllowedDeltaMovementStrength = EditorGUILayout.FloatField("Delta Movement Strength", playgroundScriptReference.maximumAllowedDeltaMovementStrength);
					playgroundScriptReference.maximumAllowedDamping = EditorGUILayout.FloatField("Damping", playgroundScriptReference.maximumAllowedDamping);
					playgroundScriptReference.maximumAllowedInitialVelocity = EditorGUILayout.FloatField("Initial Velocity", playgroundScriptReference.maximumAllowedInitialVelocity);
					playgroundScriptReference.maximumAllowedVelocity = EditorGUILayout.FloatField("Velocity", playgroundScriptReference.maximumAllowedVelocity);
					EditorGUILayout.Separator();
					playgroundScriptReference.maximumAllowedCollisionRadius = EditorGUILayout.FloatField("Collision Radius", playgroundScriptReference.maximumAllowedCollisionRadius);
					playgroundScriptReference.maximumAllowedMass = EditorGUILayout.FloatField("Mass", playgroundScriptReference.maximumAllowedMass);
					playgroundScriptReference.maximumAllowedBounciness = EditorGUILayout.FloatField("Bounciness", playgroundScriptReference.maximumAllowedBounciness);
					EditorGUILayout.Separator();
					playgroundScriptReference.minimumAllowedUpdateRate = EditorGUILayout.IntField("Update Rate", playgroundScriptReference.minimumAllowedUpdateRate);
					playgroundScriptReference.maximumRenderSliders = EditorGUILayout.FloatField("Render Sliders", playgroundScriptReference.maximumRenderSliders);
					EditorGUILayout.Separator();
					playgroundScriptReference.maximumAllowedPaintPositions = EditorGUILayout.IntField("Paint Positions", playgroundScriptReference.maximumAllowedPaintPositions);
					playgroundScriptReference.minimumAllowedBrushScale = EditorGUILayout.FloatField("Brush Size Min", playgroundScriptReference.minimumAllowedBrushScale);
					playgroundScriptReference.maximumAllowedBrushScale = EditorGUILayout.FloatField("Brush Size Max", playgroundScriptReference.maximumAllowedBrushScale);
					playgroundScriptReference.minimumEraserRadius = EditorGUILayout.FloatField("Eraser Size Min", playgroundScriptReference.minimumEraserRadius);
					playgroundScriptReference.maximumEraserRadius = EditorGUILayout.FloatField("Eraser Size Max", playgroundScriptReference.maximumEraserRadius);
					playgroundScriptReference.maximumAllowedPaintSpacing = EditorGUILayout.FloatField("Paint Spacing", playgroundScriptReference.maximumAllowedPaintSpacing);
					EditorGUILayout.Separator();
					playgroundScriptReference.maximumAllowedManipulatorSize = EditorGUILayout.FloatField("Manipulator Size", playgroundScriptReference.maximumAllowedManipulatorSize);
					playgroundScriptReference.maximumAllowedManipulatorStrength = EditorGUILayout.FloatField("Manipulator Strength", playgroundScriptReference.maximumAllowedManipulatorStrength);
					playgroundScriptReference.maximumAllowedManipulatorZeroVelocity = EditorGUILayout.FloatField("Zero Velocity Strength", playgroundScriptReference.maximumAllowedManipulatorZeroVelocity);
					EditorGUILayout.Separator();
				}
				EditorGUILayout.EndVertical();
			}
			
			EditorGUI.indentLevel--;
			
			playground.ApplyModifiedProperties();
			EditorGUILayout.EndVertical();
		} else {
			EditorGUILayout.HelpBox("The Playground Manager runs all Particle Playground Systems in the scene, you need to create one in your scene to get started.", MessageType.Info);
			if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
				PlaygroundC.ResourceInstantiate("Playground Manager");
			}
			EditorGUILayout.EndVertical();
		}
		
	}
		EditorGUILayout.EndVertical();
	}
	
	public static void RenderManipulatorSettings (ManipulatorObjectC thisManipulator, SerializedProperty serializedManipulator, bool isPlayground) {
		SerializedProperty serializedManipulatorAffects = serializedManipulator.FindPropertyRelative("affects");
		SerializedProperty serializedManipulatorSize;
		SerializedProperty serializedManipulatorStrength;
		
		thisManipulator.enabled = EditorGUILayout.ToggleLeft("Enabled", thisManipulator.enabled);
		GUI.enabled = thisManipulator.enabled;
		
		EditorGUILayout.PropertyField(serializedManipulator.FindPropertyRelative("transform"), new GUIContent("Transform"));
		if (thisManipulator.transform) {
			EditorGUI.indentLevel++;
			thisManipulator.transform.position = EditorGUILayout.Vector3Field("Position", thisManipulator.transform.position);
			thisManipulator.transform.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", thisManipulator.transform.rotation.eulerAngles));
			thisManipulator.transform.localScale = EditorGUILayout.Vector3Field("Scale", thisManipulator.transform.localScale);
			EditorGUI.indentLevel--;
		}
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(serializedManipulator.FindPropertyRelative("type"), new GUIContent("Type"));
		
		// Render properties
		if (thisManipulator.type==MANIPULATORTYPEC.Property)
			RenderManipulatorProperty(thisManipulator, thisManipulator.property, serializedManipulator.FindPropertyRelative("property"));
		if (thisManipulator.type==MANIPULATORTYPEC.Combined) {
			if (thisManipulator.properties.Count>0) {
				SerializedProperty serializedManipulatorProperties = serializedManipulator.FindPropertyRelative("properties");
				int prevPropertyCount = thisManipulator.properties.Count;
				for (int i = 0; i<thisManipulator.properties.Count; i++) {
					if (thisManipulator.properties.Count!=prevPropertyCount) return;
					EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(26));
					EditorGUILayout.BeginHorizontal();
					GUILayout.Label(i.ToString(), EditorStyles.miniLabel, GUILayout.Width(18));
					thisManipulator.properties[i].unfolded = GUILayout.Toggle(thisManipulator.properties[i].unfolded, thisManipulator.properties[i].type.ToString(), EditorStyles.foldout);
					
					EditorGUILayout.Separator();
					GUI.enabled = (thisManipulator.enabled&&thisManipulator.properties.Count>1);
					if(GUILayout.Button("U", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						serializedManipulatorProperties.MoveArrayElement(i, i==0?thisManipulator.properties.Count-1:i-1);
					}
					if(GUILayout.Button("D", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						serializedManipulatorProperties.MoveArrayElement(i, i<thisManipulator.properties.Count-1?i+1:0);
					}
					GUI.enabled = thisManipulator.enabled;
					if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						thisManipulator.properties.RemoveAt(i);
						return;
					}
					EditorGUILayout.EndHorizontal();
					
					if (thisManipulator.properties[i].unfolded)
						RenderManipulatorProperty(thisManipulator, thisManipulator.properties[i], serializedManipulatorProperties.GetArrayElementAtIndex(i));
					EditorGUILayout.EndVertical();
				}
			} else {
				EditorGUILayout.HelpBox("No properties created.", MessageType.Info);
			}
			if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
				thisManipulator.properties.Add(new ManipulatorPropertyC());
			}
		}
				
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(serializedManipulator.FindPropertyRelative("shape"), new GUIContent("Shape"));
		
		if (thisManipulator.shape==MANIPULATORSHAPEC.Sphere) {
			serializedManipulatorSize = serializedManipulator.FindPropertyRelative("size");
			serializedManipulatorSize.floatValue = EditorGUILayout.Slider("Size", serializedManipulatorSize.floatValue, 0, playgroundScriptReference.maximumAllowedManipulatorSize);
		} else {
			EditorGUILayout.PropertyField(serializedManipulator.FindPropertyRelative("bounds"), new GUIContent("Bounds"));
		}
		
		EditorGUILayout.Separator();
		serializedManipulatorStrength = serializedManipulator.FindPropertyRelative("strength");
		serializedManipulatorStrength.floatValue = EditorGUILayout.Slider("Manipulator Strength", serializedManipulatorStrength.floatValue, 0, playgroundScriptReference.maximumAllowedManipulatorStrength);
		EditorGUILayout.Separator();
		
		thisManipulator.inverseBounds = EditorGUILayout.Toggle("Inverse Bounds", thisManipulator.inverseBounds);
		
		if (isPlayground) {
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(serializedManipulatorAffects, new GUIContent("Affects"));
		}
		
	}
	
	public static void RenderManipulatorProperty (ManipulatorObjectC thisManipulator, ManipulatorPropertyC thisManipulatorProperty, SerializedProperty serializedManipulatorProperty) {
		if (thisManipulatorProperty == null) thisManipulatorProperty = new ManipulatorPropertyC();
		
		EditorGUILayout.Separator();
		
		thisManipulatorProperty.type = (MANIPULATORPROPERTYTYPEC)EditorGUILayout.EnumPopup("Property Type", thisManipulatorProperty.type);
		
		// Velocity
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Velocity || thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.AdditiveVelocity) {
			thisManipulatorProperty.velocity = EditorGUILayout.Vector3Field("Particle Velocity", thisManipulatorProperty.velocity);
			thisManipulatorProperty.strength = EditorGUILayout.FloatField("Velocity Strength", thisManipulatorProperty.strength);
			thisManipulatorProperty.useLocalRotation = EditorGUILayout.Toggle("Local Rotation", thisManipulatorProperty.useLocalRotation);
		} else 
		// Color
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Color) {
			thisManipulatorProperty.color = EditorGUILayout.ColorField("Particle Color", thisManipulatorProperty.color);
			thisManipulatorProperty.strength = EditorGUILayout.FloatField("Color Strength", thisManipulatorProperty.strength);
			thisManipulatorProperty.onlyColorInRange = EditorGUILayout.Toggle("Only Color In Range", thisManipulatorProperty.onlyColorInRange);
			thisManipulatorProperty.keepColorAlphas = EditorGUILayout.Toggle("Keep Color Alphas", thisManipulatorProperty.keepColorAlphas);
		} else
		// Size
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Size) {
			thisManipulatorProperty.size = EditorGUILayout.FloatField("Particle Size", thisManipulatorProperty.size);
			thisManipulatorProperty.strength = EditorGUILayout.FloatField("Size Strength", thisManipulatorProperty.strength);
		} else
		// Target
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Target) {
			
			// Target List
			bool hasNull = false;
			EditorGUILayout.BeginVertical(boxStyle);
			targetsFoldout = GUILayout.Toggle(targetsFoldout, "Targets ("+thisManipulatorProperty.targets.Count+")", EditorStyles.foldout);
			if (targetsFoldout) {
				if (thisManipulatorProperty.targets.Count>0) {
					for (int t = 0; t<thisManipulatorProperty.targets.Count; t++) {
						EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(26));
						EditorGUILayout.BeginHorizontal();
						
						GUILayout.Label(t.ToString(), EditorStyles.miniLabel, GUILayout.Width(18));
						thisManipulatorProperty.targets[t] = EditorGUILayout.ObjectField("", thisManipulatorProperty.targets[t], typeof(Transform), true) as Transform;
						if (!thisManipulatorProperty.targets[t]) hasNull = true;
							
						EditorGUILayout.Separator();
						if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							thisManipulatorProperty.targets.RemoveAt(t);
						}
						
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndVertical();
					}
				} else {
					EditorGUILayout.HelpBox("No targets created.", MessageType.Info);
				}
				
				if (hasNull)
					EditorGUILayout.HelpBox("All targets must be assigned.", MessageType.Warning);
				
				if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
					thisManipulatorProperty.targets.Add(thisManipulator.transform);
				}
				EditorGUILayout.Separator();
			}
			EditorGUILayout.EndVertical();
			
			thisManipulatorProperty.strength = EditorGUILayout.FloatField("Target Strength", thisManipulatorProperty.strength);
			thisManipulatorProperty.onlyPositionInRange = EditorGUILayout.Toggle("Only Position In Range", thisManipulatorProperty.onlyPositionInRange);
			thisManipulatorProperty.zeroVelocityStrength = EditorGUILayout.Slider("Zero Velocity Strength", thisManipulatorProperty.zeroVelocityStrength, 0, playgroundScriptReference.maximumAllowedManipulatorZeroVelocity);
		} else
		// Death
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Death) {
			thisManipulatorProperty.strength = EditorGUILayout.FloatField("Death Strength", thisManipulatorProperty.strength);
		} else
		// Attractor
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Attractor) {
			thisManipulatorProperty.strength = EditorGUILayout.FloatField("Attractor Strength", thisManipulatorProperty.strength);
		} else
		// Gravitational
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Gravitational) {
			thisManipulatorProperty.strength = EditorGUILayout.FloatField("Gravitational Strength", thisManipulatorProperty.strength);
		} else
		// Repellent
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Repellent) {
			thisManipulatorProperty.strength = EditorGUILayout.FloatField("Repellent Strength", thisManipulatorProperty.strength);
		} else 
		// Lifetime Color
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.LifetimeColor) {
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("lifetimeColor"), new GUIContent("Lifetime Color"));
			thisManipulatorProperty.strength = EditorGUILayout.FloatField("Color Strength", thisManipulatorProperty.strength);
			thisManipulatorProperty.onlyColorInRange = EditorGUILayout.Toggle("Only Color In Range", thisManipulatorProperty.onlyColorInRange);
		}
		
		EditorGUILayout.Separator();
		if (thisManipulatorProperty.type!=MANIPULATORPROPERTYTYPEC.None &&
			thisManipulatorProperty.type!=MANIPULATORPROPERTYTYPEC.Attractor &&
			thisManipulatorProperty.type!=MANIPULATORPROPERTYTYPEC.Gravitational &&
			thisManipulatorProperty.type!=MANIPULATORPROPERTYTYPEC.Repellent
		)
			thisManipulatorProperty.transition = (MANIPULATORPROPERTYTRANSITIONC)EditorGUILayout.EnumPopup("Transition", thisManipulatorProperty.transition);
	}
	
	// Return name of a MANIPULATORTYPE
	public static string ManipulatorTypeName (MANIPULATORTYPEC mType) {
		string returnString;
		switch (mType) {
			case MANIPULATORTYPEC.None: returnString = "None"; break;
			case MANIPULATORTYPEC.Attractor: returnString = "Attractor"; break;
			case MANIPULATORTYPEC.AttractorGravitational: returnString = "Gravitational"; break;
			case MANIPULATORTYPEC.Repellent: returnString = "Repellent"; break;
			case MANIPULATORTYPEC.Property: returnString = "Property"; break;
			case MANIPULATORTYPEC.Combined: returnString = "Combined"; break;
			default: returnString = "Manipulator"; break;
		}
		return returnString;
	}
	
	// Return name of a MANIPULATORSHAPE
	public static string ManipulatorTypeName (MANIPULATORSHAPEC mShape) {
		string returnString;
		switch (mShape) {
			case MANIPULATORSHAPEC.Sphere: returnString = "Sphere"; break;
			case MANIPULATORSHAPEC.Box: returnString = "Box"; break;
			default: returnString = "Null"; break;
		}
		return returnString;
	}
	
}