using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(ParticleSystem))]
[ExecuteInEditMode()]
public class PlaygroundParticlesC : MonoBehaviour {

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PlaygroundParticlesC variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// Playground variables
	[HideInInspector] public SOURCEC source;											// The particle source
	[HideInInspector] public int sourceDownResolution = 1;								// The source distribution over vertices (used to down resolution a skinned mesh source points)
	[HideInInspector] public int activeState;											// Current active state (when using state as source)
	[HideInInspector] public TRANSITIONC transition;									// The type of transition to use
	[HideInInspector] public float transitionTime = 1f;									// The time it takes to complete a transition
	[HideInInspector] public bool emit = true;											// If emission of particles is active on this PlaygroundParticles
	[HideInInspector] public bool loop = true;											// Should a particle re-emit when reaching the end of its lifetime?
	[HideInInspector] public bool disableOnDone = false;								// Should the GameObject of this PlaygroundParticlesC disable when not looping? 
	[HideInInspector] public int updateRate = 1;										// The rate to update this PlaygroundParticles
	[HideInInspector] public bool calculate = true;										// Calculate forces on this PlaygroundParticlesC (can be overrided by PlaygroundC.calculate)
	[HideInInspector] public bool calculateDeltaMovement = true;						// Calculate the delta movement force of this particle system
	[HideInInspector] public float deltaMovementStrength = 10f;							// The strength to multiply delta movement with
	[HideInInspector] public bool worldObjectUpdateVertices = false;					// The current world object will change its vertices over time
	[HideInInspector] public bool worldObjectUpdateNormals = false;						// The current world object will change its normals over time
	[HideInInspector] public int nearestNeighborOrigin = 0;								// The initial source position when using lifetime sorting of Nearest Neighbor / Nearest Neighbor Reversed
	[HideInInspector] public int particleCount;											// The amount of particles within this PlaygroundParticlesC object
	[HideInInspector] public float emissionRate = 1f;									// The percentage to emit of particleCount in bursts from this PlaygroundParticles
	[HideInInspector] public OVERFLOWMODEC overflowMode = OVERFLOWMODEC.SourceTransform;// The method to calculate overflow with
	[HideInInspector] public Vector3 overflowOffset;									// Offset when particle count exceeds source count
	[HideInInspector] public bool applySourceScatter = false;							// Should source position scattering be applied?
	[HideInInspector] public Vector3 sourceScatterMin;									// The minimum spread of source position scattering
	[HideInInspector] public Vector3 sourceScatterMax;									// The maximum spread of source position scattering
	[HideInInspector] public SORTINGC sorting = SORTINGC.Scrambled;						// Sort mode for particle lifetime
	[HideInInspector] public AnimationCurve lifetimeSorting;							// Custom sorting for particle lifetime (when sorting is set to Custom)
	[HideInInspector] public float sizeMin = 1f;										// Minimum size
	[HideInInspector] public float sizeMax = 1f;										// Maximum size
	[HideInInspector] public float scale = 1f;											// The scale of minimum and maximum size
	[HideInInspector] public float initialRotationMin;									// Minimum initial rotation
	[HideInInspector] public float initialRotationMax;									// Maximum initial rotation
	[HideInInspector] public float rotationSpeedMin;									// Minimum amount to rotate
	[HideInInspector] public float rotationSpeedMax;									// Maximum amount to rotate
	[HideInInspector] public bool rotateTowardsDirection = false;						// Should the particles rotate towards their movement direction
	[HideInInspector] public Vector3 rotationNormal = -Vector3.forward;					// The rotation direction normal when rotating towards direction (always normalized value)
	[HideInInspector] public float lifetime;											// The life of a particle in seconds
	[HideInInspector] public float lifetimeOffset;										// The offset in time of this particle system
	[HideInInspector] public AnimationCurve lifetimeSize;								// The size over lifetime of each particle
	[HideInInspector] public bool onlySourcePositioning = false;						// Should the particles only position on their source (and not apply any forces)?
	[HideInInspector] public bool applyLifetimeVelocity = false;						// Should lifetime velocity affect particles?
	[HideInInspector] public Vector3AnimationCurveC lifetimeVelocity;					// The velocity over lifetime of each particle
	[HideInInspector] public bool applyInitialVelocity = false;							// Should initial velocity affect particles?
	[HideInInspector] public Vector3 initialVelocityMin;								// The minimum starting velocity of each particle
	[HideInInspector] public Vector3 initialVelocityMax;								// The maximum starting velocity of each particle
	[HideInInspector] public bool applyInitialLocalVelocity = false;					// Should initial local velocity affect particles?
	[HideInInspector] public Vector3 initialLocalVelocityMin;							// The minimum starting velocity of each particle with normal or transform direction
	[HideInInspector] public Vector3 initialLocalVelocityMax;							// The maximum starting velocity of each particle with normal or transform direction
	[HideInInspector] public bool applyInitialVelocityShape = false;					// Should the initial velocity shape be applied on particle re/birth?
	[HideInInspector] public Vector3AnimationCurveC initialVelocityShape;				// The amount of velocity to apply of the spawning particle's initial/local velocity in form of a Vector3AnimationCurve
	[HideInInspector] public bool applyVelocityBending;									// Should bending affect particles velocity?
	[HideInInspector] public Vector3 velocityBending;									// The amount to bend velocity of each particle
	[HideInInspector] public Vector3 gravity;											// The constant force towards gravitational vector
	[HideInInspector] public float maxVelocity = 100f;									// The maximum positive- and negative velocity of each particle
	[HideInInspector] public PlaygroundAxisConstraintsC axisConstraints = new PlaygroundAxisConstraintsC(); // The force axis constraints of each particle
	[HideInInspector] public float damping;												// Particles inertia over time
	[HideInInspector] public Gradient lifetimeColor;									// The color over lifetime
	[HideInInspector] public COLORSOURCEC colorSource = COLORSOURCEC.Source;			// The source to read color from (fallback on Lifetime Color if no source color is available)
	[HideInInspector] public bool sourceUsesLifetimeAlpha;								// Should the source color use alpha from Lifetime Color instead of the source's original alpha?
	[HideInInspector] public bool applyLocalSpaceMovementCompensation = true;			// Should the movement of the particle system transform when in local simulation space be compensated for?
	[HideInInspector] public bool applyRandomSizeOnRebirth = true;						// Should particles get a new random size upon rebirth?
	[HideInInspector] public bool applyRandomRotationOnRebirth = true;					// Should particles get a new random rotation upon rebirth?
	[HideInInspector] public bool applyRandomScatterOnRebirth = false;					// Should particles get a new scatter position upon rebirth?

	// Source Script variables
	[HideInInspector] public int scriptedEmissionIndex;									// When using Emit() the index will point to the next particle in pool to emit
	[HideInInspector] public Vector3 scriptedEmissionPosition;							// When using Emit() the passed in position will determine the position for this particle
	[HideInInspector] public Vector3 scriptedEmissionVelocity;							// When using Emit() the passed in velocity will determine the speed and direction for this particle
	[HideInInspector] public Color scriptedEmissionColor = Color.white;					// When using Emit() the passed in color will decide the color for this particle if colorSource is set to COLORSOURCEC.Source
	[HideInInspector] public Transform scriptedEmissionParent;							// When using Emit() the passed in transform will decide which transform this source position belongs to
	
	// Collision detection
	[HideInInspector] public bool collision = false;									// Can particles collide?
	[HideInInspector] public bool affectRigidbodies = true;								// Should particles affect rigidbodies?
	[HideInInspector] public float mass = .01f;											// The mass of a particle (calculated in collision with rigidbodies)
	[HideInInspector] public float collisionRadius = 1f;								// The spherical radius of a particle
	[HideInInspector] public LayerMask collisionMask;									// The layers these particles will collide with
	[HideInInspector] public float lifetimeLoss;										// The amount a particle will loose of its lifetime on collision
	[HideInInspector] public float bounciness = .5f;									// The amount a particle will bounce on collision
	[HideInInspector] public Vector3 bounceRandomMin;									// The minimum amount of random bounciness (seen as negative offset from the collided surface's normal direction)
	[HideInInspector] public Vector3 bounceRandomMax;									// The maximum amount of random bounciness (seen as positive offset from the collided surface's normal direction)
	[HideInInspector] public List<PlaygroundColliderC> colliders;						// The Playground Colliders of this particle system
	
	// States (source)
	public List<ParticleStateC> states = new List<ParticleStateC>();					// The states of this PlaygroundParticles
	
	// Scene objects (source)
	[HideInInspector] public WorldObject worldObject;									// A mesh calculated within the scene
	[HideInInspector] public SkinnedWorldObject skinnedWorldObject;						// A skinned mesh calculated within the scene
	[HideInInspector] public Transform sourceTransform;									// A transform calculated within the scene
	
	// Paint
	[HideInInspector] public PaintObjectC paint;											// The paint source of this PlaygroundParticles
	
	// Projection
	[HideInInspector] public ParticleProjectionC projection;								// The projection source of this PlaygroundParticles
	
	// Manipulators
	public List<ManipulatorObjectC> manipulators;										// List of manipulator objects handled by this PlaygroundParticlesC object
	
	// Cache
	[HideInInspector] public PlaygroundCache playgroundCache = new PlaygroundCache();	// Data for each particle
	[HideInInspector] public ParticleCache particleCache = new ParticleCache();			// Particle pool
	
	// Components
	[HideInInspector] public ParticleSystem shurikenParticleSystem;						// This ParticleSystem (Shuriken) component
	[HideInInspector] public int particleSystemId;										// The id of this PlaygroundParticlesC object
	[HideInInspector] public GameObject particleSystemGameObject;						// This GameObject
	[HideInInspector] public Transform particleSystemTransform;							// This Transform
	[HideInInspector] public Renderer particleSystemRenderer;							// This Renderer
	[HideInInspector] public ParticleSystemRenderer particleSystemRenderer2;			// This ParticleSystemRenderer
	
	// Internally used variables
	bool inTransition = false;			
	int previousParticleCount = -1;
	float previousEmissionRate = 1f;
	bool cameFromNonCalculatedFrame = false;
	float previousSizeMin;
	float previousSizeMax;
	float previousInitialRotationMin;
	float previousInitialRotationMax;
	float previousRotationSpeedMin;
	float previousRotationSpeedMax;
	Vector3 previousVelocityMin;
	Vector3 previousVelocityMax;
	Vector3 previousLocalVelocityMin;
	Vector3 previousLocalVelocityMax;
	Vector3 previousTransformPosition;
	float previousLifetime;
	bool previousEmission = true;
	bool previousWorldObjectUpdateVertices = false;
	float emissionStopped = 0f;
	[HideInInspector] public int previousActiveState;
	[HideInInspector] public bool isPainting = false;
	[HideInInspector] public float simulationStarted;
	[HideInInspector] public bool loopExceeded = false;
	[HideInInspector] public int loopExceededOnParticle;
	
	[HideInInspector] float particleTimescale = 1f;

