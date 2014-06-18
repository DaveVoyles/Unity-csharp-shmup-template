#pragma strict

import System.Collections.Generic;


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Particle Playground Example - Features (How to create and alter a Particle Playground Object by script)
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Sources to apply to our PlaygroundParticles Object
var worldObjects : Transform[];								// The world objects to use as source
var skinnedWorldObjectTransform : Transform;				// The skinned world object to use as source
var skinnedWorldObjectGameObject : GameObject;				// The skinned world object to activate (through GUI)
var skinnedWorldObjectVisibleMaterial : Material;			// The visible material of the robot
var skinnedWorldObjectInvisibleMaterial : Material;			// the invisible material of the robot
var stateImages : Texture2D[];								// The images to use as source
var stateDepthmap : Texture2D;								// The depth map to apply to our source

// Color and rendering
var lifetimeColors : GradientWrapper[];						// The colors to assign to the PlaygroundParticles Object (we do it with GradientWrapper because of the insufficient built-in gradient array support)
var materials : Material[];									// The materials to assign to the PlaygroundParticles Object (Playground.SetMaterial(PlaygroundParticles, Material))
var lifetimeSizes : AnimationCurveWrapper[];				// The lifetime sizes to assign to the PlaygroundParticles Object (we do it with AnimationCurveWrapper because of the insufficient built-in AnimationCurve array support)
var lifetimeVelocities : Vector3AnimationCurveWrapper[];	// The lifetime velocities to assign to the PlaygroundParticles Object

// Cached components
private var particles : PlaygroundParticles;				// The cached script of our Playground Particle System (also exposed through Playground.GetParticles(int))
private var cam : Camera;									// Main Camera
private var camTransform : Transform;						// Main Camera Transform
private var thisTransform : Transform;						// Camera Pivot Transform
private var robotAnimator : Animator;						// Animator for Robot Kyle

// Manipulator
var manipulatorTransform : Transform;
var manipulatorType : MANIPULATORTYPE;
var manipulatorMask : LayerMask;
var manipulatorSize : float;
var manipulatorStrength: float;
private var manipulator : ManipulatorObject;
private var manipulatorRenderer : Renderer;
private var manipulatorCollider : Collider;
private var manipulatorGo : GameObject;
private var manipulatorRb : Rigidbody;

// Other
var worldObjectCreationPosition : Transform;				// Position for World Objects
var camTarget : Transform;									// Target for our camera
var clampRect : Rect;										// World boundaries
var interactionLayer : LayerMask;							// Interaction layer mask
var planeCollider : Transform;								// The plane collider transform to be added (the particles can't ever fall through this)
var particleCollisionLayer : LayerMask;						// Collision layer mask for particles
var robotLayer : LayerMask;									// Robot layer mask

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// MonoBehaviours
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function Awake () {
	
	// Cache components
	cam = Camera.main;
	camTransform = cam.transform;
	thisTransform = transform;
	robotAnimator = skinnedWorldObjectGameObject.GetComponent(Animator);
	manipulatorGuiSizeRenderer = manipulatorGuiSizeTransform.renderer;
	manipulatorRenderer = manipulatorTransform.renderer;
	manipulatorGo = manipulatorTransform.gameObject;
	manipulatorRb = manipulatorTransform.rigidbody;
	manipulatorCollider = manipulatorTransform.collider;
	manipulatorCollider.enabled = false;
	
	// Fill an array of lifetime color images (for the GUI)
	lifetimeColorImages = new Texture2D[lifetimeColors.Length];
	for (x=0; x<lifetimeColorImages.Length; x++)
		lifetimeColorImages[x] = lifetimeColors[x].image;
	
	// Fill an array of lifetime size images (for the GUI)
	lifetimeSizeImages = new Texture2D[lifetimeSizes.Length];
	for (x=0; x<lifetimeSizeImages.Length; x++)
		lifetimeSizeImages[x] = lifetimeSizes[x].image;
		
	// Fill an array of lifetime velocities (for the GUI)
	lifetimeVelocityImages = new Texture2D[lifetimeVelocities.Length];
	for (x=0; x<lifetimeVelocityImages.Length; x++)
		lifetimeVelocityImages[x] = lifetimeVelocities[x].image;
}

