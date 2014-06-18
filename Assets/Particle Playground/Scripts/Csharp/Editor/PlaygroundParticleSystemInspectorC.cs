using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor (typeof(PlaygroundParticlesC))]
class PlaygroundParticleSystemInspectorC : Editor {
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PlaygroundParticles variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public static PlaygroundParticlesC playgroundParticlesScriptReference;
	public static SerializedObject playgroundParticles;				// PlaygroundParticlesC
	public static SerializedProperty source;						// SOURCEC
	public static SerializedProperty sorting;						// SORTINGC
	public static SerializedProperty lifetimeSorting;				// AnimationCurve
	public static SerializedProperty nearestNeighborOrigin;			// int
	public static SerializedProperty activeState;					// int
	public static SerializedProperty transition;					// TRANSITION
	public static SerializedProperty transitionTime;				// float
	public static SerializedProperty particleCount;					// int
	public static SerializedProperty emissionRate;					// float
	public static SerializedProperty updateRate;					// int
	public static SerializedProperty worldObjectUpdateVertices;		// boolean
	public static SerializedProperty worldObjectUpdateNormals;		// boolean
	public static SerializedProperty emit;							// boolean
	public static SerializedProperty loop;							// boolean
	public static SerializedProperty disableOnDone;					// boolean
	public static SerializedProperty calculate;						// boolean
	public static SerializedProperty deltaMovementStrength;			// float
	public static SerializedProperty particleTimescale;				// float
	public static SerializedProperty sizeMin;						// float
	public static SerializedProperty sizeMax;						// float
	public static SerializedProperty lifetime;						// float
	public static SerializedProperty lifetimeSize;					// AnimationCurve
	public static SerializedProperty onlySourcePositioning;			// boolean
	public static SerializedProperty applyLifetimeVelocity;			// boolean
	public static SerializedProperty applyInitialVelocity;			// boolean
	public static SerializedProperty applyInitialLocalVelocity;		// boolean
	public static SerializedProperty applyVelocityBending;			// boolean
	public static SerializedProperty lifetimeVelocity;				// Vector3AnimationCurveC
	public static SerializedProperty initialVelocityShape;			// Vector3AnimationCurveC
	public static SerializedProperty overflowOffset;				// Vector3
	public static SerializedProperty overflowMode;					// OVERFLOWMODEC
	public static SerializedProperty initialVelocityMin;			// Vector3
	public static SerializedProperty initialVelocityMax;			// Vector3
	public static SerializedProperty initialLocalVelocityMin;		// Vector3
	public static SerializedProperty initialLocalVelocityMax;		// Vector3
	public static SerializedProperty lifetimeColor;					// Gradient
	public static SerializedProperty colorSource;					// COLORSOURCEC
	public static SerializedProperty collision;						// boolean
	public static SerializedProperty affectRigidbodies;				// boolean
	public static SerializedProperty mass;							// float
	public static SerializedProperty collisionRadius;				// float
	public static SerializedProperty collisionMask;					// LayerMask
	public static SerializedProperty bounciness;					// float
	public static Object particleMaterial;							// Material
	
	public static SerializedProperty states;						// ParticleStateC[]
	public static SerializedProperty worldObject;					// WorldObjectC
	public static SerializedProperty worldObjectGameObject;			// GameObject
	public static SerializedProperty skinnedWorldObject;			// SkinnedWorldObjectC
	public static SerializedProperty skinnedWorldObjectGameObject; 	// GameObject
	public static SerializedProperty sourceTransform;				// Transform
	public static SerializedProperty sourcePaint;					// PaintObjectC
	public static SerializedProperty sourceProjection;				// ParticleProjectionC
	
	public static SerializedProperty lifeTimeVelocityX;				// AnimationCurve
	public static SerializedProperty lifeTimeVelocityY;				// AnimationCurve
	public static SerializedProperty lifeTimeVelocityZ;				// AnimationCurve
	
	public static SerializedProperty initialVelocityShapeX;			// AnimationCurve
	public static SerializedProperty initialVelocityShapeY;			// AnimationCurve
	public static SerializedProperty initialVelocityShapeZ;			// AnimationCurve
	
	public static SerializedProperty manipulators;					// List.<ManipulatorObject>
	public static ParticleSystemRenderer shurikenRenderer;			// ParticleSystemRenderer
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Playground variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public static PlaygroundC playgroundScriptReference;			// PlaygroundC
	public static SerializedObject playground;						// PlaygroundC
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PlaygroundParticleSystemInspector variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// States
	public static int meshOrImage;
	public static string addStateName = "";
	public static Object addStateMesh;
	public static Object addStateTexture;
	public static Object addStateTransform;
	public static Object addStateDepthmap;
	public static float addStateDepthmapStrength = 1f;
	public static float addStateSize = 1f;
	public static float addStateScale = 1f;
	public static Vector3 addStateOffset;
	
	// Foldouts
	public static bool particlesFoldout = true;
	public static bool statesFoldout = true;
	public static bool createNewStateFoldout = false;
	public static bool sourceFoldout = false;
	public static bool particleSettingsFoldout = false;
	public static bool forcesFoldout = false;
	public static bool manipulatorsFoldout = false;
	public static bool collisionFoldout = false;
	public static bool collisionPlanesFoldout = false;
	public static bool renderingFoldout = false;
	public static bool advancedFoldout = false;
	public static  List<bool> statesListFoldout;
	public static bool toolboxFoldout = true;
	public static bool paintToolboxSettingsFoldout = false;
	public static List<bool> manipulatorListFoldout;
	
	// Paint variables
	public static int brushListStyle = 0;
	public static Color32 paintColor = new Color(1,1,1,1);
	public static bool useBrushColor = true;
	public static int selectedPaintMode;
	public static GUIStyle sceneBrushStyle;
	public static Object[] brushPrefabs;
	public static string[] brushNames;
	public static float[] paintSpacings;
	public static bool[] exceedMaxStopsPaintList;
	public static bool inPaintMode = false;
	public static Object paintTexture;
	public static PlaygroundBrushC[] brushPresets;
	public static int selectedBrushPreset = -1;
	public static bool brushPresetFoldout = false;
	public static SerializedProperty paintLayerMask;
	public static Tool lastActiveTool = Tool.None;
	public static float eraserRadius = 1f;
	private bool showNoAlphaWarning = false;
	
	// Projection variables
	public static SerializedProperty projectionMask;
	
	// GUI
	public static GUIStyle boxStyle;
	
	private Keyframe[] prevLifetimeSortingKeys;
	private SOURCEC previousSource;
	
	void OnEnable () {
	
		lastActiveTool = Tools.current;
		
		// Playground Particles
		playgroundParticlesScriptReference = target as PlaygroundParticlesC;
		playgroundParticles = new SerializedObject(playgroundParticlesScriptReference);
		
		shurikenRenderer = playgroundParticlesScriptReference.particleSystemGameObject.particleSystem.renderer as ParticleSystemRenderer;
		
		manipulators = playgroundParticles.FindProperty("manipulators");
		source = playgroundParticles.FindProperty("source");
		sorting = playgroundParticles.FindProperty("sorting");
		lifetimeSorting = playgroundParticles.FindProperty("lifetimeSorting");
		nearestNeighborOrigin = playgroundParticles.FindProperty("nearestNeighborOrigin");
		transition = playgroundParticles.FindProperty("transition");
		transitionTime = playgroundParticles.FindProperty("transitionTime");
		activeState = playgroundParticles.FindProperty("activeState");
		particleCount = playgroundParticles.FindProperty("particleCount");
		emissionRate = playgroundParticles.FindProperty("emissionRate");
		updateRate = playgroundParticles.FindProperty("updateRate");
		emit = playgroundParticles.FindProperty("emit");
		loop = playgroundParticles.FindProperty("loop");
		disableOnDone = playgroundParticles.FindProperty("disableOnDone");
		calculate = playgroundParticles.FindProperty("calculate");
		deltaMovementStrength = playgroundParticles.FindProperty("deltaMovementStrength");
		particleTimescale = playgroundParticles.FindProperty("particleTimescale");
		sizeMin = playgroundParticles.FindProperty("sizeMin");
		sizeMax = playgroundParticles.FindProperty("sizeMax");
		overflowOffset = playgroundParticles.FindProperty("overflowOffset");
		overflowMode = playgroundParticles.FindProperty("overflowMode");
		lifetime = playgroundParticles.FindProperty("lifetime");
		lifetimeSize = playgroundParticles.FindProperty("lifetimeSize");
		lifetimeVelocity = playgroundParticles.FindProperty("lifetimeVelocity");
		initialVelocityShape = playgroundParticles.FindProperty("initialVelocityShape");
		initialVelocityMin = playgroundParticles.FindProperty("initialVelocityMin");
		initialVelocityMax = playgroundParticles.FindProperty("initialVelocityMax");
		initialLocalVelocityMin = playgroundParticles.FindProperty("initialLocalVelocityMin");
		initialLocalVelocityMax = playgroundParticles.FindProperty("initialLocalVelocityMax");
		lifetimeColor = playgroundParticles.FindProperty("lifetimeColor");
		colorSource = playgroundParticles.FindProperty("colorSource");
		collision = playgroundParticles.FindProperty("collision");
		affectRigidbodies = playgroundParticles.FindProperty("affectRigidbodies");
		mass = playgroundParticles.FindProperty("mass");
		collisionRadius = playgroundParticles.FindProperty("collisionRadius");
		collisionMask = playgroundParticles.FindProperty("collisionMask");
		bounciness = playgroundParticles.FindProperty("bounciness");
		states = playgroundParticles.FindProperty("states");
		worldObject = playgroundParticles.FindProperty("worldObject");
		skinnedWorldObject = playgroundParticles.FindProperty("skinnedWorldObject");
		sourceTransform = playgroundParticles.FindProperty("sourceTransform");
		worldObjectUpdateVertices = playgroundParticles.FindProperty ("worldObjectUpdateVertices");
		worldObjectUpdateNormals = playgroundParticles.FindProperty("worldObjectUpdateNormals");
		sourcePaint = playgroundParticles.FindProperty("paint");
		sourceProjection = playgroundParticles.FindProperty("projection");
		
		playgroundParticlesScriptReference.shurikenParticleSystem = playgroundParticlesScriptReference.GetComponent<ParticleSystem>();
		playgroundParticlesScriptReference.particleSystemRenderer = playgroundParticlesScriptReference.shurikenParticleSystem.renderer;
		particleMaterial = playgroundParticlesScriptReference.particleSystemRenderer.sharedMaterial;
		
		onlySourcePositioning = playgroundParticles.FindProperty("onlySourcePositioning");
		
		applyLifetimeVelocity = playgroundParticles.FindProperty("applyLifetimeVelocity");
		lifeTimeVelocityX = lifetimeVelocity.FindPropertyRelative("x");
		lifeTimeVelocityY = lifetimeVelocity.FindPropertyRelative("y");
		lifeTimeVelocityZ = lifetimeVelocity.FindPropertyRelative("z");
		
		initialVelocityShapeX = initialVelocityShape.FindPropertyRelative("x");
		initialVelocityShapeY = initialVelocityShape.FindPropertyRelative("y");
		initialVelocityShapeZ = initialVelocityShape.FindPropertyRelative("z");
		
		applyInitialVelocity = playgroundParticles.FindProperty("applyInitialVelocity");
		applyInitialLocalVelocity = playgroundParticles.FindProperty("applyInitialLocalVelocity");
		applyVelocityBending = playgroundParticles.FindProperty("applyVelocityBending");
		
		worldObjectGameObject = worldObject.FindPropertyRelative("gameObject");
		skinnedWorldObjectGameObject = skinnedWorldObject.FindPropertyRelative("gameObject");
		
		// Sorting
		prevLifetimeSortingKeys = playgroundParticlesScriptReference.lifetimeSorting.keys;
		
		// Manipulator list
		manipulatorListFoldout = new List<bool>();
		manipulatorListFoldout.AddRange(new bool[playgroundParticlesScriptReference.manipulators.Count]);
		
		// States foldout
		statesListFoldout = new List<bool>();
		statesListFoldout.AddRange(new bool[playgroundParticlesScriptReference.states.Count]);

		previousSource = playgroundParticlesScriptReference.source;
		
		// Playground
		playgroundScriptReference = FindObjectOfType<PlaygroundC>();
		
		
		// Create a manager if no existing instance is in the scene
		if (!playgroundScriptReference && Selection.activeTransform!=null) {
			PlaygroundC.ResourceInstantiate("Playground Manager");
			playgroundScriptReference = FindObjectOfType<PlaygroundC>();
		}
		
		if (playgroundScriptReference!=null) {
			PlaygroundC.reference = playgroundScriptReference;

			// Serialize Playground
			playground = new SerializedObject(playgroundScriptReference);
			
			PlaygroundInspectorC.Initialize(playgroundScriptReference);
			
			
			// Add this PlaygroundParticles if not existing in Playground list
			if (!playgroundScriptReference.particleSystems.Contains(playgroundParticlesScriptReference) && Selection.activeTransform!=null)
				playgroundScriptReference.particleSystems.Add(playgroundParticlesScriptReference);
				
			// Cache components
			playgroundParticlesScriptReference.particleSystemGameObject = playgroundParticlesScriptReference.gameObject;
			playgroundParticlesScriptReference.particleSystemTransform = playgroundParticlesScriptReference.transform;
			playgroundParticlesScriptReference.particleSystemRenderer = playgroundParticlesScriptReference.renderer;
			playgroundParticlesScriptReference.shurikenParticleSystem = playgroundParticlesScriptReference.particleSystemGameObject.GetComponent<ParticleSystem>();
			playgroundParticlesScriptReference.particleSystemRenderer2 = playgroundParticlesScriptReference.particleSystemGameObject.particleSystem.renderer as ParticleSystemRenderer;
			
			// Set manager as parent 
			if (PlaygroundC.reference.autoGroup && playgroundParticlesScriptReference.particleSystemTransform!=null && playgroundParticlesScriptReference.particleSystemTransform.parent == null && Selection.activeTransform!=null)
				playgroundParticlesScriptReference.particleSystemTransform.parent = PlaygroundC.referenceTransform;
			
			// Issue a quick refresh
			if (!EditorApplication.isPlaying)
				foreach (PlaygroundParticlesC p in PlaygroundC.reference.particleSystems)
					p.Start ();
		}

		// State initial values
		if (addStateTransform==null)
			addStateTransform = (Transform)playgroundParticlesScriptReference.particleSystemTransform;
		
		// Visiblity of Shuriken component in Inspector
		if (!playgroundScriptReference || playgroundScriptReference && !playgroundScriptReference.showShuriken)
			playgroundParticlesScriptReference.shurikenParticleSystem.hideFlags = HideFlags.HideInInspector;
		else
			playgroundParticlesScriptReference.shurikenParticleSystem.hideFlags = HideFlags.None;
				
		// Set paint init
		paintLayerMask = sourcePaint.FindPropertyRelative("layerMask");
		
		LoadBrushes();
		
		// Set projection init
		projectionMask = sourceProjection.FindPropertyRelative("projectionMask");
	}
	