	// Clone settings by passing in a reference
	public void CopyTo (PlaygroundParticlesC playgroundParticles) {
		
		// Playground variables
		playgroundParticles.source 										= this.source;
		playgroundParticles.sourceDownResolution						= this.sourceDownResolution;
		playgroundParticles.activeState 								= this.activeState;
		playgroundParticles.transition 									= this.transition;
		playgroundParticles.transitionTime 								= this.transitionTime;
		playgroundParticles.emit										= this.emit;
		playgroundParticles.updateRate 									= this.updateRate;
		playgroundParticles.calculate 									= this.calculate;					
		playgroundParticles.calculateDeltaMovement						= this.calculateDeltaMovement;
		playgroundParticles.deltaMovementStrength 						= this.deltaMovementStrength;
		playgroundParticles.worldObjectUpdateVertices					= this.worldObjectUpdateVertices;
		playgroundParticles.worldObjectUpdateNormals 					= this.worldObjectUpdateNormals;
		playgroundParticles.nearestNeighborOrigin 						= this.nearestNeighborOrigin;							
		playgroundParticles.particleCount 								= this.particleCount;									
		playgroundParticles.emissionRate 								= this.emissionRate;									
		playgroundParticles.overflowMode 								= this.overflowMode;	
		playgroundParticles.overflowOffset 								= this.overflowOffset;									
		playgroundParticles.applySourceScatter							= this.applySourceScatter;
		playgroundParticles.sourceScatterMin							= this.sourceScatterMin;
		playgroundParticles.sourceScatterMax							= this.sourceScatterMax;
		playgroundParticles.sorting 									= this.sorting;
		playgroundParticles.lifetimeSorting								= this.lifetimeSorting;					
		playgroundParticles.sizeMin 									= this.sizeMin;										
		playgroundParticles.sizeMax 									= this.sizeMax;										
		playgroundParticles.initialRotationMin 							= this.initialRotationMin;									
		playgroundParticles.initialRotationMax 							= this.initialRotationMax;								
		playgroundParticles.rotationSpeedMin 							= this.rotationSpeedMin;								
		playgroundParticles.rotationSpeedMax 							= this.rotationSpeedMax;									
		playgroundParticles.rotateTowardsDirection 						= this.rotateTowardsDirection; 				
		playgroundParticles.rotationNormal 								= this.rotationNormal;
		playgroundParticles.lifetime 									= this.lifetime;											
		playgroundParticles.lifetimeOffset 								= this.lifetimeOffset;									
		playgroundParticles.lifetimeSize 								= this.lifetimeSize;							
		playgroundParticles.onlySourcePositioning 						= this.onlySourcePositioning;					
		playgroundParticles.applyLifetimeVelocity 						= this.applyLifetimeVelocity;				
		playgroundParticles.lifetimeVelocity 							= this.lifetimeVelocity;					
		playgroundParticles.applyInitialVelocity 						= this.applyInitialVelocity;						
		playgroundParticles.initialVelocityMin 							= this.initialVelocityMin;								
		playgroundParticles.initialVelocityMax 							= this.initialVelocityMax;								
		playgroundParticles.applyInitialLocalVelocity 					= this.applyInitialLocalVelocity;		
		playgroundParticles.initialLocalVelocityMin 					= this.initialLocalVelocityMin;							
		playgroundParticles.initialLocalVelocityMax 					= this.initialLocalVelocityMax;							
		playgroundParticles.applyVelocityBending 						= this.applyVelocityBending;								
		playgroundParticles.velocityBending 							= this.velocityBending;									
		playgroundParticles.applyInitialVelocityShape					= this.applyInitialVelocityShape;
		playgroundParticles.initialVelocityShape						= this.initialVelocityShape;
		playgroundParticles.gravity 									= this.gravity;											
		playgroundParticles.damping 									= this.damping;
		playgroundParticles.maxVelocity									= this.maxVelocity;								
		playgroundParticles.lifetimeColor 								= this.lifetimeColor;				
		playgroundParticles.colorSource 								= this.colorSource;				
		playgroundParticles.sourceUsesLifetimeAlpha 					= this.sourceUsesLifetimeAlpha;

		// Scripted source variables
		playgroundParticles.scriptedEmissionIndex						= this.scriptedEmissionIndex;
		playgroundParticles.scriptedEmissionPosition					= this.scriptedEmissionPosition;
		playgroundParticles.scriptedEmissionVelocity					= this.scriptedEmissionVelocity;
		playgroundParticles.scriptedEmissionColor						= this.scriptedEmissionColor;
		playgroundParticles.scriptedEmissionParent						= this.scriptedEmissionParent;
		
		// Collision detection
		playgroundParticles.collision 									= this.collision;
		playgroundParticles.affectRigidbodies 							= this.affectRigidbodies;
		playgroundParticles.mass 										= this.mass; 
		playgroundParticles.collisionRadius 							= this.collisionRadius;
		playgroundParticles.collisionMask 								= this.collisionMask;					
		playgroundParticles.bounciness 									= this.bounciness;
		playgroundParticles.lifetimeLoss 								= this.lifetimeLoss;
		playgroundParticles.bounceRandomMin								= this.bounceRandomMin;
		playgroundParticles.bounceRandomMax								= this.bounceRandomMax;
		playgroundParticles.colliders									= new List<PlaygroundColliderC>();
		int i;
		for (i = 0; i<playgroundParticles.colliders.Count; i++)
			playgroundParticles.colliders.Add(this.colliders[i].Clone());
		
		// States (source)
		playgroundParticles.states 										= new List<ParticleStateC>();
		for (i = 0; i<this.states.Count; i++)
			playgroundParticles.states.Add(this.states[i].Clone());
		
		// Scene objects (source)
		playgroundParticles.worldObject 								= new WorldObject();
		playgroundParticles.worldObject.gameObject = this.gameObject;
		playgroundParticles.skinnedWorldObject 							= new SkinnedWorldObject();
		playgroundParticles.worldObject.gameObject = this.gameObject;
		
		playgroundParticles.sourceTransform 							= this.sourceTransform;
		
		// Paint
		playgroundParticles.paint 										= this.paint.Clone();
		
		// Other
		playgroundParticles.particleTimescale							= this.particleTimescale;
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PlaygroundParticlesC functions
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// Emit a single particle at position with velocity, color and transform point parent (Source Mode SOURCEC.Script required)
	public int Emit (Vector3 givePosition, Vector3 giveVelocity, Color32 giveColor, Transform giveParent) {
		scriptedEmissionIndex=Mathf.Clamp(scriptedEmissionIndex, 0, scriptedEmissionIndex%particleCount);
		int returnIndex = scriptedEmissionIndex;
		
		scriptedEmissionPosition = givePosition;
		scriptedEmissionVelocity = giveVelocity;
		scriptedEmissionColor = giveColor;
		scriptedEmissionParent = giveParent;
		
		Rebirth(this, scriptedEmissionIndex);
		playgroundCache.parent[scriptedEmissionIndex] = giveParent;
		playgroundCache.lifetimeOffset[scriptedEmissionIndex] = 0;
		playgroundCache.life[scriptedEmissionIndex] = 0;
		playgroundCache.birth[scriptedEmissionIndex] = PlaygroundC.globalTime;
		playgroundCache.death[scriptedEmissionIndex] = playgroundCache.birth[scriptedEmissionIndex]+lifetime;
		playgroundCache.emission[scriptedEmissionIndex] = true;
		playgroundCache.rebirth[scriptedEmissionIndex] = true;
		playgroundCache.color[scriptedEmissionIndex] = giveColor;

		emit = true;
		simulationStarted = PlaygroundC.globalTime;
		loopExceeded = false;
		loopExceededOnParticle = -1;
		particleSystemGameObject.SetActive(true);
		
		scriptedEmissionIndex++;scriptedEmissionIndex=scriptedEmissionIndex%particleCount;
		return returnIndex;
	}
	
	// Set emission on/off
	public void Emit (bool setEmission) {
		emit = setEmission;
		if (emit) {
			simulationStarted = PlaygroundC.globalTime;
			loopExceeded = false;
			loopExceededOnParticle = -1;
			particleSystemGameObject.SetActive(true);
			Emission(this, true, true);
		} else {
			emissionStopped = PlaygroundC.globalTime;
		}
	}
	
	// Is this particle system still alive?
	public bool IsAlive () {
		return loopExceeded;
	}

	// Create a new PlaygroundParticlesC object
	public static PlaygroundParticlesC CreatePlaygroundParticles (Texture2D[] images, string name, Vector3 position, Quaternion rotation, Vector3 offset, float particleSize, float scale, Material material) {
		PlaygroundParticlesC playgroundParticles = CreateParticleObject(name,position,rotation,particleSize,material);
		
		int[] quantityList = new int[images.Length];
		int i = 0;
		for (; i<images.Length; i++)
			quantityList[i] = images[i].width*images[i].height;
		playgroundParticles.particleCache = new ParticleCache();
		playgroundParticles.particleCache.particles = new ParticleSystem.Particle[quantityList[PlaygroundC.Largest(quantityList)]];
		OnCreatePlaygroundParticles(playgroundParticles);	
		
		for (i = 0; i<images.Length; i++) {
			playgroundParticles.states.Add(new ParticleStateC());
			playgroundParticles.states[playgroundParticles.states.Count-1].ConstructParticles(images[i],scale,offset,"State 0",null);
		}
		
		return playgroundParticles;
	}
	
	// Set default settings for PlaygroundParticlesC object
	public static void OnCreatePlaygroundParticles (PlaygroundParticlesC playgroundParticles) {
		playgroundParticles.playgroundCache = new PlaygroundCache();
		playgroundParticles.paint = new PaintObjectC();
		playgroundParticles.states = new List<ParticleStateC>();
		playgroundParticles.projection = new ParticleProjectionC();
		playgroundParticles.colliders = new List<PlaygroundColliderC>();
		playgroundParticles.particleSystemId = PlaygroundC.particlesQuantity;
		playgroundParticles.projection.projectionTransform = playgroundParticles.particleSystemTransform;
		
		playgroundParticles.playgroundCache.size = new float[playgroundParticles.particleCount];
		playgroundParticles.playgroundCache.size = PlaygroundC.RandomFloat(playgroundParticles.playgroundCache.size.Length, playgroundParticles.sizeMin, playgroundParticles.sizeMax);
		
		playgroundParticles.previousParticleCount = playgroundParticles.particleCount;
		playgroundParticles.lifetimeSize = new AnimationCurve(new Keyframe(0,1), new Keyframe(1,1));
		
		playgroundParticles.shurikenParticleSystem.Emit(playgroundParticles.particleCount);
		playgroundParticles.shurikenParticleSystem.GetParticles(playgroundParticles.particleCache.particles);
		for (int p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
			playgroundParticles.particleCache.particles[p].size = playgroundParticles.playgroundCache.size[p];
		}
		
		PlaygroundParticlesC.SetParticleCount(playgroundParticles, playgroundParticles.particleCount);
		
		if (PlaygroundC.reference!=null) {
			PlaygroundC.particlesQuantity++;
			PlaygroundC.reference.particleSystems.Add(playgroundParticles);
			playgroundParticles.particleSystemId = PlaygroundC.particlesQuantity;
		}
	}
	
	// Create a Shuriken Particle System
	public static PlaygroundParticlesC CreateParticleObject (string name, Vector3 position, Quaternion rotation, float particleSize, Material material) {
		GameObject go = PlaygroundC.ResourceInstantiate("Particle Playground System");
		PlaygroundParticlesC playgroundParticles = go.GetComponent<PlaygroundParticlesC>();
		playgroundParticles.particleSystemGameObject = go;
		playgroundParticles.particleSystemGameObject.name = name;
		playgroundParticles.shurikenParticleSystem = playgroundParticles.particleSystemGameObject.GetComponent<ParticleSystem>();
		playgroundParticles.particleSystemRenderer = playgroundParticles.shurikenParticleSystem.renderer;
		playgroundParticles.particleSystemRenderer2 = playgroundParticles.shurikenParticleSystem.renderer as ParticleSystemRenderer;
		playgroundParticles.particleSystemTransform = playgroundParticles.particleSystemGameObject.transform;
		playgroundParticles.sourceTransform = playgroundParticles.particleSystemTransform;
		playgroundParticles.source = SOURCEC.Transform;
		playgroundParticles.particleSystemTransform.position = position;
		playgroundParticles.particleSystemTransform.rotation = rotation;
		//if (PlaygroundC.reference==null)
		//	PlaygroundC.ResourceInstantiate("Playground Manager");
		if (PlaygroundC.reference.autoGroup && playgroundParticles.particleSystemTransform.parent==null)
			playgroundParticles.particleSystemTransform.parent = PlaygroundC.referenceTransform;
		
		if (playgroundParticles.particleSystemRenderer.sharedMaterial==null)
			playgroundParticles.particleSystemRenderer.sharedMaterial = material;
		playgroundParticles.shurikenParticleSystem.startSize = particleSize;
		
		return playgroundParticles;
	}
	
	// Create a new WorldObject
	public static WorldObject NewWorldObject (PlaygroundParticlesC playgroundParticles, Transform meshTransform) {
		WorldObject worldObject = new WorldObject();
		if (meshTransform.GetComponentInChildren<MeshFilter>()) {
			worldObject.transform = meshTransform;
			worldObject.gameObject = meshTransform.gameObject;
			worldObject.rigidbody = meshTransform.rigidbody;
			worldObject.renderer = meshTransform.GetComponentInChildren<Renderer>();
			worldObject.meshFilter = meshTransform.GetComponentInChildren<MeshFilter>();
			worldObject.mesh = worldObject.meshFilter.sharedMesh;
			worldObject.vertexPositions = new Vector3[worldObject.mesh.vertexCount];
			worldObject.normals = worldObject.mesh.normals;
			worldObject.cachedId = worldObject.gameObject.GetInstanceID();
			playgroundParticles.worldObject = worldObject;

			worldObject.Initialize();

		} else Debug.Log("Could not find a mesh in "+meshTransform.name+".");
		
		return worldObject;
	}
	
	// Create a new SkinnedWorldObject
	public static SkinnedWorldObject NewSkinnedWorldObject (PlaygroundParticlesC playgroundParticles, Transform meshTransform) {
		SkinnedWorldObject worldObject = new SkinnedWorldObject();
		if (meshTransform.GetComponentInChildren<SkinnedMeshRenderer>()) {
			worldObject.transform = meshTransform;
			worldObject.gameObject = meshTransform.gameObject;
			worldObject.rigidbody = meshTransform.rigidbody;
			worldObject.renderer = meshTransform.GetComponentInChildren<SkinnedMeshRenderer>();
			worldObject.mesh = worldObject.renderer.sharedMesh;
			worldObject.vertexPositions = new Vector3[worldObject.mesh.vertexCount];
			worldObject.normals = worldObject.mesh.normals;
			worldObject.cachedId = worldObject.gameObject.GetInstanceID();
			
			playgroundParticles.skinnedWorldObject = worldObject;
			
		} else Debug.Log("Could not find a skinned mesh in "+meshTransform.name+".");
		
		return worldObject;
	}
	
	// Create a new PaintObject
	public static PaintObjectC NewPaintObject (PlaygroundParticlesC playgroundParticles) {
		PaintObjectC paintObject = new PaintObjectC();
		playgroundParticles.paint = paintObject;
		playgroundParticles.paint.Initialize();
		return paintObject;
	}
	
	// Create a new ParticleProjection object
	public static ParticleProjectionC NewProjectionObject (PlaygroundParticlesC playgroundParticles) {
		ParticleProjectionC projectionObject = new ParticleProjectionC();
		playgroundParticles.projection = projectionObject;
		playgroundParticles.projection.Initialize();
		return projectionObject;
	}
	
	// Create a new ManipulatorObject and attach to the Playground Manager
	public static ManipulatorObjectC NewManipulatorObject (MANIPULATORTYPEC type, LayerMask affects, Transform manipulatorTransform, float size, float strength, PlaygroundParticlesC playgroundParticles) {
		ManipulatorObjectC manipulatorObject = new ManipulatorObjectC();
		manipulatorObject.type = type;
		manipulatorObject.affects = affects;
		manipulatorObject.transform = manipulatorTransform;
		manipulatorObject.size = size;
		manipulatorObject.strength = strength;
		manipulatorObject.bounds = new Bounds(Vector3.zero, new Vector3(size, size, size));
		manipulatorObject.property = new ManipulatorPropertyC();

		// Add this to Playground Manager or the passed in playgroundParticles
		if (playgroundParticles==null)
			PlaygroundC.reference.manipulators.Add(manipulatorObject);
		else
			playgroundParticles.manipulators.Add(manipulatorObject);
		
		return manipulatorObject;
	}

	// Lerp to specified state in this PlaygroundParticles
	public static void Lerp (PlaygroundParticlesC playgroundParticles, int to, float time, LERPTYPEC lerpType) {
		if (to<0) {to=playgroundParticles.states.Count;} to=to%playgroundParticles.states.Count;
		if(time<0) time = 0f;
		Color color = new Color();
		for (int i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			if (lerpType==LERPTYPEC.PositionColor||lerpType==LERPTYPEC.Position) 
				playgroundParticles.particleCache.particles[i].position = Vector3.Lerp(playgroundParticles.particleCache.particles[i].position, playgroundParticles.states[to].GetPosition(i%playgroundParticles.states[to].positionLength), time);
			if (lerpType==LERPTYPEC.PositionColor||lerpType==LERPTYPEC.Color) {
				color = playgroundParticles.states[to].GetColor(i%playgroundParticles.states[to].colorLength);
				playgroundParticles.particleCache.particles[i].color = Color.Lerp(playgroundParticles.particleCache.particles[i].color, color, time);
			}
		}
		Update(playgroundParticles);
	}
	
	// Lerp to state object
	public static void Lerp (PlaygroundParticlesC playgroundParticles, ParticleStateC state, float time, LERPTYPEC lerpType) {
		if(time<0) time = 0f;
		Color color = new Color();
		for (int i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			if (lerpType==LERPTYPEC.PositionColor||lerpType==LERPTYPEC.Position) 
				playgroundParticles.particleCache.particles[i].position = Vector3.Lerp(playgroundParticles.particleCache.particles[i].position, state.GetPosition(i%state.positionLength), time);
			if (lerpType==LERPTYPEC.PositionColor||lerpType==LERPTYPEC.Color) {
				color = state.GetColor(i%state.colorLength);
				playgroundParticles.particleCache.particles[i].color = Color.Lerp(playgroundParticles.particleCache.particles[i].color, color, time);
			}
		}
	}
	
	// Lerp to a Skinned World Object
	public static void Lerp (PlaygroundParticlesC playgroundParticles, SkinnedWorldObject particleStateWorldObject, float time) {
		if(time<0) time = 0f;
		Vector3[] vertices = particleStateWorldObject.mesh.vertices;
		BoneWeight[] weights = particleStateWorldObject.mesh.boneWeights;
		Matrix4x4[] boneMatrices = new Matrix4x4[particleStateWorldObject.renderer.bones.Length];

		int i;
		for (i = 0; i<boneMatrices.Length; i++)
			boneMatrices[i] = particleStateWorldObject.renderer.bones[i].localToWorldMatrix * particleStateWorldObject.mesh.bindposes[i];
		
		Matrix4x4 vertexMatrix = new Matrix4x4();
		for (i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			BoneWeight weight = weights[i];
			Matrix4x4 m0 = boneMatrices[weight.boneIndex0];
			Matrix4x4 m1 = boneMatrices[weight.boneIndex1];
			Matrix4x4 m2 = boneMatrices[weight.boneIndex2];
			Matrix4x4 m3 = boneMatrices[weight.boneIndex3];
			
			for(int n=0;n<16;n++){
				vertexMatrix[n] =
					m0[n] * weight.weight0 +
						m1[n] * weight.weight1 +
						m2[n] * weight.weight2 +
						m3[n] * weight.weight3;
			}
			
			playgroundParticles.particleCache.particles[i].position = Vector3.Lerp(playgroundParticles.particleCache.particles[i].position, vertexMatrix.MultiplyPoint3x4(vertices[i%vertices.Length]), time);
		}
	}

	// Set position from PixelParticle object
	public static void SetPosition (PlaygroundParticlesC playgroundParticles, int to, bool runUpdate) {
		
		for (int i = 0; i<playgroundParticles.particleCache.particles.Length; i++)
			playgroundParticles.particleCache.particles[i].position = playgroundParticles.states[to].GetPosition(i%playgroundParticles.states[to].positionLength)+GetOverflowOffset(ref playgroundParticles, i, playgroundParticles.states[to].positionLength);
		if (runUpdate)
			Update(playgroundParticles);
	}
	
	// Set color from PixelParticle object
	public static void  SetColor (PlaygroundParticlesC playgroundParticles, int to) {
		Color color = new Color();
		for (int i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			color = playgroundParticles.states[to].GetColor(i%playgroundParticles.states[to].colorLength);
			playgroundParticles.particleCache.particles[i].color = color;
		}
	}
	
	// Set color from Color
	public static void SetColor (PlaygroundParticlesC playgroundParticles, Color color) {
		for (int i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			playgroundParticles.particleCache.particles[i].color = color;
		}
	}
	
	// Set position from Mesh World Object
	public static void SetPosition (ref PlaygroundParticlesC playgroundParticles, ref WorldObject particleStateWorldObject) {
		if (playgroundParticles.worldObject==null || playgroundParticles.worldObject.mesh==null) {
			Debug.Log("There is no mesh assigned to "+playgroundParticles.particleSystemGameObject.name+"'s worlObject.");
			return;
		}
		Vector3[] meshVertices = particleStateWorldObject.mesh.vertices;
		for (int i = 0; i<playgroundParticles.particleCache.particles.Length; i++)
			playgroundParticles.particleCache.particles[i].position = particleStateWorldObject.transform.TransformPoint(meshVertices[i%meshVertices.Length])+GetOverflowOffset(ref playgroundParticles, i, meshVertices.Length);
	}
	
	// Set position from Skinned Mesh World Object
	public static void SetPosition (ref PlaygroundParticlesC playgroundParticles, ref SkinnedWorldObject particleStateWorldObject) {
		if (playgroundParticles.skinnedWorldObject==null || playgroundParticles.skinnedWorldObject.mesh==null) {
			Debug.Log("There is no skinned mesh assigned to "+playgroundParticles.particleSystemGameObject.name+"'s skinnedWorlObject.");
			return;
		}
		Vector3[] vertices = particleStateWorldObject.mesh.vertices;
		BoneWeight[] weights = particleStateWorldObject.mesh.boneWeights;
		Matrix4x4[] bindposes = particleStateWorldObject.mesh.bindposes;
		Matrix4x4[] boneMatrices = new Matrix4x4[particleStateWorldObject.renderer.bones.Length];

		int i = 0;
		for (; i<boneMatrices.Length; i++)
			boneMatrices[i] = particleStateWorldObject.renderer.bones[i].localToWorldMatrix * bindposes[i];
		
		Matrix4x4 vertexMatrix = new Matrix4x4();
		for (i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			BoneWeight weight = weights[i];
			Matrix4x4 m0 = boneMatrices[weight.boneIndex0];
			Matrix4x4 m1 = boneMatrices[weight.boneIndex1];
			Matrix4x4 m2 = boneMatrices[weight.boneIndex2];
			Matrix4x4 m3 = boneMatrices[weight.boneIndex3];
			
			for(int n=0;n<16;n++){
				vertexMatrix[n] =
					m0[n] * weight.weight0 +
						m1[n] * weight.weight1 +
						m2[n] * weight.weight2 +
						m3[n] * weight.weight3;
			}
			
			playgroundParticles.particleCache.particles[i].position = vertexMatrix.MultiplyPoint3x4(vertices[i%vertices.Length])+GetOverflowOffset(ref playgroundParticles, i, vertices.Length);
		}
	}
	
	// Get vertices from a skinned world object in a Vector3-array
	public static void GetPosition (ref Vector3[] v3, ref Vector3[] norm, ref SkinnedWorldObject particleStateWorldObject) {
		Vector3[] vertices = particleStateWorldObject.mesh.vertices;
		norm = particleStateWorldObject.mesh.normals;
		BoneWeight[] weights = particleStateWorldObject.mesh.boneWeights;
		Matrix4x4[] bindPoses = particleStateWorldObject.mesh.bindposes;
		Matrix4x4[] boneMatrices = new Matrix4x4[particleStateWorldObject.renderer.bones.Length];
		if (v3.Length!=vertices.Length) v3 = new Vector3[vertices.Length];

		int i = 0;
		for (; i<boneMatrices.Length; i++)
			boneMatrices[i] = particleStateWorldObject.renderer.bones[i].localToWorldMatrix * bindPoses[i];
		
		Matrix4x4 vertexMatrix = new Matrix4x4();
		for (i = 0; i<v3.Length; i++) {
			BoneWeight weight = weights[i];
			Matrix4x4 m0 = boneMatrices[weight.boneIndex0];
			Matrix4x4 m1 = boneMatrices[weight.boneIndex1];
			Matrix4x4 m2 = boneMatrices[weight.boneIndex2];
			Matrix4x4 m3 = boneMatrices[weight.boneIndex3];
			
			for(int n=0;n<16;n++){
				vertexMatrix[n] =
					m0[n] * weight.weight0 +
						m1[n] * weight.weight1 +
						m2[n] * weight.weight2 +
						m3[n] * weight.weight3;
			}
			
			v3[i] = vertexMatrix.MultiplyPoint3x4(vertices[(i)%vertices.Length]);
		}
	}
	
	// Get position from Mesh World Object
	public static void GetPosition (ref Vector3[] v3, ref WorldObject particleStateWorldObject) {
		if (particleStateWorldObject.meshFilter.sharedMesh!=particleStateWorldObject.mesh)
			particleStateWorldObject.mesh = particleStateWorldObject.meshFilter.sharedMesh;
		v3 = particleStateWorldObject.mesh.vertices;
	}

	// Get procedural position from Mesh World Object
	public static void GetProceduralPosition (ref Vector3[] v3, ref WorldObject particleStateWorldObject) {
		if (particleStateWorldObject.meshFilter.sharedMesh!=particleStateWorldObject.mesh)
			particleStateWorldObject.mesh = particleStateWorldObject.meshFilter.sharedMesh;
		Vector3[] vertices = particleStateWorldObject.mesh.vertices;
		if (v3.Length!=vertices.Length) v3 = new Vector3[vertices.Length];
		for (int i = 0; i<v3.Length; i++) {
			v3[i] = particleStateWorldObject.transform.TransformPoint(vertices[i%vertices.Length]);
		}
	}
	
	// Get normals from Mesh World Object
	public static void GetNormals (ref Vector3[] v3, ref WorldObject particleStateWorldObject) {
		v3 = particleStateWorldObject.mesh.normals;
	}

	// Returns the offset as a remainder using the particleCount towards maxVal
	public static Vector3 GetOverflowOffset (ref PlaygroundParticlesC playgroundParticles, int currentVal, int maxVal) {
		float iteration = (currentVal/maxVal);
		
		// Trying to use the transform of the current source
		if (playgroundParticles.overflowMode == OVERFLOWMODEC.SourceTransform) {
			
			// State
			if (playgroundParticles.source == SOURCEC.State && playgroundParticles.states[playgroundParticles.activeState].stateTransform) {
				return playgroundParticles.states[playgroundParticles.activeState].stateTransform.TransformPoint(
					playgroundParticles.overflowOffset.x*iteration,
					playgroundParticles.overflowOffset.y*iteration,
					playgroundParticles.overflowOffset.z*iteration
					)-playgroundParticles.states[playgroundParticles.activeState].stateTransform.position;
			} else
				
				// World Object
			if (playgroundParticles.source == SOURCEC.WorldObject && playgroundParticles.worldObject.transform) {
				return playgroundParticles.worldObject.transform.TransformPoint(
					playgroundParticles.overflowOffset.x*iteration,
					playgroundParticles.overflowOffset.y*iteration,
					playgroundParticles.overflowOffset.z*iteration
					)-playgroundParticles.worldObject.transform.position;
			} else
				
				// Skinned World Object
			if (playgroundParticles.source == SOURCEC.SkinnedWorldObject && playgroundParticles.skinnedWorldObject.transform) {
				return playgroundParticles.skinnedWorldObject.transform.TransformPoint(
					playgroundParticles.overflowOffset.x*iteration,
					playgroundParticles.overflowOffset.y*iteration,
					playgroundParticles.overflowOffset.z*iteration
					)-playgroundParticles.skinnedWorldObject.transform.position;
			} else
				
				// Transform
			if (playgroundParticles.source == SOURCEC.Transform && playgroundParticles.sourceTransform) {
				return playgroundParticles.sourceTransform.TransformPoint(
					playgroundParticles.overflowOffset.x*iteration,
					playgroundParticles.overflowOffset.y*iteration,
					playgroundParticles.overflowOffset.z*iteration
					)-playgroundParticles.sourceTransform.position;
			} else
				
				// Paint
			if (playgroundParticles.source == SOURCEC.Paint) {
				Transform paintParent = playgroundParticles.paint.GetParent(currentVal);
				return paintParent.TransformPoint(
					playgroundParticles.overflowOffset.x*iteration,
					playgroundParticles.overflowOffset.y*iteration,
					playgroundParticles.overflowOffset.z*iteration
					)-paintParent.position;
			} else
				
				// Projection
			if (playgroundParticles.source == SOURCEC.Projection) {
				Transform projectionParent = playgroundParticles.projection.GetParent(currentVal);
				return projectionParent.TransformPoint(
					playgroundParticles.overflowOffset.x*iteration,
					playgroundParticles.overflowOffset.y*iteration,
					playgroundParticles.overflowOffset.z*iteration
					)-projectionParent.position;
			}
			
			// Using the transform of the Particle System
		} else if (playgroundParticles.overflowMode == OVERFLOWMODEC.ParticleSystemTransform) {
			return playgroundParticles.particleSystemTransform.TransformPoint(
				playgroundParticles.overflowOffset.x*iteration,
				playgroundParticles.overflowOffset.y*iteration,
				playgroundParticles.overflowOffset.z*iteration
				)-playgroundParticles.particleSystemTransform.position;
			
			// Using the source point
		} else if (playgroundParticles.overflowMode == OVERFLOWMODEC.SourcePoint) {
			
			// State
			if (playgroundParticles.source == SOURCEC.State && playgroundParticles.states[playgroundParticles.activeState].stateTransform) {
				Vector3 statePos = playgroundParticles.states[playgroundParticles.activeState].stateTransform.TransformPoint(playgroundParticles.states[playgroundParticles.activeState].GetPosition(currentVal));
				Vector3 stateNorm = playgroundParticles.states[playgroundParticles.activeState].stateTransform.TransformDirection(playgroundParticles.states[playgroundParticles.activeState].GetNormal(currentVal));
				
				return new Vector3(
					statePos.x+(stateNorm.x*playgroundParticles.overflowOffset.z*iteration),
					statePos.y+(stateNorm.y*playgroundParticles.overflowOffset.z*iteration),
					statePos.z+(stateNorm.z*playgroundParticles.overflowOffset.z*iteration)
					)-statePos;
			} else
				
				// World Object
			if (playgroundParticles.source == SOURCEC.WorldObject && playgroundParticles.worldObject.transform) {
				Vector3 woPos = playgroundParticles.worldObject.vertexPositions[currentVal%playgroundParticles.worldObject.vertexPositions.Length];
				Vector3 woNorm = playgroundParticles.worldObject.transform.TransformDirection(playgroundParticles.worldObject.normals[currentVal%playgroundParticles.worldObject.normals.Length]);
				
				return new Vector3(
					woPos.x+(woNorm.x*playgroundParticles.overflowOffset.z*iteration),
					woPos.y+(woNorm.y*playgroundParticles.overflowOffset.z*iteration),
					woPos.z+(woNorm.z*playgroundParticles.overflowOffset.z*iteration)
					)-woPos;
			} else
				
				// Skinned World Object
			if (playgroundParticles.source == SOURCEC.SkinnedWorldObject && playgroundParticles.skinnedWorldObject.transform) {
				Vector3 swoPos = playgroundParticles.skinnedWorldObject.vertexPositions[currentVal%playgroundParticles.skinnedWorldObject.vertexPositions.Length];
				Vector3 swoNorm = playgroundParticles.skinnedWorldObject.normals[currentVal%playgroundParticles.skinnedWorldObject.normals.Length];
				
				return new Vector3(
					swoPos.x+(swoNorm.x*playgroundParticles.overflowOffset.z*iteration),
					swoPos.y+(swoNorm.y*playgroundParticles.overflowOffset.z*iteration),
					swoPos.z+(swoNorm.z*playgroundParticles.overflowOffset.z*iteration)
					)-swoPos;
			} else
				
				// Transform
			if (playgroundParticles.source == SOURCEC.Transform && playgroundParticles.sourceTransform) {
				return (playgroundParticles.sourceTransform.forward*playgroundParticles.overflowOffset.z*iteration);
			} else
				
				// Paint
			if (playgroundParticles.source == SOURCEC.Paint) {
				Vector3 paintPos = playgroundParticles.paint.GetPosition(currentVal);
				Vector3 paintNorm = playgroundParticles.paint.GetNormal(currentVal);
				
				return new Vector3(
					paintPos.x+(paintNorm.x*playgroundParticles.overflowOffset.z*iteration),
					paintPos.y+(paintNorm.y*playgroundParticles.overflowOffset.z*iteration),
					paintPos.z+(paintNorm.z*playgroundParticles.overflowOffset.z*iteration)
					)-paintPos;
			}
			
			// Projection
			if (playgroundParticles.source == SOURCEC.Projection) {
				Vector3 projectionPos = playgroundParticles.projection.GetPosition(currentVal);
				Vector3 projectionNorm = playgroundParticles.projection.GetNormal(currentVal);
				
				return new Vector3(
					projectionPos.x+(projectionNorm.x*playgroundParticles.overflowOffset.z*iteration),
					projectionPos.y+(projectionNorm.y*playgroundParticles.overflowOffset.z*iteration),
					projectionPos.z+(projectionNorm.z*playgroundParticles.overflowOffset.z*iteration)
					)-projectionPos;
			}
		}
		
		// Return world position if all else fails
		return new Vector3(
			playgroundParticles.overflowOffset.x*iteration,
			playgroundParticles.overflowOffset.y*iteration,
			playgroundParticles.overflowOffset.z*iteration
			);
	}
	
	// Returns the offset as a remainder from a specified transform
	public static Vector3 GetOverflowOffset (ref PlaygroundParticlesC playgroundParticles, int currentVal, int maxVal, Transform overflowTransform) {
		float iteration = (currentVal/maxVal);
		return overflowTransform.TransformPoint(
			playgroundParticles.overflowOffset.x*iteration,
			playgroundParticles.overflowOffset.y*iteration,
			playgroundParticles.overflowOffset.z*iteration
			)-overflowTransform.position;
	}
	
	// Set size for particles
	public static void SetSize (PlaygroundParticlesC playgroundParticles, float size) {
		for (int i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			playgroundParticles.playgroundCache.size[i] = size;
			playgroundParticles.particleCache.particles[i].size = size;
		}
	}
	
	// Set random size for particles within sizeMinimum- and sizeMaximum range 
	public static void SetSizeRandom (PlaygroundParticlesC playgroundParticles, float sizeMinimum, float sizeMaximum) {
		playgroundParticles.playgroundCache.size = PlaygroundC.RandomFloat(playgroundParticles.particleCache.particles.Length, sizeMinimum, sizeMaximum);
		for (int i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			playgroundParticles.particleCache.particles[i].size = playgroundParticles.playgroundCache.size[i];
		}
		playgroundParticles.sizeMin = sizeMinimum;
		playgroundParticles.sizeMax = sizeMaximum;
		playgroundParticles.previousSizeMin = playgroundParticles.sizeMin;
		playgroundParticles.previousSizeMax = playgroundParticles.sizeMax;
	}
	
	// Set random rotation for particles within rotationMinimum- and rotationMaximum range
	public static void SetRotationRandom (PlaygroundParticlesC playgroundParticles, float rotationMinimum, float rotationMaximum) {
		playgroundParticles.playgroundCache.rotationSpeed = PlaygroundC.RandomFloat(playgroundParticles.particleCache.particles.Length, rotationMinimum, rotationMaximum);
		for (int i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			playgroundParticles.particleCache.particles[i].rotation = playgroundParticles.playgroundCache.initialRotation[i];
		}
		playgroundParticles.rotationSpeedMin = rotationMinimum;
		playgroundParticles.rotationSpeedMax = rotationMaximum;
		playgroundParticles.previousRotationSpeedMin = playgroundParticles.rotationSpeedMin;
		playgroundParticles.previousRotationSpeedMax = playgroundParticles.rotationSpeedMax;
	}
	
	// Set random initial rotation for particles within rotationMinimum- and rotationMaximum range
	public static void SetInitialRotationRandom (PlaygroundParticlesC playgroundParticles, float rotationMinimum, float rotationMaximum) {
		playgroundParticles.playgroundCache.initialRotation = PlaygroundC.RandomFloat(playgroundParticles.particleCache.particles.Length, rotationMinimum, rotationMaximum);
		for (int i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			playgroundParticles.particleCache.particles[i].rotation = playgroundParticles.playgroundCache.initialRotation[i];
		}
		playgroundParticles.initialRotationMin = rotationMinimum;
		playgroundParticles.initialRotationMax = rotationMaximum;
		playgroundParticles.previousInitialRotationMin = playgroundParticles.initialRotationMin;
		playgroundParticles.previousInitialRotationMax = playgroundParticles.initialRotationMax;
	}
	
	// Set initial random velocity for particles within velocityMinimum- and velocityMaximum range
	public static void SetVelocityRandom (PlaygroundParticlesC playgroundParticles, Vector3 velocityMinimum, Vector3 velocityMaximum) {
		playgroundParticles.playgroundCache.initialVelocity = PlaygroundC.RandomVector3(playgroundParticles.particleCache.particles.Length, velocityMinimum, velocityMaximum);
		
		playgroundParticles.initialVelocityMin = velocityMinimum;
		playgroundParticles.initialVelocityMax = velocityMaximum;
		playgroundParticles.previousVelocityMin = playgroundParticles.initialVelocityMin;
		playgroundParticles.previousVelocityMax = playgroundParticles.initialVelocityMax;
	}
	
	// Set initial random local velocity for particles within velocityMinimum- and velocityMaximum range
	public static void SetLocalVelocityRandom (PlaygroundParticlesC playgroundParticles, Vector3 velocityMinimum, Vector3 velocityMaximum) {
		playgroundParticles.playgroundCache.initialLocalVelocity = PlaygroundC.RandomVector3(playgroundParticles.particleCache.particles.Length, velocityMinimum, velocityMaximum);
		
		playgroundParticles.initialLocalVelocityMin = velocityMinimum;
		playgroundParticles.initialLocalVelocityMax = velocityMaximum;
		playgroundParticles.previousLocalVelocityMin = playgroundParticles.initialLocalVelocityMin;
		playgroundParticles.previousLocalVelocityMax = playgroundParticles.initialLocalVelocityMax;
	}
	
	// Set material for particle system
	public static void SetMaterial (PlaygroundParticlesC playgroundParticles, Material particleMaterial) {
		playgroundParticles.particleSystemRenderer.sharedMaterial = particleMaterial;
	}
	
	// Set alphas for particles
	public static void SetAlpha (PlaygroundParticlesC playgroundParticles, float alpha) {
		Color pColor;
		for (int i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			pColor = playgroundParticles.particleCache.particles[i].color;
			pColor.a = alpha;
			playgroundParticles.particleCache.particles[i].color = pColor;
		}
	}
	
	// Set particle random particle positions within min- and max range
	public static void Random (PlaygroundParticlesC playgroundParticles, Vector3 min, Vector3 max) {
		for (int p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
			playgroundParticles.particleCache.particles[p].position = new Vector3(
				UnityEngine.Random.Range(min.x, max.x),
				UnityEngine.Random.Range(min.y, max.y),
				UnityEngine.Random.Range(min.z, max.z)
				);
		}
		Update(playgroundParticles);
	}
	
	// Move all particles in direction
	public static void Translate (PlaygroundParticlesC playgroundParticles, Vector3 direction) {
		for (int i = 0; i<playgroundParticles.particleCache.particles.Length; i++)
			playgroundParticles.particleCache.particles[i].position += direction;
	}
	
	// Add new state from state
	public static void Add (PlaygroundParticlesC playgroundParticles, ParticleStateC state) {
		playgroundParticles.states.Add(state);
		state.Initialize();
	}
	
	// Add new state from image
	public static void Add (PlaygroundParticlesC playgroundParticles, Texture2D image, float scale, Vector3 offset, string stateName, Transform stateTransform) {
		playgroundParticles.states.Add(new ParticleStateC());
		playgroundParticles.states[playgroundParticles.states.Count-1].ConstructParticles(image,scale,offset,stateName,stateTransform);
	}
	
	// Add new state from image with depthmap
	public static void Add (PlaygroundParticlesC playgroundParticles, Texture2D image, Texture2D depthmap, float depthmapStrength, float scale, Vector3 offset, string stateName, Transform stateTransform) {
		playgroundParticles.states.Add(new ParticleStateC());
		playgroundParticles.states[playgroundParticles.states.Count-1].ConstructParticles(image,scale,offset,stateName,stateTransform);
		playgroundParticles.states[playgroundParticles.states.Count-1].stateDepthmap = depthmap;
		playgroundParticles.states[playgroundParticles.states.Count-1].stateDepthmapStrength = depthmapStrength;
	}
	
	// Destroy a PlaygroundParticlesC object
	public static void Destroy (PlaygroundParticlesC playgroundParticles) {
		Clear(playgroundParticles);
		MonoBehaviour.DestroyImmediate(playgroundParticles.particleSystemGameObject);
		playgroundParticles = null;
	}

	// Sorts the particles in lifetime
	public static void SetLifetime (PlaygroundParticlesC playgroundParticles, float time) {
		if (playgroundParticles.playgroundCache.targetPosition==null) return;
		playgroundParticles.lifetime = time;
		playgroundParticles.playgroundCache.lifetimeOffset = new float[playgroundParticles.particleCount];
		int currentCount = playgroundParticles.particleCount;
		
		if (playgroundParticles.source!=SOURCEC.Script) {
			switch (playgroundParticles.sorting) {
			case SORTINGC.Scrambled:
				playgroundParticles.playgroundCache.lifetimeOffset = PlaygroundC.RandomFloat(playgroundParticles.particleCount, 0f, playgroundParticles.lifetime);
				break;
			case SORTINGC.ScrambledLinear:
				float slPerc;
				for (int sl = 0; sl<playgroundParticles.particleCount; sl++) {
					if (currentCount!=playgroundParticles.particleCount) return;
					slPerc = (sl*1f)/(playgroundParticles.particleCount*1f);
					playgroundParticles.playgroundCache.lifetimeOffset[sl] = playgroundParticles.lifetime*slPerc;
				}
				PlaygroundC.ShuffleFloat(playgroundParticles.playgroundCache.lifetimeOffset);
				break;
			case SORTINGC.Burst:
				// No action needed for spawning all particles at once
				break;
			case SORTINGC.Reversed:
				float lPerc;
				for (int l = 0; l<playgroundParticles.particleCount; l++) {
					if (currentCount!=playgroundParticles.particleCount) return;
					lPerc = (l*1f)/(playgroundParticles.particleCount*1f);
					playgroundParticles.playgroundCache.lifetimeOffset[l] = playgroundParticles.lifetime*lPerc;
				}
				break;
			case SORTINGC.Linear:
				float rPerc;
				int rInc = 0;
				for (int r = playgroundParticles.particleCount-1; r>=0; r--) {
					if (currentCount!=playgroundParticles.particleCount) return;
					rPerc = (rInc*1f)/(playgroundParticles.particleCount*1f);
					rInc++;
					playgroundParticles.playgroundCache.lifetimeOffset[r] = playgroundParticles.lifetime*rPerc;
				}
				break;
			case SORTINGC.NearestNeighbor:
				playgroundParticles.nearestNeighborOrigin = Mathf.Clamp(playgroundParticles.nearestNeighborOrigin, 0, playgroundParticles.particleCount-1);
				float[] nnDist = new float[playgroundParticles.particleCount];
				float nnHighest = 0;
				int nn = 0;
				for (; nn<playgroundParticles.particleCount; nn++) {
					if (currentCount!=playgroundParticles.particleCount) return;
					nnDist[nn%playgroundParticles.particleCount] = Vector3.Distance(playgroundParticles.playgroundCache.targetPosition[playgroundParticles.nearestNeighborOrigin%playgroundParticles.particleCount], playgroundParticles.playgroundCache.targetPosition[nn%playgroundParticles.particleCount]);
					if (nnDist[nn%playgroundParticles.particleCount]>nnHighest)
						nnHighest = nnDist[nn%playgroundParticles.particleCount];
				}
				if (nnHighest>0) {
					for (nn = 0; nn<playgroundParticles.particleCount; nn++) {
						//if (currentCount!=playgroundParticles.particleCount) return;
						playgroundParticles.playgroundCache.lifetimeOffset[nn%playgroundParticles.particleCount] = Mathf.Lerp(playgroundParticles.lifetime, 0, (nnDist[nn%playgroundParticles.particleCount]/nnHighest));
					}
				} else {
					for (nn = 0; nn<playgroundParticles.particleCount; nn++)
						playgroundParticles.playgroundCache.lifetimeOffset[nn%playgroundParticles.particleCount] = 0;
				}
				break;
			case SORTINGC.NearestNeighborReversed:
				playgroundParticles.nearestNeighborOrigin = Mathf.Clamp(playgroundParticles.nearestNeighborOrigin, 0, playgroundParticles.particleCount-1);
				float[] nnrDist = new float[playgroundParticles.particleCount];
				float nnrHighest = 0;
				int nnr = 0;
				for (; nnr<playgroundParticles.particleCount; nnr++) {
					if (currentCount!=playgroundParticles.particleCount) return;
					nnrDist[nnr%playgroundParticles.particleCount] = Vector3.Distance(playgroundParticles.playgroundCache.targetPosition[playgroundParticles.nearestNeighborOrigin], playgroundParticles.playgroundCache.targetPosition[nnr%playgroundParticles.particleCount]);
					if (nnrDist[nnr%playgroundParticles.particleCount]>nnrHighest)
						nnrHighest = nnrDist[nnr%playgroundParticles.particleCount];
				}
				if (nnrHighest>0) {
					for (nnr = 0; nnr<playgroundParticles.particleCount; nnr++) {
						playgroundParticles.playgroundCache.lifetimeOffset[nnr%playgroundParticles.particleCount] = Mathf.Lerp(0, playgroundParticles.lifetime, (nnrDist[nnr%playgroundParticles.particleCount]/nnrHighest));
					}
				} else {
					for (nnr = 0; nnr<playgroundParticles.particleCount; nnr++)
						playgroundParticles.playgroundCache.lifetimeOffset[nnr%playgroundParticles.particleCount] = 0;
				}
				break;
			case SORTINGC.Custom:
				for (int cs = playgroundParticles.particleCount-1; cs>=0; cs--) {
					playgroundParticles.playgroundCache.lifetimeOffset[cs] = playgroundParticles.lifetime*playgroundParticles.lifetimeSorting.Evaluate(cs*1f/playgroundParticles.particleCount*1f);
				}
				break;
			}
		}
		SetEmissionRate(playgroundParticles);
		SetParticleTimeNow(playgroundParticles);
		playgroundParticles.previousLifetime = time;
	}
	
	// Set emission rate percentage of particle count
	public static void SetEmissionRate (PlaygroundParticlesC playgroundParticles) {
		float rateCount = playgroundParticles.lifetime*playgroundParticles.emissionRate;
		int currentCount = playgroundParticles.particleCount;
		for (int p = 0; p<playgroundParticles.particleCount; p++) {
			if (currentCount!=playgroundParticles.particleCount) return;
			if (playgroundParticles.emissionRate!=0 && playgroundParticles.source!=SOURCEC.Script) {
				if (playgroundParticles.sorting!=SORTINGC.Burst || playgroundParticles.sorting==SORTINGC.NearestNeighbor && playgroundParticles.overflowOffset!=Vector3.zero || playgroundParticles.sorting==SORTINGC.NearestNeighborReversed && playgroundParticles.overflowOffset!=Vector3.zero) {
					playgroundParticles.playgroundCache.emission[p] = (playgroundParticles.playgroundCache.lifetimeOffset[p]>=playgroundParticles.lifetime-rateCount && playgroundParticles.emit);
				} else {
					playgroundParticles.playgroundCache.emission[p] = (playgroundParticles.emit && playgroundParticles.emissionRate>(p/currentCount));
				}
			} else playgroundParticles.playgroundCache.emission[p] = false;
			if (playgroundParticles.playgroundCache.emission[p])
				playgroundParticles.playgroundCache.rebirth[p] = true;
			else if (playgroundParticles.source==SOURCEC.Script)
				playgroundParticles.playgroundCache.rebirth[p] = false;
		}
		playgroundParticles.previousEmissionRate = playgroundParticles.emissionRate;
	}
	
	// Set life and death of particles
	public static void SetParticleTimeNow (PlaygroundParticlesC playgroundParticles) {
		if (playgroundParticles.playgroundCache.lifetimeOffset==null || playgroundParticles.playgroundCache.lifetimeOffset.Length!=playgroundParticles.particleCount) return;
		playgroundParticles.playgroundCache.birth = new float[playgroundParticles.particleCount];
		playgroundParticles.playgroundCache.birthDelay = new float[playgroundParticles.particleCount];
		playgroundParticles.playgroundCache.death = new float[playgroundParticles.particleCount];  
		playgroundParticles.playgroundCache.life = new float[playgroundParticles.particleCount];
		if (playgroundParticles.source!=SOURCEC.Script) {
			float currentTime = PlaygroundC.globalTime+playgroundParticles.lifetimeOffset;
			int currentCount = playgroundParticles.particleCount;
			playgroundParticles.simulationStarted = currentTime;
			int p;	
			if (playgroundParticles.sorting!=SORTINGC.Burst || playgroundParticles.sorting==SORTINGC.NearestNeighbor && playgroundParticles.overflowOffset!=Vector3.zero || playgroundParticles.sorting==SORTINGC.NearestNeighborReversed && playgroundParticles.overflowOffset!=Vector3.zero) {
				for (p = 0; p<playgroundParticles.particleCount; p++) {
					if (currentCount!=playgroundParticles.particleCount) return;
					playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime-(playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeOffset[p]);
					playgroundParticles.playgroundCache.birth[p] = currentTime-playgroundParticles.playgroundCache.life[p];
					playgroundParticles.playgroundCache.death[p] = currentTime+(playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeOffset[p]);
					playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].startLifetime = playgroundParticles.lifetime;
					playgroundParticles.playgroundCache.targetPosition[p] = PlaygroundC.initialTargetPosition;
					playgroundParticles.particleCache.particles[p].position = PlaygroundC.initialTargetPosition;
				}
			} else {
				currentTime = PlaygroundC.globalTime;
				// Force recalculation for burst mode to initiate sequence directly
				for (p = 0; p<playgroundParticles.particleCount; p++) {
					if (currentCount!=playgroundParticles.particleCount) return;
					playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime;
					playgroundParticles.playgroundCache.birth[p] = currentTime-playgroundParticles.lifetime;
					playgroundParticles.playgroundCache.death[p] = currentTime;
					playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].startLifetime = playgroundParticles.lifetime;
					Rebirth(playgroundParticles, p);
				}
			}
		}
	}
	
	// Set life and death of particles after emit has changed
	public static void SetParticleTimeNowWithRestEmission (PlaygroundParticlesC playgroundParticles) {
		float currentTime = PlaygroundC.globalTime+playgroundParticles.lifetimeOffset;
		float emissionDelta = PlaygroundC.globalTime-playgroundParticles.emissionStopped;
		bool applyDelta = false;
		if (emissionDelta<playgroundParticles.lifetime && emissionDelta>0)
			applyDelta = true;
		for (int p = 0; p<playgroundParticles.particleCount; p++) {
			if (!playgroundParticles.playgroundCache.rebirth[p]) {	 
				playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime-(playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeOffset[p]);
				playgroundParticles.playgroundCache.birth[p] = currentTime-playgroundParticles.playgroundCache.life[p];
				playgroundParticles.playgroundCache.death[p] = currentTime+(playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeOffset[p]);
			} else {
				if (applyDelta)
					playgroundParticles.playgroundCache.birthDelay[p] = emissionDelta;
			}
			playgroundParticles.playgroundCache.emission[p] = playgroundParticles.emit;
		}
	}
	
	// Get color from evaluated lifetime color value where time is normalized
	public static Color32 GetColorAtLifetime (PlaygroundParticlesC playgroundParticles, float time) {
		return playgroundParticles.lifetimeColor.Evaluate(time/playgroundParticles.lifetime);
	}
	
	// Color all particles from evaluated lifetime color value where time is normalized
	public static void SetColorAtLifetime (PlaygroundParticlesC playgroundParticles, float time) {
		Color32 c = playgroundParticles.lifetimeColor.Evaluate(time/playgroundParticles.lifetime);
		for (int p = 0; p<playgroundParticles.particleCount; p++)
			playgroundParticles.particleCache.particles[p].color = c;
		playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache.particles, playgroundParticles.particleCache.particles.Length);
	}
	
	// Color all particles from lifetime span with sorting
	public static void SetColorWithLifetimeSorting (PlaygroundParticlesC playgroundParticles) {
		SetLifetime(playgroundParticles, playgroundParticles.lifetime);
		Color32 c;
		for (int p = 0; p<playgroundParticles.particleCount; p++) {
			c = playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime);
			playgroundParticles.particleCache.particles[p].color = c;
		}
		playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache.particles, playgroundParticles.particleCache.particles.Length);
	}
	
	// Set position of emission when using a Transform together with Overflow Offset
	public static void SetTransformPositionWithOffset (PlaygroundParticlesC playgroundParticles) {
		for (int p = 0; p<playgroundParticles.particleCount; p++)
			playgroundParticles.particleCache.particles[p].position = playgroundParticles.sourceTransform.position+GetOverflowOffset(ref playgroundParticles, p, 1);
		playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache.particles, playgroundParticles.particleCache.particles.Length);
	}
	
	// Sets the amount of particles and initiates the necessary arrays
	public static void SetParticleCount (PlaygroundParticlesC playgroundParticles, int amount) {
		if (amount<0) amount = 0;

		// Null Cache
		playgroundParticles.particleCache.particles = null;
		playgroundParticles.playgroundCache.velocity = null;
		playgroundParticles.playgroundCache.targetPosition = null;
		playgroundParticles.playgroundCache.previousTargetPosition = null;
		playgroundParticles.playgroundCache.previousParticlePosition = null;
		playgroundParticles.playgroundCache.scatterPosition = null;
		playgroundParticles.playgroundCache.life = null;
		playgroundParticles.playgroundCache.birth = null;
		playgroundParticles.playgroundCache.birthDelay = null;
		playgroundParticles.playgroundCache.death = null;
		playgroundParticles.playgroundCache.size = null;
		playgroundParticles.playgroundCache.rebirth = null;
		playgroundParticles.playgroundCache.emission = null;
		playgroundParticles.playgroundCache.color = null;
		playgroundParticles.playgroundCache.changedByProperty = null;
		playgroundParticles.playgroundCache.changedByPropertyColor = null;
		playgroundParticles.playgroundCache.changedByPropertyColorLerp = null;
		playgroundParticles.playgroundCache.changedByPropertyColorKeepAlpha = null;
		playgroundParticles.playgroundCache.changedByPropertySize = null;
		playgroundParticles.playgroundCache.changedByPropertyTarget = null;
		playgroundParticles.playgroundCache.changedByPropertyDeath = null;
		playgroundParticles.playgroundCache.propertyTarget = null;
		playgroundParticles.playgroundCache.propertyId = null;
		playgroundParticles.playgroundCache.propertyColorId = null;
		playgroundParticles.playgroundCache.parent = null;
		playgroundParticles.inTransition = false;

		// Rebuild Cache
		playgroundParticles.playgroundCache.rebirth = new bool[amount];
		playgroundParticles.playgroundCache.emission = new bool[amount];
		playgroundParticles.playgroundCache.changedByProperty = new bool[amount];
		playgroundParticles.playgroundCache.changedByPropertyColor = new bool[amount];
		playgroundParticles.playgroundCache.changedByPropertyColorLerp = new bool[amount];
		playgroundParticles.playgroundCache.changedByPropertyColorKeepAlpha = new bool[amount];
		playgroundParticles.playgroundCache.changedByPropertySize = new bool[amount];
		playgroundParticles.playgroundCache.changedByPropertyTarget = new bool[amount];
		playgroundParticles.playgroundCache.changedByPropertyDeath = new bool[amount];
		playgroundParticles.playgroundCache.propertyTarget = new int[amount];
		playgroundParticles.playgroundCache.propertyId = new int[amount];
		playgroundParticles.playgroundCache.propertyColorId = new int[amount];
		playgroundParticles.playgroundCache.parent = new Transform[amount];
		playgroundParticles.playgroundCache.color = new Color32[amount];
		playgroundParticles.playgroundCache.targetPosition = new Vector3[amount];
		playgroundParticles.playgroundCache.previousTargetPosition = new Vector3[amount];
		playgroundParticles.playgroundCache.previousParticlePosition = new Vector3[amount];
		playgroundParticles.playgroundCache.scatterPosition = new Vector3[amount];
		playgroundParticles.playgroundCache.velocity = new Vector3[amount];
		playgroundParticles.particleCount = amount;
		playgroundParticles.previousParticleCount = playgroundParticles.particleCount;

		// Create Particles
		playgroundParticles.particleCache.particles = new ParticleSystem.Particle[amount];
		playgroundParticles.shurikenParticleSystem.Emit(amount);
		playgroundParticles.shurikenParticleSystem.GetParticles(playgroundParticles.particleCache.particles);

		// Set sizes
		SetSizeRandom(playgroundParticles, playgroundParticles.sizeMin, playgroundParticles.sizeMax);
		playgroundParticles.previousSizeMin = playgroundParticles.sizeMin;
		playgroundParticles.previousSizeMax = playgroundParticles.sizeMax;
		
		// Set rotations
		playgroundParticles.playgroundCache.initialRotation = PlaygroundC.RandomFloat(amount, playgroundParticles.initialRotationMin, playgroundParticles.initialRotationMax);
		playgroundParticles.playgroundCache.rotationSpeed = PlaygroundC.RandomFloat(amount, playgroundParticles.rotationSpeedMin, playgroundParticles.rotationSpeedMax);
		playgroundParticles.previousInitialRotationMin = playgroundParticles.initialRotationMin;
		playgroundParticles.previousInitialRotationMax = playgroundParticles.initialRotationMax;
		playgroundParticles.previousRotationSpeedMin = playgroundParticles.rotationSpeedMin;
		playgroundParticles.previousRotationSpeedMax = playgroundParticles.rotationSpeedMax;
	
		// Set velocities
		SetVelocityRandom(playgroundParticles, playgroundParticles.initialVelocityMin, playgroundParticles.initialVelocityMax);
		SetLocalVelocityRandom(playgroundParticles, playgroundParticles.initialLocalVelocityMin, playgroundParticles.initialLocalVelocityMax);
		
		// Garbage Collect
		if (PlaygroundC.reference!=null && PlaygroundC.reference.garbageCollectOnResize)
			GC.Collect();
		
		// Set Lifetime
		SetLifetime(playgroundParticles, playgroundParticles.lifetime);
		
		// Set Emission
		Emission(playgroundParticles, playgroundParticles.emit, false);
		
		// Make sure scatter is available first lifetime cycle
		if (playgroundParticles.applySourceScatter)
			playgroundParticles.RefreshScatter();
		

		// Update source if calculation is off
		if (!playgroundParticles.calculate) {
			if (playgroundParticles.source==SOURCEC.Transform && playgroundParticles.overflowOffset!=Vector3.zero) {
				SetTransformPositionWithOffset(playgroundParticles);
				SetColorWithLifetimeSorting(playgroundParticles);
			} else if (playgroundParticles.source==SOURCEC.WorldObject && playgroundParticles.worldObject!=null) {
				SetPosition(ref playgroundParticles, ref playgroundParticles.worldObject);
				SetColorWithLifetimeSorting(playgroundParticles);
			} else if (playgroundParticles.source==SOURCEC.SkinnedWorldObject && playgroundParticles.skinnedWorldObject!=null) {
				SetPosition(ref playgroundParticles, ref playgroundParticles.skinnedWorldObject);
				SetColorWithLifetimeSorting(playgroundParticles);
			}
		}

		// Misc
		playgroundParticles.isPainting = false;
	}

	// Updates a PlaygroundParticlesC object (called from Playground)
	public static void Update (PlaygroundParticlesC playgroundParticles) {
		
		// Particle count
		if (playgroundParticles.particleCount!=playgroundParticles.previousParticleCount) {
			SetParticleCount(playgroundParticles, playgroundParticles.particleCount);
			playgroundParticles.Start();
		}
		
		// Particle emission
		if (playgroundParticles.emit!=playgroundParticles.previousEmission)
			Emission(playgroundParticles, playgroundParticles.emit, true);
		
		// Particle size
		if (playgroundParticles.sizeMin!=playgroundParticles.previousSizeMin || playgroundParticles.sizeMax!=playgroundParticles.previousSizeMax)
			SetSizeRandom(playgroundParticles, playgroundParticles.sizeMin, playgroundParticles.sizeMax);
		
		// Particle rotation
		if (playgroundParticles.initialRotationMin!=playgroundParticles.previousInitialRotationMin || playgroundParticles.initialRotationMax!=playgroundParticles.previousInitialRotationMax)
			SetInitialRotationRandom(playgroundParticles, playgroundParticles.initialRotationMin, playgroundParticles.initialRotationMax);
		if (playgroundParticles.rotationSpeedMin!=playgroundParticles.previousRotationSpeedMin || playgroundParticles.rotationSpeedMax!=playgroundParticles.previousRotationSpeedMax)
			SetRotationRandom(playgroundParticles, playgroundParticles.rotationSpeedMin, playgroundParticles.rotationSpeedMax);
		
		// Particle velocity
		if (playgroundParticles.applyInitialVelocity)
			if (playgroundParticles.initialVelocityMin!=playgroundParticles.previousVelocityMin || playgroundParticles.initialVelocityMax!=playgroundParticles.previousVelocityMax || playgroundParticles.playgroundCache.initialVelocity==null || playgroundParticles.playgroundCache.initialVelocity.Length!=playgroundParticles.particleCount)
				SetVelocityRandom(playgroundParticles, playgroundParticles.initialVelocityMin, playgroundParticles.initialVelocityMax);
		
		// Particle local velocity
		if (playgroundParticles.applyInitialLocalVelocity)
			if (playgroundParticles.initialLocalVelocityMin!=playgroundParticles.previousLocalVelocityMin || playgroundParticles.initialLocalVelocityMax!=playgroundParticles.previousLocalVelocityMax || playgroundParticles.playgroundCache.initialLocalVelocity==null || playgroundParticles.playgroundCache.initialLocalVelocity.Length!=playgroundParticles.particleCount)
				SetLocalVelocityRandom(playgroundParticles, playgroundParticles.initialLocalVelocityMin, playgroundParticles.initialLocalVelocityMax);
		
		// Particle life
		if (playgroundParticles.previousLifetime!=playgroundParticles.lifetime)
			SetLifetime(playgroundParticles, playgroundParticles.lifetime);
		
		// Particle emission rate
		if (playgroundParticles.previousEmissionRate!=playgroundParticles.emissionRate)
			SetEmissionRate(playgroundParticles);
		
		// Particle state change transition
		if (playgroundParticles.source==SOURCEC.State && playgroundParticles.activeState!=playgroundParticles.previousActiveState) {
			if (playgroundParticles.states[playgroundParticles.activeState].positionLength>playgroundParticles.particleCount)
				SetParticleCount(playgroundParticles, playgroundParticles.states[playgroundParticles.activeState].positionLength);
			playgroundParticles.InitTransition(playgroundParticles);
			playgroundParticles.previousActiveState = playgroundParticles.activeState;
		}
		
		// Particle calculation
		if (PlaygroundC.reference.calculate && playgroundParticles.calculate && !playgroundParticles.inTransition)
			Calculate(ref playgroundParticles);
		else playgroundParticles.cameFromNonCalculatedFrame = true;
		
		// Assign all particles into the particle system
		if (!playgroundParticles.inTransition && playgroundParticles.particleCache.particles.Length>0)
			playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache.particles, playgroundParticles.particleCache.particles.Length);
		
		// Make sure that variables are reset till next frame
		playgroundParticles.isPainting = false;		
	}
	
	// Initial target position
	public static void SetInitialTargetPosition (PlaygroundParticlesC playgroundParticles, Vector3 position) {
		for (int p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
			playgroundParticles.playgroundCache.previousTargetPosition[p] = position;
			playgroundParticles.playgroundCache.targetPosition[p] = position;
			playgroundParticles.particleCache.particles[p].position = position;
		}
		playgroundParticles.particleSystem.SetParticles(playgroundParticles.particleCache.particles, playgroundParticles.particleCache.particles.Length);
	}
	
	// Transition event on activeState change
	public void InitTransition (PlaygroundParticlesC playgroundParticles) {
		StartCoroutine(playgroundParticles.Transition(playgroundParticles));
	}
	IEnumerator Transition (PlaygroundParticlesC playgroundParticles) {
		
		playgroundParticles.inTransition = true;
		
		int p;
		float t = .0f;
		float timeTransitionStarted = Time.realtimeSinceStartup;
		float timeTransitionFinishes = timeTransitionStarted+playgroundParticles.transitionTime;
		int thisState = playgroundParticles.activeState;
		Vector3[] currentPosition = new Vector3[0];
		Color32[] currentColor = new Color32[0];
		Color32 targetColor = new Color32();
		Vector3 calculatedoverflowOffset = new Vector3();
		
		if (playgroundParticles.states[playgroundParticles.activeState].stateMesh!=null && playgroundParticles.states[playgroundParticles.activeState].stateTexture==null)
			targetColor = GetColorAtLifetime(playgroundParticles, 0);
		
		switch (playgroundParticles.transition) {
			
		case TRANSITIONC.None:
			// Set all particles directly
			for (p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
				
				// overflow offset
				if (playgroundParticles.overflowOffset!=Vector3.zero)
					calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, p, playgroundParticles.states[playgroundParticles.activeState].positionLength);
				
				if (playgroundParticles.states[playgroundParticles.activeState].stateTransform!=null) 
					playgroundParticles.playgroundCache.targetPosition[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.states[playgroundParticles.activeState].GetParentedPosition(p%playgroundParticles.particleCache.particles.Length)+calculatedoverflowOffset;
				else
					playgroundParticles.playgroundCache.targetPosition[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.states[playgroundParticles.activeState].GetPosition(p%playgroundParticles.particleCache.particles.Length)+calculatedoverflowOffset;
				
				if (playgroundParticles.states[playgroundParticles.activeState].stateMesh!=null && playgroundParticles.states[playgroundParticles.activeState].stateTexture==null)
					playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].color = targetColor;
				else
					playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].color = playgroundParticles.states[playgroundParticles.activeState%playgroundParticles.states.Count].GetColor(p%playgroundParticles.particleCache.particles.Length);
				playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].position = playgroundParticles.playgroundCache.targetPosition[p%playgroundParticles.particleCache.particles.Length];
			}
			break;
			
		case TRANSITIONC.Lerp:
			currentPosition = new Vector3[playgroundParticles.particleCache.particles.Length];
			currentColor = new Color32[playgroundParticles.particleCache.particles.Length];
			
			// Set target, current positions and colors
			for (p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
				
				// Overflow offset
				if (playgroundParticles.overflowOffset!=Vector3.zero)
					calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, p, playgroundParticles.states[playgroundParticles.activeState].positionLength);
				
				// Set current position and color, target position (with overflowOffset)
				currentPosition[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].position;
				
				if (playgroundParticles.states[playgroundParticles.activeState].stateTransform!=null) 
					playgroundParticles.playgroundCache.targetPosition[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.states[playgroundParticles.activeState].GetParentedPosition(p%playgroundParticles.particleCache.particles.Length)+calculatedoverflowOffset;
				else
					playgroundParticles.playgroundCache.targetPosition[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.states[playgroundParticles.activeState].GetPosition(p%playgroundParticles.particleCache.particles.Length)+calculatedoverflowOffset;
				
				currentColor[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].color;
			}
			
			// Lerp towards target
			timeTransitionStarted = Time.realtimeSinceStartup;
			while (timeTransitionStarted+t<timeTransitionFinishes && playgroundParticles.activeState==thisState && playgroundParticles.inTransition) {
				
				// Update time
				t = (Time.realtimeSinceStartup-timeTransitionStarted);
				
				// Iterate particles
				for (p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
					
					// Position over time
					playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].position = Vector3.Lerp(
						currentPosition[p%playgroundParticles.particleCache.particles.Length],
						playgroundParticles.playgroundCache.targetPosition[p%playgroundParticles.particleCache.particles.Length],
						t/playgroundParticles.transitionTime
						);
					
					// Color over time
					if (playgroundParticles.states[playgroundParticles.activeState].stateTexture!=null)
						targetColor = playgroundParticles.states[playgroundParticles.activeState%playgroundParticles.states.Count].GetColor(p%playgroundParticles.particleCache.particles.Length);
					
					playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].color = Color.Lerp(
						currentColor[p%playgroundParticles.particleCache.particles.Length],
						targetColor,
						t/playgroundParticles.transitionTime
						);
					
				}
				
				// Assign the particles back
				playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache.particles, playgroundParticles.particleCache.particles.Length);
				
				yield return null;
			}
			break;
			
		case TRANSITIONC.Fade: case TRANSITIONC.Fade2:
			
		bool isFade2 = (playgroundParticles.transition==TRANSITIONC.Fade2);
			
			// Set colors (and positions if Fade2)
			currentColor = new Color32[playgroundParticles.particleCache.particles.Length];
			if (isFade2)
				currentPosition = new Vector3[playgroundParticles.particleCache.particles.Length];
			for (p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
				currentColor[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].color;
				if (isFade2) {
					currentPosition[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].position;
					
					if (playgroundParticles.overflowOffset!=Vector3.zero)
						calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, p, playgroundParticles.states[playgroundParticles.activeState].positionLength);
					if (playgroundParticles.states[playgroundParticles.activeState].stateTransform!=null) 
						playgroundParticles.playgroundCache.targetPosition[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.states[playgroundParticles.activeState].GetParentedPosition(p%playgroundParticles.particleCache.particles.Length)+calculatedoverflowOffset;
					else
						playgroundParticles.playgroundCache.targetPosition[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.states[playgroundParticles.activeState].GetPosition(p%playgroundParticles.particleCache.particles.Length)+calculatedoverflowOffset;
				}
			}
			
			Color transitionColor;
			float transitionAlpha;
			bool fadeOut = true;
			bool setPositions = true;
			
			timeTransitionStarted = Time.realtimeSinceStartup;
			
			while (timeTransitionStarted+t<timeTransitionFinishes && playgroundParticles.activeState==thisState && playgroundParticles.inTransition) {
				
				// Update time
				t = (Time.realtimeSinceStartup-timeTransitionStarted);
				
				for (p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
					
					// Get color and set alpha
					if (fadeOut) {
						transitionColor = currentColor[p%playgroundParticles.particleCache.particles.Length];
						transitionAlpha = Mathf.Lerp(transitionColor.a, 0f, t/(playgroundParticles.transitionTime*.5f));
						if (t/(playgroundParticles.transitionTime*.5f)>=1f) {
							transitionAlpha = 0;
							fadeOut=false;
						}
					} else {
						if (playgroundParticles.states[playgroundParticles.activeState].stateMesh==null && playgroundParticles.states[playgroundParticles.activeState].stateTexture!=null)
							transitionColor = playgroundParticles.states[playgroundParticles.activeState%playgroundParticles.states.Count].GetColor(p%playgroundParticles.particleCache.particles.Length);
						else
							transitionColor = targetColor;
						transitionAlpha = Mathf.Lerp(0f, transitionColor.a, -1f+(t/(playgroundParticles.transitionTime*.5f)));
					}
					transitionColor.a = transitionAlpha;
					
					// Assign color to the particle
					playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].color = transitionColor;
					
					// Position if Transition is Fade2
					if (isFade2)
						playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].position = Vector3.Lerp(
							currentPosition[p%playgroundParticles.particleCache.particles.Length],
							playgroundParticles.playgroundCache.targetPosition[p%playgroundParticles.particleCache.particles.Length],
							t/playgroundParticles.transitionTime
							);
				}
				
				if (!fadeOut && setPositions) {
					setPositions = false;
					SetPosition(playgroundParticles, thisState, false);
				}
				
				// Assign the particles back
				playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache.particles, playgroundParticles.particleCache.particles.Length);
				
				yield return null;
			}
			
			break;
		}
		
		// No longer in transition
		if (playgroundParticles.activeState==thisState) {
			SetParticleTimeNow(playgroundParticles);
			playgroundParticles.inTransition = false;
		}
	}

	// Set emission of PlaygroundParticlesC object
	public static void Emission (PlaygroundParticlesC playgroundParticles, bool emission, bool callRestEmission) {
		playgroundParticles.previousEmission = emission;
		if (emission) {
			for (int p = 0; p<playgroundParticles.playgroundCache.rebirth.Length; p++)
				playgroundParticles.playgroundCache.rebirth[p] = true;
			if (callRestEmission)
				SetParticleTimeNowWithRestEmission(playgroundParticles);
		}
	}
	
	// Returns the angle between a and b with normal direction
	public static float SignedAngle (Vector3 a, Vector3 b, Vector3 n) {
		return (Vector3.Angle(a, b)*Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b))));
	}
	
	// Returns a random value between negative- and positive Vector3
	public static Vector3 RandomVector3 (Vector3 v1, Vector3 v2) {
		return new Vector3(UnityEngine.Random.Range(v1.x,v2.x), UnityEngine.Random.Range(v1.y,v2.y), UnityEngine.Random.Range(v1.z,v2.z));
	}

	// Run all calculations for this PlaygroundParticlesC object
	public static void Calculate (ref PlaygroundParticlesC playgroundParticles) {
		
		if (!playgroundParticles.enabled) return;

		float distance = 0f;
		int prevP;
		int spreadP;
		int prevSpreadP;
		Vector3 deltaVelocity;
		bool localSpace = (playgroundParticles.shurikenParticleSystem.simulationSpace == ParticleSystemSimulationSpace.Local);
		RaycastHit hitInfo;
		Ray ray;
		SOURCEC currentSource = playgroundParticles.source;
		int currentState = playgroundParticles.activeState;
		Color lifetimeColor;
		Vector3 calculatedoverflowOffset = new Vector3();
		int m;
		float evaluatedLife;
		float t = (PlaygroundC.globalDeltaTime*playgroundParticles.particleTimescale)*(playgroundParticles.updateRate*1f);
		
		// Get data from source
		if (playgroundParticles.emit) {
			switch (playgroundParticles.source) {
			case SOURCEC.State:
				if (playgroundParticles.states.Count==0)
					return;
				break;
			case SOURCEC.Transform:
				if (playgroundParticles.sourceTransform==null)
					return;
				break;
			case SOURCEC.WorldObject:
				// Handle vertex data in active World Object
				if (playgroundParticles.worldObject.gameObject!=null && playgroundParticles.worldObject.mesh!=null) {
					if (playgroundParticles.worldObject.gameObject.GetInstanceID()!=playgroundParticles.worldObject.cachedId) {
						NewWorldObject(playgroundParticles, playgroundParticles.worldObject.gameObject.transform);
					}
					if (playgroundParticles.previousWorldObjectUpdateVertices!=playgroundParticles.worldObjectUpdateVertices) {
						playgroundParticles.worldObject.Initialize();
						playgroundParticles.previousWorldObjectUpdateVertices = playgroundParticles.worldObjectUpdateVertices;
					}
					if (playgroundParticles.worldObjectUpdateVertices)
						GetProceduralPosition(ref playgroundParticles.worldObject.vertexPositions, ref playgroundParticles.worldObject);
					if (playgroundParticles.worldObjectUpdateNormals)
						GetNormals(ref playgroundParticles.worldObject.normals, ref playgroundParticles.worldObject);
				} else return;
				break;
			case SOURCEC.SkinnedWorldObject:
				// Handle vertex data in active Skinned World Object
				if (playgroundParticles.skinnedWorldObject.gameObject!=null && playgroundParticles.skinnedWorldObject.mesh!=null) {
					if (playgroundParticles.skinnedWorldObject.gameObject.GetInstanceID()!=playgroundParticles.skinnedWorldObject.cachedId)
						NewSkinnedWorldObject(playgroundParticles, playgroundParticles.skinnedWorldObject.gameObject.transform);
					if (Time.frameCount%PlaygroundC.skinnedUpdateRate==0) {
						GetPosition(ref playgroundParticles.skinnedWorldObject.vertexPositions, ref playgroundParticles.skinnedWorldObject.normals, ref playgroundParticles.skinnedWorldObject);
					}
				} else return;
				break;
			case SOURCEC.Paint:
				if (playgroundParticles.paint==null)
					return;
				break;
			case SOURCEC.Projection:
				if (playgroundParticles.projection==null)
					return;
				break;
			}
		}
		
		// Prepare the infinite collision planes
		if (playgroundParticles.collision && playgroundParticles.collisionRadius>0 && playgroundParticles.colliders.Count>0) {
			for (m = 0; m<playgroundParticles.colliders.Count; m++) {
				playgroundParticles.colliders[m].UpdatePlane(playgroundParticles.calculateDeltaMovement);
			}
		}
		
		if (playgroundParticles.cameFromNonCalculatedFrame) {
			SetLifetime(playgroundParticles, playgroundParticles.lifetime);
			playgroundParticles.cameFromNonCalculatedFrame = false;
		}
		
		// Calculate each particle
		for (int p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
			if (currentSource!=playgroundParticles.source || playgroundParticles.particleCache.particles.Length!=playgroundParticles.particleCount) return;
			
			// Zero out Shuriken velocity (for the applied stretching)
			playgroundParticles.particleCache.particles[p].velocity = Vector3.zero;
			
			// If this particle is set to rebirth
			if (playgroundParticles.playgroundCache.rebirth[p]) {
				
				// Calculate lifetime
				evaluatedLife = (PlaygroundC.globalTime-playgroundParticles.playgroundCache.birth[p])/playgroundParticles.lifetime;
				
				// Previous particle
				prevP = p>0?p-1:playgroundParticles.particleCache.particles.Length-1;
				
				// Lifetime
				if (playgroundParticles.playgroundCache.life[p]<playgroundParticles.lifetime) {
					playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime*evaluatedLife;
					playgroundParticles.particleCache.particles[p].lifetime = Mathf.Clamp(playgroundParticles.lifetime - playgroundParticles.playgroundCache.life[p], .1f, playgroundParticles.lifetime);
				} else {

					// Loop exceeded
					if (!playgroundParticles.loop && PlaygroundC.globalTime>playgroundParticles.simulationStarted+playgroundParticles.lifetime-.01f) {
						playgroundParticles.loopExceeded = true;
						
						if (playgroundParticles.disableOnDone && playgroundParticles.loopExceededOnParticle==p && evaluatedLife>2)
							playgroundParticles.particleSystemGameObject.SetActive(false);
						if (playgroundParticles.loopExceededOnParticle==-1)
							playgroundParticles.loopExceededOnParticle = p;
					}
					
					// New cycle begins
					if (PlaygroundC.globalTime>=playgroundParticles.playgroundCache.birth[p]+playgroundParticles.playgroundCache.birthDelay[p] && !playgroundParticles.loopExceeded && playgroundParticles.source!=SOURCEC.Script) {
						if (!playgroundParticles.playgroundCache.changedByPropertyDeath[p] || playgroundParticles.playgroundCache.changedByPropertyDeath[p] && PlaygroundC.globalTime>playgroundParticles.playgroundCache.death[p])
							Rebirth(playgroundParticles, p);
						else playgroundParticles.particleCache.particles[p].position = PlaygroundC.initialTargetPosition;
						
					} else playgroundParticles.particleCache.particles[p].position = PlaygroundC.initialTargetPosition;
					
				}
				
				// Particle is alive
				if (playgroundParticles.playgroundCache.life[p]<=PlaygroundC.globalTime+playgroundParticles.lifetime) {
					
					// Lifetime size
					if (playgroundParticles.lifetime>0) {
						if (!playgroundParticles.playgroundCache.changedByPropertySize[p])
							playgroundParticles.particleCache.particles[p].size = playgroundParticles.playgroundCache.size[p]*playgroundParticles.lifetimeSize.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime)*playgroundParticles.scale;
					} else playgroundParticles.particleCache.particles[p].size = playgroundParticles.playgroundCache.size[p]*playgroundParticles.scale;
					
					// Rotation
					if (!playgroundParticles.rotateTowardsDirection)
						playgroundParticles.particleCache.particles[p].rotation += playgroundParticles.playgroundCache.rotationSpeed[p]*t;
					else {
						playgroundParticles.particleCache.particles[p].rotation = playgroundParticles.playgroundCache.initialRotation[p]+SignedAngle(
							Vector3.up, 
							playgroundParticles.playgroundCache.velocity[p],
							playgroundParticles.rotationNormal
						);
					}
					
					// Source for particle emission
					switch (playgroundParticles.source) {
					case SOURCEC.Script:
						
						// Set scripted color
						if (playgroundParticles.colorSource==COLORSOURCEC.Source)
							playgroundParticles.particleCache.particles[p].color = playgroundParticles.playgroundCache.color[p];
						
						break;
					case SOURCEC.State:
						
						// Calculate State
						if (playgroundParticles.activeState!=currentState)
							return;
						if (playgroundParticles.states.Count>0 && playgroundParticles.states[playgroundParticles.activeState]!=null && playgroundParticles.states[playgroundParticles.activeState].initialized) {
							
							// Previous target position (for delta movement, available when having a transform assigned)
							if (playgroundParticles.states[playgroundParticles.activeState].stateTransform) {
								if (playgroundParticles.overflowOffset!=Vector3.zero)
									calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, prevP, playgroundParticles.states[playgroundParticles.activeState].positionLength);
								playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.states[playgroundParticles.activeState].GetParentedPosition(prevP)+calculatedoverflowOffset;
							} else {
								playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.playgroundCache.targetPosition[prevP];
							}
							
							// Overflow offset this particle
							if (playgroundParticles.overflowOffset!=Vector3.zero)
								calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, p, playgroundParticles.states[playgroundParticles.activeState].positionLength);
							
							// Position and color
							if (playgroundParticles.states[playgroundParticles.activeState].stateTransform!=null) 
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.states[playgroundParticles.activeState].GetParentedPosition(p)+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.states[playgroundParticles.activeState].stateTransform.position);		
							else
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.states[playgroundParticles.activeState].GetPosition(p)+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.particleSystemTransform.position);
							
							
							if (playgroundParticles.colorSource==COLORSOURCEC.Source) 
								playgroundParticles.particleCache.particles[p].color = playgroundParticles.states[playgroundParticles.activeState].GetColor(p);
							
							// Transform direction velocity
							if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.playgroundCache.life[p]==0 && playgroundParticles.states[playgroundParticles.activeState].stateTransform!=null && !playgroundParticles.onlySourcePositioning) {
								playgroundParticles.playgroundCache.initialLocalVelocity[p] = !localSpace?
									playgroundParticles.states[playgroundParticles.activeState].stateTransform.TransformDirection(
										new Vector3(UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
									        UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
									        UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
									        )
										)
										:
										new Vector3(UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
										        UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
										        UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
										        );
								playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.playgroundCache.initialLocalVelocity[p];
							}
							
							// Set target positions accordingly to local simulation space
							if (localSpace) {
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.targetPosition[p]);
								playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.previousTargetPosition[p]);
							}
							
						} else {
							playgroundParticles.playgroundCache.previousTargetPosition[p] = PlaygroundC.initialTargetPosition;
							playgroundParticles.playgroundCache.targetPosition[p] = PlaygroundC.initialTargetPosition;
						}
						break;
					case SOURCEC.Transform:
						
						// Calculate Transform
						if (playgroundParticles.sourceTransform!=null) {
							
							// Overflow offset previous particle
							if (playgroundParticles.overflowOffset!=Vector3.zero)
								calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, prevP, 1);
							
							// Previous target position (for delta movement)
							playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.sourceTransform.position+calculatedoverflowOffset;
							
							// Overflow offset this particle
							if (playgroundParticles.overflowOffset!=Vector3.zero)
								calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, p, 1);
							
							// Position on transform
							if (playgroundParticles.overflowOffset!=Vector3.zero)
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.sourceTransform.position+GetOverflowOffset(ref playgroundParticles, p, 1)+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.sourceTransform.position);
							else
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.sourceTransform.position+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.sourceTransform.position);
							
							// Normal direction velocity
							if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.playgroundCache.life[p]==0 && !playgroundParticles.onlySourcePositioning) {
								playgroundParticles.playgroundCache.initialLocalVelocity[p] = !localSpace?
									playgroundParticles.sourceTransform.TransformDirection(
										new Vector3(UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
									        UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
									        UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
									        )
										)
										:
										new Vector3(UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
										        UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
										        UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
										        );
								playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.playgroundCache.initialLocalVelocity[p];
								
								// Give this spawning particle its velocity shape
								if (playgroundParticles.applyInitialVelocityShape)
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((p*1f)/(playgroundParticles.particleCount*1f)));
							}
							
							// Set target positions accordingly to local simulation space
							if (localSpace) {
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.sourceTransform.InverseTransformPoint(playgroundParticles.playgroundCache.targetPosition[p]);
								playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.sourceTransform.InverseTransformPoint(playgroundParticles.playgroundCache.previousTargetPosition[p]);
							}
						}
						break;
					case SOURCEC.WorldObject:
						
						// Calculate World Object
						if (playgroundParticles.worldObject.gameObject!=null) {

							// Overflow offset previous particle
							if (playgroundParticles.overflowOffset!=Vector3.zero)
								calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, prevP, playgroundParticles.worldObject.vertexPositions.Length);
							
							// Previous target position (for delta movement)
							if (!playgroundParticles.worldObjectUpdateVertices)
								playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.worldObject.transform.TransformPoint(playgroundParticles.worldObject.vertexPositions[prevP%playgroundParticles.worldObject.vertexPositions.Length])+calculatedoverflowOffset;
							else
								playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.worldObject.vertexPositions[prevP%playgroundParticles.worldObject.vertexPositions.Length]+calculatedoverflowOffset;

							// Overflow offset this particle
							if (playgroundParticles.overflowOffset!=Vector3.zero)
								calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, p, playgroundParticles.worldObject.vertexPositions.Length);
							
							// Position towards vertices
							if (!playgroundParticles.worldObjectUpdateVertices)	
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.worldObject.transform.TransformPoint(playgroundParticles.worldObject.vertexPositions[p%playgroundParticles.worldObject.vertexPositions.Length])+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.worldObject.transform.position);
							else
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.worldObject.vertexPositions[p%playgroundParticles.worldObject.vertexPositions.Length]+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.worldObject.transform.position);

							// Normal direction velocity
							if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.playgroundCache.initialLocalVelocity[p]!=Vector3.zero && playgroundParticles.playgroundCache.life[p]==0 && !playgroundParticles.onlySourcePositioning) {
								playgroundParticles.playgroundCache.initialLocalVelocity[p] = !localSpace?
									playgroundParticles.worldObject.transform.TransformDirection(
										new Vector3(playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length].x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
									        playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length].y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
									        playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length].z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
									        )
										)
										:
										new Vector3(playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length].x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
										        playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length].y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
										        playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length].z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
										        );
								playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.playgroundCache.initialLocalVelocity[p];
								
								// Give this spawning particle its velocity shape
								if (playgroundParticles.applyInitialVelocityShape)
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((p*1f)/(playgroundParticles.particleCount*1f)));
							}
							
							// Set target positions accordingly to local simulation space
							if (localSpace) {
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.targetPosition[p]);
								playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.previousTargetPosition[p]);
							}
						}
						break;
					case SOURCEC.SkinnedWorldObject:
						
						// Calculate Skinned World Object
						if (playgroundParticles.skinnedWorldObject.gameObject!=null) {
							
							// Source target spread
							spreadP = Mathf.RoundToInt(p*playgroundParticles.sourceDownResolution)%playgroundParticles.skinnedWorldObject.vertexPositions.Length;
							prevSpreadP = Mathf.RoundToInt(prevP*playgroundParticles.sourceDownResolution)%playgroundParticles.skinnedWorldObject.vertexPositions.Length;
							
							// Overflow offset previous particle
							if (playgroundParticles.overflowOffset!=Vector3.zero)
								calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, prevP, playgroundParticles.sourceDownResolution<=1?playgroundParticles.skinnedWorldObject.vertexPositions.Length:playgroundParticles.skinnedWorldObject.vertexPositions.Length/playgroundParticles.sourceDownResolution);
							
							// Previous target position (for delta movement)
							playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.skinnedWorldObject.vertexPositions[prevSpreadP]+calculatedoverflowOffset;
							
							// Overflow offset this particle
							if (playgroundParticles.overflowOffset!=Vector3.zero)
								calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, p, playgroundParticles.sourceDownResolution<=1?playgroundParticles.skinnedWorldObject.vertexPositions.Length:playgroundParticles.skinnedWorldObject.vertexPositions.Length/playgroundParticles.sourceDownResolution);
							
							// Position towards vertices
							playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.skinnedWorldObject.vertexPositions[spreadP]+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.skinnedWorldObject.transform.position);
							
							// Normal direction velocity
							if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.playgroundCache.initialLocalVelocity[p]!=Vector3.zero && playgroundParticles.playgroundCache.life[p]==0 && !playgroundParticles.onlySourcePositioning) {
								playgroundParticles.playgroundCache.initialLocalVelocity[p] = !localSpace?
									playgroundParticles.skinnedWorldObject.transform.TransformDirection(
										new Vector3(playgroundParticles.skinnedWorldObject.normals[spreadP%playgroundParticles.skinnedWorldObject.normals.Length].x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
									        playgroundParticles.skinnedWorldObject.normals[spreadP%playgroundParticles.skinnedWorldObject.normals.Length].y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
									        playgroundParticles.skinnedWorldObject.normals[spreadP%playgroundParticles.skinnedWorldObject.normals.Length].z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
									        )
										)
										:
										new Vector3(playgroundParticles.skinnedWorldObject.normals[spreadP%playgroundParticles.skinnedWorldObject.normals.Length].x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
										        playgroundParticles.skinnedWorldObject.normals[spreadP%playgroundParticles.skinnedWorldObject.normals.Length].y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
										        playgroundParticles.skinnedWorldObject.normals[spreadP%playgroundParticles.skinnedWorldObject.normals.Length].z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
										        );
								playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.playgroundCache.initialLocalVelocity[p];
								
								// Give this spawning particle its velocity shape
								if (playgroundParticles.applyInitialVelocityShape)
									playgroundParticles.playgroundCache.velocity[spreadP] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[spreadP], playgroundParticles.initialVelocityShape.Evaluate((spreadP*1f)/(playgroundParticles.particleCount*1f)));
							}
							
							// Set target positions accordingly to local simulation space
							if (localSpace) {
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.targetPosition[p]);
								playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.previousTargetPosition[p]);
							}
						}
						break;
					case SOURCEC.Paint:
						
						// Calculate Paint
						if (playgroundParticles.paint!=null && playgroundParticles.paint.positionLength>0) {
							
							// Update current paint position regarding its transform parent
							playgroundParticles.paint.Update(p);
							
							// Overflow offset previous particle
							if (playgroundParticles.overflowOffset!=Vector3.zero)
								calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, prevP, playgroundParticles.paint.positionLength);
							
							// Previous target position (for delta movement)
							playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.paint.GetPosition(prevP)+calculatedoverflowOffset;
							
							// Overflow offset this particle
							if (playgroundParticles.overflowOffset!=Vector3.zero)
								calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, p, playgroundParticles.paint.positionLength);
							
							// Position and color
							playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.paint.GetPosition(p)+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.paint.GetParent(p).TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.paint.GetParent(p).position);
							
							if (playgroundParticles.colorSource==COLORSOURCEC.Source && !playgroundParticles.playgroundCache.changedByPropertyColor[p] && !playgroundParticles.playgroundCache.changedByPropertyColorLerp[p]) 
								playgroundParticles.particleCache.particles[p].color = playgroundParticles.paint.GetColor(p);
							
							// Normal direction velocity
							if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.playgroundCache.initialLocalVelocity[p]!=Vector3.zero && playgroundParticles.playgroundCache.life[p]==0 && !playgroundParticles.onlySourcePositioning) {
								Vector3 normal = playgroundParticles.paint.GetNormal(p);
								playgroundParticles.playgroundCache.initialLocalVelocity[p] = !localSpace?
									playgroundParticles.paint.GetParent(p).TransformDirection(
										new Vector3(normal.x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
									        normal.y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
									        normal.z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
									        )
										)
										:
										new Vector3(normal.x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
										        normal.y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
										        normal.z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
										        );
								playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.playgroundCache.initialLocalVelocity[p];
								
								// Give this spawning particle its velocity shape
								if (playgroundParticles.applyInitialVelocityShape)
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((p*1f)/(playgroundParticles.particleCount*1f)));
								
							}
							
							// Set target positions accordingly to local simulation space
							if (localSpace) {
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.targetPosition[p]);
								playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.previousTargetPosition[p]);
							}
						} else {
							playgroundParticles.playgroundCache.previousTargetPosition[p] = PlaygroundC.initialTargetPosition;
							playgroundParticles.playgroundCache.targetPosition[p] = PlaygroundC.initialTargetPosition;
						}
						break;
					case SOURCEC.Projection:
						if (playgroundParticles.projection.initialized && playgroundParticles.projection.positionLength>0) {
							
							// Update current projected position regarding its texture and transform
							if (playgroundParticles.projection.liveUpdate)
								playgroundParticles.projection.Update(p);
							
							if (playgroundParticles.projection.GetParent(p)!=null) {
								
								// Previous target position and overflow
								if (playgroundParticles.projection.GetParent(prevP)) {
									if (playgroundParticles.overflowOffset!=Vector3.zero) {
										calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, prevP, playgroundParticles.projection.positionLength);
									}
									
									// Previous target position
									playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.projection.GetPosition(prevP)+calculatedoverflowOffset;
								}
								
								// Overflow offset this particle
								if (playgroundParticles.overflowOffset!=Vector3.zero)
									calculatedoverflowOffset = GetOverflowOffset(ref playgroundParticles, p, playgroundParticles.projection.positionLength);
								
								// Position and color
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.projection.GetPosition(p)+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.projection.GetParent(p).TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.projection.GetParent(p).position);
								
								if (playgroundParticles.colorSource==COLORSOURCEC.Source && !playgroundParticles.playgroundCache.changedByPropertyColor[p] && !playgroundParticles.playgroundCache.changedByPropertyColorLerp[p]) 
									playgroundParticles.particleCache.particles[p].color = playgroundParticles.projection.GetColor(p);
								
								// Normal direction velocity
								if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.playgroundCache.initialLocalVelocity[p]!=Vector3.zero && playgroundParticles.playgroundCache.life[p]==0 && !playgroundParticles.onlySourcePositioning) {
									Vector3 projNormal = playgroundParticles.projection.GetNormal(p);
									playgroundParticles.playgroundCache.initialLocalVelocity[p] = !localSpace?
										playgroundParticles.projection.GetParent(p).TransformDirection(
											new Vector3(projNormal.x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
										        projNormal.y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
										        projNormal.z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
										        )
											)
											:
											new Vector3(projNormal.x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
											        projNormal.y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
											        projNormal.z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
											        );
									playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.playgroundCache.initialLocalVelocity[p];
									
									// Give this spawning particle its velocity shape
									if (playgroundParticles.applyInitialVelocityShape)
										playgroundParticles.playgroundCache.velocity[p] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((p*1f)/(playgroundParticles.particleCount*1f)));
								}
								
								// Set target positions accordingly to local simulation space
								if (localSpace) {
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.targetPosition[p]);
									playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.previousTargetPosition[p]);
								}
							} else {
								playgroundParticles.playgroundCache.previousTargetPosition[prevP] = PlaygroundC.initialTargetPosition;
								playgroundParticles.playgroundCache.targetPosition[prevP] = PlaygroundC.initialTargetPosition;
								playgroundParticles.playgroundCache.previousTargetPosition[p] = PlaygroundC.initialTargetPosition;
								playgroundParticles.playgroundCache.targetPosition[p] = PlaygroundC.initialTargetPosition;
							}
						} else {
							playgroundParticles.playgroundCache.previousTargetPosition[prevP] = PlaygroundC.initialTargetPosition;
							playgroundParticles.playgroundCache.targetPosition[prevP] = PlaygroundC.initialTargetPosition;
							playgroundParticles.playgroundCache.previousTargetPosition[p] = PlaygroundC.initialTargetPosition;
							playgroundParticles.playgroundCache.targetPosition[p] = PlaygroundC.initialTargetPosition;
						}
						break;
					}
					
					// Lifetime coloring
					if (!playgroundParticles.playgroundCache.changedByPropertyColor[p] && !playgroundParticles.playgroundCache.changedByPropertyColorLerp[p]) {
						if (playgroundParticles.colorSource!=COLORSOURCEC.Source || playgroundParticles.colorSource==COLORSOURCEC.Source && playgroundParticles.source!=SOURCEC.State && playgroundParticles.source!=SOURCEC.Paint && playgroundParticles.source!=SOURCEC.Projection && playgroundParticles.source!=SOURCEC.Script || playgroundParticles.source==SOURCEC.State && playgroundParticles.states[playgroundParticles.activeState].stateMesh!=null && playgroundParticles.states[playgroundParticles.activeState].stateTexture==null)
							playgroundParticles.particleCache.particles[p].color = playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.lifetime>0?playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime:0);
						else if (playgroundParticles.colorSource==COLORSOURCEC.Source && playgroundParticles.sourceUsesLifetimeAlpha) {
							lifetimeColor = playgroundParticles.particleCache.particles[p].color;
							lifetimeColor.a = Mathf.Clamp(playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime).a, 0, lifetimeColor.a);
							playgroundParticles.particleCache.particles[p].color = lifetimeColor;
						}
					} else if (playgroundParticles.playgroundCache.changedByPropertyColorKeepAlpha[p]) {
						lifetimeColor = playgroundParticles.particleCache.particles[p].color;
						lifetimeColor.a = Mathf.Clamp(playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime).a, 0, lifetimeColor.a);
						playgroundParticles.particleCache.particles[p].color = lifetimeColor;
					}
					
					if (!playgroundParticles.onlySourcePositioning) {
						
						// Velocity bending
						if (playgroundParticles.applyVelocityBending && playgroundParticles.playgroundCache.velocity[p]!=Vector3.zero) {
							playgroundParticles.playgroundCache.velocity[p] += Vector3.Reflect(
								new Vector3(
								playgroundParticles.playgroundCache.velocity[p].x*playgroundParticles.velocityBending.x,
								playgroundParticles.playgroundCache.velocity[p].y*playgroundParticles.velocityBending.y, 
								playgroundParticles.playgroundCache.velocity[p].z*playgroundParticles.velocityBending.z
								),
								(playgroundParticles.playgroundCache.targetPosition[p]-playgroundParticles.particleCache.particles[p].position).normalized
								)*t;
						}
						
						// Delta velocity
						if (playgroundParticles.calculateDeltaMovement && playgroundParticles.playgroundCache.life[p]==0 && playgroundParticles.source!=SOURCEC.Script && !playgroundParticles.isPainting) {
							deltaVelocity = playgroundParticles.playgroundCache.targetPosition[p]-(playgroundParticles.playgroundCache.previousTargetPosition[p]+playgroundParticles.playgroundCache.scatterPosition[p]);
							playgroundParticles.playgroundCache.velocity[p] += deltaVelocity*playgroundParticles.deltaMovementStrength;
							playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.playgroundCache.targetPosition[p];
						}
						
					}
					
					// Local Manipulators
					for (m = 0; m<playgroundParticles.manipulators.Count; m++)
						if (playgroundParticles.manipulators[m].transform!=null)
							CalculateManipulator(playgroundParticles, playgroundParticles.manipulators[m], ref p, ref t, playgroundParticles.particleCache.particles[p].position, localSpace?playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.manipulators[m].transform.position):playgroundParticles.manipulators[m].transform.position, ref localSpace);
					// Global Manipulators
					for (m = 0; m<PlaygroundC.reference.manipulators.Count; m++)
						if (PlaygroundC.reference.manipulators[m].transform!=null)
							CalculateManipulator(playgroundParticles, PlaygroundC.reference.manipulators[m], ref p, ref t, playgroundParticles.particleCache.particles[p].position, localSpace?playgroundParticles.particleSystemTransform.InverseTransformPoint(PlaygroundC.reference.manipulators[m].transform.position):PlaygroundC.reference.manipulators[m].transform.position, ref localSpace);
					
					if (!playgroundParticles.onlySourcePositioning) {
						
						// Gravity
						playgroundParticles.playgroundCache.velocity[p] -= playgroundParticles.gravity*t;
						
						// Lifetime additive velocity
						if (playgroundParticles.applyLifetimeVelocity) 
							playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.lifetimeVelocity.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime)*t;
						
						// Damping, max velocity, constraints and final positioning
						if (playgroundParticles.playgroundCache.velocity[p]!=Vector3.zero) {
							if (playgroundParticles.playgroundCache.velocity[p].sqrMagnitude>playgroundParticles.maxVelocity)
								playgroundParticles.playgroundCache.velocity[p] = Vector3.ClampMagnitude(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.maxVelocity);
							
							// Damping
							playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, playgroundParticles.damping*t);
							
							// Axis constraints
							if (playgroundParticles.axisConstraints.x)
								playgroundParticles.playgroundCache.velocity[p].x = 0;
							if (playgroundParticles.axisConstraints.y)
								playgroundParticles.playgroundCache.velocity[p].y = 0;
							if (playgroundParticles.axisConstraints.z)
								playgroundParticles.playgroundCache.velocity[p].z = 0;
							
							// Set particle velocity to be able to stretch towards movement
							if (!playgroundParticles.onlySourcePositioning && playgroundParticles.particleSystemRenderer2 && playgroundParticles.particleSystemRenderer2.renderMode==ParticleSystemRenderMode.Stretch)
								playgroundParticles.particleCache.particles[p].velocity = playgroundParticles.playgroundCache.velocity[p];
							
							// Relocate
							playgroundParticles.particleCache.particles[p].position += playgroundParticles.playgroundCache.velocity[p]*t;

							// Local space movement compensation
							//if (localSpace && playgroundParticles.applyLocalSpaceMovementCompensation) 
							//	playgroundParticles.particleCache.particles[p].position -= playgroundParticles.particleSystemTransform.position-playgroundParticles.previousTransformPosition;
						}
						
						// Collision detection 
						if (playgroundParticles.collision && playgroundParticles.collisionRadius>0) {
							
							// Playground Plane colliders (never exceed these)
							for (m = 0; m<playgroundParticles.colliders.Count; m++) {
								if (playgroundParticles.colliders[m].enabled && playgroundParticles.colliders[m].transform && !playgroundParticles.colliders[m].plane.GetSide(!localSpace?playgroundParticles.particleCache.particles[p].position : playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.particleCache.particles[p].position))) {
									
									// Set particle to location
									ray = new Ray(!localSpace?playgroundParticles.particleCache.particles[p].position : playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.particleCache.particles[p].position), playgroundParticles.colliders[m].plane.normal);
									if (playgroundParticles.colliders[m].plane.Raycast(ray, out distance))
										playgroundParticles.particleCache.particles[p].position = !localSpace?ray.GetPoint(distance) : playgroundParticles.particleSystemTransform.InverseTransformPoint(ray.GetPoint(distance));
									
									// Reflect particle
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Reflect(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.colliders[m].plane.normal+RandomVector3(playgroundParticles.bounceRandomMin, playgroundParticles.bounceRandomMax))*playgroundParticles.bounciness;
									
									// Apply lifetime loss
									playgroundParticles.playgroundCache.life[p] = playgroundParticles.playgroundCache.life[p]/(1-playgroundParticles.lifetimeLoss);
									
								}
							}
							
							// Colliders in scene
							if (playgroundParticles.playgroundCache.velocity[p].magnitude>PlaygroundC.collisionSleepVelocity) {
								
								// Collide by checking for potential passed collider in the direction of this particle's velocity from the previous frame
								// Origin, Direction, OutInfo, Distance, LayerMask
								if (Physics.Raycast(
									playgroundParticles.playgroundCache.previousParticlePosition[p], 
									((!localSpace?playgroundParticles.particleCache.particles[p].position : playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.particleCache.particles[p].position))-playgroundParticles.playgroundCache.previousParticlePosition[p]).normalized, 
									out hitInfo, 
									Vector3.Distance(playgroundParticles.playgroundCache.previousParticlePosition[p], !localSpace?playgroundParticles.particleCache.particles[p].position : playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.particleCache.particles[p].position))+playgroundParticles.collisionRadius, 
									playgroundParticles.collisionMask)) 
								{
									
									// Set particle to location
									playgroundParticles.particleCache.particles[p].position = !localSpace?playgroundParticles.playgroundCache.previousParticlePosition[p] : playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.previousParticlePosition[p]);
									
									// Reflect particle
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Reflect(playgroundParticles.playgroundCache.velocity[p], hitInfo.normal+RandomVector3(playgroundParticles.bounceRandomMin, playgroundParticles.bounceRandomMax))*playgroundParticles.bounciness;
									
									// Apply lifetime loss
									playgroundParticles.playgroundCache.life[p] = playgroundParticles.playgroundCache.life[p]/(1-playgroundParticles.lifetimeLoss);
									
									// Add force to rigidbody
									if (playgroundParticles.affectRigidbodies && hitInfo.rigidbody)
										hitInfo.rigidbody.AddForceAtPosition(playgroundParticles.playgroundCache.velocity[p]*playgroundParticles.mass, playgroundParticles.particleCache.particles[p].position);
								}
							} else {
								playgroundParticles.playgroundCache.velocity[p] = Vector3.zero;
							}
						}
						
						// Store this frame's particle position to use next frame
						playgroundParticles.playgroundCache.previousParticlePosition[p] = !localSpace?playgroundParticles.particleCache.particles[p].position : playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.particleCache.particles[p].position);
						
					} else {
						
						// Only Source Positioning
						// Set particle velocity to be able to stretch towards movement
						if (playgroundParticles.particleSystemRenderer2 && playgroundParticles.particleSystemRenderer2.renderMode==ParticleSystemRenderMode.Stretch)
							playgroundParticles.particleCache.particles[p].velocity = (playgroundParticles.playgroundCache.targetPosition[p]-playgroundParticles.playgroundCache.previousTargetPosition[p]);
						if (playgroundParticles.source!=SOURCEC.Script) {
							playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.playgroundCache.targetPosition[p];
							playgroundParticles.particleCache.particles[p].position = playgroundParticles.playgroundCache.targetPosition[p];
						} else if (playgroundParticles.playgroundCache.parent[p]) {
							playgroundParticles.particleCache.particles[p].position = playgroundParticles.playgroundCache.parent[p].TransformPoint(playgroundParticles.playgroundCache.targetPosition[p]);
						}
					}
					
					
				} else {
					playgroundParticles.playgroundCache.targetPosition[p] = PlaygroundC.initialTargetPosition;
					playgroundParticles.particleCache.particles[p].position = PlaygroundC.initialTargetPosition;
				}
				
			} else {
				
				// Particle is set to not rebirth
				playgroundParticles.playgroundCache.targetPosition[p] = PlaygroundC.initialTargetPosition;
				playgroundParticles.particleCache.particles[p].position = PlaygroundC.initialTargetPosition;
			}
		}
		//playgroundParticles.previousTransformPosition = playgroundParticles.particleSystemTransform.position;
	}

	// Calculate the effect from a manipulator
	public static void CalculateManipulator (PlaygroundParticlesC playgroundParticles, ManipulatorObjectC thisManipulator, ref int p, ref float t, Vector3 particlePosition, Vector3 manipulatorPosition, ref bool localSpace) {
		if (thisManipulator.enabled && thisManipulator.transform!=null && thisManipulator.strength!=0 && (thisManipulator.affects.value & 1<<playgroundParticles.particleSystemGameObject.layer)!=0) {
			float manipulatorDistance;
			if (!playgroundParticles.onlySourcePositioning) {
				// Attractors
				if (thisManipulator.type==MANIPULATORTYPEC.Attractor) {
					manipulatorDistance = Vector3.Distance(manipulatorPosition, particlePosition);
					if (thisManipulator.Contains(particlePosition, manipulatorPosition)) {
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition-particlePosition)*(thisManipulator.strength/manipulatorDistance), t*(thisManipulator.strength/manipulatorDistance));
					}
				} else
					
					// Attractors Gravitational
				if (thisManipulator.type==MANIPULATORTYPEC.AttractorGravitational) {
					manipulatorDistance = Vector3.Distance(manipulatorPosition, particlePosition);
					if (thisManipulator.Contains(particlePosition, manipulatorPosition)) {
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition-particlePosition)*thisManipulator.strength/manipulatorDistance, t);
					}
				} else
					
					// Repellents 
				if (thisManipulator.type==MANIPULATORTYPEC.Repellent) {
					manipulatorDistance = Vector3.Distance(manipulatorPosition, particlePosition);
					if (thisManipulator.Contains(particlePosition, manipulatorPosition)) {
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (particlePosition-manipulatorPosition)*(thisManipulator.strength/manipulatorDistance), t*(thisManipulator.strength/manipulatorDistance));
					}
				}
			}
			
			// Properties
			if (thisManipulator.type==MANIPULATORTYPEC.Property) {
				PropertyManipulator(playgroundParticles, thisManipulator, thisManipulator.property, ref p, ref t, ref particlePosition, ref manipulatorPosition, ref localSpace);
			}
			
			// Combined
			if (thisManipulator.type==MANIPULATORTYPEC.Combined) {
				for (int i = 0; i<thisManipulator.properties.Count; i++)
					PropertyManipulator(playgroundParticles, thisManipulator, thisManipulator.properties[i], ref p, ref t, ref particlePosition, ref manipulatorPosition, ref localSpace);
			}
		}
	}
	
	// Calculate the effect from manipulator properties
	public static void PropertyManipulator (PlaygroundParticlesC playgroundParticles, ManipulatorObjectC thisManipulator, ManipulatorPropertyC thisManipulatorProperty, ref int p, ref float t, ref Vector3 particlePosition, ref Vector3 manipulatorPosition, ref bool localSpace) {
		if (thisManipulator.Contains(particlePosition, manipulatorPosition)) {
			switch (thisManipulatorProperty.type) {
				
				// Velocity Property
			case MANIPULATORPROPERTYTYPEC.Velocity:
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
					playgroundParticles.playgroundCache.velocity[p] = thisManipulatorProperty.useLocalRotation?
						thisManipulator.transform.TransformPoint(thisManipulatorProperty.velocity)-manipulatorPosition
						:
						thisManipulatorProperty.velocity;
				else
					playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], thisManipulatorProperty.useLocalRotation?
						thisManipulator.transform.TransformPoint(thisManipulatorProperty.velocity)-manipulatorPosition
					:
						thisManipulatorProperty.velocity, t*thisManipulatorProperty.strength*thisManipulator.strength);
				break;
				
				// Additive Velocity Property
			case MANIPULATORPROPERTYTYPEC.AdditiveVelocity:
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
					playgroundParticles.playgroundCache.velocity[p] += thisManipulatorProperty.useLocalRotation?
						thisManipulator.transform.TransformPoint(thisManipulatorProperty.velocity*(t*thisManipulatorProperty.strength*thisManipulator.strength))-manipulatorPosition
						:
						thisManipulatorProperty.velocity*(t*thisManipulatorProperty.strength*thisManipulator.strength);
				else
					playgroundParticles.playgroundCache.velocity[p] += Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], thisManipulatorProperty.useLocalRotation? thisManipulator.transform.TransformPoint(thisManipulatorProperty.velocity)-manipulatorPosition : thisManipulatorProperty.velocity, t*thisManipulatorProperty.strength*thisManipulator.strength)*(t*thisManipulatorProperty.strength*thisManipulator.strength);
				break;
				
				// Color Property
			case MANIPULATORPROPERTYTYPEC.Color:
				Color staticColor;
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None) {
					if (thisManipulatorProperty.keepColorAlphas) {
						staticColor = thisManipulatorProperty.color;
						staticColor.a = Mathf.Clamp(playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime).a, 0, staticColor.a);
						playgroundParticles.particleCache.particles[p].color = staticColor;
					} else playgroundParticles.particleCache.particles[p].color = thisManipulatorProperty.color;
				} else {
					if (thisManipulatorProperty.keepColorAlphas) {
						staticColor = thisManipulatorProperty.color;
						staticColor.a = Mathf.Clamp(playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime).a, 0, staticColor.a);
						playgroundParticles.particleCache.particles[p].color = Color.Lerp(playgroundParticles.particleCache.particles[p].color, staticColor, t*thisManipulatorProperty.strength*thisManipulator.strength);
					} else playgroundParticles.particleCache.particles[p].color = Color.Lerp(playgroundParticles.particleCache.particles[p].color, thisManipulatorProperty.color, t*thisManipulatorProperty.strength*thisManipulator.strength);
					playgroundParticles.playgroundCache.changedByPropertyColorLerp[p] = true;
				}
				
				// Only color in range of manipulator boundaries
				if (!thisManipulatorProperty.onlyColorInRange)
					playgroundParticles.playgroundCache.changedByPropertyColor[p] = true;

				// Keep alpha of original color
				if (thisManipulatorProperty.keepColorAlphas)
					playgroundParticles.playgroundCache.changedByPropertyColorKeepAlpha[p] = true;
				
				// Set color pairing key
				else if (playgroundParticles.playgroundCache.propertyColorId[p] != thisManipulator.transform.GetInstanceID()) {
					playgroundParticles.playgroundCache.propertyColorId[p] = thisManipulator.transform.GetInstanceID();
				}
				break;
				
				// Lifetime Color Property
			case MANIPULATORPROPERTYTYPEC.LifetimeColor:
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None) {
					playgroundParticles.particleCache.particles[p].color = thisManipulatorProperty.lifetimeColor.Evaluate(playgroundParticles.lifetime>0?playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime:0);
				} else {
					playgroundParticles.particleCache.particles[p].color = Color.Lerp(playgroundParticles.particleCache.particles[p].color, thisManipulatorProperty.lifetimeColor.Evaluate(playgroundParticles.lifetime>0?playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime:0), t*thisManipulatorProperty.strength*thisManipulator.strength);
					playgroundParticles.playgroundCache.changedByPropertyColorLerp[p] = true;
				}
				
				// Only color in range of manipulator boundaries
				if (!thisManipulatorProperty.onlyColorInRange)
					playgroundParticles.playgroundCache.changedByPropertyColor[p] = true;
				
				// Set color pairing key
				else if (playgroundParticles.playgroundCache.propertyColorId[p] != thisManipulator.transform.GetInstanceID()) {
					playgroundParticles.playgroundCache.propertyColorId[p] = thisManipulator.transform.GetInstanceID();
				}
				break;
				
				// Size Property
			case MANIPULATORPROPERTYTYPEC.Size:
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
					playgroundParticles.particleCache.particles[p].size = thisManipulatorProperty.size;
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
					playgroundParticles.particleCache.particles[p].size = Mathf.Lerp(playgroundParticles.particleCache.particles[p].size, thisManipulatorProperty.size, t*thisManipulatorProperty.strength*thisManipulator.strength);
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear)
					playgroundParticles.particleCache.particles[p].size = Mathf.MoveTowards(playgroundParticles.particleCache.particles[p].size, thisManipulatorProperty.size, t*thisManipulatorProperty.strength*thisManipulator.strength);
				playgroundParticles.playgroundCache.changedByPropertySize[p] = true;
				break;
				
				// Target Property
			case MANIPULATORPROPERTYTYPEC.Target:
				if (thisManipulatorProperty.targets.Count>0 && thisManipulatorProperty.targets[thisManipulatorProperty.targetPointer%thisManipulatorProperty.targets.Count]) {
					
					
					// Set target pointer
					if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.transform.GetInstanceID()) {
						playgroundParticles.playgroundCache.propertyTarget[p] = thisManipulatorProperty.targetPointer;
						thisManipulatorProperty.targetPointer++; thisManipulatorProperty.targetPointer=thisManipulatorProperty.targetPointer%thisManipulatorProperty.targets.Count;
						playgroundParticles.playgroundCache.propertyId[p] = thisManipulator.transform.GetInstanceID();
					}
					
					// Teleport or lerp to position based on transition type
					if (playgroundParticles.playgroundCache.propertyId[p] == thisManipulator.transform.GetInstanceID() && thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count]) {
						if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
							playgroundParticles.particleCache.particles[p].position = localSpace? 
								playgroundParticles.particleSystemTransform.InverseTransformPoint(thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position)
							: 
								thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position;
						else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp) {
							playgroundParticles.particleCache.particles[p].position = localSpace? 
								Vector3.Lerp(particlePosition, playgroundParticles.particleSystemTransform.InverseTransformPoint(thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position), t*thisManipulatorProperty.strength*thisManipulator.strength)
							:
								Vector3.Lerp(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position, t*thisManipulatorProperty.strength*thisManipulator.strength);
							if (thisManipulatorProperty.zeroVelocityStrength>0)
								playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
						} else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear) {
							playgroundParticles.particleCache.particles[p].position = localSpace?
								Vector3.MoveTowards(particlePosition, playgroundParticles.particleSystemTransform.InverseTransformPoint(thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position), t*thisManipulatorProperty.strength*thisManipulator.strength)
							:
								Vector3.MoveTowards(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position, t*thisManipulatorProperty.strength*thisManipulator.strength);
							if (thisManipulatorProperty.zeroVelocityStrength>0)
								playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
						}
						
						// This particle was changed by a target property
						playgroundParticles.playgroundCache.changedByPropertyTarget[p] = true;
					}
				}
				break;
				
				// Death Property
			case MANIPULATORPROPERTYTYPEC.Death:
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
					playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime;
				else
					playgroundParticles.playgroundCache.birth[p] -= t*thisManipulatorProperty.strength*thisManipulator.strength;
				
				// This particle was changed by a death property
				playgroundParticles.playgroundCache.changedByPropertyDeath[p] = true;
				break;
				
				
				// Attractors
			case MANIPULATORPROPERTYTYPEC.Attractor:
				if (!playgroundParticles.onlySourcePositioning) {
					float attractorDistance = Vector3.Distance(manipulatorPosition, particlePosition);
					playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition-particlePosition)*((thisManipulatorProperty.strength*thisManipulator.strength)/attractorDistance), t*((thisManipulatorProperty.strength*thisManipulator.strength)/attractorDistance));
				}
				break;
				
				// Attractors Gravitational
			case MANIPULATORPROPERTYTYPEC.Gravitational:
				if (!playgroundParticles.onlySourcePositioning) {
					playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition-particlePosition)*(thisManipulatorProperty.strength*thisManipulator.strength)/Vector3.Distance(manipulatorPosition, particlePosition), t);
				}
			break;
				
				// Repellents 
			case MANIPULATORPROPERTYTYPEC.Repellent:
				if (!playgroundParticles.onlySourcePositioning) {
					float repellentsDistance = Vector3.Distance(manipulatorPosition, particlePosition);
					playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (particlePosition-manipulatorPosition)*((thisManipulatorProperty.strength*thisManipulator.strength)/repellentsDistance), t*((thisManipulatorProperty.strength*thisManipulator.strength)/repellentsDistance));
				}
			break;
			}
			
			playgroundParticles.playgroundCache.changedByProperty[p] = true;
			
		} else {
			
			// Handle colors outside of property manipulator range
			if (playgroundParticles.playgroundCache.propertyColorId[p] == thisManipulator.transform.GetInstanceID() && (thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.Color || thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.LifetimeColor)) {

				// Lerp back color with previous set key
				if (playgroundParticles.playgroundCache.changedByPropertyColorLerp[p] && thisManipulatorProperty.transition != MANIPULATORPROPERTYTRANSITIONC.None && thisManipulatorProperty.onlyColorInRange)
					playgroundParticles.particleCache.particles[p].color = Color.Lerp(playgroundParticles.particleCache.particles[p].color, playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime), t*thisManipulatorProperty.strength*thisManipulator.strength);
			}

			// Target positioning outside of range
			if (thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.Target && thisManipulatorProperty.transition != MANIPULATORPROPERTYTRANSITIONC.None) {
				if (thisManipulatorProperty.targets.Count>0 && thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count]!=null) {
					if (playgroundParticles.playgroundCache.changedByPropertyTarget[p] && !thisManipulatorProperty.onlyPositionInRange && thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count] && playgroundParticles.playgroundCache.propertyId[p] == thisManipulator.transform.GetInstanceID()) {
						if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
							playgroundParticles.particleCache.particles[p].position = localSpace?
								Vector3.Lerp(particlePosition, playgroundParticles.particleSystemTransform.InverseTransformPoint(thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position), t*thisManipulatorProperty.strength*thisManipulator.strength)
								:	
								Vector3.Lerp(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position, t*thisManipulatorProperty.strength*thisManipulator.strength);
						else
							playgroundParticles.particleCache.particles[p].position = localSpace?
								Vector3.MoveTowards(particlePosition, playgroundParticles.particleSystemTransform.InverseTransformPoint(thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position), t*thisManipulatorProperty.strength*thisManipulator.strength)
								:
								Vector3.MoveTowards(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position, t*thisManipulatorProperty.strength*thisManipulator.strength);
						
						if (thisManipulatorProperty.zeroVelocityStrength>0)
							playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
					}
				}
			}
		}
	}
	
	// Update the source scatter
	public void RefreshScatter () {
		for (int p = 0; p<particleCount; p++) {
			playgroundCache.scatterPosition[p] = applySourceScatter?new Vector3(
				UnityEngine.Random.Range(sourceScatterMin.x, sourceScatterMax.x),
				UnityEngine.Random.Range(sourceScatterMin.y, sourceScatterMax.y),
				UnityEngine.Random.Range(sourceScatterMin.z, sourceScatterMax.z)
			) : Vector3.zero;
		}
	}
	
	// Rebirth of a specified particle
	public static void Rebirth (PlaygroundParticlesC playgroundParticles, int p) {
		playgroundParticles.playgroundCache.birthDelay[p] = 0f;
		playgroundParticles.playgroundCache.life[p] = 0f;
		playgroundParticles.playgroundCache.birth[p] = playgroundParticles.playgroundCache.death[p]; 
		playgroundParticles.playgroundCache.death[p] += playgroundParticles.lifetime;
		playgroundParticles.playgroundCache.rebirth[p] = playgroundParticles.source==SOURCEC.Script?true:(playgroundParticles.emit && playgroundParticles.playgroundCache.emission[p]);
		playgroundParticles.playgroundCache.velocity[p] = Vector3.zero;

		// Set new random size
		if (playgroundParticles.applyRandomSizeOnRebirth)
			playgroundParticles.playgroundCache.size[p] = UnityEngine.Random.Range(playgroundParticles.sizeMin, playgroundParticles.sizeMax);
		
		// Initial velocity
		if (playgroundParticles.applyInitialVelocity && !playgroundParticles.onlySourcePositioning) {
			playgroundParticles.playgroundCache.initialVelocity[p] = new Vector3(
				UnityEngine.Random.Range(playgroundParticles.initialVelocityMin.x, playgroundParticles.initialVelocityMax.x),
				UnityEngine.Random.Range(playgroundParticles.initialVelocityMin.y, playgroundParticles.initialVelocityMax.y),
				UnityEngine.Random.Range(playgroundParticles.initialVelocityMin.z, playgroundParticles.initialVelocityMax.z)
			);
			playgroundParticles.playgroundCache.velocity[p] = playgroundParticles.playgroundCache.initialVelocity[p];
			
			// Give this spawning particle its velocity shape
			if (playgroundParticles.applyInitialVelocityShape)
				playgroundParticles.playgroundCache.velocity[p] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((p*1f)/(playgroundParticles.particleCount*1f)));
		}
		if (playgroundParticles.source==SOURCEC.Script) {
			// Velocity for script mode
			if (!playgroundParticles.onlySourcePositioning)
				playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.scriptedEmissionVelocity;
			playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.scriptedEmissionPosition;
		}
		
		if (playgroundParticles.playgroundCache.rebirth[p]) {

			// Set new random rotation
			if (playgroundParticles.applyRandomRotationOnRebirth)
				playgroundParticles.playgroundCache.initialRotation[p] = UnityEngine.Random.Range(playgroundParticles.initialRotationMin, playgroundParticles.initialRotationMax);
			playgroundParticles.particleCache.particles[p].rotation = playgroundParticles.playgroundCache.initialRotation[p];
			
			// Source Scattering
			if (playgroundParticles.applySourceScatter && playgroundParticles.source!=SOURCEC.Script) {
				if (playgroundParticles.playgroundCache.scatterPosition[p]==Vector3.zero || playgroundParticles.applyRandomScatterOnRebirth)
					playgroundParticles.playgroundCache.scatterPosition[p] = new Vector3(
						UnityEngine.Random.Range(playgroundParticles.sourceScatterMin.x, playgroundParticles.sourceScatterMax.x),
						UnityEngine.Random.Range(playgroundParticles.sourceScatterMin.y, playgroundParticles.sourceScatterMax.y),
						UnityEngine.Random.Range(playgroundParticles.sourceScatterMin.z, playgroundParticles.sourceScatterMax.z)
					);
			} else playgroundParticles.playgroundCache.scatterPosition[p]=Vector3.zero;
			
			playgroundParticles.particleCache.particles[p].position = playgroundParticles.playgroundCache.targetPosition[p];
			playgroundParticles.playgroundCache.previousParticlePosition[p] = playgroundParticles.particleCache.particles[p].position;
		} else playgroundParticles.particleCache.particles[p].position = PlaygroundC.initialTargetPosition;
		
		// Reset manipulators influence
		playgroundParticles.playgroundCache.changedByProperty[p] = false;
		playgroundParticles.playgroundCache.changedByPropertyColor[p] = false;
		playgroundParticles.playgroundCache.changedByPropertyColorLerp[p] = false;
		playgroundParticles.playgroundCache.changedByPropertyColorKeepAlpha[p] = false;
		playgroundParticles.playgroundCache.changedByPropertySize[p] = false;
		playgroundParticles.playgroundCache.changedByPropertyTarget[p] = false;
		playgroundParticles.playgroundCache.changedByPropertyDeath[p] = false;
		playgroundParticles.playgroundCache.propertyTarget[p] = 0;
		playgroundParticles.playgroundCache.propertyId[p] = 0;
		playgroundParticles.playgroundCache.propertyColorId[p] = 0;
	}
	
	// Delete a state from states list
	public void RemoveState (int i) {
		int newState = this.activeState;
		newState = (newState%this.states.Count)-1;
		if (newState<0) newState = 0;
		
		this.states[newState].Initialize();
		this.activeState = newState;
		this.states.RemoveAt(i);
	}
	
	// Wipe out particles in current PlaygroundParticlesC object
	public static void Clear (PlaygroundParticlesC playgroundParticles) {
		playgroundParticles.inTransition = false;
		playgroundParticles.particleCache.particles = new ParticleSystem.Particle[0];
		playgroundParticles.playgroundCache.velocity = null;
		playgroundParticles.playgroundCache.targetPosition = null;
		playgroundParticles.playgroundCache.previousTargetPosition = null;
		playgroundParticles.playgroundCache.life = null;
		playgroundParticles.playgroundCache.size = null;
		playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache.particles,0);
		playgroundParticles.shurikenParticleSystem.Clear();
	}

	// Check all needed references
	void CheckReferences () {
		if (PlaygroundC.reference==null)
			PlaygroundC.reference = FindObjectOfType<PlaygroundC>();
		if (PlaygroundC.reference==null)
			PlaygroundC.ResourceInstantiate("Playground Manager");
		if (this.playgroundCache==null)
			this.playgroundCache = new PlaygroundCache();
		if (this.particleCache==null)
			this.particleCache = new ParticleCache();

		if (this.particleSystemGameObject==null) {
			this.particleSystemGameObject = gameObject;
			this.particleSystemTransform = transform;
			this.particleSystemRenderer = renderer;
			this.shurikenParticleSystem = particleSystemGameObject.GetComponent<ParticleSystem>();
		}
	}

	// YieldedRefresh makes sure that Playground Manager and simulation time is ready before this particle system
	public IEnumerator YieldedRefresh () {
		yield return null;
		SetLifetime(this, lifetime);
		#if UNITY_EDITOR
		if (!UnityEditor.EditorApplication.isPlaying && sorting!=SORTINGC.Burst) {
			Update (this);
		}
		#endif
		if (sorting==SORTINGC.Burst) {
			Update (this);
		}
	}


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// MonoBehaviours
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	void Awake () {
		// Make sure World Object is initialized
		if (worldObject!=null)
			worldObject.Initialize();
		
		// Make sure that state data is initialized
		for (int x = 0; x<states.Count; x++) {
			states[x].Initialize();
		}
		
		// Make sure projection is initialized
		if (projection!=null && projection.projectionTexture)
			projection.Initialize();
	}

	void OnEnable () {

		// Make sure all references exists
		CheckReferences();

		// Set 0 size to avoid one-frame flash
		shurikenParticleSystem.startSize = 0f;
		shurikenParticleSystem.Play();

		// Set initial values
		previousEmission = emit;
		loopExceeded = false;
		loopExceededOnParticle = -1;
		
		// Initiate all arrays by setting particle count
		if (particleCache.particles==null) {
			SetParticleCount(this, particleCount);
		} else {
			// Clean up particle positions
			SetInitialTargetPosition(this, PlaygroundC.initialTargetPosition);

			if (sorting==SORTINGC.Burst)
				StartCoroutine(YieldedRefresh());

			// Make sure that particles have current time
			SetParticleTimeNow(this);
		}
	}
	
	public void Start () {
		particleSystemRenderer2 = gameObject.particleSystem.renderer as ParticleSystemRenderer;
		if (PlaygroundC.reference!=null) {
			if (!PlaygroundC.reference.particleSystems.Contains(this))
				PlaygroundC.reference.particleSystems.Add(this);
			if (particleSystemTransform.parent==null && PlaygroundC.reference.autoGroup)
				particleSystemTransform.parent = PlaygroundC.referenceTransform;
		}

		if (particleSystemGameObject.activeSelf)
			StartCoroutine(YieldedRefresh());
	}
	
	public void OnDestroy () {
		// Remove this PlaygroundParticlesC object from Particle Systems list
		if (PlaygroundC.reference)
			PlaygroundC.reference.particleSystems.Remove(this);
	}
	
}	

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// MeshParticles - Extension class for PlaygroundParticlesC which creates mesh states. 
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public class MeshParticles : PlaygroundParticlesC {
	
	public static PlaygroundParticlesC CreateMeshParticles (Mesh[] meshes, Texture2D[] textures, Texture2D[] heightMap, string name, Vector3 position, Quaternion rotation, float particleScale, Vector3[] offsets, Material material) {
		PlaygroundParticlesC meshParticles = PlaygroundParticlesC.CreateParticleObject(name,position,rotation,particleScale,material);
		meshParticles.states = new List<ParticleStateC>();
		
		int[] quantityList = new int[meshes.Length];
		int i = 0;
		for (; i<textures.Length; i++)
			quantityList[i] = meshes[i].vertexCount;
		meshParticles.particleCache.particles = new ParticleSystem.Particle[quantityList[PlaygroundC.Largest(quantityList)]];
		meshParticles.shurikenParticleSystem.Emit(meshParticles.particleCache.particles.Length);
		meshParticles.shurikenParticleSystem.GetParticles(meshParticles.particleCache.particles);
		for (i = 0; i<meshes.Length; i++) {
			meshParticles.states.Add(new ParticleStateC());
			meshParticles.states[0].ConstructParticles(meshes[i],textures[i],particleScale,offsets[i], "State "+i,null);
		}
		
		PlaygroundC.Update(meshParticles);
		PlaygroundC.particlesQuantity++;
		
		return meshParticles;
	}
	
	public static void Add (PlaygroundParticlesC meshParticles, Mesh mesh, float scale, Vector3 offset, string stateName, Transform stateTransform) {
		meshParticles.states.Add(new ParticleStateC());
		meshParticles.states[meshParticles.states.Count-1].ConstructParticles(mesh,scale,offset,stateName,stateTransform);
	}
	
	public static void Add (PlaygroundParticlesC meshParticles, Mesh mesh, Texture2D texture, float scale, Vector3 offset, string stateName, Transform stateTransform) {
		meshParticles.states.Add(new ParticleStateC());
		meshParticles.states[meshParticles.states.Count-1].ConstructParticles(mesh,texture,scale,offset,stateName,stateTransform);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Cache
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public class PlaygroundCache {
	[NonSerialized] public float[] size;									// The size of each particle
	[NonSerialized] public float[] life;									// The life time of each particle
	[NonSerialized] public float[] birth;									// The time of birth for each particle
	[NonSerialized] public float[] birthDelay;								// The delayed time of birth when emission has changed
	[NonSerialized] public float[] death;									// The time of death for each particle
	[NonSerialized] public bool[] emission;									// The emission for each particle (controlled by emission rate)
	[NonSerialized] public bool[] rebirth;									// The rebirth for each particle
	[NonSerialized] public float[] lifetimeOffset;							// The offset in birth-death (sorting)
	[NonSerialized] public Vector3[] velocity;								// The velocity of each particle in this PlaygroundParticles
	[NonSerialized] public Vector3[] initialVelocity;						// The initial velocity of each particle in this PlaygroundParticles
	[NonSerialized] public Vector3[] initialLocalVelocity;					// The initial local velocity of each particle in this PlaygroundParticles
	[NonSerialized] public Vector3[] targetPosition;						// The source position for each particle
	[NonSerialized] public Vector3[] previousTargetPosition; 				// The previous source position for each particle (used to calculate delta movement)
	[NonSerialized] public Vector3[] previousParticlePosition;				// The previous calculated frame's particle position
	[NonSerialized] public Vector3[] scatterPosition;						// The scattered position to apply on each particle birth in this PlaygroundParticles
	[NonSerialized] public float[] initialRotation;							// The initial rotation of each particle in this PlaygroundParticles
	[NonSerialized] public float[] rotationSpeed;							// The rotation speed of each particle in this PlaygroundParticles
	[NonSerialized] public Transform[] parent;								// The transform parent of each particle in this PlaygroundParticles
	[NonSerialized] public Color32[] color;									// The color of each particle in this PlaygroundParticles
	
	[NonSerialized] public bool[] changedByProperty;						// The interaction with property manipulators of each particle
	[NonSerialized] public bool[] changedByPropertyColor;					// The interaction with property manipulators that change color of each particle
	[NonSerialized] public bool[] changedByPropertyColorLerp; 				// The interaction with property manipulators that change color over time of each particle
	[NonSerialized] public bool[] changedByPropertyColorKeepAlpha;			// The interaction with property manipulators that change color and wants to keep alpha
	[NonSerialized] public bool[] changedByPropertySize;					// The interaction with property manipulators that change size of each particle
	[NonSerialized] public bool[] changedByPropertyTarget;					// The interaction with property manipulators that change target of each particle
	[NonSerialized] public bool[] changedByPropertyDeath;					// The interaction with death manipulators that forces a particle to a sooner end
	[NonSerialized] public int[] propertyTarget;							// The property target pointer for each particle
	[NonSerialized] public int[] propertyId;								// The property target id for each particle (pairing a particle's target to a manipulator)
	[NonSerialized] public int[] propertyColorId;							// The property color id for each particles (pairing a particle's color to a manipulator
	
	// Copy this PlaygroundCache struct
	public PlaygroundCache Clone () {
		PlaygroundCache playgroundCache = new PlaygroundCache();
		playgroundCache.size = this.size.Clone() as float[];
		playgroundCache.life = this.life.Clone() as float[];
		playgroundCache.birth = this.birth.Clone() as float[];
		playgroundCache.birthDelay = this.birthDelay.Clone() as float[];
		playgroundCache.death = this.death.Clone() as float[];
		playgroundCache.emission = this.birth.Clone() as bool[];
		playgroundCache.rebirth = this.rebirth.Clone() as bool[];
		playgroundCache.lifetimeOffset = this.lifetimeOffset.Clone() as float[];
		playgroundCache.velocity = this.velocity.Clone() as Vector3[];
		playgroundCache.targetPosition = this.targetPosition.Clone() as Vector3[];
		playgroundCache.previousTargetPosition = this.previousTargetPosition.Clone() as Vector3[];
		playgroundCache.previousParticlePosition = this.previousParticlePosition.Clone() as Vector3[];
		playgroundCache.scatterPosition = this.scatterPosition.Clone() as Vector3[];
		playgroundCache.initialRotation = this.initialRotation.Clone() as float[];
		playgroundCache.rotationSpeed = this.rotationSpeed.Clone() as float[];
		playgroundCache.parent = this.parent.Clone() as Transform[];
		playgroundCache.color = this.color.Clone() as Color32[];
		playgroundCache.changedByProperty = this.changedByProperty.Clone() as bool[];
		playgroundCache.changedByPropertyColor = this.changedByPropertyColor.Clone() as bool[];
		playgroundCache.changedByPropertyColorLerp = this.changedByPropertyColorLerp.Clone() as bool[];
		playgroundCache.changedByPropertyColorKeepAlpha = this.changedByPropertyColorKeepAlpha.Clone () as bool[];
		playgroundCache.changedByPropertySize = this.changedByPropertySize.Clone() as bool[];
		playgroundCache.changedByPropertyTarget = this.changedByPropertyTarget.Clone() as bool[];
		playgroundCache.changedByPropertyDeath = this.changedByPropertyDeath.Clone() as bool[];
		playgroundCache.propertyTarget = this.propertyTarget.Clone() as int[];
		playgroundCache.propertyId = this.propertyId.Clone() as int[];
		playgroundCache.propertyColorId = this.propertyColorId.Clone() as int[];
		return playgroundCache;
	}
}

public class ParticleCache {
	[NonSerialized] public ParticleSystem.Particle[] particles;			// The particle pool of this PlaygroundParticlesC object
}