function Start () {
	
	// Create a new PlaygroundParticles object and assign it to our variable 'particles'.
	// A Playground Manager will instantiate automatically to drive the system(s).
	particles = Playground.Particle();
	
	// Set particle material
	Playground.SetMaterial(particles, materials[0]);
	
	// Assign the first object in 'worldObjects' as a World Object
	SetWorldObject(worldObjects[0]);
	
	// Assign the skinned mesh (Robot Kyle) as our skinned world object
	Playground.SkinnedWorldObject(particles, skinnedWorldObjectTransform);
	
	// Set our particle source
	particles.source = SOURCE.WorldObject;
	
	// Assign the selected source (to get correct amount of particles on initiation)
	SetSource(selectedSource);
	
	// Create all states with Polyfied logo and Unity logo (four states as we want to apply depthmap to the later two)
	// Playground.Add(Particle System as PlaygroundParticles, Image as Texture2D, Scale as float, Offset as Vector3, Name as string)
	for (x = 0; x<4; x++)
		Playground.Add(particles, stateImages[x%2], .1, Vector3(-8, 0, 0), "state "+x.ToString());
	
	// Set depthmaps for state 2 and 3 (state 0 and 1 will be flat)
	particles.states[2].stateDepthmap = stateDepthmap;
	particles.states[3].stateDepthmap = stateDepthmap;
	
	// Apply new settings by initializing the states
	particles.states[2].Initialize();
	particles.states[3].Initialize();
	
	// Set state transition type
	particles.transition = TRANSITION.Lerp;
	
	// Set minimum- and maximum size
	particles.sizeMin = .01;
	particles.sizeMax = .1;
	
	// Add plane collider
	Playground.AddCollider(particles, planeCollider);
	
	// Set collision mask
	particles.collisionMask = particleCollisionLayer;
	
	// Set particle system childed to camera (Shuriken Particle System may not render when out of frustrum)
	particles.particleSystemTransform.parent = camTransform;
	
	// Add our manipulator to Playground and create a reference to 'manipulator'. The overload Playground.ManipulatorObject(Transform) is also available.
	manipulator = Playground.ManipulatorObject(manipulatorType, manipulatorMask, manipulatorTransform, manipulatorSize, manipulatorStrength);
	
	// Disable the manipulator at first
	manipulator.enabled = false;
	
	// Set velocities of our particle system
	// The initial velocities are set by minimum and maximum values to create velocity ranges (new since version 1.1)
	// Velocity Bending will curve the current velocity path, which can create interesting movement patterns
	particles.applyInitialVelocity = true;
	particles.applyInitialLocalVelocity = true;
	particles.applyVelocityBending = true;
	particles.initialVelocityMin = Vector3.zero;
	particles.initialVelocityMax = Vector3.zero;
	particles.initialLocalVelocityMin = Vector3.zero;
	particles.initialLocalVelocityMax = Vector3(1,1,1);
	particles.velocityBending = Vector3.zero;
}

// GUI variables
var smallLabel : GUIStyle;
var smallButton : GUIStyle;
var worldObjectImages : Texture2D[];
var stateGuiImages : Texture2D[];
var manipulatorGuiSizeTransform : Transform;
private var manipulatorGuiSizeRenderer : Renderer;
private var showGui : boolean = true;
private var scrollPosition : Vector2;
private var selectedCategory : int;
private var selectedSource : int;
private var selectedSourcePrevious : int;
private var selectedWorldObject : int;
private var selectedWorldObjectPrevious : int;
private var selectedTransition : int;
private var selectedManipulatorType : int;
private var selectedManipulatorTypePrevious : int;
private var showSkinned : boolean = true;
private var showSkinnedPrevious : boolean = true;
private var overflowOffset : Vector3;
private var lifetimeColorImages : Texture2D[];
private var lifetimeSizeImages : Texture2D[];
private var lifetimeVelocityImages : Texture2D[];
private var horizontalColorPos : float;
private var selectedLifetimeSize : float;
private var selectedLifetimeColor : int; 
private var selectedLifetimeVelocity : int;
private var selectedMaterial : int;
private var selectedLifetimeSorting : int;
private var points : int;
private var x : int;
private var guiWidth : float = 479;