	public static void LoadBrushes () {
		// Set brush presets and custom brush texture
		brushPrefabs = Resources.LoadAll("Csharp/Brushes", typeof(PlaygroundBrushPresetC));
		brushNames = new string[brushPrefabs.Length];
		paintSpacings = new float[brushPrefabs.Length];
		brushPresets = new PlaygroundBrushC[brushPrefabs.Length];
		exceedMaxStopsPaintList = new bool[brushPrefabs.Length];
		for (int i = 0; i<brushPresets.Length; i++) {
			PlaygroundBrushPresetC thisBrushPrefab = brushPrefabs[i] as PlaygroundBrushPresetC;
			brushNames[i] = thisBrushPrefab.presetName;
			brushPresets[i] = new PlaygroundBrushC();
			brushPresets[i].texture = thisBrushPrefab.texture as Texture2D;
			brushPresets[i].detail = thisBrushPrefab.detail;
			brushPresets[i].scale = thisBrushPrefab.scale;
			brushPresets[i].distance = thisBrushPrefab.distance;
			
			paintSpacings[i] = thisBrushPrefab.spacing;
			exceedMaxStopsPaintList[i] = thisBrushPrefab.exceedMaxStopsPaint;
		}
		
		if (source.intValue==5 && paintTexture!=null)
			SetBrush(selectedBrushPreset);
		
		
		if (playgroundParticlesScriptReference.paint!=null && playgroundParticlesScriptReference.paint.brush!=null && playgroundParticlesScriptReference.paint.brush.texture!=null) {
			paintTexture = playgroundParticlesScriptReference.paint.brush.texture;
		}
	}
	
	public static void SetBrush (int i) {
		if (i>=0) {
			TextureImporter tAssetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(brushPresets[i].texture as UnityEngine.Object)) as TextureImporter;
			if (!tAssetImporter.isReadable) {
				Debug.Log(tAssetImporter.assetPath+" is not readable. Please change Read/Write Enabled on its Import Settings.");
				return; 
			}
			selectedBrushPreset = i;
			paintTexture = brushPresets[selectedBrushPreset].texture;
			playgroundParticlesScriptReference.paint.brush.SetTexture(brushPresets[selectedBrushPreset].texture);
			playgroundParticlesScriptReference.paint.brush.scale = brushPresets[selectedBrushPreset].scale;
			playgroundParticlesScriptReference.paint.brush.detail = brushPresets[selectedBrushPreset].detail;
			playgroundParticlesScriptReference.paint.brush.distance = brushPresets[selectedBrushPreset].distance;
			
			playgroundParticlesScriptReference.paint.spacing = paintSpacings[selectedBrushPreset];
			playgroundParticlesScriptReference.paint.exceedMaxStopsPaint = exceedMaxStopsPaintList[selectedBrushPreset];
		} else {
			playgroundParticlesScriptReference.paint.brush.SetTexture(paintTexture as Texture2D);
		}
		