function OnGUI () {
	
	if (showGui) {
		GUILayout.BeginVertical("box", GUILayout.Width(guiWidth));
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		
		GUILayout.BeginHorizontal(); GUILayout.Label("Particle Playground"); if (GUILayout.Button("Reset")) Application.LoadLevel(Application.loadedLevel); if (GUILayout.Button("Minimize")) showGui=false; GUILayout.EndHorizontal();
		
		selectedCategory = GUILayout.Toolbar(selectedCategory, ["Particles", "Manipulator", "About"]);
		
		///////////////////////
		// About
		///////////////////////
		
		if (selectedCategory==2) {
			GUILayout.TextArea(
			"Particle Playground v"+Playground.version+"\nOpen up new possibilities to your particles and control them like never before. \nParticle Playground is a toolset which enables new ways of creating, altering and rendering particles.\n\nParticle Playground is developed by Polyfied, Stockholm 2014."
			);
			GUILayout.BeginHorizontal();
   			GUILayout.FlexibleSpace();
   			GUILayout.Label(stateGuiImages[0], GUILayout.Width(16)); GUILayout.Label("polyfied.com");
   			GUILayout.FlexibleSpace();
   			GUILayout.EndHorizontal();
			
		}
		
		///////////////////////
		// Manipulators
		///////////////////////
		
		else if (selectedCategory==1) {
		
		GUILayout.BeginVertical("box");
		GUILayout.Label("Manipulator Settings", smallLabel);
		
		// Enabling the manipulator
		if (GUI.changed)
			manipulator.enabled = true;
		manipulator.enabled = GUILayout.Toggle(manipulator.enabled, "Manipulator Enabled");
		if (GUI.changed)
			SetManipulatorType(selectedManipulatorType);
			
		if (manipulator.enabled) {
			GUILayout.Label("Type");
			selectedManipulatorTypePrevious = selectedManipulatorType;
			selectedManipulatorType = GUILayout.Toolbar(selectedManipulatorType, ["Attractor", "Gravitational", "Repellent"]);
			GUILayout.BeginHorizontal(); GUILayout.Label("Size", GUILayout.Width(80)); manipulator.size = GUILayout.HorizontalSlider(manipulator.size, 1, 30); GUILayout.Label(manipulator.size.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("Strength", GUILayout.Width(80)); manipulator.strength = GUILayout.HorizontalSlider(manipulator.strength, 0, 100); GUILayout.Label(manipulator.strength.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			if (GUI.changed) {
				manipulatorGuiSizeTransform.localScale = Vector3(manipulator.size, manipulator.size, manipulator.size)*2;
				manipulatorGuiSizeRenderer.sharedMaterial.SetColor("_TintColor", new Color(.5,.5,.5,manipulator.strength/100));
			}
			if (selectedManipulatorTypePrevious!=selectedManipulatorType) {
				// Set manipulator type
				SetManipulatorType(selectedManipulatorType);
			}
		}
		
		GUILayout.EndVertical();
		
		} else if (selectedCategory==0) {
		
		///////////////////////
		// Source
		///////////////////////
		
		GUILayout.BeginVertical("box");
		GUILayout.Label("Source Settings", smallLabel);
		
		// Set source
		GUILayout.Label("Source");
		selectedSourcePrevious = selectedSource;
		selectedSource = GUILayout.Toolbar(selectedSource, ["World Object", "Skinned World Object", "State"]);
		if (selectedSourcePrevious!=selectedSource)
			SetSource(selectedSource);
			
		switch (selectedSource) {
			case 0:
				selectedWorldObjectPrevious = selectedWorldObject;
				selectedWorldObject = GUILayout.Toolbar(selectedWorldObject, worldObjectImages);
				if (selectedWorldObjectPrevious!=selectedWorldObject)
					SetWorldObject(worldObjects[selectedWorldObject]);
				particles.worldObject.renderer.enabled = GUILayout.Toggle(particles.worldObject.renderer.enabled, "Render Mesh");
			break;
			case 1:
				showSkinnedPrevious = showSkinned;
				showSkinned = GUILayout.Toggle(showSkinned, "Render Mesh");
				if (showSkinnedPrevious!=showSkinned)
					particles.skinnedWorldObject.renderer.material = showSkinned?skinnedWorldObjectVisibleMaterial:skinnedWorldObjectInvisibleMaterial;
			break;
			case 2:
				GUILayout.Label("States");
				particles.activeState = GUILayout.Toolbar(particles.activeState, stateGuiImages);
				
				if (particles.activeState>1) {
					GUILayout.Label("Depth Map Strength");
					GUILayout.BeginHorizontal(); particles.states[particles.activeState].stateDepthmapStrength = GUILayout.HorizontalSlider(particles.states[particles.activeState].stateDepthmapStrength, -20, 20); GUILayout.Label(particles.states[particles.activeState].stateDepthmapStrength.ToString("f2"), GUILayout.Width(60));
					if  (GUI.changed) {
						particles.states[particles.activeState].Initialize();
						particles.previousActiveState = -1;
					}
					GUILayout.EndHorizontal();
				}
					
				GUILayout.Label("Transition");
				selectedTransition = GUILayout.Toolbar(selectedTransition, ["None", "Lerp", "Fade"]);
				if (GUI.changed)
					SetTransition(selectedTransition);
				
				GUILayout.Label("Transition Time");
				GUILayout.BeginHorizontal(); particles.transitionTime = GUILayout.HorizontalSlider(particles.transitionTime, .1, 10); GUILayout.Label(particles.transitionTime.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			break;
		}
		
		// Get source points
		if (GUI.changed)
			switch (particles.source) {
				case SOURCE.WorldObject: 		points = particles.worldObject.vertexPositions.Length; 				break;
				case SOURCE.SkinnedWorldObject:	points = particles.skinnedWorldObject.vertexPositions.Length; 		break;
				case SOURCE.State: 				points = particles.states[particles.activeState].positionLength;	break;
			}
		GUILayout.BeginHorizontal(); 
			GUILayout.Label("Points: "+points, GUILayout.Width(120)); 
			if (GUILayout.Button("Set Count", smallButton)) 
				Playground.SetParticleCount(particles, points); 
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
		
		
		///////////////////////////
		// Count, Offset & Size
		///////////////////////////
		
		GUILayout.BeginVertical("box");
		GUILayout.Label("Particle Settings", smallLabel);
		
		// Particle count
		GUILayout.Label("Particle Count ("+particles.particleCount.ToString()+")");
		particles.particleCount = GUILayout.HorizontalSlider(particles.particleCount, 0, 50000);
		
		// Overflow offset
		GUILayout.Label("Overflow Offset");
		overflowOffset = particles.overflowOffset;
		GUILayout.BeginHorizontal(); GUILayout.Label("x", GUILayout.Width(40)); particles.overflowOffset.x = GUILayout.HorizontalSlider(particles.overflowOffset.x, -10, 10); GUILayout.Label(particles.overflowOffset.x.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(); GUILayout.Label("y", GUILayout.Width(40)); particles.overflowOffset.y = GUILayout.HorizontalSlider(particles.overflowOffset.y, -10, 10); GUILayout.Label(particles.overflowOffset.y.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(); GUILayout.Label("z", GUILayout.Width(40)); particles.overflowOffset.z = GUILayout.HorizontalSlider(particles.overflowOffset.z, -10, 10); GUILayout.Label(particles.overflowOffset.z.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
		if (!particles.calculate && overflowOffset!=particles.overflowOffset) Playground.SetParticleCount(particles, particles.particleCount);
		
		// Minimum- and Maximum Size
		GUILayout.Label("Size");
		GUILayout.BeginHorizontal(); GUILayout.Label("Min", GUILayout.Width(60)); particles.sizeMin = GUILayout.HorizontalSlider(particles.sizeMin, .01, 10); GUILayout.Label(particles.sizeMin.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(); GUILayout.Label("Max", GUILayout.Width(60)); particles.sizeMax = GUILayout.HorizontalSlider(particles.sizeMax, .01, 10); GUILayout.Label(particles.sizeMax.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
		
		GUILayout.Space(10);
		
		///////////////////////////
		// Emission
		///////////////////////////
		
		particles.emit = GUILayout.Toggle(particles.emit, "Emit Particles");
		
		///////////////////////////
		// Calculation
		///////////////////////////
		
		particles.calculate = GUILayout.Toggle(particles.calculate, "Calculate Particles");
		if (particles.calculate) {
			
			// Delta Movement
			particles.calculateDeltaMovement = GUILayout.Toggle(particles.calculateDeltaMovement, "Calculate Delta Movement");
			if (particles.calculateDeltaMovement) {
				GUILayout.Label("Delta Movement Strength");
				GUILayout.BeginHorizontal(); particles.deltaMovementStrength = GUILayout.HorizontalSlider(particles.deltaMovementStrength, 0, 20); GUILayout.Label(particles.deltaMovementStrength.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			}
			
			// Minimum- and Maximum Rotation
			GUILayout.Label("Rotation Speed");
			GUILayout.BeginHorizontal(); GUILayout.Label("Min", GUILayout.Width(60)); particles.rotationSpeedMin = GUILayout.HorizontalSlider(particles.rotationSpeedMin, -360, 360); GUILayout.Label(particles.rotationSpeedMin.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("Max", GUILayout.Width(60)); particles.rotationSpeedMax = GUILayout.HorizontalSlider(particles.rotationSpeedMax, -360, 360); GUILayout.Label(particles.rotationSpeedMax.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			
			// Lifetime
			GUILayout.Label("Lifetime");
			GUILayout.BeginHorizontal(); particles.lifetime = GUILayout.HorizontalSlider(particles.lifetime, 0, 100); GUILayout.Label(particles.lifetime.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();

			// Lifetime size
			GUILayout.Label("Lifetime Size");
			selectedLifetimeSize = GUILayout.SelectionGrid (selectedLifetimeSize, lifetimeSizeImages, 3);
			if (GUI.changed)
				SetLifetimeSize(lifetimeSizes[selectedLifetimeSize].animationCurve);
			
			GUILayout.Space(10);
			
			
			///////////////////////////
			// Forces
			///////////////////////////
			
			GUILayout.Label("Forces", smallLabel);
			
			particles.onlySourcePositioning = GUILayout.Toggle(particles.onlySourcePositioning, "Only Source Positions");
			
			particles.applyLifetimeVelocity = GUILayout.Toggle(particles.applyLifetimeVelocity, "Lifetime Velocity");
			if (particles.applyLifetimeVelocity) {
				selectedLifetimeVelocity = GUILayout.SelectionGrid (selectedLifetimeVelocity, lifetimeVelocityImages, 3);
				if (GUI.changed)
					SetLifetimeVelocity(lifetimeVelocities[selectedLifetimeVelocity].vector3AnimationCurve);
			}
			
			GUILayout.Label("Initial Velocity Min");
			GUILayout.BeginHorizontal(); GUILayout.Label("x", GUILayout.Width(40)); particles.initialVelocityMin.x = GUILayout.HorizontalSlider(particles.initialVelocityMin.x, -20, 20); GUILayout.Label(particles.initialVelocityMin.x.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("y", GUILayout.Width(40)); particles.initialVelocityMin.y = GUILayout.HorizontalSlider(particles.initialVelocityMin.y, -20, 20); GUILayout.Label(particles.initialVelocityMin.y.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("z", GUILayout.Width(40)); particles.initialVelocityMin.z = GUILayout.HorizontalSlider(particles.initialVelocityMin.z, -20, 20); GUILayout.Label(particles.initialVelocityMin.z.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			
			GUILayout.Label("Initial Velocity Max");
			GUILayout.BeginHorizontal(); GUILayout.Label("x", GUILayout.Width(40)); particles.initialVelocityMax.x = GUILayout.HorizontalSlider(particles.initialVelocityMax.x, -20, 20); GUILayout.Label(particles.initialVelocityMax.x.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("y", GUILayout.Width(40)); particles.initialVelocityMax.y = GUILayout.HorizontalSlider(particles.initialVelocityMax.y, -20, 20); GUILayout.Label(particles.initialVelocityMax.y.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("z", GUILayout.Width(40)); particles.initialVelocityMax.z = GUILayout.HorizontalSlider(particles.initialVelocityMax.z, -20, 20); GUILayout.Label(particles.initialVelocityMax.z.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			
			GUI.enabled = (selectedSource!=2);
			
			GUILayout.Label("Initial Local Velocity Min");
			GUILayout.BeginHorizontal(); GUILayout.Label("x", GUILayout.Width(40)); particles.initialLocalVelocityMin.x = GUILayout.HorizontalSlider(particles.initialLocalVelocityMin.x, -20, 20); GUILayout.Label(particles.initialLocalVelocityMin.x.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("y", GUILayout.Width(40)); particles.initialLocalVelocityMin.y = GUILayout.HorizontalSlider(particles.initialLocalVelocityMin.y, -20, 20); GUILayout.Label(particles.initialLocalVelocityMin.y.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("z", GUILayout.Width(40)); particles.initialLocalVelocityMin.z = GUILayout.HorizontalSlider(particles.initialLocalVelocityMin.z, -20, 20); GUILayout.Label(particles.initialLocalVelocityMin.z.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			
			GUILayout.Label("Initial Local Velocity Max");
			GUILayout.BeginHorizontal(); GUILayout.Label("x", GUILayout.Width(40)); particles.initialLocalVelocityMax.x = GUILayout.HorizontalSlider(particles.initialLocalVelocityMax.x, -20, 20); GUILayout.Label(particles.initialLocalVelocityMax.x.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("y", GUILayout.Width(40)); particles.initialLocalVelocityMax.y = GUILayout.HorizontalSlider(particles.initialLocalVelocityMax.y, -20, 20); GUILayout.Label(particles.initialLocalVelocityMax.y.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("z", GUILayout.Width(40)); particles.initialLocalVelocityMax.z = GUILayout.HorizontalSlider(particles.initialLocalVelocityMax.z, -20, 20); GUILayout.Label(particles.initialLocalVelocityMax.z.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			
			GUI.enabled = true;
			
			GUILayout.Label("Velocity Bending");
			GUILayout.BeginHorizontal(); GUILayout.Label("x", GUILayout.Width(40)); particles.velocityBending.x = GUILayout.HorizontalSlider(particles.velocityBending.x, -20, 20); GUILayout.Label(particles.velocityBending.x.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("y", GUILayout.Width(40)); particles.velocityBending.y = GUILayout.HorizontalSlider(particles.velocityBending.y, -20, 20); GUILayout.Label(particles.velocityBending.y.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("z", GUILayout.Width(40)); particles.velocityBending.z = GUILayout.HorizontalSlider(particles.velocityBending.z, -20, 20); GUILayout.Label(particles.velocityBending.z.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			
			GUILayout.Label("Gravity");
			GUILayout.BeginHorizontal(); GUILayout.Label("x", GUILayout.Width(40)); particles.gravity.x = GUILayout.HorizontalSlider(particles.gravity.x, -20, 20); GUILayout.Label(particles.gravity.x.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("y", GUILayout.Width(40)); particles.gravity.y = GUILayout.HorizontalSlider(particles.gravity.y, -20, 20); GUILayout.Label(particles.gravity.y.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); GUILayout.Label("z", GUILayout.Width(40)); particles.gravity.z = GUILayout.HorizontalSlider(particles.gravity.z, -20, 20); GUILayout.Label(particles.gravity.z.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			
			GUILayout.Label("Damping");
			GUILayout.BeginHorizontal(); particles.damping = GUILayout.HorizontalSlider(particles.damping, 0, 10); GUILayout.Label(particles.damping.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			
			GUILayout.Space(10);
			
			///////////////////////////
			// Collision
			///////////////////////////
			
			particles.collision = GUILayout.Toggle(particles.collision, "Collision");
			if (particles.collision) {
				GUILayout.Label("Bounciness");
				GUILayout.BeginHorizontal(); particles.bounciness = GUILayout.HorizontalSlider(particles.bounciness, 0, 2); GUILayout.Label(particles.bounciness.ToString("f2"), GUILayout.Width(60)); GUILayout.EndHorizontal();
			}
			
			GUILayout.Space(10);
		
		}
		
		GUILayout.EndVertical();
		
		///////////////////////////
		// Rendering
		///////////////////////////
		
		GUILayout.BeginVertical("box");
		GUILayout.Label("Rendering", smallLabel);
		
		// Lifetime Color
		GUILayout.Label("Lifetime Color");
		selectedLifetimeColor = GUILayout.SelectionGrid (selectedLifetimeColor, lifetimeColorImages, 3);
		if (GUI.changed) 
			SetLifetimeColor(lifetimeColors[selectedLifetimeColor].gradient);
			
		// Material
		GUILayout.Label("Material");
		selectedMaterial = GUILayout.Toolbar (selectedMaterial, ["Additive", "Multiply", "Vertex"]);
		if (GUI.changed)
			Playground.SetMaterial(particles, materials[selectedMaterial]);
		
		GUILayout.EndVertical();
		
		
		///////////////////////////
		// Advanced
		///////////////////////////
		
		GUILayout.BeginVertical("box");
		GUILayout.Label("Advanced", smallLabel);
		
		// Lifetime Sorting
		GUILayout.Label("Lifetime Sorting");
		var currentLifetimeSorting = selectedLifetimeSorting;
		selectedLifetimeSorting = GUILayout.Toolbar(selectedLifetimeSorting, ["Scrambled", "Burst", "Linear", "Reversed", "Neighbor"]);
		if (currentLifetimeSorting!=selectedLifetimeSorting) {
			SetLifetimeSorting(selectedLifetimeSorting);
		}
		if (selectedLifetimeSorting==4) {
			GUILayout.BeginHorizontal();
				GUILayout.Label("Origin");
				var currentOrigin = particles.nearestNeighborOrigin;
				particles.nearestNeighborOrigin = GUILayout.HorizontalSlider(particles.nearestNeighborOrigin, 0, particles.particleCount); GUILayout.Label(particles.nearestNeighborOrigin.ToString(), GUILayout.Width(60)); 
				if (particles.nearestNeighborOrigin!=currentOrigin)
					PlaygroundParticles.SetLifetime(particles, particles.lifetime);
			GUILayout.EndHorizontal();
		}
		
		// Update Rate
		GUILayout.Label("Update Rate");
		GUILayout.BeginHorizontal(); particles.updateRate = GUILayout.HorizontalSlider(particles.updateRate, 10, 1); GUILayout.Label(particles.updateRate.ToString(), GUILayout.Width(60)); GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
				
		
	
	}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
	
	} else {
		GUILayout.BeginVertical("box");
		GUILayout.BeginHorizontal(); GUILayout.Label("Particle Playground"); if (GUILayout.Button("Reset")) Application.LoadLevel(Application.loadedLevel); if (GUILayout.Button(">>")) showGui=true; GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
}

function Update () {
	
	// Set camera rotation from input
	var newRotation : Vector3 = thisTransform.rotation.eulerAngles;
	newRotation += Vector3(
		Input.GetAxis("Vertical")*100*Time.deltaTime, 
		-Input.GetAxis("Horizontal")*100*Time.deltaTime%360, 
		0
	);
	newRotation.x = Mathf.Clamp(newRotation.x, 0, 25);
	thisTransform.rotation = Quaternion.Euler(newRotation);
	
	// Set camera zoom from input
	var fov : float = cam.fieldOfView;
	if (Input.GetKey(KeyCode.Q)) fov += 100*Time.deltaTime;
	if (Input.GetKey(KeyCode.E)) fov -= 100*Time.deltaTime;
	cam.fieldOfView = Mathf.Clamp(fov, 26, 80);
	
	// Set smooth camera look direction
	var rotation : Quaternion = Quaternion.LookRotation(camTarget.position - camTransform.position);
	camTransform.rotation = Quaternion.Slerp(camTransform.rotation, rotation, Time.deltaTime);
	
	// Knock out robot
	if (particles.source==SOURCE.SkinnedWorldObject && Input.GetMouseButtonDown(0)) {
		var ray : Ray = cam.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, 100, robotLayer))
			robotAnimator.SetBool("Knock", true);
		else
			robotAnimator.SetBool("Knock", false);
	}
	if (particles.source==SOURCE.SkinnedWorldObject && Input.GetMouseButtonUp(0))
		robotAnimator.SetBool("Knock", false);
	
	// Manipulator world gui	
	if (manipulator.enabled) {
		manipulatorGuiSizeTransform.LookAt(camTransform);
	}
	
	// Interact with rigidbodies
	if (Input.GetMouseButtonDown(0))
		LiftRigidbody();
		
	// Lerp GUI width
	if (Input.mousePosition.x<guiWidth+10)
		guiWidth = Mathf.Lerp(guiWidth, 479, 6*Time.deltaTime);
	else guiWidth = Mathf.Lerp(guiWidth, 200, 4*Time.deltaTime);
}

function FixedUpdate () {
	
	// Clamp rigidbody into Rect
	if (particles.source==SOURCE.WorldObject)
		ClampObject(particles.worldObject.rigidbody);
	if (manipulator.enabled)
		ClampObject(manipulatorRb);
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Functions
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Set source by assigning SOURCE (enum) to PlaygroundParticles.source
function SetSource (newSource : int) {

	// Reset world object (and camera target)
	SetWorldObject(null);
	
	// Reset skinned world object
	skinnedWorldObjectGameObject.SetActive(false);
	particles.skinnedWorldObject.renderer.material = skinnedWorldObjectInvisibleMaterial;
	
	// Set source
	switch (newSource) {
		case 0:
			// Mesh in scene
			particles.source = SOURCE.WorldObject;
			SetWorldObject(worldObjects[selectedWorldObject]);
			if (particles.particleCount<particles.worldObject.vertexPositions.Length)
				Playground.SetParticleCount(particles, particles.worldObject.vertexPositions.Length); 
		break;
		case 1:
			// Skinned Mesh in scene
			skinnedWorldObjectGameObject.SetActive(true);
			robotAnimator.SetBool("Knock", false);
			particles.source = SOURCE.SkinnedWorldObject;
			camTarget.parent = skinnedWorldObjectTransform;
			camTarget.localPosition = Vector3(0,.7,0);
			particles.skinnedWorldObject.renderer.material = showSkinned?skinnedWorldObjectVisibleMaterial:skinnedWorldObjectInvisibleMaterial;
			if (particles.particleCount<particles.skinnedWorldObject.vertexPositions.Length)
				Playground.SetParticleCount(particles, particles.skinnedWorldObject.vertexPositions.Length); 
		break;
		case 2:
			// Texture or Mesh in a static state
			particles.source = SOURCE.State;
			if (particles.particleCount<particles.states[particles.activeState].positionLength)
				Playground.SetParticleCount(particles, particles.states[particles.activeState].positionLength); 
		break;
	}
}

// Assign a new World Object by calling the Playground Wrapper 'Playground.WorldObject(PlaygroundParticles, Transform)' or directly through PlaygroundParticles.NewWorldObject(PlaygroundParticles, Transform)
function SetWorldObject (newWorldObjectTransform : Transform) {
	
	if (newWorldObjectTransform==null) {
		particles.worldObject.renderer.enabled = false;
		camTarget.parent = null;
		camTarget.position = Vector3.zero;
		return;
	}
	
	// Set old object inactive
	if (particles.worldObject.gameObject)
		particles.worldObject.gameObject.SetActive(false);
	
	// Set new object active
	newWorldObjectTransform.gameObject.SetActive(true);
	
	// Assign World Object to PlaygroundParticles
	Playground.WorldObject(particles, newWorldObjectTransform);
	
	// Reset forces
	if (particles.worldObject.rigidbody) {
		particles.worldObject.rigidbody.velocity = Vector3.zero;
		particles.worldObject.rigidbody.angularVelocity = Vector3.zero;
	}
	
	// Position
	particles.worldObject.transform.position = worldObjectCreationPosition.position;
	
	// Rotation
	particles.worldObject.transform.rotation = worldObjectCreationPosition.rotation;
	
	// Render
	particles.worldObject.renderer.enabled = true;
	
	// Set Camera Target
	camTarget.parent = newWorldObjectTransform;
	camTarget.localPosition = Vector3.zero;
	
	// Update point info
	points = particles.worldObject.vertexPositions.Length;
}

// Set transition by assigning TRANSITION (enum) to PlaygroundParticles.transition
function SetTransition (newTransition : int) {
	switch (newTransition) {
		case 0:
			// None
			particles.transition = TRANSITION.None;
		break;
		case 1:
			// Lerp
			particles.transition = TRANSITION.Lerp;
		break;
		case 2:
			// Fade
			particles.transition = TRANSITION.Fade;
		break;
	}
}

// Set lifetime size by assigning an animation curve to PlaygroundParticles.lifetimeSize
function SetLifetimeSize (animationCurve : AnimationCurve) {
	particles.lifetimeSize = animationCurve;
}

// Set lifetime velocity by assigning an animation curve to PlaygroundParticles.lifetimeVelocity
function SetLifetimeVelocity (vector3AnimationCurve : Vector3AnimationCurve) {
	particles.lifetimeVelocity = vector3AnimationCurve;
}

// Set lifetime color by assigning a gradient to PlaygroundParticles.lifetimeColor
function SetLifetimeColor (gradient : Gradient) {
	particles.lifetimeColor = gradient;
}

// Set lifetime sorting by assigning SORTING (enum) to PlaygroundParticles.sorting
function SetLifetimeSorting (newSorting : int) {
	switch (newSorting) {
		case 0:
			// Causes "random" (but non-equal) lifetime behaviour 
			particles.sorting = SORTING.ScrambledLinear;
		break;
		case 1:
			// Causes "one shot" lifetime behaviour
			particles.sorting = SORTING.Burst;
		break;
		case 2:
			// Causes "alpha to omega" lifetime behaviour
			particles.sorting = SORTING.Linear;
		break;
		case 3:
			// Causes "omega to alpha" lifetime behaviour
			particles.sorting = SORTING.Reversed;
		break;
		case 4:
			particles.sorting = SORTING.NearestNeighbor;
		break;
	}
	
	// Refresh lifetime
	Playground.SetLifetime(particles, particles.lifetime);
}

// Set manipulator type by assigning MANIPULATORTYPE (enum) to ManipulatorObject
function SetManipulatorType (newManipulatorType : int) {
	
	// Set manipulator type
	switch (newManipulatorType) {
		case 0:
			manipulator.type = MANIPULATORTYPE.Attractor;
		break;
		case 1:
			manipulator.type = MANIPULATORTYPE.AttractorGravitational;
		break;
		case 2:
			manipulator.type = MANIPULATORTYPE.Repellent;
		break;
	}
	
	// Enable components
	manipulatorCollider.enabled = manipulator.enabled;
	manipulatorGo.SetActive(manipulator.enabled);
	manipulatorRenderer.enabled = manipulator.enabled;
	manipulatorTransform.position = worldObjectCreationPosition.position;
}

// Lift a rigidbody
function LiftRigidbody () {

	// Send a Raycast
	var hit : RaycastHit;
	var ray : Ray = cam.ScreenPointToRay(Input.mousePosition);
	if (Physics.Raycast(ray, hit, 100, interactionLayer) && hit.rigidbody) {
		
		// Object
		var rb : Rigidbody = hit.rigidbody;
		var dragPos : Vector3;
		var previousPos : Vector2;
		var dragDelta : Vector2;
		rb.angularVelocity = Vector3.zero;
		rb.velocity = Vector3.zero;
		
		// Fixed Joint
		var sjGameObject : GameObject = new GameObject("Fixed Joint");
		var sjTransform : Transform = sjGameObject.transform;
		var sjRigidbody : Rigidbody = sjGameObject.AddComponent(Rigidbody);
		var springJoint : SpringJoint = sjGameObject.AddComponent(SpringJoint);
		sjTransform.position = hit.point;
		sjRigidbody.isKinematic = true;
		springJoint.connectedBody = rb;
		springJoint.spring = 1000;
		springJoint.damper = 1;
		springJoint.breakForce = Mathf.Infinity;
		springJoint.breakTorque = Mathf.Infinity;
		
		while (Input.GetMouseButton(0)) {
			dragPos = cam.ScreenToWorldPoint(Vector3(Input.mousePosition.x, Input.mousePosition.y, camTransform.InverseTransformPoint(hit.point).z));
			sjTransform.position = Vector3.Lerp(sjTransform.position, camTransform.TransformDirection(camTransform.InverseTransformPoint(dragPos).x, dragPos.y, dragPos.z), 10*Time.deltaTime);
			dragDelta = (Input.mousePosition-previousPos);
			previousPos = Input.mousePosition;
			
			// Clamp
		    if (sjTransform.position.y>clampRect.yMax) {
		       sjTransform.position.y = clampRect.yMax;
		    } else
		    if (sjTransform.position.y<clampRect.yMin) {
		       sjTransform.position.y = clampRect.yMin;
		    }
		    if (sjTransform.position.x<clampRect.xMin) {
		       sjTransform.position.x = clampRect.xMin;
		    } else
		    if (sjTransform.position.x>clampRect.xMax) {
		    	sjTransform.position.x = clampRect.xMax;
		    }
		   	if (sjTransform.position.z<clampRect.xMin) {
		   		sjTransform.position.z = clampRect.xMin;
		   	} else
	   		if (sjTransform.position.z>clampRect.xMax) {
		   		sjTransform.position.z = clampRect.xMax;
		   	}
		    
			yield WaitForFixedUpdate;
		}
		Destroy(sjGameObject);
		rb.AddForce(camTransform.TransformDirection(dragDelta.x, dragDelta.y, 0), ForceMode.Impulse);
	}
}

// Clamp a rigidbody into rect
function ClampObject (r : Rigidbody) {
 	
    // Set limits within the frustrum of the camera
    var objectPosition : Vector3 = r.position;
    
    // Clamp top
    if (objectPosition.y>clampRect.yMax) {
       r.position.y = clampRect.yMax;
       r.velocity.y = -r.velocity.y * .5;
    } else
 
    // Clamp bottom
    if (objectPosition.y<clampRect.yMin) {
       r.position.y = clampRect.yMin;
       r.velocity.y = -r.velocity.y * .5;
    }
 
    // Clamp left
    if (objectPosition.x<clampRect.xMin) {
       r.position.x = clampRect.xMin;
       r.velocity.x = -r.velocity.x * .5;
    } else
 
    // Clamp right
    if (objectPosition.x>clampRect.xMax) {
       r.position.x = clampRect.xMax;
       r.velocity.x = -r.velocity.x * .5;
    }
    
    // Clamp depth negative
    if (objectPosition.z<clampRect.xMin) {
       r.position.z = clampRect.xMin;
       r.velocity.z = -r.velocity.z * .5;
    } else
 
    // Clamp depth positive
    if (objectPosition.z>clampRect.xMax) {
       r.position.z = clampRect.xMax;
       r.velocity.z = -r.velocity.z * .5;
    }
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Classes
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// A gradient wrapper for using gradients in an array
class GradientWrapper {
	var gradient : Gradient;
	var image : Texture2D;
}

// An AnimationCurve wrapper for using animation curves in an array
class AnimationCurveWrapper {
	var animationCurve : AnimationCurve;
	var image : Texture2D;
}

// A Vector3AnimationCurve wrappre for using Vector3AnimationCurve in an array
class Vector3AnimationCurveWrapper {
	var vector3AnimationCurve : Vector3AnimationCurve;
	var image : Texture2D;
}