		// Set brush preview style
		sceneBrushStyle = new GUIStyle();
		sceneBrushStyle.imagePosition = ImagePosition.ImageOnly;
		sceneBrushStyle.border = new RectOffset(0,0,0,0);
		sceneBrushStyle.stretchWidth = true;
		sceneBrushStyle.stretchHeight = true;
		SetBrushStyle();
	}
	
	public static void SetBrushStyle () {
		if (playgroundParticlesScriptReference.paint.brush==null || playgroundParticlesScriptReference.paint.brush.texture==null || sceneBrushStyle==null) return;
		float brushScale = playgroundParticlesScriptReference.paint.brush.scale;
		sceneBrushStyle.fixedWidth = playgroundParticlesScriptReference.paint.brush.texture.width*brushScale;
		sceneBrushStyle.fixedHeight = playgroundParticlesScriptReference.paint.brush.texture.height*brushScale;
		sceneBrushStyle.contentOffset = -new Vector2(playgroundParticlesScriptReference.paint.brush.texture.width/2, playgroundParticlesScriptReference.paint.brush.texture.height/2)*brushScale;
	}
	
	void OnDestroy () {
		brushPresets = null;
		inPaintMode = false;
		Tools.current = lastActiveTool;
	}
	
	public override void OnInspectorGUI () {
		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");
		
		if (Selection.activeTransform==null) {
			EditorGUILayout.LabelField("Please edit this from Hieararchy only.");
			return;
		}
		
		if (Event.current.type == EventType.ValidateCommand &&
			Event.current.commandName == "UndoRedoPerformed") {			
				PlaygroundParticlesC.SetParticleCount(playgroundParticlesScriptReference, playgroundParticlesScriptReference.particleCount);
				LifetimeSorting();
		}
		
		playgroundParticles.Update();
		
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Particle Playground v"+PlaygroundC.version.ToString(), EditorStyles.largeLabel, GUILayout.Height(20));
		
		EditorGUILayout.Separator();
		
		if(GUILayout.Button("Open Playground Wizard", EditorStyles.toolbarButton)) {
			PlaygroundParticleWindowC.ShowWindow();
		}
		GUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		
		// Particles
		EditorGUILayout.BeginVertical(boxStyle);
		particlesFoldout = GUILayout.Toggle(particlesFoldout, "Playground Particles", EditorStyles.foldout);
		if (particlesFoldout) {
			
			EditorGUILayout.BeginVertical(boxStyle);
			
			// Source Settings
			if (GUILayout.Button("Source", EditorStyles.toolbarDropDown)) sourceFoldout=!sourceFoldout;
			if (sourceFoldout) {
				
				EditorGUILayout.Separator();
				
				if (previousSource!=playgroundParticlesScriptReference.source) {
					LifetimeSorting();
				}
				EditorGUILayout.PropertyField(source, new GUIContent(
					"Source", 
					"Source is the target method for the particles in this Particle Playground System.\n\nState: Target position and color in a stored state\n\nTransform: Target transforms live in the scene\n\nWorldObject: Target each vertex in a mesh live in the scene\n\nSkinnedWorldObject: Target each vertex in a skinned mesh live in the scene\n\nScript: Behaviour controlled by custom scripts\n\nPaint: Target painted positions and colors made with a brush\n\nProjection: Target projected positions and colors made with a texture")
				);
				
				EditorGUILayout.Separator();
				
				// Source is State
				if (source.intValue == 0) {
					RenderStateSettings();
					
				// Source is Projection
				} else if (source.intValue == 6) {
					RenderProjectionSettings();
				
				// Source is Transforms
				} else if (source.intValue == 1) {
					GUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Transform");
					sourceTransform.objectReferenceValue = EditorGUILayout.ObjectField(sourceTransform.objectReferenceValue, typeof(Transform), true);
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Points:");
					EditorGUILayout.SelectableLabel(sourceTransform.objectReferenceValue!=null?"1":"0", GUILayout.MaxWidth(80));
					EditorGUILayout.Separator();
					if(GUILayout.Button("Set Particle Count", EditorStyles.toolbarButton)){
						PlaygroundParticlesC.SetParticleCount(playgroundParticlesScriptReference, sourceTransform.objectReferenceValue!=null?1:0);
						playgroundParticlesScriptReference.Start();
					}
					GUI.enabled = (sourceTransform.objectReferenceValue!=null);
					if(GUILayout.Button("++", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}))
						particleCount.intValue = particleCount.intValue+1;
					GUI.enabled = true;
					GUILayout.EndHorizontal();
					
				// Source is World Object
				} else if (source.intValue == 2) {
					GUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("World Object", GUILayout.Width(Mathf.FloorToInt(Screen.width/2.2f)-46));
					
					int worldObjRefId = worldObjectGameObject.objectReferenceInstanceIDValue;
					worldObjectGameObject.objectReferenceValue = EditorGUILayout.ObjectField(worldObjectGameObject.objectReferenceValue, typeof(GameObject), true);
					GUILayout.EndHorizontal();
					if (worldObjRefId!=playgroundParticlesScriptReference.worldObject.cachedId && worldObjectGameObject.objectReferenceValue!=null) {
						GameObject woGo = worldObjectGameObject.objectReferenceValue as GameObject;
						PlaygroundParticlesC.NewWorldObject(playgroundParticlesScriptReference, woGo.GetComponent<Transform>() as Transform);
					}
						
					GUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Points:");
					EditorGUILayout.SelectableLabel(playgroundParticlesScriptReference.worldObject.vertexPositions.Length.ToString(), GUILayout.MaxWidth(80));
					
					EditorGUILayout.Separator();
					if(GUILayout.Button("Set Particle Count", EditorStyles.toolbarButton)){
						PlaygroundParticlesC.SetParticleCount(playgroundParticlesScriptReference, playgroundParticlesScriptReference.worldObject.vertexPositions.Length);
						playgroundParticlesScriptReference.Start();
					}
					if(GUILayout.Button("++", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}))
						particleCount.intValue = particleCount.intValue+playgroundParticlesScriptReference.worldObject.vertexPositions.Length;
					GUILayout.EndHorizontal();

					GUILayout.BeginVertical(boxStyle);
					EditorGUILayout.LabelField("Procedural Options");
					EditorGUILayout.PropertyField(worldObjectUpdateVertices, new GUIContent(
						"Mesh Vertices Update",
						"Enable this if the World Object's mesh is procedural and changes vertices over time."
					));
					EditorGUILayout.PropertyField(worldObjectUpdateNormals, new GUIContent(
						"Mesh Normals Update",
						"Enable this if the World Object's mesh is procedural and changes normals over time."
					));
					GUILayout.EndVertical();
					
				// Source is Skinned World Object
				} else if (source.intValue == 3) {
					GUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Skinned World Object");
					
					int skinnedWorldObjRefId = skinnedWorldObjectGameObject.objectReferenceInstanceIDValue;
					skinnedWorldObjectGameObject.objectReferenceValue = EditorGUILayout.ObjectField(skinnedWorldObjectGameObject.objectReferenceValue, typeof(GameObject), true);
					GUILayout.EndHorizontal();
					if (skinnedWorldObjRefId!=playgroundParticlesScriptReference.skinnedWorldObject.cachedId && skinnedWorldObjectGameObject.objectReferenceValue!=null) {
						GameObject swoGo = skinnedWorldObjectGameObject.objectReferenceValue as GameObject;
						PlaygroundParticlesC.NewSkinnedWorldObject(playgroundParticlesScriptReference, swoGo.GetComponent<Transform>() as Transform);
					}
					
					if (playgroundParticlesScriptReference.skinnedWorldObject.mesh) {
						int prevDownResolutionSkinned = playgroundParticlesScriptReference.sourceDownResolution;
						playgroundParticlesScriptReference.sourceDownResolution = EditorGUILayout.IntSlider("Source Down Resolution", playgroundParticlesScriptReference.sourceDownResolution, 1, Mathf.RoundToInt (playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length/2));
						if (prevDownResolutionSkinned!=playgroundParticlesScriptReference.sourceDownResolution)
							LifetimeSorting();
					}
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Points:");
					if (playgroundParticlesScriptReference.sourceDownResolution<=1)
						EditorGUILayout.SelectableLabel(playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length.ToString(), GUILayout.MaxWidth(80));
					else
						EditorGUILayout.SelectableLabel((playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length/playgroundParticlesScriptReference.sourceDownResolution).ToString()+" ("+playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length.ToString()+")", GUILayout.MaxWidth(160));
					
					EditorGUILayout.Separator();
					if(GUILayout.Button("Set Particle Count", EditorStyles.toolbarButton)){
						PlaygroundParticlesC.SetParticleCount(playgroundParticlesScriptReference, playgroundParticlesScriptReference.sourceDownResolution<=1?playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length:playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length/playgroundParticlesScriptReference.sourceDownResolution);
						playgroundParticlesScriptReference.Start();
					}
					if(GUILayout.Button("++", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}))
						particleCount.intValue = particleCount.intValue+(playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length/playgroundParticlesScriptReference.sourceDownResolution);
					GUILayout.EndHorizontal();
					
				// Source is Script
				} else if (source.intValue == 4) {
					
					EditorGUILayout.HelpBox("This Particle Playground System is controlled by script. You can only emit particles from script in this source mode using PlaygroundParticlesC.Emit(position, velocity, color, parent). Please see the manual for more details.", MessageType.Info);
					EditorGUILayout.Separator();
					
					EditorGUILayout.BeginVertical(boxStyle);
					EditorGUILayout.BeginHorizontal();
					playgroundParticlesScriptReference.scriptedEmissionIndex = EditorGUILayout.IntField("Emission Index", Mathf.Clamp(playgroundParticlesScriptReference.scriptedEmissionIndex, 0, playgroundParticlesScriptReference.particleCount-1));
					if(GUILayout.Button("►", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)}) || GUILayout.RepeatButton("►►", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(32), GUILayout.Height(16)})) {
						PlaygroundC.Emit(playgroundParticlesScriptReference);
					}
					EditorGUILayout.EndHorizontal();
					
					playgroundParticlesScriptReference.scriptedEmissionPosition = EditorGUILayout.Vector3Field("Position", playgroundParticlesScriptReference.scriptedEmissionPosition);
					playgroundParticlesScriptReference.scriptedEmissionVelocity = EditorGUILayout.Vector3Field("Velocity", playgroundParticlesScriptReference.scriptedEmissionVelocity);
					
					if (playgroundParticlesScriptReference.colorSource == COLORSOURCEC.LifetimeColor) EditorGUILayout.HelpBox("Color is set by Rendering > Lifetime Color.", MessageType.Info);
					GUI.enabled = playgroundParticlesScriptReference.colorSource == COLORSOURCEC.Source;
					playgroundParticlesScriptReference.scriptedEmissionColor = EditorGUILayout.ColorField("Color", playgroundParticlesScriptReference.scriptedEmissionColor);
					GUI.enabled = true;
					playgroundParticlesScriptReference.scriptedEmissionParent = EditorGUILayout.ObjectField("Parent", playgroundParticlesScriptReference.scriptedEmissionParent as Transform, typeof(Transform), true) as Transform;
					
					EditorGUILayout.EndVertical();
					
				// Source is Paint
				} else if (source.intValue == 5) {
					
					if (playgroundParticlesScriptReference.paint==null) {
						PlaygroundC.PaintObject(playgroundParticlesScriptReference);
					}
					
					// Paint Mode
					EditorGUILayout.BeginVertical(boxStyle);
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Paint Mode");
					selectedPaintMode = GUILayout.Toolbar (selectedPaintMode, new string[]{"Dot","Brush","Eraser"}, EditorStyles.toolbarButton);
					EditorGUILayout.EndHorizontal();
					
					// Dot
					if (selectedPaintMode!=0) {
						EditorGUILayout.Separator();
					}
					
					// Brush
					if (selectedPaintMode==1) {
						EditorGUI.indentLevel++;
						EditorGUILayout.BeginVertical(boxStyle);
						brushPresetFoldout = GUILayout.Toggle(brushPresetFoldout, "Brush Presets", EditorStyles.foldout);
						EditorGUI.indentLevel--;
						if (brushPresetFoldout) {
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.Separator();
							brushListStyle = GUILayout.Toolbar (brushListStyle, new string[]{"Icons","List"}, EditorStyles.toolbarButton, GUILayout.MaxWidth(120));
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.Separator();
							int i;
							
							// Icons
							if (brushListStyle==0) {
								GUILayout.BeginHorizontal();
								for (i = 0; i<brushPresets.Length; i++) {
									EditorGUILayout.BeginVertical(new GUILayoutOption[]{GUILayout.Width(50), GUILayout.Height(62)});
									
									if (GUILayout.Button(brushPresets[i].texture, new GUILayoutOption[]{GUILayout.Width(32), GUILayout.Height(32)})){
										selectedBrushPreset = i;
										SetBrush(i);
									}
									if (brushNames.Length>0) {
										EditorGUILayout.LabelField(brushNames[i], EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[]{GUILayout.Width(50), GUILayout.Height(30)});
									}
									EditorGUILayout.EndVertical();
									if (i%(Screen.width/80)==0 && i>0) {
										EditorGUILayout.EndHorizontal();
										EditorGUILayout.BeginHorizontal();
									}
								}
								EditorGUILayout.EndHorizontal();
								
								
							// List
							} else {
								for (i = 0; i<brushPresets.Length; i++) {
									EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(22));
									EditorGUILayout.BeginHorizontal();
									if (GUILayout.Button(brushNames[i], EditorStyles.label)) {
										selectedBrushPreset = i;
										SetBrush(i);
									}
									EditorGUILayout.Separator();
									if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
										if (EditorUtility.DisplayDialog("Permanently delete this brush?", 
											"The brush "+brushNames[i]+" will be removed, are you sure?", 
											"Yes", 
											"No")) {
												AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(brushPrefabs[i] as UnityEngine.Object));
												LoadBrushes();
											}
									}
									EditorGUILayout.EndHorizontal();
									EditorGUILayout.EndVertical();
								}
							}
							
							// Create new brush
							if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
								PlaygroundCreateBrushWindowC.ShowWindow();
							}
						}
						
						EditorGUILayout.EndVertical();
						EditorGUILayout.Separator();
						
						GUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel("Brush Shape");
						paintTexture = EditorGUILayout.ObjectField(paintTexture, typeof(Texture2D), false) as Texture2D;
						GUILayout.EndHorizontal();
						playgroundParticlesScriptReference.paint.brush.detail = (BRUSHDETAILC)EditorGUILayout.EnumPopup("Detail", playgroundParticlesScriptReference.paint.brush.detail);
						playgroundParticlesScriptReference.paint.brush.scale = EditorGUILayout.Slider("Brush Scale", playgroundParticlesScriptReference.paint.brush.scale, playgroundScriptReference.minimumAllowedBrushScale, playgroundScriptReference.maximumAllowedBrushScale);
						playgroundParticlesScriptReference.paint.brush.distance = EditorGUILayout.FloatField("Brush Distance", playgroundParticlesScriptReference.paint.brush.distance);
						
						if (paintTexture!=null && paintTexture!=playgroundParticlesScriptReference.paint.brush.texture) {
							playgroundParticlesScriptReference.paint.brush.SetTexture(paintTexture as Texture2D);
							selectedBrushPreset = -1;
						}
						
						useBrushColor = EditorGUILayout.Toggle("Use Brush Color", useBrushColor);
					}
					
					
					// Eraser
					if (selectedPaintMode==2) {
						eraserRadius = EditorGUILayout.Slider("Eraser Radius", eraserRadius, playgroundScriptReference.minimumEraserRadius, playgroundScriptReference.maximumEraserRadius);
					}
					
					EditorGUILayout.EndVertical();
					EditorGUILayout.Separator();
					
					if (selectedPaintMode==1 && useBrushColor) GUI.enabled = false;
					paintColor = EditorGUILayout.ColorField("Color", paintColor);
					GUI.enabled = true;
					if (showNoAlphaWarning && !useBrushColor) {
						EditorGUILayout.HelpBox("You have no alpha in the color. No particle positions will be painted.", MessageType.Warning);
					}
					showNoAlphaWarning = (paintColor.a == 0);
					
					EditorGUILayout.PropertyField(paintLayerMask, new GUIContent("Paint Mask"));
					playgroundParticlesScriptReference.paint.spacing = EditorGUILayout.Slider("Paint Spacing", playgroundParticlesScriptReference.paint.spacing, .0f, playgroundScriptReference.maximumAllowedPaintSpacing);
					PlaygroundC.reference.paintMaxPositions = EditorGUILayout.IntSlider("Max Paint Positions", PlaygroundC.reference.paintMaxPositions, 0, playgroundScriptReference.maximumAllowedPaintPositions);
					playgroundParticlesScriptReference.paint.exceedMaxStopsPaint = EditorGUILayout.Toggle("Exceed Max Stops Paint", playgroundParticlesScriptReference.paint.exceedMaxStopsPaint);
					if (playgroundParticlesScriptReference.paint.exceedMaxStopsPaint && playgroundParticlesScriptReference.paint.positionLength>=PlaygroundC.reference.paintMaxPositions) {
						EditorGUILayout.HelpBox("You have exceeded max positions. No new paint positions are possible when Exceed Max Stops Paint is enabled.", MessageType.Warning);
					}
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Paint:");
					ProgressBar((playgroundParticlesScriptReference.paint.positionLength*1f)/PlaygroundC.reference.paintMaxPositions, playgroundParticlesScriptReference.paint.positionLength+"/"+PlaygroundC.reference.paintMaxPositions, Mathf.FloorToInt(Screen.width/2.2f)-65);
					EditorGUILayout.Separator();
					if(GUILayout.Button("Set Particle Count", EditorStyles.toolbarButton)){
						PlaygroundParticlesC.SetParticleCount(playgroundParticlesScriptReference, playgroundParticlesScriptReference.paint.positionLength);
						playgroundParticlesScriptReference.Start();
					}
					if(GUILayout.Button("++", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}))
						particleCount.intValue = particleCount.intValue+playgroundParticlesScriptReference.paint.positionLength;
					GUILayout.EndHorizontal();
					
					EditorGUILayout.Separator();
					
					GUILayout.BeginHorizontal();
					GUI.enabled = !(selectedPaintMode==1 && paintTexture==null);
					if (GUILayout.Button((inPaintMode?"Stop":"Start")+" Paint ", EditorStyles.toolbarButton, GUILayout.Width(80))){
						StartStopPaint();
					}
					
					GUI.enabled = (playgroundParticlesScriptReference.paint.positionLength>0);
					if(GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(50))){
						ClearPaint();
					}
					GUI.enabled = true;
					GUILayout.EndHorizontal();
					EditorGUILayout.Separator();
					
					if (playgroundParticlesScriptReference.paint.positionLength-1>playgroundParticlesScriptReference.particleCount)
						EditorGUILayout.HelpBox("You have more paint positions than particles. Increase Particle Count to see all painted positions.", MessageType.Warning);
					
					if (GUI.changed) {
						SetBrushStyle();
					}
					
				}
				EditorGUILayout.Separator();
				
			}
			
			// Particle Settings
			if (GUILayout.Button("Particle Settings", EditorStyles.toolbarDropDown)) particleSettingsFoldout=!particleSettingsFoldout;
			if (particleSettingsFoldout) {
				
				EditorGUILayout.Separator();
				
				if (source.intValue==4)
					EditorGUILayout.HelpBox("Some features are inactivated as this Particle Playground System is running in script mode.", MessageType.Info);
				
				GUILayout.BeginHorizontal();
				particleCount.intValue = EditorGUILayout.IntSlider("Particle Count", particleCount.intValue, 0, playgroundScriptReference.maximumAllowedParticles);
				if(GUILayout.Button("x2", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}))
					particleCount.intValue *= 2;
				GUILayout.EndHorizontal();
				
				GUI.enabled=(source.intValue!=4);
				emissionRate.floatValue = EditorGUILayout.Slider("Emisson Rate", emissionRate.floatValue, 0, 1f);
				
				EditorGUILayout.Separator();
				EditorGUILayout.PropertyField(overflowMode, new GUIContent(
					"Overflow Mode", 
					"The method to align the Overflow Offset by.")
				);
				if (playgroundParticlesScriptReference.overflowMode!=OVERFLOWMODEC.SourcePoint)
					overflowOffset.vector3Value = EditorGUILayout.Vector3Field("Overflow Offset", overflowOffset.vector3Value);
				else
					playgroundParticlesScriptReference.overflowOffset.z = EditorGUILayout.Slider("Overflow Offset (Z)", playgroundParticlesScriptReference.overflowOffset.z, -100.0f, 100.0f);
				EditorGUILayout.Separator();
				GUI.enabled=true;
				
				// Source Scattering
				GUI.enabled=(source.intValue!=4);
				bool prevScatterEnabled = playgroundParticlesScriptReference.applySourceScatter;
				Vector3 prevScatterMin = playgroundParticlesScriptReference.sourceScatterMin;
				Vector3 prevScatterMax = playgroundParticlesScriptReference.sourceScatterMax;
				playgroundParticlesScriptReference.applySourceScatter = EditorGUILayout.ToggleLeft("Source Scatter", playgroundParticlesScriptReference.applySourceScatter);
				GUI.enabled = (source.intValue!=4 && playgroundParticlesScriptReference.applySourceScatter);
					// X
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);
					GUILayout.Label("X", GUILayout.Width(50));
					EditorGUILayout.Separator();
					float sourceScatterMinX = playgroundParticlesScriptReference.sourceScatterMin.x;
					float sourceScatterMaxX = playgroundParticlesScriptReference.sourceScatterMax.x;
					EditorGUILayout.MinMaxSlider(ref sourceScatterMinX, ref sourceScatterMaxX, -playgroundScriptReference.maximumAllowedSourceScatter, playgroundScriptReference.maximumAllowedSourceScatter, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
					playgroundParticlesScriptReference.sourceScatterMin.x = sourceScatterMinX;
					playgroundParticlesScriptReference.sourceScatterMax.x = sourceScatterMaxX;
					playgroundParticlesScriptReference.sourceScatterMin.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sourceScatterMin.x, GUILayout.Width(50));
					playgroundParticlesScriptReference.sourceScatterMax.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sourceScatterMax.x, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					// Y
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);
					GUILayout.Label("Y");
					EditorGUILayout.Separator();
					float sourceScatterMinY = playgroundParticlesScriptReference.sourceScatterMin.y;
					float sourceScatterMaxY = playgroundParticlesScriptReference.sourceScatterMax.y;
					EditorGUILayout.MinMaxSlider(ref sourceScatterMinY, ref sourceScatterMaxY, -playgroundScriptReference.maximumAllowedSourceScatter, playgroundScriptReference.maximumAllowedSourceScatter, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
					playgroundParticlesScriptReference.sourceScatterMin.y = sourceScatterMinY;
					playgroundParticlesScriptReference.sourceScatterMax.y = sourceScatterMaxY;
					playgroundParticlesScriptReference.sourceScatterMin.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sourceScatterMin.y, GUILayout.Width(50));
					playgroundParticlesScriptReference.sourceScatterMax.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sourceScatterMax.y, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					// Z
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);
					GUILayout.Label("Z");
					EditorGUILayout.Separator();
					float sourceScatterMinZ = playgroundParticlesScriptReference.sourceScatterMin.z;
					float sourceScatterMaxZ = playgroundParticlesScriptReference.sourceScatterMax.z;
					EditorGUILayout.MinMaxSlider(ref sourceScatterMinZ, ref sourceScatterMaxZ, -playgroundScriptReference.maximumAllowedSourceScatter, playgroundScriptReference.maximumAllowedSourceScatter, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
					playgroundParticlesScriptReference.sourceScatterMin.z = sourceScatterMinZ;
					playgroundParticlesScriptReference.sourceScatterMax.z = sourceScatterMaxZ;
					playgroundParticlesScriptReference.sourceScatterMin.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sourceScatterMin.z, GUILayout.Width(50));
					playgroundParticlesScriptReference.sourceScatterMax.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sourceScatterMax.z, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					

				GUI.enabled = true;
				
				if (prevScatterEnabled!=playgroundParticlesScriptReference.applySourceScatter || prevScatterMin!=playgroundParticlesScriptReference.sourceScatterMin || prevScatterMax!=playgroundParticlesScriptReference.sourceScatterMax) {
					LifetimeSorting();
					playgroundParticlesScriptReference.RefreshScatter();
				}
				
				EditorGUILayout.Separator();
				
				// Emission
				bool prevEmit = playgroundParticlesScriptReference.emit;
				bool prevLoop = playgroundParticlesScriptReference.loop;
				playgroundParticlesScriptReference.emit = EditorGUILayout.Toggle("Emit Particles", playgroundParticlesScriptReference.emit);
				playgroundParticlesScriptReference.loop = EditorGUILayout.Toggle("Loop", playgroundParticlesScriptReference.loop);
				if (prevEmit!=playgroundParticlesScriptReference.emit || prevLoop!=playgroundParticlesScriptReference.loop&&playgroundParticlesScriptReference.loop) {
					playgroundParticlesScriptReference.simulationStarted = PlaygroundC.globalTime;
					playgroundParticlesScriptReference.loopExceeded = false;
					playgroundParticlesScriptReference.loopExceededOnParticle = -1;
					playgroundParticlesScriptReference.particleSystemGameObject.SetActive(true);
				}
				GUI.enabled = !loop.boolValue;
				disableOnDone.boolValue = EditorGUILayout.Toggle("Disable On Done", disableOnDone.boolValue);
				GUI.enabled = true;
				calculate.boolValue = EditorGUILayout.Toggle("Calculate Particles", calculate.boolValue);
				
				EditorGUILayout.Separator();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Size");
				EditorGUILayout.Separator();
				float sizeMin = playgroundParticlesScriptReference.sizeMin;
				float sizeMax = playgroundParticlesScriptReference.sizeMax;
				EditorGUILayout.MinMaxSlider(ref sizeMin, ref sizeMax, 0, playgroundScriptReference.maximumAllowedSize, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
				playgroundParticlesScriptReference.sizeMin = sizeMin;
				playgroundParticlesScriptReference.sizeMax = sizeMax;
				playgroundParticlesScriptReference.sizeMin = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sizeMin, GUILayout.Width(50));
				playgroundParticlesScriptReference.sizeMax = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sizeMax, GUILayout.Width(50));
				GUILayout.EndHorizontal();

				playgroundParticlesScriptReference.scale = EditorGUILayout.Slider("Scale", playgroundParticlesScriptReference.scale, 0, playgroundScriptReference.maximumAllowedScale);
				
				EditorGUILayout.Separator();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Inital Rotation");
				EditorGUILayout.Separator();
				float initialRotationMin = playgroundParticlesScriptReference.initialRotationMin;
				float initialRotationMax = playgroundParticlesScriptReference.initialRotationMax;
				EditorGUILayout.MinMaxSlider(ref initialRotationMin, ref initialRotationMax, -playgroundScriptReference.maximumAllowedRotation, playgroundScriptReference.maximumAllowedRotation, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
				playgroundParticlesScriptReference.initialRotationMin = initialRotationMin;
				playgroundParticlesScriptReference.initialRotationMax = initialRotationMax;
				playgroundParticlesScriptReference.initialRotationMin = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialRotationMin, GUILayout.Width(50));
				playgroundParticlesScriptReference.initialRotationMax = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialRotationMax, GUILayout.Width(50));
				GUILayout.EndHorizontal();
				
				GUI.enabled = !playgroundParticlesScriptReference.rotateTowardsDirection;
				GUILayout.BeginHorizontal();
				GUILayout.Label("Rotation");
				EditorGUILayout.Separator();
				float rotationSpeedMin = playgroundParticlesScriptReference.rotationSpeedMin;
				float rotationSpeedMax = playgroundParticlesScriptReference.rotationSpeedMax;
				EditorGUILayout.MinMaxSlider(ref rotationSpeedMin, ref rotationSpeedMax, -playgroundScriptReference.maximumAllowedRotation, playgroundScriptReference.maximumAllowedRotation, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
				playgroundParticlesScriptReference.rotationSpeedMin = rotationSpeedMin;
				playgroundParticlesScriptReference.rotationSpeedMax = rotationSpeedMax;
				playgroundParticlesScriptReference.rotationSpeedMin = EditorGUILayout.FloatField(playgroundParticlesScriptReference.rotationSpeedMin, GUILayout.Width(50));
				playgroundParticlesScriptReference.rotationSpeedMax = EditorGUILayout.FloatField(playgroundParticlesScriptReference.rotationSpeedMax, GUILayout.Width(50));

				GUILayout.EndHorizontal();
				
				GUI.enabled = true;
				
				playgroundParticlesScriptReference.rotateTowardsDirection = EditorGUILayout.Toggle("Rotate Towards Direction", playgroundParticlesScriptReference.rotateTowardsDirection);
				GUI.enabled = playgroundParticlesScriptReference.rotateTowardsDirection;
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.rotationNormal = EditorGUILayout.Vector3Field("Rotation Normal", playgroundParticlesScriptReference.rotationNormal);
					playgroundParticlesScriptReference.rotationNormal.x = Mathf.Clamp(playgroundParticlesScriptReference.rotationNormal.x, -1, 1);
					playgroundParticlesScriptReference.rotationNormal.y = Mathf.Clamp(playgroundParticlesScriptReference.rotationNormal.y, -1, 1);
					playgroundParticlesScriptReference.rotationNormal.z = Mathf.Clamp(playgroundParticlesScriptReference.rotationNormal.z, -1, 1);
					EditorGUI.indentLevel--;
				
				GUI.enabled = true;
				
				EditorGUILayout.Separator();
				
				lifetime.floatValue = EditorGUILayout.Slider("Lifetime", lifetime.floatValue, 0, playgroundScriptReference.maximumAllowedLifetime);
				lifetimeSize.animationCurveValue = EditorGUILayout.CurveField("Lifetime Size", lifetimeSize.animationCurveValue);
				
				// Sorting
				GUI.enabled=(source.intValue!=4);
				if (selectedSort!=sorting.intValue || selectedOrigin!=nearestNeighborOrigin.intValue) {
					LifetimeSorting();
				}
				selectedSort = sorting.intValue;
				selectedOrigin = nearestNeighborOrigin.intValue;
				EditorGUILayout.PropertyField(sorting, new GUIContent(
					"Lifetime Sorting", 
					"Determines how the particles are ordered on rebirth.\nScrambled: Randomly placed.\nScrambled Linear: Randomly placed but never at the same time.\nBurst: Alfa and Omega.\nLinear: Alfa to Omega.\nReversed: Omega to Alfa.")
				);
				
				if (sorting.intValue==5||sorting.intValue==6) {
					EditorGUI.indentLevel++;
					nearestNeighborOrigin.intValue = EditorGUILayout.IntSlider("Sort Origin", nearestNeighborOrigin.intValue, 0, playgroundParticlesScriptReference.particleCount);
					EditorGUI.indentLevel--;
				}
				
				// Custom lifetime sorting
				if (sorting.intValue==7) {
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.lifetimeSorting = EditorGUILayout.CurveField("Custom Sorting", playgroundParticlesScriptReference.lifetimeSorting);
					EditorGUI.indentLevel--;
					bool changed = prevLifetimeSortingKeys.Length!=playgroundParticlesScriptReference.lifetimeSorting.keys.Length;
					if (!changed)
						for (int k = 0; k<prevLifetimeSortingKeys.Length; k++) {
							if (playgroundParticlesScriptReference.lifetimeSorting.keys[k].value != prevLifetimeSortingKeys[k].value || playgroundParticlesScriptReference.lifetimeSorting.keys[k].time != prevLifetimeSortingKeys[k].time) {
								changed = true;
							}
						}
					if (changed) {
						LifetimeSorting();
						prevLifetimeSortingKeys = playgroundParticlesScriptReference.lifetimeSorting.keys;
					}
				}
				
				float prevLifetimeOffset = playgroundParticlesScriptReference.lifetimeOffset;
				playgroundParticlesScriptReference.lifetimeOffset = EditorGUILayout.Slider("Lifetime Offset", playgroundParticlesScriptReference.lifetimeOffset, -playgroundParticlesScriptReference.lifetime, playgroundParticlesScriptReference.lifetime);
				if (prevLifetimeOffset!=playgroundParticlesScriptReference.lifetimeOffset) {
					LifetimeSortingAll();
				}
				GUI.enabled = true;
				
				EditorGUILayout.Separator();
			}
						
			// Force Settings
			if (GUILayout.Button("Forces", EditorStyles.toolbarDropDown)) forcesFoldout=!forcesFoldout;
			if (forcesFoldout) {
				
				EditorGUILayout.Separator();
				
				onlySourcePositioning.boolValue = EditorGUILayout.Toggle("Only Source Positions", onlySourcePositioning.boolValue);
				
				EditorGUILayout.Separator();
				
				GUI.enabled = !onlySourcePositioning.boolValue;
				
				// Delta Movement
				if (playgroundParticlesScriptReference.source==SOURCEC.State && playgroundParticlesScriptReference.states!=null && playgroundParticlesScriptReference.states.Count>0 && playgroundParticlesScriptReference.states[playgroundParticlesScriptReference.activeState].stateTransform==null) {
					EditorGUILayout.HelpBox("Assign a transform to the active state to enable Delta Movement.", MessageType.Info);
					GUI.enabled = false;
				} else GUI.enabled = (source.intValue!=4 && !onlySourcePositioning.boolValue);
					playgroundParticlesScriptReference.calculateDeltaMovement = EditorGUILayout.ToggleLeft("Delta Movement", playgroundParticlesScriptReference.calculateDeltaMovement);
				GUI.enabled = (GUI.enabled && playgroundParticlesScriptReference.calculateDeltaMovement && !onlySourcePositioning.boolValue);
					EditorGUI.indentLevel++;
					deltaMovementStrength.floatValue = EditorGUILayout.Slider("Delta Movement Strength", deltaMovementStrength.floatValue, 0, playgroundScriptReference.maximumAllowedDeltaMovementStrength);
					EditorGUI.indentLevel--;
				GUI.enabled = !onlySourcePositioning.boolValue;
				EditorGUILayout.Separator();
				
				// Lifetime velocity
				applyLifetimeVelocity.boolValue = EditorGUILayout.ToggleLeft("Lifetime Velocity", applyLifetimeVelocity.boolValue);
				GUI.enabled = (applyLifetimeVelocity.boolValue&&!onlySourcePositioning.boolValue);
				EditorGUI.indentLevel++;
				lifeTimeVelocityX.animationCurveValue = EditorGUILayout.CurveField("X", lifeTimeVelocityX.animationCurveValue);
				lifeTimeVelocityY.animationCurveValue = EditorGUILayout.CurveField("Y", lifeTimeVelocityY.animationCurveValue);
				lifeTimeVelocityZ.animationCurveValue = EditorGUILayout.CurveField("Z", lifeTimeVelocityZ.animationCurveValue);
				EditorGUI.indentLevel--;
				GUI.enabled = !onlySourcePositioning.boolValue;
				EditorGUILayout.Separator();
				
				// Initial Velocity
				EditorGUILayout.Separator();
				applyInitialVelocity.boolValue = EditorGUILayout.ToggleLeft("Initial Velocity", applyInitialVelocity.boolValue);
				GUI.enabled = (applyInitialVelocity.boolValue&&!onlySourcePositioning.boolValue);
					
					// X
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);
					GUILayout.Label("X", GUILayout.Width(50));
					EditorGUILayout.Separator();
					float initialVelocityMinX = playgroundParticlesScriptReference.initialVelocityMin.x;
					float initialVelocityMaxX = playgroundParticlesScriptReference.initialVelocityMax.x;
					EditorGUILayout.MinMaxSlider(ref initialVelocityMinX, ref initialVelocityMaxX, -playgroundScriptReference.maximumAllowedInitialVelocity, playgroundScriptReference.maximumAllowedInitialVelocity, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
					playgroundParticlesScriptReference.initialVelocityMin.x = initialVelocityMinX;
					playgroundParticlesScriptReference.initialVelocityMax.x = initialVelocityMaxX;
					playgroundParticlesScriptReference.initialVelocityMin.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialVelocityMin.x, GUILayout.Width(50));
					playgroundParticlesScriptReference.initialVelocityMax.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialVelocityMax.x, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					// Y
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);
					GUILayout.Label("Y");
					EditorGUILayout.Separator();
					float initialVelocityMinY = playgroundParticlesScriptReference.initialVelocityMin.y;
					float initialVelocityMaxY = playgroundParticlesScriptReference.initialVelocityMax.y;
					EditorGUILayout.MinMaxSlider(ref initialVelocityMinY, ref initialVelocityMaxY, -playgroundScriptReference.maximumAllowedInitialVelocity, playgroundScriptReference.maximumAllowedInitialVelocity, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
					playgroundParticlesScriptReference.initialVelocityMin.y = initialVelocityMinY;
					playgroundParticlesScriptReference.initialVelocityMax.y = initialVelocityMaxY;
					playgroundParticlesScriptReference.initialVelocityMin.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialVelocityMin.y, GUILayout.Width(50));
					playgroundParticlesScriptReference.initialVelocityMax.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialVelocityMax.y, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					// Z
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);
					GUILayout.Label("Z");
					EditorGUILayout.Separator();
					float initialVelocityMinZ = playgroundParticlesScriptReference.initialVelocityMin.z;
					float initialVelocityMaxZ = playgroundParticlesScriptReference.initialVelocityMax.z;
					EditorGUILayout.MinMaxSlider(ref initialVelocityMinZ, ref initialVelocityMaxZ, -playgroundScriptReference.maximumAllowedInitialVelocity, playgroundScriptReference.maximumAllowedInitialVelocity, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
					playgroundParticlesScriptReference.initialVelocityMin.z = initialVelocityMinZ;
					playgroundParticlesScriptReference.initialVelocityMax.z = initialVelocityMaxZ;
					playgroundParticlesScriptReference.initialVelocityMin.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialVelocityMin.z, GUILayout.Width(50));
					playgroundParticlesScriptReference.initialVelocityMax.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialVelocityMax.z, GUILayout.Width(50));
					GUILayout.EndHorizontal();

				GUI.enabled = !onlySourcePositioning.boolValue;
				
				// Initial Local Velocity
				EditorGUILayout.Separator();
				GUI.enabled=(source.intValue!=4 && !onlySourcePositioning.boolValue);
				
				if (source.intValue==4) {
					GUI.enabled = true;
					EditorGUILayout.HelpBox("Initial Local Velocity is controlled by passed in velocity to Emit() in script mode.", MessageType.Info);
					GUI.enabled = false;
				}
				applyInitialLocalVelocity.boolValue = EditorGUILayout.ToggleLeft("Initial Local Velocity", applyInitialLocalVelocity.boolValue);
				if (playgroundParticlesScriptReference.source==SOURCEC.State && playgroundParticlesScriptReference.states!=null && playgroundParticlesScriptReference.states.Count>0 && playgroundParticlesScriptReference.states[playgroundParticlesScriptReference.activeState].stateTransform==null) {
					EditorGUILayout.HelpBox("Assign a transform to the active state to enable Initial Local Velocity.", MessageType.Info);
					GUI.enabled = false;
				} else GUI.enabled = (applyInitialLocalVelocity.boolValue&&!onlySourcePositioning.boolValue&&source.intValue!=4);
					
					// X
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);
					GUILayout.Label("X", GUILayout.Width(50));
					EditorGUILayout.Separator();
					float initialLocalVelocityMinX = playgroundParticlesScriptReference.initialLocalVelocityMin.x;
					float initialLocalVelocityMaxX = playgroundParticlesScriptReference.initialLocalVelocityMax.x;
					EditorGUILayout.MinMaxSlider(ref initialLocalVelocityMinX, ref initialLocalVelocityMaxX, -playgroundScriptReference.maximumAllowedInitialVelocity, playgroundScriptReference.maximumAllowedInitialVelocity, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
					playgroundParticlesScriptReference.initialLocalVelocityMin.x = initialLocalVelocityMinX;
					playgroundParticlesScriptReference.initialLocalVelocityMax.x = initialLocalVelocityMaxX;
					playgroundParticlesScriptReference.initialLocalVelocityMin.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialLocalVelocityMin.x, GUILayout.Width(50));
					playgroundParticlesScriptReference.initialLocalVelocityMax.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialLocalVelocityMax.x, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					// Y
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);
					GUILayout.Label("Y");
					EditorGUILayout.Separator();
					float initialLocalVelocityMinY = playgroundParticlesScriptReference.initialLocalVelocityMin.y;
					float initialLocalVelocityMaxY = playgroundParticlesScriptReference.initialLocalVelocityMax.y;
					EditorGUILayout.MinMaxSlider(ref initialLocalVelocityMinY, ref initialLocalVelocityMaxY, -playgroundScriptReference.maximumAllowedInitialVelocity, playgroundScriptReference.maximumAllowedInitialVelocity, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
					playgroundParticlesScriptReference.initialLocalVelocityMin.y = initialLocalVelocityMinY;
					playgroundParticlesScriptReference.initialLocalVelocityMax.y = initialLocalVelocityMaxY;
					playgroundParticlesScriptReference.initialLocalVelocityMin.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialLocalVelocityMin.y, GUILayout.Width(50));
					playgroundParticlesScriptReference.initialLocalVelocityMax.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialLocalVelocityMax.y, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					// Z
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);
					GUILayout.Label("Z");
					EditorGUILayout.Separator();
					float initialLocalVelocityMinZ = playgroundParticlesScriptReference.initialLocalVelocityMin.z;
					float initialLocalVelocityMaxZ = playgroundParticlesScriptReference.initialLocalVelocityMax.z;
					EditorGUILayout.MinMaxSlider(ref initialLocalVelocityMinZ, ref initialLocalVelocityMaxZ, -playgroundScriptReference.maximumAllowedInitialVelocity, playgroundScriptReference.maximumAllowedInitialVelocity, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
					playgroundParticlesScriptReference.initialLocalVelocityMin.z = initialLocalVelocityMinZ;
					playgroundParticlesScriptReference.initialLocalVelocityMax.z = initialLocalVelocityMaxZ;
					playgroundParticlesScriptReference.initialLocalVelocityMin.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialLocalVelocityMin.z, GUILayout.Width(50));
					playgroundParticlesScriptReference.initialLocalVelocityMax.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialLocalVelocityMax.z, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					
				EditorGUILayout.Separator();
				GUI.enabled = !onlySourcePositioning.boolValue;
				
				// Initial velocity shape
				playgroundParticlesScriptReference.applyInitialVelocityShape = EditorGUILayout.ToggleLeft("Initial Velocity Shape", playgroundParticlesScriptReference.applyInitialVelocityShape);
				GUI.enabled = (playgroundParticlesScriptReference.applyInitialVelocityShape&&!onlySourcePositioning.boolValue);
				EditorGUI.indentLevel++;
				initialVelocityShapeX.animationCurveValue = EditorGUILayout.CurveField("X", initialVelocityShapeX.animationCurveValue);
				initialVelocityShapeY.animationCurveValue = EditorGUILayout.CurveField("Y", initialVelocityShapeY.animationCurveValue);
				initialVelocityShapeZ.animationCurveValue = EditorGUILayout.CurveField("Z", initialVelocityShapeZ.animationCurveValue);
				EditorGUI.indentLevel--;
				GUI.enabled = !onlySourcePositioning.boolValue;
				
				// Velocity Bending
				EditorGUILayout.Separator();
				applyVelocityBending.boolValue = EditorGUILayout.ToggleLeft("Velocity Bending", applyVelocityBending.boolValue);
				GUI.enabled = (applyVelocityBending.boolValue&&!onlySourcePositioning.boolValue);
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.velocityBending = EditorGUILayout.Vector3Field("", playgroundParticlesScriptReference.velocityBending);
					EditorGUI.indentLevel--;
				EditorGUILayout.Separator();
				GUI.enabled = !onlySourcePositioning.boolValue;
				
				playgroundParticlesScriptReference.gravity = EditorGUILayout.Vector3Field("Gravity", playgroundParticlesScriptReference.gravity);
				playgroundParticlesScriptReference.damping = EditorGUILayout.Slider("Damping", playgroundParticlesScriptReference.damping, 0f, playgroundScriptReference.maximumAllowedDamping);
				playgroundParticlesScriptReference.maxVelocity = EditorGUILayout.Slider("Max Velocity", playgroundParticlesScriptReference.maxVelocity, 0, playgroundScriptReference.maximumAllowedVelocity);
				
				// Axis constraints
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Axis Constraints", GUILayout.Width(Mathf.FloorToInt(Screen.width/2.2f)-46));
				
				GUILayout.Label("X", GUILayout.Width(10));
				playgroundParticlesScriptReference.axisConstraints.x = EditorGUILayout.Toggle(playgroundParticlesScriptReference.axisConstraints.x, GUILayout.Width(16));
				GUILayout.Label("Y", GUILayout.Width(10));
				playgroundParticlesScriptReference.axisConstraints.y = EditorGUILayout.Toggle(playgroundParticlesScriptReference.axisConstraints.y, GUILayout.Width(16));
				GUILayout.Label("Z", GUILayout.Width(10));
				playgroundParticlesScriptReference.axisConstraints.z = EditorGUILayout.Toggle(playgroundParticlesScriptReference.axisConstraints.z, GUILayout.Width(16));
				GUILayout.EndHorizontal();
				GUI.enabled = true;
				
				EditorGUILayout.Separator();
		
			}
			
			// Manipulators Settings
			if (GUILayout.Button("Manipulators ("+playgroundParticlesScriptReference.manipulators.Count+")", EditorStyles.toolbarDropDown)) manipulatorsFoldout=!manipulatorsFoldout;
			if (manipulatorsFoldout) {
				
				EditorGUILayout.Separator();
				
				if (playgroundParticlesScriptReference.manipulators.Count>0) {
							
					for (int i = 0; i<playgroundParticlesScriptReference.manipulators.Count; i++) {
						string mName;
						if (playgroundParticlesScriptReference.manipulators[i].transform) {
							mName = playgroundParticlesScriptReference.manipulators[i].transform.name;
							if (mName.Length>24)
								mName = mName.Substring(0, 24)+"...";
						} else mName = "(Missing Transform!)";
						
						EditorGUILayout.BeginVertical("box");
						
						EditorGUILayout.BeginHorizontal();
						
						GUILayout.Label(i.ToString(), EditorStyles.miniLabel, GUILayout.Width(18));
						manipulatorListFoldout[i] = GUILayout.Toggle(manipulatorListFoldout[i], PlaygroundInspectorC.ManipulatorTypeName(playgroundParticlesScriptReference.manipulators[i].type), EditorStyles.foldout, GUILayout.Width(Screen.width/4));
						if (playgroundParticlesScriptReference.manipulators[i].transform) {
							if (GUILayout.Button(" ("+mName+")", EditorStyles.label)) {
								Selection.activeGameObject = playgroundParticlesScriptReference.manipulators[i].transform.gameObject;
							}
						} else {
							GUILayout.Button(PlaygroundInspectorC.ManipulatorTypeName(playgroundParticlesScriptReference.manipulators[i].type)+" (Missing Transform!)", EditorStyles.label);
						}
						EditorGUILayout.Separator();
						GUI.enabled = (playgroundParticlesScriptReference.manipulators.Count>1);
						if(GUILayout.Button("U", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							manipulators.MoveArrayElement(i, i==0?playgroundParticlesScriptReference.manipulators.Count-1:i-1);
						}
						if(GUILayout.Button("D", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							manipulators.MoveArrayElement(i, i<playgroundParticlesScriptReference.manipulators.Count-1?i+1:0);
						}
						GUI.enabled = true;
						if(GUILayout.Button("+", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							playgroundParticlesScriptReference.manipulators.Add(playgroundParticlesScriptReference.manipulators[i].Clone());
							manipulatorListFoldout.Add(manipulatorListFoldout[i]);
						}
						if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							
							if (EditorUtility.DisplayDialog(
								"Remove "+PlaygroundInspectorC.ManipulatorTypeName(playgroundParticlesScriptReference.manipulators[i].type)+" Manipulator "+i+"?",
								"Are you sure you want to remove the Manipulator assigned to "+mName+"? (GameObject in Scene will remain intact)", 
								"Yes", "No")) {
									manipulators.DeleteArrayElementAtIndex(i);
									manipulatorListFoldout.RemoveAt(i);
									playgroundParticles.ApplyModifiedProperties();
									return;
								}
						}
						
						EditorGUILayout.EndHorizontal();
						
						if (manipulatorListFoldout[i] && i<manipulators.arraySize) {
							PlaygroundInspectorC.RenderManipulatorSettings(playgroundParticlesScriptReference.manipulators[i], manipulators.GetArrayElementAtIndex(i), false);
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
						PlaygroundC.ManipulatorObject(Selection.activeGameObject.transform, playgroundParticlesScriptReference);
					else
						manipulators.InsertArrayElementAtIndex(manipulators.arraySize);
					manipulatorListFoldout.Add(true);
					SceneView.RepaintAll();
				}
				
				EditorGUILayout.Separator();
			}
			
			// Collision Settings
			if (GUILayout.Button("Collision", EditorStyles.toolbarDropDown)) collisionFoldout=!collisionFoldout;
			if (collisionFoldout) {
			
				EditorGUILayout.Separator();
						
				collision.boolValue = EditorGUILayout.ToggleLeft("Collision", collision.boolValue);
				EditorGUI.indentLevel++;
				GUI.enabled = collision.boolValue;
				EditorGUILayout.PropertyField(collisionMask, new GUIContent("Collision Mask"));
				affectRigidbodies.boolValue = EditorGUILayout.Toggle("Collide With Rigidbodies", affectRigidbodies.boolValue);
				mass.floatValue = EditorGUILayout.Slider("Mass", mass.floatValue, 0, playgroundScriptReference.maximumAllowedMass);
				collisionRadius.floatValue = EditorGUILayout.Slider("Collision Radius", collisionRadius.floatValue, 0, playgroundScriptReference.maximumAllowedCollisionRadius);
				playgroundParticlesScriptReference.lifetimeLoss = EditorGUILayout.Slider("Lifetime Loss", playgroundParticlesScriptReference.lifetimeLoss, 0f, 1f);
				
				EditorGUILayout.Separator();
				bounciness.floatValue = EditorGUILayout.Slider("Bounciness", bounciness.floatValue, 0, playgroundScriptReference.maximumAllowedBounciness);
				EditorGUILayout.PrefixLabel("Random Bounce");
				// X
				GUILayout.BeginHorizontal();
				GUILayout.Space(32);
				GUILayout.Label("X", GUILayout.Width(50));
				EditorGUILayout.Separator();
				float bounceRandomMinX = playgroundParticlesScriptReference.bounceRandomMin.x;
				float bounceRandomMaxX = playgroundParticlesScriptReference.bounceRandomMax.x;
				EditorGUILayout.MinMaxSlider(ref bounceRandomMinX, ref bounceRandomMaxX, -1f, 1f, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
				playgroundParticlesScriptReference.bounceRandomMin.x = bounceRandomMinX;
				playgroundParticlesScriptReference.bounceRandomMax.x = bounceRandomMaxX;
				playgroundParticlesScriptReference.bounceRandomMin.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.bounceRandomMin.x, GUILayout.Width(50));
				playgroundParticlesScriptReference.bounceRandomMax.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.bounceRandomMax.x, GUILayout.Width(50));
				GUILayout.EndHorizontal();
				// Y
				GUILayout.BeginHorizontal();
				GUILayout.Space(32);
				GUILayout.Label("Y");
				EditorGUILayout.Separator();
				float bounceRandomMinY = playgroundParticlesScriptReference.bounceRandomMin.y;
				float bounceRandomMaxY = playgroundParticlesScriptReference.bounceRandomMax.y;
				EditorGUILayout.MinMaxSlider(ref bounceRandomMinY, ref bounceRandomMaxY, -1f, 1f, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
				playgroundParticlesScriptReference.bounceRandomMin.y = bounceRandomMinY;
				playgroundParticlesScriptReference.bounceRandomMax.y = bounceRandomMaxY;
				playgroundParticlesScriptReference.bounceRandomMin.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.bounceRandomMin.y, GUILayout.Width(50));
				playgroundParticlesScriptReference.bounceRandomMax.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.bounceRandomMax.y, GUILayout.Width(50));
				GUILayout.EndHorizontal();
				// Z
				GUILayout.BeginHorizontal();
				GUILayout.Space(32);
				GUILayout.Label("Z");
				EditorGUILayout.Separator();
				float bounceRandomMinZ = playgroundParticlesScriptReference.bounceRandomMin.z;
				float bounceRandomMaxZ = playgroundParticlesScriptReference.bounceRandomMax.z;
				EditorGUILayout.MinMaxSlider(ref bounceRandomMinZ, ref bounceRandomMaxZ, -1f, 1f, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-105));
				playgroundParticlesScriptReference.bounceRandomMin.z = bounceRandomMinZ;
				playgroundParticlesScriptReference.bounceRandomMax.z = bounceRandomMaxZ;
				playgroundParticlesScriptReference.bounceRandomMin.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.bounceRandomMin.z, GUILayout.Width(50));
				playgroundParticlesScriptReference.bounceRandomMax.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.bounceRandomMax.z, GUILayout.Width(50));
				GUILayout.EndHorizontal();
				
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();

				// Collision planes List
				EditorGUILayout.BeginVertical(boxStyle);
				collisionPlanesFoldout = GUILayout.Toggle(collisionPlanesFoldout, "Collision Planes ("+playgroundParticlesScriptReference.colliders.Count+")", EditorStyles.foldout);
				if (collisionPlanesFoldout) {
					if (playgroundParticlesScriptReference.colliders.Count>0) {
						for (int c = 0; c<playgroundParticlesScriptReference.colliders.Count; c++) {
							EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(26));
							EditorGUILayout.BeginHorizontal();
							
							playgroundParticlesScriptReference.colliders[c].enabled = EditorGUILayout.Toggle("", playgroundParticlesScriptReference.colliders[c].enabled, GUILayout.Width(16));
							GUI.enabled = (playgroundParticlesScriptReference.colliders[c].enabled&&collision.boolValue);
							playgroundParticlesScriptReference.colliders[c].transform = EditorGUILayout.ObjectField("", playgroundParticlesScriptReference.colliders[c].transform, typeof(Transform), true) as Transform;
							GUI.enabled = collision.boolValue;
							
							EditorGUILayout.Separator();
							if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								playgroundParticlesScriptReference.colliders.RemoveAt(c);
							}
							
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.EndVertical();
						}
					} else {
						EditorGUILayout.HelpBox("No collision planes created.", MessageType.Info);
					}
					
					if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
						playgroundParticlesScriptReference.colliders.Add(new PlaygroundColliderC());
					}
					
					EditorGUILayout.Separator();
					playgroundScriptReference.collisionPlaneScale = EditorGUILayout.Slider("Gizmo Scale", playgroundScriptReference.collisionPlaneScale, 0, 1);
					EditorGUILayout.Separator();
				}
				EditorGUILayout.EndVertical();
				
				GUI.enabled = true;
				
				EditorGUILayout.Separator();
			}
			
			// Render Settings
			if (GUILayout.Button("Rendering", EditorStyles.toolbarDropDown)) renderingFoldout=!renderingFoldout;
			if (renderingFoldout) {
				
				EditorGUILayout.Separator();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Material");
				Material currentMat = particleMaterial as Material;
				particleMaterial = EditorGUILayout.ObjectField(particleMaterial, typeof(Material), false);
				if (currentMat!=particleMaterial) 
					PlaygroundParticlesC.SetMaterial(playgroundParticlesScriptReference, particleMaterial as Material);
				GUILayout.EndHorizontal();
				
				EditorGUILayout.PropertyField(lifetimeColor, new GUIContent("Lifetime Color"));
				
				EditorGUILayout.PropertyField(colorSource, new GUIContent("Color Source"));
				
				playgroundParticlesScriptReference.sourceUsesLifetimeAlpha = EditorGUILayout.Toggle("Source Uses Lifetime Alpha", playgroundParticlesScriptReference.sourceUsesLifetimeAlpha);
				
				EditorGUILayout.Separator();
				
				GUILayout.BeginVertical(boxStyle);
				
				// Render mode
				shurikenRenderer.renderMode = (ParticleSystemRenderMode)EditorGUILayout.EnumPopup("Render Mode", shurikenRenderer.renderMode);
				switch (shurikenRenderer.renderMode) {
					case ParticleSystemRenderMode.Stretch:
						EditorGUI.indentLevel++;
						shurikenRenderer.cameraVelocityScale = EditorGUILayout.Slider("Camera Scale", shurikenRenderer.cameraVelocityScale, -playgroundScriptReference.maximumRenderSliders, playgroundScriptReference.maximumRenderSliders);
						shurikenRenderer.velocityScale = EditorGUILayout.Slider("Speed Scale", shurikenRenderer.velocityScale, -playgroundScriptReference.maximumRenderSliders, playgroundScriptReference.maximumRenderSliders);
						shurikenRenderer.lengthScale = EditorGUILayout.Slider("Length Scale", shurikenRenderer.lengthScale, -playgroundScriptReference.maximumRenderSliders, playgroundScriptReference.maximumRenderSliders);	
						EditorGUI.indentLevel--;
					break;
					case ParticleSystemRenderMode.Mesh:
						shurikenRenderer.mesh = EditorGUILayout.ObjectField(shurikenRenderer.mesh, typeof(Mesh), false) as Mesh;
						GUI.enabled = false;
					break;
				}
				shurikenRenderer.maxParticleSize = EditorGUILayout.Slider("Max Particle Size", shurikenRenderer.maxParticleSize, 0f, 1f);
				GUI.enabled = true;
				
				GUILayout.EndVertical();
				
				// Sort order/layer
				/*
				GUILayout.BeginVertical(boxStyle);
				playgroundParticlesScriptReference.particleSystemRenderer.sortingOrder = EditorGUILayout.IntField("Sorting Order", playgroundParticlesScriptReference.particleSystemRenderer.sortingOrder);
				playgroundParticlesScriptReference.particleSystemRenderer.sortingLayerName = EditorGUILayout.TextField("Sorting Layer Name", playgroundParticlesScriptReference.particleSystemRenderer.sortingLayerName);
				GUILayout.EndVertical();
				*/
				EditorGUILayout.Separator();
			}
			
			// Advanced Settings
			if (GUILayout.Button("Advanced", EditorStyles.toolbarDropDown)) advancedFoldout=!advancedFoldout;
			if (advancedFoldout) {
				
				EditorGUILayout.Separator();
				
				// Update rate
				updateRate.intValue = EditorGUILayout.IntSlider("Update Rate (Frames)", updateRate.intValue, playgroundScriptReference.minimumAllowedUpdateRate, 1);
				
				EditorGUILayout.Separator();
				
				playgroundParticlesScriptReference.particleSystem.simulationSpace = (ParticleSystemSimulationSpace)EditorGUILayout.EnumPopup("Simulation Space", playgroundParticlesScriptReference.particleSystem.simulationSpace);
				/*if (playgroundParticlesScriptReference.particleSystem.simulationSpace==ParticleSystemSimulationSpace.Local) {
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.applyLocalSpaceMovementCompensation = EditorGUILayout.Toggle ("Movement Compensation", playgroundParticlesScriptReference.applyLocalSpaceMovementCompensation);
					EditorGUI.indentLevel--;
				}*/

				EditorGUILayout.Separator();
				GUILayout.BeginVertical(boxStyle);
				EditorGUILayout.LabelField("Rebirth Options");
				playgroundParticlesScriptReference.applyRandomSizeOnRebirth = EditorGUILayout.Toggle ("Random Size", playgroundParticlesScriptReference.applyRandomSizeOnRebirth);
				playgroundParticlesScriptReference.applyRandomRotationOnRebirth = EditorGUILayout.Toggle ("Random Rotation", playgroundParticlesScriptReference.applyRandomRotationOnRebirth);
				playgroundParticlesScriptReference.applyRandomScatterOnRebirth = EditorGUILayout.Toggle ("Random Scatter", playgroundParticlesScriptReference.applyRandomScatterOnRebirth);
				GUILayout.EndVertical();
				EditorGUILayout.Separator();

				GUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Particle Pool");
				
				// Clear
				if(GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(50))){
					PlaygroundParticlesC.Clear(playgroundParticlesScriptReference);
				}
				
				// Rebuild
				if(GUILayout.Button("Rebuild", EditorStyles.toolbarButton, GUILayout.Width(50))){
					PlaygroundParticlesC.SetParticleCount(playgroundParticlesScriptReference, playgroundParticlesScriptReference.particleCount);
					LifetimeSorting();
				}
				GUILayout.EndHorizontal();
				
				EditorGUILayout.Separator();
				
			}

			EditorGUILayout.EndVertical();
			
			if (playgroundParticlesScriptReference.shurikenParticleSystem.isPaused || playgroundParticlesScriptReference.shurikenParticleSystem.isStopped)
				playgroundParticlesScriptReference.shurikenParticleSystem.Play();
			
			previousSource = playgroundParticlesScriptReference.source;
			playgroundParticles.ApplyModifiedProperties();
			
		}
		
		EditorGUILayout.EndVertical();
		
		// Playground Manager - Particle Systems, Manipulators
		PlaygroundInspectorC.RenderPlaygroundSettings();
		
	}
	
	public void ProgressBar (float val, string label, float width) {
		Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
		rect.width = width;
		rect.height = 16;
		if (val<0) val = 0;
		EditorGUI.ProgressBar (rect, val, label);
		EditorGUILayout.Space ();
	}
	
	public void RenderStateSettings () {
		
		if (states.arraySize>0) {
			activeState.intValue = EditorGUILayout.IntSlider("Active State", activeState.intValue, 0, states.arraySize-1);
			EditorGUILayout.PropertyField(transition, new GUIContent("Transition", "The transition type between states."));
			if (transition.intValue!=0) {
				transitionTime.floatValue = EditorGUILayout.Slider("Transition Time", transitionTime.floatValue, 0f, playgroundScriptReference.maximumAllowedTransitionTime);
			}
		}
		
		EditorGUILayout.Separator();
		
		EditorGUILayout.BeginVertical(boxStyle);
		statesFoldout = GUILayout.Toggle(statesFoldout, "States ("+states.arraySize+")", EditorStyles.foldout);
		if (statesFoldout) {
			if (states.arraySize>0) {
				SerializedProperty thisState;
				SerializedProperty thisName;
				SerializedProperty thisPoints;
				SerializedProperty thisTexture;
				SerializedProperty thisMesh;
				SerializedProperty thisDepthmap;
				SerializedProperty thisDepthmapStrength;
				SerializedProperty thisTransform;
				SerializedProperty thisStateScale;
				SerializedProperty thisStateOffset;
				
				for (int i = 0; i<states.arraySize; i++) {
					thisState = states.GetArrayElementAtIndex(i);
					
					GUILayout.BeginVertical(boxStyle);
					GUILayout.BeginHorizontal(GUILayout.MinHeight(20));
					
					// State title with foldout
					if (playgroundParticlesScriptReference.activeState==i) GUILayout.BeginHorizontal(boxStyle);
					
					GUI.enabled = (playgroundParticlesScriptReference.states.Count>1);
					if (GUILayout.Button(i.ToString(), EditorStyles.toolbarButton, GUILayout.Width(20))) playgroundParticlesScriptReference.activeState=i;
					GUI.enabled = true;
					
					statesListFoldout[i] = GUILayout.Toggle(statesListFoldout[i], playgroundParticlesScriptReference.states[i].stateName, EditorStyles.foldout);

					EditorGUILayout.Separator();
					GUI.enabled = (playgroundParticlesScriptReference.states.Count>1);
					if(GUILayout.Button("U", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						int moveUp = i==0?playgroundParticlesScriptReference.states.Count-1:i-1;
						if (playgroundParticlesScriptReference.activeState==i) playgroundParticlesScriptReference.activeState = moveUp;
						playgroundParticlesScriptReference.previousActiveState = playgroundParticlesScriptReference.activeState;
						states.MoveArrayElement(i, moveUp);
						playgroundParticles.ApplyModifiedProperties();
						
						playgroundParticlesScriptReference.states[i].Initialize();
						playgroundParticlesScriptReference.states[moveUp].Initialize();
						
					}
					if(GUILayout.Button("D", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						int moveDown = i<playgroundParticlesScriptReference.states.Count-1?i+1:0;
						if (playgroundParticlesScriptReference.activeState==i) playgroundParticlesScriptReference.activeState = moveDown;
						playgroundParticlesScriptReference.previousActiveState = playgroundParticlesScriptReference.activeState;
						states.MoveArrayElement(i, moveDown);
						playgroundParticles.ApplyModifiedProperties();
						
						playgroundParticlesScriptReference.states[i].Initialize();
						playgroundParticlesScriptReference.states[moveDown].Initialize();
					}
					GUI.enabled = true;
					if(GUILayout.Button("+", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						PlaygroundC.Add(playgroundParticlesScriptReference, playgroundParticlesScriptReference.states[i].Clone());
						statesListFoldout.Add(statesListFoldout[i]);
						if (!playgroundParticlesScriptReference.states[playgroundParticlesScriptReference.states.Count-1].stateName.Contains("(Clone)"))
							playgroundParticlesScriptReference.states[playgroundParticlesScriptReference.states.Count-1].stateName = playgroundParticlesScriptReference.states[playgroundParticlesScriptReference.states.Count-1].stateName+" (Clone)";
					}
					if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						if (EditorUtility.DisplayDialog(
							"Remove "+playgroundParticlesScriptReference.states[i].stateName+"?",
							"Are you sure you want to remove the state "+playgroundParticlesScriptReference.states[i].stateName+" at list position "+i.ToString()+"?", 
							"Yes", "No")) {
								RemoveState(i);
								statesListFoldout.RemoveAt(i);
								playgroundParticles.ApplyModifiedProperties();
								return;
							}
					}
					if (playgroundParticlesScriptReference.activeState==i) GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
					
					if (statesListFoldout[i]) {
						
						if (i<states.arraySize) {
							
							EditorGUILayout.Separator();
							
							thisName = thisState.FindPropertyRelative("stateName");
							EditorGUILayout.PropertyField(thisName, new GUIContent("Name"));
							
							thisMesh = thisState.FindPropertyRelative("stateMesh");
							EditorGUILayout.PropertyField(thisMesh, new GUIContent("Mesh", "The source mesh to construct particles from vertices. When a mesh is used the texture is used to color each vertex."));
							
							thisTexture = thisState.FindPropertyRelative("stateTexture");
							EditorGUILayout.PropertyField(thisTexture, new GUIContent("Texture", "The source texture to construct particles from pixels. When a mesh is used this texture is used to color each vertex."));
							
							thisDepthmap = thisState.FindPropertyRelative("stateDepthmap");
							EditorGUILayout.PropertyField(thisDepthmap, new GUIContent("Depthmap", "The source texture to apply depthmap onto Texture's pixels. Not compatible with meshes."));
							if (thisDepthmap.objectReferenceValue!=null) {
								thisDepthmapStrength = thisState.FindPropertyRelative("stateDepthmapStrength");
								float currentDS = thisDepthmapStrength.floatValue;
								EditorGUILayout.PropertyField(thisDepthmapStrength, new GUIContent("Depthmap Strength", "How much the grayscale of the depthmap will affect Z-value."));
								if (currentDS!=thisDepthmapStrength.floatValue)
									playgroundParticlesScriptReference.states[i].Initialize();
							}
							
							thisTransform = thisState.FindPropertyRelative("stateTransform");
							EditorGUILayout.PropertyField(thisTransform, new GUIContent("Transform", "The transform to parent this state."));
							
							thisStateScale = thisState.FindPropertyRelative("stateScale");
							EditorGUILayout.PropertyField(thisStateScale, new GUIContent("Scale", "The scale of width-height."));
							
							thisStateOffset = thisState.FindPropertyRelative("stateOffset");
							EditorGUILayout.PropertyField(thisStateOffset, new GUIContent("Offset", "The offset from Particle System origin."));
							
							GUILayout.BeginHorizontal();
							EditorGUILayout.PrefixLabel("Points:");
							thisPoints = thisState.FindPropertyRelative("positionLength");
							EditorGUILayout.SelectableLabel(thisPoints.intValue.ToString(), GUILayout.MaxWidth(80));
							EditorGUILayout.Separator();
							if(GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(50))){
								ParticleStateC thisStateClass;
								thisStateClass = playgroundParticlesScriptReference.states[i];
								thisStateClass.Initialize();
							}
							if(GUILayout.Button("Set Particle Count", EditorStyles.toolbarButton)){
								PlaygroundParticlesC.SetParticleCount(playgroundParticlesScriptReference, thisPoints.intValue);
								playgroundParticlesScriptReference.Start();
							}
							if(GUILayout.Button("++", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}))
								particleCount.intValue = particleCount.intValue+thisPoints.intValue;
							GUILayout.EndHorizontal();
							
						}
					}
					GUILayout.EndVertical();
				}
			} else {
				EditorGUILayout.HelpBox("No states created.", MessageType.Info);
			}
		}
		
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.BeginVertical(boxStyle);
		createNewStateFoldout = GUILayout.Toggle(createNewStateFoldout, "Create State", EditorStyles.foldout);
		if (createNewStateFoldout) {
			EditorGUILayout.Separator();
			meshOrImage = GUILayout.Toolbar (meshOrImage, new string[]{"Image","Mesh"}, EditorStyles.toolbarButton);
			EditorGUILayout.Separator();
			// Add image or mesh
			if (meshOrImage==1)
				addStateMesh = EditorGUILayout.ObjectField("Mesh", addStateMesh, typeof(Mesh), true);
			addStateTexture = EditorGUILayout.ObjectField("Texture", addStateTexture, typeof(Texture2D), true);
			if (meshOrImage==0) {
				addStateDepthmap = EditorGUILayout.ObjectField("Depthmap", addStateDepthmap, typeof(Texture2D), true);
			if (addStateDepthmap!=null)
				addStateDepthmapStrength = EditorGUILayout.FloatField("Depthmap Strength", addStateDepthmapStrength);
			}
			addStateTransform = EditorGUILayout.ObjectField("Transform", addStateTransform, typeof(Transform), true);
			addStateName = EditorGUILayout.TextField("Name", addStateName);
			addStateScale = EditorGUILayout.FloatField("Scale", addStateScale);
			addStateOffset = EditorGUILayout.Vector3Field("Offset", addStateOffset);
			
			EditorGUILayout.Separator();
			
			if (meshOrImage==0)
				GUI.enabled = (addStateTexture!=null);
			else
				GUI.enabled = (addStateMesh!=null);
			
			if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
				
				// Check read/write
				if (addStateTexture!=null) {
					TextureImporter tAssetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(addStateTexture as UnityEngine.Object)) as TextureImporter;
					
					// If no Import Settings are found
					if (!tAssetImporter) {
						Debug.Log("Could not read the Import Settings of the selected texture.");
						return; 
					}
					
					// If the texture isn't readable
					if (!tAssetImporter.isReadable) {
						Debug.Log(tAssetImporter.assetPath+" is not readable. Please change Read/Write Enabled on its Import Settings.");
						return; 
					}
				}
				if (addStateMesh!=null) {
					ModelImporter mAssetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(addStateMesh as UnityEngine.Object)) as ModelImporter;
					if (mAssetImporter==null) {
						Debug.Log("Could not read the Import Settings of the selected mesh.");
						return; 
					}
					if (!mAssetImporter.isReadable) {
						Debug.Log(mAssetImporter.assetPath+" is not readable. Please change Read/Write Enabled on its Import Settings.");
						return; 
					}
				}
				
				if (addStateName=="" || addStateName==null) addStateName = "State "+(states.arraySize).ToString();
				if (meshOrImage==0) {
					if (addStateDepthmap==null)
						PlaygroundC.Add(playgroundParticlesScriptReference, addStateTexture as Texture2D, addStateScale, addStateOffset, addStateName, addStateTransform as Transform);
					else
						PlaygroundC.Add(playgroundParticlesScriptReference, addStateTexture as Texture2D, addStateDepthmap as Texture2D, addStateDepthmapStrength, addStateScale, addStateOffset, addStateName, addStateTransform as Transform);
				} else {
					if (addStateTexture==null)
						PlaygroundC.Add(playgroundParticlesScriptReference, addStateMesh as Mesh, addStateScale, addStateOffset, addStateName, addStateTransform as Transform);
					else
						PlaygroundC.Add(playgroundParticlesScriptReference, addStateMesh as Mesh, addStateTexture as Texture2D, addStateScale, addStateOffset, addStateName, addStateTransform as Transform);
				}
				playgroundParticlesScriptReference.Start();
				
				statesFoldout = true;
				statesListFoldout.Add(true);
				
				addStateName = "";
				addStateMesh = null;
				addStateTexture = null;
				addStateTransform = null;
				addStateDepthmap = null;
				addStateDepthmapStrength = 1f;
				addStateScale = 1f;
				addStateOffset = Vector3.zero;
			}
			GUI.enabled = true;
		}
		EditorGUILayout.EndVertical();
	}
	
	public void RenderProjectionSettings () {
		
		if (playgroundParticlesScriptReference.projection==null) {
			playgroundParticlesScriptReference.projection = new ParticleProjectionC();
			playgroundParticlesScriptReference.projection.projectionTransform = playgroundParticlesScriptReference.particleSystemTransform;
		}
		
		// Projection texture
		Texture2D prevTexture = playgroundParticlesScriptReference.projection.projectionTexture;
		playgroundParticlesScriptReference.projection.projectionTexture = EditorGUILayout.ObjectField("Projection Texture", playgroundParticlesScriptReference.projection.projectionTexture, typeof(Texture2D), true) as Texture2D;
		
		// Texture changed
		if (prevTexture!=playgroundParticlesScriptReference.projection.projectionTexture) {
			TextureImporter tAssetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(playgroundParticlesScriptReference.projection.projectionTexture as UnityEngine.Object)) as TextureImporter;
			
			// If no Import Settings are found
			if (!tAssetImporter) {
				Debug.Log("Could not read the Import Settings of the selected texture.");
				playgroundParticlesScriptReference.projection.projectionTexture = null;
				return; 
			}
			
			// If the texture isn't readable
			if (!tAssetImporter.isReadable) {
				Debug.Log(tAssetImporter.assetPath+" is not readable. Please change Read/Write Enabled on its Import Settings.");
				playgroundParticlesScriptReference.projection.projectionTexture = null;
				return; 
			}
			
			playgroundParticlesScriptReference.projection.Construct(playgroundParticlesScriptReference.projection.projectionTexture, playgroundParticlesScriptReference.projection.projectionTransform);
		}
		
		playgroundParticlesScriptReference.projection.projectionTransform = EditorGUILayout.ObjectField("Transform", playgroundParticlesScriptReference.projection.projectionTransform, typeof(Transform), true) as Transform;
		playgroundParticlesScriptReference.projection.liveUpdate = EditorGUILayout.Toggle("Live Update", playgroundParticlesScriptReference.projection.liveUpdate);
		playgroundParticlesScriptReference.projection.projectionOrigin = EditorGUILayout.Vector2Field("Origin Offset", playgroundParticlesScriptReference.projection.projectionOrigin);
		playgroundParticlesScriptReference.projection.projectionDistance = EditorGUILayout.FloatField("Projection Distance", playgroundParticlesScriptReference.projection.projectionDistance);
		playgroundParticlesScriptReference.projection.projectionScale = EditorGUILayout.FloatField("Projection Scale", playgroundParticlesScriptReference.projection.projectionScale);
		playgroundParticlesScriptReference.projection.surfaceOffset = EditorGUILayout.FloatField("Surface Offset", playgroundParticlesScriptReference.projection.surfaceOffset);
		EditorGUILayout.PropertyField(projectionMask, new GUIContent("Projection Mask"));
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Points:");
		EditorGUILayout.SelectableLabel(playgroundParticlesScriptReference.projection.positionLength.ToString(), GUILayout.MaxWidth(80));
		EditorGUILayout.Separator();
		if(GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(50))){
			playgroundParticlesScriptReference.projection.Initialize();
		}
		if(GUILayout.Button("Set Particle Count", EditorStyles.toolbarButton)){
			PlaygroundParticlesC.SetParticleCount(playgroundParticlesScriptReference, playgroundParticlesScriptReference.projection.positionLength);
			playgroundParticlesScriptReference.Start();
		}
		if(GUILayout.Button("++", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}))
			particleCount.intValue = particleCount.intValue+playgroundParticlesScriptReference.projection.positionLength;
		GUILayout.EndHorizontal();
	}
	
	public static int selectedSort;
	public static int selectedOrigin;
	
	public void LifetimeSorting () {
		playgroundParticlesScriptReference.Start();
	}
	
	public void LifetimeSortingAll () {
		foreach (PlaygroundParticlesC p in PlaygroundC.reference.particleSystems)
			p.Start();
	}
	
	public void RemoveState (int i) {
		playgroundParticlesScriptReference.RemoveState(i);
	}
	
	
	public void StartStopPaint () {
		inPaintMode = !inPaintMode;
		playgroundParticlesScriptReference.Start();
		if (inPaintMode) {
			if (!playgroundParticlesScriptReference.paint.initialized) {
				PlaygroundC.PaintObject(playgroundParticlesScriptReference);
			}
			
			if (selectedPaintMode==1)
				SetBrush(selectedBrushPreset);
			
			Tools.current = Tool.None;
		} else {
			Tools.current = lastActiveTool;
		}
	}
	
	public void ClearPaint () {
		if (EditorUtility.DisplayDialog(
			"Clear Paint?",
			"Are you sure you want to remove all painted source positions?", 
			"Yes", "No")) {
				PlaygroundC.ClearPaint(playgroundParticlesScriptReference);
				PlaygroundParticlesC.SetParticleCount(playgroundParticlesScriptReference, playgroundParticlesScriptReference.particleCount);
			}
	}
	
	public void DrawCollisionPlane (PlaygroundColliderC pc) {
		float scale = playgroundScriptReference.collisionPlaneScale;
		if (scale<=0) return;
		Vector3 p1;
		Vector3 p2;
		Handles.color = pc.enabled?new Color(0f,.8f,.1f,.25f):new Color(0f,.8f,.1f,.05f);
		for (int x = 0; x<11; x++) {
			p1 = pc.transform.TransformPoint(new Vector3((x*10f)-50f, 0f, 50f)*scale);
			p2 = pc.transform.TransformPoint(new Vector3((x*10f)-50f, 0f, -50f)*scale);
			Handles.DrawLine(p1, p2);
		}
		for (int y = 0; y<11; y++) {
			p1 = pc.transform.TransformPoint(new Vector3(50f, 0f, (y*10f)-50f)*scale);
			p2 = pc.transform.TransformPoint(new Vector3(-50f, 0f, (y*10f)-50f)*scale);
			Handles.DrawLine(p1, p2);
		}
	}
	
	private bool keyPressed = false;
	private int foldoutHeight = 0;
	void OnSceneGUI () {
		
		// Collision Planes
		if (playgroundScriptReference.drawGizmos && collisionFoldout && playgroundParticlesScriptReference.collision && playgroundParticlesScriptReference.colliders.Count>0) {
			for (int c = 0; c<playgroundParticlesScriptReference.colliders.Count; c++) {
				
				if (playgroundParticlesScriptReference.colliders[c].transform==null) continue;
				
				DrawCollisionPlane(playgroundParticlesScriptReference.colliders[c]);
				
				if (playgroundParticlesScriptReference.colliders[c].enabled) {
					// Position
					if (Tools.current==Tool.Move)
						playgroundParticlesScriptReference.colliders[c].transform.position = Handles.PositionHandle(playgroundParticlesScriptReference.colliders[c].transform.position, Tools.pivotRotation==PivotRotation.Global? Quaternion.identity : playgroundParticlesScriptReference.colliders[c].transform.rotation);
					// Rotation
					else if (Tools.current==Tool.Rotate)
						playgroundParticlesScriptReference.colliders[c].transform.rotation = Handles.RotationHandle(playgroundParticlesScriptReference.colliders[c].transform.rotation, playgroundParticlesScriptReference.colliders[c].transform.position);
					// Scale
					else if (Tools.current==Tool.Scale)
						playgroundParticlesScriptReference.colliders[c].transform.localScale = Handles.ScaleHandle(playgroundParticlesScriptReference.colliders[c].transform.localScale, playgroundParticlesScriptReference.colliders[c].transform.position, playgroundParticlesScriptReference.colliders[c].transform.rotation, HandleUtility.GetHandleSize(playgroundParticlesScriptReference.colliders[c].transform.position));
				}
			}
		}
		
		// Projection mode
		if (playgroundParticlesScriptReference.source == SOURCEC.Projection) {
			
			// Projector preview
			if (playgroundScriptReference.drawGizmos && playgroundParticlesScriptReference.projection!=null && playgroundParticlesScriptReference.projection.projectionTexture!=null && playgroundParticlesScriptReference.projection.projectionTransform!=null) {
				//Handles.Label(playgroundParticlesScriptReference.projection.projectionTransform.position, GUIContent(playgroundParticlesScriptReference.projection.projectionTexture));
				RaycastHit projectorHit;
				Vector3 p2 = playgroundParticlesScriptReference.projection.projectionTransform.position+(playgroundParticlesScriptReference.projection.projectionTransform.forward*playgroundParticlesScriptReference.projection.projectionDistance);
				bool projectorHasSurface = false;
				if (Physics.Raycast(playgroundParticlesScriptReference.projection.projectionTransform.position, playgroundParticlesScriptReference.projection.projectionTransform.forward, out projectorHit, playgroundParticlesScriptReference.projection.projectionDistance, playgroundParticlesScriptReference.projection.projectionMask)) {
					p2 = projectorHit.point;
					projectorHasSurface = true;
				}
				Handles.color = projectorHasSurface?new Color(1f,1f,.25f,.6f):new Color(1f,1f,.25f,.2f);
				Handles.DrawLine(playgroundParticlesScriptReference.projection.projectionTransform.position, p2);
			}
		}
		
		// Paint mode
		if (playgroundParticlesScriptReference.source == SOURCEC.Paint) {
			Event e = Event.current;
			if (e.type == EventType.Layout) {
				HandleUtility.AddDefaultControl(0);
			}
			
			// Paint Toolbox in Scene View
			Rect toolboxRect = new Rect(10f,Screen.height-(138f+foldoutHeight),300f,103f+foldoutHeight);
			if (PlaygroundC.reference.paintToolbox) {
				if (!paintToolboxSettingsFoldout) {
					foldoutHeight = 0;
				} else {
					switch (selectedPaintMode) {
						case 0: foldoutHeight = 54; break;
						case 1: foldoutHeight = 144; break;
						case 2: foldoutHeight = 36; break;
					}
				}
				if (!toolboxFoldout) foldoutHeight=-69;
				Handles.BeginGUI();
					GUILayout.BeginArea(toolboxRect);
					if (boxStyle==null)
						boxStyle = GUI.skin.FindStyle("box");
					GUILayout.BeginVertical(boxStyle);
					toolboxFoldout = GUILayout.Toggle(toolboxFoldout, "Playground Paint", EditorStyles.foldout);
					if (toolboxFoldout) {
						selectedPaintMode = GUILayout.Toolbar (selectedPaintMode, new string[]{"Dot","Brush","Eraser"}, EditorStyles.toolbarButton);
						
						// Settings
						GUILayout.BeginVertical(boxStyle);
						paintToolboxSettingsFoldout = GUILayout.Toggle(paintToolboxSettingsFoldout, "Settings", EditorStyles.foldout);
						if (paintToolboxSettingsFoldout) {
							switch (selectedPaintMode) {
								case 0:
									paintColor = EditorGUILayout.ColorField("Color", paintColor);
									playgroundParticlesScriptReference.paint.spacing = EditorGUILayout.Slider("Paint Spacing", playgroundParticlesScriptReference.paint.spacing, .0f, playgroundScriptReference.maximumAllowedPaintSpacing);
									EditorGUILayout.PropertyField(paintLayerMask, new GUIContent("Paint Mask"));
								break;
								case 1:
									GUILayout.BeginHorizontal();
									EditorGUILayout.PrefixLabel("Brush Shape");
									paintTexture = EditorGUILayout.ObjectField(paintTexture, typeof(Texture2D), false) as Texture2D;
									GUILayout.EndHorizontal();
									if (paintTexture!=null && paintTexture!=playgroundParticlesScriptReference.paint.brush.texture) {
										selectedBrushPreset = -1;
										SetBrush(selectedBrushPreset);
									}
									playgroundParticlesScriptReference.paint.brush.detail = (BRUSHDETAILC)EditorGUILayout.EnumPopup("Detail", playgroundParticlesScriptReference.paint.brush.detail);
									playgroundParticlesScriptReference.paint.brush.scale = EditorGUILayout.Slider("Scale", playgroundParticlesScriptReference.paint.brush.scale, playgroundScriptReference.minimumAllowedBrushScale, playgroundScriptReference.maximumAllowedBrushScale);
									playgroundParticlesScriptReference.paint.brush.distance = EditorGUILayout.FloatField("Distance", playgroundParticlesScriptReference.paint.brush.distance);
									useBrushColor = EditorGUILayout.Toggle("Use Brush Color", useBrushColor);
									GUI.enabled = !useBrushColor;
									paintColor = EditorGUILayout.ColorField("Color", paintColor);
									GUI.enabled = true;
									playgroundParticlesScriptReference.paint.spacing = EditorGUILayout.Slider("Paint Spacing", playgroundParticlesScriptReference.paint.spacing, .0f, playgroundScriptReference.maximumAllowedPaintSpacing);
									EditorGUILayout.PropertyField(paintLayerMask, new GUIContent("Paint Mask"));
								break;
								case 2:
									eraserRadius = EditorGUILayout.Slider("Radius", eraserRadius, playgroundScriptReference.minimumEraserRadius, playgroundScriptReference.maximumEraserRadius);
									EditorGUILayout.PropertyField(paintLayerMask, new GUIContent("Paint Mask"));
								break;
							}
						}
						GUILayout.EndVertical();
						GUILayout.BeginHorizontal();
						GUI.enabled = !(selectedPaintMode==1 && paintTexture==null);
						if(GUILayout.Button((inPaintMode?"Stop":"Start")+" Paint", EditorStyles.toolbarButton))
			 				StartStopPaint();
			 			GUI.enabled = (playgroundParticlesScriptReference.paint.positionLength>0);
			 			if(GUILayout.Button("Clear", EditorStyles.toolbarButton))
			 				ClearPaint();
			 			GUI.enabled = true;
			 			ProgressBar((playgroundParticlesScriptReference.paint.positionLength*1f)/PlaygroundC.reference.paintMaxPositions, playgroundParticlesScriptReference.paint.positionLength+"/"+PlaygroundC.reference.paintMaxPositions, 115f);
			 			GUILayout.EndHorizontal();
		 			}
		 			GUILayout.EndVertical();
		 			GUILayout.EndArea();
		 		Handles.EndGUI();
	 		}
	 		
			if (inPaintMode) {
				if (e.type == EventType.Layout) {
					HandleUtility.AddDefaultControl(0);
				}
				
				Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);

				// Brush preview
				if (selectedPaintMode==1 && playgroundParticlesScriptReference.paint.brush.texture!=null && sceneBrushStyle!=null && !toolboxRect.Contains(e.mousePosition)) {
					Handles.Label(mouseRay.origin, new GUIContent(playgroundParticlesScriptReference.paint.brush.texture as Texture2D), sceneBrushStyle);
				}
					
				// Eraser preview
				RaycastHit eraserHit = new RaycastHit();
				if (selectedPaintMode==2 && !toolboxRect.Contains(e.mousePosition)) {
					if (Physics.Raycast(mouseRay, out eraserHit, 10000f, playgroundParticlesScriptReference.paint.layerMask)) {
						Handles.color = new Color(0f,0f,0f,.4f);
						Handles.CircleCap(-1, eraserHit.point, Quaternion.LookRotation(mouseRay.direction), eraserRadius);
					}
				}
				
				// Spacing preview
				if (selectedPaintMode!=2) {
					Handles.color = new Color(.3f,1f,.3f,.3f);
					Handles.CircleCap(-1, playgroundParticlesScriptReference.paint.lastPaintPosition, Quaternion.LookRotation(Camera.current.transform.forward), playgroundParticlesScriptReference.paint.spacing);
				}
				
				if (e.type  == EventType.KeyDown)
					keyPressed = true;
				else if (e.type == EventType.KeyUp)
					keyPressed = false;
				
				// Paint from the Brush's texture into the Scene View
				if (!keyPressed && e.button == 0 && e.isMouse && !e.alt) {
					if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown) {
						
						switch (selectedPaintMode) {
							// Dot
							case 0:
								if (playgroundParticlesScriptReference.paint.exceedMaxStopsPaint && playgroundParticlesScriptReference.paint.positionLength>=PlaygroundC.reference.paintMaxPositions) return;
								RaycastHit dotHit;
								if (Physics.Raycast(mouseRay, out dotHit, 10000f, playgroundParticlesScriptReference.paint.layerMask)) {
									if (e.type != EventType.MouseDown)
										if (Vector3.Distance(dotHit.point, playgroundParticlesScriptReference.paint.lastPaintPosition)<=playgroundParticlesScriptReference.paint.spacing) return;
									PlaygroundC.Paint(playgroundParticlesScriptReference, dotHit.point, dotHit.normal, dotHit.transform, paintColor);
									playgroundParticlesScriptReference.paint.lastPaintPosition = dotHit.point;
								}
							break;
							// Brush
							case 1:
								if (playgroundParticlesScriptReference.paint.exceedMaxStopsPaint && playgroundParticlesScriptReference.paint.positionLength>=PlaygroundC.reference.paintMaxPositions || !playgroundParticlesScriptReference.paint.brush.texture) return;
								if (e.type != EventType.MouseDown) {
								RaycastHit brushHit;
									if (Physics.Raycast(mouseRay, out brushHit, 10000, playgroundParticlesScriptReference.paint.layerMask))
										if (Vector3.Distance(brushHit.point, playgroundParticlesScriptReference.paint.lastPaintPosition)<=playgroundParticlesScriptReference.paint.spacing) return;
								}
								int detail = 0;
								switch (playgroundParticlesScriptReference.paint.brush.detail) {
									case BRUSHDETAILC.Perfect: detail=0; break;
									case BRUSHDETAILC.High: detail=2; break;
									case BRUSHDETAILC.Medium: detail=4; break;
									case BRUSHDETAILC.Low: detail=6; break;
								}
								Color32 pixelColor;
								for (int x = 0; x<playgroundParticlesScriptReference.paint.brush.texture.width; x++) {
									for (int y = 0; y<playgroundParticlesScriptReference.paint.brush.texture.height; y++) {
										if (detail==0 || ((x+1)*(y+1)-1)%detail==0) {
											pixelColor = playgroundParticlesScriptReference.paint.brush.GetColor((x+1)*(y+1)-1);
											if (!useBrushColor) pixelColor = new Color(paintColor.r, paintColor.g, paintColor.b, pixelColor.a);
											if (pixelColor.a!=0) {
												mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition+new Vector2((-playgroundParticlesScriptReference.paint.brush.texture.width/2f)+x,(-playgroundParticlesScriptReference.paint.brush.texture.height/2)+y)*playgroundParticlesScriptReference.paint.brush.scale);
												playgroundParticlesScriptReference.paint.Paint(mouseRay, pixelColor);
											}
										}
									}
								}
							break;
							// Eraser
							case 2:
								if (eraserHit.collider!=null) {
									playgroundParticlesScriptReference.paint.Erase(eraserHit.point, eraserRadius);
								}
							break;
						}
						
					}
					SceneView.RepaintAll();
				}
				
				
				if (e.type == EventType.MouseUp) {
					playgroundParticlesScriptReference.paint.lastPaintPosition = PlaygroundC.initialTargetPosition;
					
					// No positions to emit from, reset particle system by rebuilding
					if (eraserHit.collider!=null && playgroundParticlesScriptReference.paint.positionLength==0) {
						PlaygroundC.SetParticleCount(playgroundParticlesScriptReference, playgroundParticlesScriptReference.particleCount);
					}
				}
			}
		}
		
		// Render global manipulators
		int i = 0;
		if (playgroundScriptReference!=null && playgroundScriptReference.drawGizmos && PlaygroundInspectorC.manipulatorsFoldout)
			for (; i<playgroundScriptReference.manipulators.Count; i++)
				PlaygroundInspectorC.RenderManipulatorInScene(playgroundScriptReference.manipulators[i], playgroundScriptReference.manipulators[i].inverseBounds?new Color(1f,.6f,.4f,1f):new Color(.4f,.6f,1f,1f));
		// Render local manipulators
		if (playgroundScriptReference.drawGizmos && manipulatorsFoldout)
			for (i = 0; i<playgroundParticlesScriptReference.manipulators.Count; i++)
				PlaygroundInspectorC.RenderManipulatorInScene(playgroundParticlesScriptReference.manipulators[i], playgroundParticlesScriptReference.manipulators[i].inverseBounds?new Color(1f,1f,.4f,1f):new Color(.4f,1f,1f,1f));
		
		if (GUI.changed)
            EditorUtility.SetDirty (target);
	}
	
}