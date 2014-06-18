#pragma strict

import System.Collections.Generic;

@script RequireComponent (ParticleSystem)
@script ExecuteInEditMode()

class PlaygroundParticles extends MonoBehaviour {

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PlaygroundParticles variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// Playground variables
	@HideInInspector var source : SOURCE;											// The particle source
	@HideInInspector var sourceDownResolution : int = 1;							// The source distribution over vertices (used to down resolution a skinned mesh source points)
	@HideInInspector var activeState : int;											// Current active state (when using state as source)
	@HideInInspector var transition : TRANSITION;									// The type of transition to use
	@HideInInspector var transitionTime : float = 1.0;								// The time it takes to complete a transition
	@HideInInspector var emit : boolean = true;										// If emission of particles is active on this PlaygroundParticles
	@HideInInspector var loop : boolean = true;										// Should a particle re-emit when reaching the end of its lifetime?
	@HideInInspector var disableOnDone : boolean = false;							// Should the GameObject of this PlaygroundParticles disable when not looping? 
	@HideInInspector var updateRate : int = 1;										// The rate to update this PlaygroundParticles
	@HideInInspector var calculate : boolean = true;								// Calculate forces on this PlaygroundParticles (can be overrided by Playground.calculate)
	@HideInInspector var calculateDeltaMovement : boolean = true;					// Calculate the delta movement force of this particle system
	@HideInInspector var deltaMovementStrength : float = 10.0;						// The strength to multiply delta movement with
	@HideInInspector var worldObjectUpdateVertices : boolean = false;				// The current world object will change its vertices over time
	@HideInInspector var worldObjectUpdateNormals : boolean = false;				// The current world object will change its normals over time
	@HideInInspector var nearestNeighborOrigin : int = 0;							// The initial source position when using lifetime sorting of Nearest Neighbor / Nearest Neighbor Reversed
	@HideInInspector var particleCount : int;										// The amount of particles within this PlaygroundParticles object
	@HideInInspector var emissionRate : float = 1.0;								// The percentage to emit of particleCount in bursts from this PlaygroundParticles
	@HideInInspector var overflowMode : OVERFLOWMODE = OVERFLOWMODE.SourceTransform;// The method to calculate overflow with
	@HideInInspector var overflowOffset : Vector3;									// Offset when particle count exceeds source count
	@HideInInspector var applySourceScatter : boolean = false;						// Should source position scattering be applied?
	@HideInInspector var sourceScatterMin : Vector3;								// The minimum spread of source position scattering
	@HideInInspector var sourceScatterMax : Vector3;								// The maximum spread of source position scattering
	@HideInInspector var sorting : SORTING = SORTING.Scrambled;						// Sort mode for particle lifetime
	@HideInInspector var lifetimeSorting : AnimationCurve;							// Custom sorting for particle lifetime (when sorting is set to Custom)
	@HideInInspector var sizeMin : float;											// Minimum size
	@HideInInspector var sizeMax : float;											// Maximum size
	@HideInInspector var scale : float = 1.0;										// The scale of minimum and maximum size
	@HideInInspector var initialRotationMin : float;								// Minimum initial rotation
	@HideInInspector var initialRotationMax : float;								// Maximum initial rotation
	@HideInInspector var rotationSpeedMin : float;									// Minimum amount to rotate
	@HideInInspector var rotationSpeedMax : float;									// Maximum amount to rotate
	@HideInInspector var rotateTowardsDirection : boolean = false;					// Should the particles rotate towards their movement direction
	@HideInInspector var rotationNormal : Vector3 = -Vector3.forward;				// The rotation direction normal when rotating towards direction (always normalized value)
	@HideInInspector var lifetime : float;											// The life of a particle in seconds
	@HideInInspector var lifetimeOffset : float;									// The offset in time of this particle system
	@HideInInspector var lifetimeSize : AnimationCurve;								// The size over lifetime of each particle
	@HideInInspector var onlySourcePositioning : boolean = false;					// Should the particles only position on their source (and not apply any forces)?
	@HideInInspector var applyLifetimeVelocity : boolean = false;					// Should lifetime velocity affect particles?
	@HideInInspector var lifetimeVelocity : Vector3AnimationCurve;					// The velocity over lifetime of each particle
	@HideInInspector var applyInitialVelocity : boolean = false;					// Should initial velocity affect particles?
	@HideInInspector var initialVelocityMin : Vector3;								// The minimum starting velocity of each particle
	@HideInInspector var initialVelocityMax : Vector3;								// The maximum starting velocity of each particle
	@HideInInspector var applyInitialLocalVelocity : boolean = false;				// Should initial local velocity affect particles?
	@HideInInspector var initialLocalVelocityMin : Vector3;							// The minimum starting velocity of each particle with normal or transform direction
	@HideInInspector var initialLocalVelocityMax : Vector3;							// The maximum starting velocity of each particle with normal or transform direction
	@HideInInspector var applyInitialVelocityShape : boolean = false;				// Should the initial velocity shape be applied on particle re/birth?
	@HideInInspector var initialVelocityShape : Vector3AnimationCurve;				// The amount of velocity to apply of the spawning particle's initial/local velocity in form of a Vector3AnimationCurve
	@HideInInspector var applyVelocityBending : boolean;							// Should bending affect particles velocity?
	@HideInInspector var velocityBending : Vector3;									// The amount to bend velocity of each particle
	@HideInInspector var gravity : Vector3;											// The constant force towards gravitational vector
	@HideInInspector var maxVelocity : float = 100;									// The maximum positive- and negative velocity of each particle
	@HideInInspector var axisConstraints : PlaygroundAxisConstraints = new PlaygroundAxisConstraints(); // The force axis constraints of each particle
	@HideInInspector var damping : float;											// Particles inertia over time
	@HideInInspector var lifetimeColor : Gradient;									// The color over lifetime
	@HideInInspector var colorSource : COLORSOURCE = COLORSOURCE.Source;			// The source to read color from (fallback on Lifetime Color if no source color is available)
	@HideInInspector var sourceUsesLifetimeAlpha : boolean;							// Should the source color use alpha from Lifetime Color instead of the source's original alpha?
	@HideInInspector var applyLocalSpaceMovementCompensation = true;				// Should the movement of the particle system transform when in local simulation space be compensated for?
	@HideInInspector var applyRandomSizeOnRebirth : boolean = true;					// Should particles get a new random size upon rebirth?
	@HideInInspector var applyRandomRotationOnRebirth : boolean = true;				// Should particles get a new random rotation upon rebirth?
	@HideInInspector var applyRandomScatterOnRebirth : boolean = false;				// Should particles get a new scatter position upon rebirth?
	
	// Source Script variables
	@HideInInspector var scriptedEmissionIndex : int;								// When using Emit() the index will point to the next particle in pool to emit
	@HideInInspector var scriptedEmissionPosition : Vector3;						// When using Emit() the passed in position will determine the position for this particle
	@HideInInspector var scriptedEmissionVelocity : Vector3;						// When using Emit() the passed in velocity will determine the speed and direction for this particle
	@HideInInspector var scriptedEmissionColor : Color = Color.white;				// When using Emit() the passed in color will decide the color for this particle if colorSource is set to COLORSOURCE.Source
	@HideInInspector var scriptedEmissionParent : Transform;						// When using Emit() the passed in transform will decide which transform this source position belongs to
	
	// Collision detection
	@HideInInspector var collision : boolean = true;								// Can particles collide?
	@HideInInspector var affectRigidbodies : boolean = true;						// Should particles affect rigidbodies?
	@HideInInspector var mass : float = .01;										// The mass of a particle (calculated in collision with rigidbodies)
	@HideInInspector var collisionRadius : float = 1.0;								// The spherical radius of a particle
	@HideInInspector var collisionMask : LayerMask;									// The layers these particles will collide with
	@HideInInspector var lifetimeLoss : float;										// The amount a particle will loose of its lifetime on collision
	@HideInInspector var bounciness : float = .5;									// The amount a particle will bounce on collision
	@HideInInspector var bounceRandomMin : Vector3;									// The minimum amount of random bounciness (seen as negative offset from the collided surface's normal direction)
	@HideInInspector var bounceRandomMax : Vector3;									// The maximum amount of random bounciness (seen as positive offset from the collided surface's normal direction)
	@HideInInspector var colliders : List.<PlaygroundCollider>;						// The Playground Colliders of this particle system
	
	// States (source)
	var states : List.<ParticleState>;												// The states of this PlaygroundParticles

	// Scene objects (source)
	@HideInInspector var worldObject : WorldObject;									// A mesh calculated within the scene
	@HideInInspector var skinnedWorldObject : SkinnedWorldObject;					// A skinned mesh calculated within the scene
	@HideInInspector var sourceTransform : Transform;								// A transform calculated within the scene
	
	// Paint
	@HideInInspector var paint : PaintObject;										// The paint source of this PlaygroundParticles
	
	// Projection
	@HideInInspector var projection : ParticleProjection;							// The projection source of this PlaygroundParticles
	
	// Manipulators
	var manipulators : List.<ManipulatorObject>;									// List of manipulator objects handled by this PlaygroundParticles object
	
	// Cache
	@HideInInspector var playgroundCache : PlaygroundCache;							// Data for each particle
	@HideInInspector var particleCache : ParticleCache;								// Particle pool

	// Components
	@HideInInspector var shurikenParticleSystem : ParticleSystem;					// This ParticleSystem (Shuriken) component
	@HideInInspector var particleSystemId : int;									// The id of this PlaygroundParticles object
	@HideInInspector var particleSystemGameObject : GameObject;						// This GameObject
	@HideInInspector var particleSystemTransform : Transform;						// This Transform
	@HideInInspector var particleSystemRenderer : Renderer;							// This Renderer
	@HideInInspector var particleSystemRenderer2 : ParticleSystemRenderer;			// This ParticleSystemRenderer
	
	// Internally used variables
	@HideInInspector var inTransition : boolean = false;			
	@HideInInspector var previousActiveState : int;
	@HideInInspector var previousParticleCount : int = -1;
	@HideInInspector var previousEmissionRate : float = 1.0;
	@HideInInspector var cameFromNonCalculatedFrame : boolean = false;
	@HideInInspector var previousSizeMin : float;
	@HideInInspector var previousSizeMax : float;
	@HideInInspector var previousInitialRotationMin : float;
	@HideInInspector var previousInitialRotationMax : float;
	@HideInInspector var previousRotationSpeedMin : float;
	@HideInInspector var previousRotationSpeedMax : float;
	@HideInInspector var previousVelocityMin : Vector3;
	@HideInInspector var previousVelocityMax : Vector3;
	@HideInInspector var previousLocalVelocityMin : Vector3;
	@HideInInspector var previousLocalVelocityMax : Vector3;
	@HideInInspector var previousTransformPosition : Vector3;
	@HideInInspector var previousLifetime : float;
	@HideInInspector var previousEmission : boolean = true;
	@HideInInspector var previousWorldObjectUpdateVertices : boolean;
	@HideInInspector var isPainting : boolean = false;
	@HideInInspector var simulationStarted : float;
	@HideInInspector var loopExceeded : boolean = false;
	@HideInInspector var loopExceededOnParticle : int;
	@HideInInspector var emissionStopped : float;
	
	@HideInInspector var particleTimescale : float = 1.0;
	
	
	// Clone settings by passing in a reference
	function CopyTo (playgroundParticles : PlaygroundParticles) {
	
		// Playground variables
		playgroundParticles.source 										= this.source;
		playgroundParticles.sourceDownResolution							= this.sourceDownResolution;
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
		playgroundParticles.colliders									= new List.<PlaygroundCollider>();
		for (var i = 0; i<playgroundParticles.colliders.Count; i++)
			playgroundParticles.colliders.Add(this.colliders[i].Clone());
		
		// States (source)
		playgroundParticles.states 										= new List.<ParticleState>();
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
	// PlaygroundParticles functions
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// Emit a single particle at position with velocity, color and transform point parent (Source Mode SOURCE.Script required)
	function Emit (givePosition : Vector3, giveVelocity : Vector3, giveColor : Color32, giveParent : Transform) : int {
		scriptedEmissionIndex=Mathf.Clamp(scriptedEmissionIndex, 0, scriptedEmissionIndex%particleCount);
		var returnIndex : int = scriptedEmissionIndex;
		
		scriptedEmissionPosition = givePosition;
		scriptedEmissionVelocity = giveVelocity;
		scriptedEmissionColor = giveColor;
		scriptedEmissionParent = giveParent;
		
		Rebirth(this, scriptedEmissionIndex);
		playgroundCache.parent[scriptedEmissionIndex] = giveParent;
		playgroundCache.lifetimeOffset[scriptedEmissionIndex] = 0;
		playgroundCache.life[scriptedEmissionIndex] = 0;
		playgroundCache.birth[scriptedEmissionIndex] = Playground.globalTime;
		playgroundCache.death[scriptedEmissionIndex] = playgroundCache.birth[scriptedEmissionIndex]+lifetime;
		playgroundCache.emission[scriptedEmissionIndex] = true;
		playgroundCache.rebirth[scriptedEmissionIndex] = true;
		playgroundCache.color[scriptedEmissionIndex] = giveColor;
		
		emit = true;
		simulationStarted = Playground.globalTime;
		loopExceeded = false;
		loopExceededOnParticle = -1;
		particleSystemGameObject.SetActive(true);
		
		scriptedEmissionIndex++;scriptedEmissionIndex=scriptedEmissionIndex%particleCount;
		return returnIndex;
	}
	
	// Set emission on/off
	function Emit (setEmission : boolean) {
		emit = setEmission;
		if (emit) {
			simulationStarted = Playground.globalTime;
			loopExceeded = false;
			loopExceededOnParticle = -1;
			particleSystemGameObject.SetActive(true);
			Emission(this, true, true);
		} else {
			emissionStopped = Playground.globalTime;
		}
	}
	
	// Is this particle system still alive?
	function IsAlive () : boolean {
		return loopExceeded;
	}
	
	// Create a new PlaygroundParticles object
	static function CreatePlaygroundParticles (images:Texture2D[], name:String, position:Vector3, rotation:Quaternion, offset : Vector3, particleSize : float, scale : float, material:Material) : PlaygroundParticles {
		var playgroundParticles : PlaygroundParticles = CreateParticleObject(name,position,rotation,particleSize,material);

		var quantityList : int[] = new int[images.Length];
		for (var i = 0; i<images.Length; i++)
			quantityList[i] = images[i].width*images[i].height;
		playgroundParticles.particleCache = new ParticleCache();
		playgroundParticles.particleCache.particles = new ParticleSystem.Particle[quantityList[Playground.Largest(quantityList)]];
		OnCreatePlaygroundParticles(playgroundParticles);	
		
		for (i = 0; i<images.Length; i++) {
			playgroundParticles.states.Add(new ParticleState());
			playgroundParticles.states[playgroundParticles.states.Count-1].ConstructParticles(images[i],scale,offset,"State 0",null);
		}
		
		return playgroundParticles;
	}
	
	// Set default settings for PlaygroundParticles object
	static function OnCreatePlaygroundParticles (playgroundParticles : PlaygroundParticles) {
		playgroundParticles.playgroundCache = new PlaygroundCache();
		playgroundParticles.paint = new PaintObject();
		playgroundParticles.states = new List.<ParticleState>();
		playgroundParticles.projection = new ParticleProjection();
		playgroundParticles.colliders = new List.<PlaygroundCollider>();
		playgroundParticles.particleSystemId = Playground.particlesQuantity;
		playgroundParticles.projection.projectionTransform = playgroundParticles.particleSystemTransform;
		
		playgroundParticles.playgroundCache.size = new float[playgroundParticles.particleCount];
		playgroundParticles.playgroundCache.size = Playground.RandomFloat(playgroundParticles.playgroundCache.size.Length, playgroundParticles.sizeMin, playgroundParticles.sizeMax);
		
		playgroundParticles.previousParticleCount = playgroundParticles.particleCount;
		playgroundParticles.lifetimeSize = new AnimationCurve(Keyframe(0,1), Keyframe(1,1));
		
		playgroundParticles.shurikenParticleSystem.Emit(playgroundParticles.particleCount);
		playgroundParticles.shurikenParticleSystem.GetParticles(playgroundParticles.particleCache.particles);
		for (var p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
			playgroundParticles.particleCache.particles[p].size = playgroundParticles.playgroundCache.size[p];
		}
		
		PlaygroundParticles.SetParticleCount(playgroundParticles, playgroundParticles.particleCount);
		
		if (Playground.reference!=null) {
			Playground.particlesQuantity++;
			Playground.reference.particleSystems.Add(playgroundParticles);
			playgroundParticles.particleSystemId = Playground.particlesQuantity;
		}
	}
	
	// Create a Shuriken Particle System
	static function CreateParticleObject (name : String, position : Vector3, rotation : Quaternion, particleSize : float, material : Material) : PlaygroundParticles {
		var go = Playground.ResourceInstantiate("Particle Playground System");
		var playgroundParticles : PlaygroundParticles = go.GetComponent(PlaygroundParticles);
		playgroundParticles.particleSystemGameObject = go;
		playgroundParticles.particleSystemGameObject.name = name;
		playgroundParticles.shurikenParticleSystem = playgroundParticles.particleSystemGameObject.GetComponent(ParticleSystem);
		playgroundParticles.particleSystemRenderer = playgroundParticles.shurikenParticleSystem.renderer;
		playgroundParticles.particleSystemRenderer2 = playgroundParticles.shurikenParticleSystem.renderer as ParticleSystemRenderer;
		playgroundParticles.particleSystemTransform = playgroundParticles.particleSystemGameObject.transform;
		playgroundParticles.sourceTransform = playgroundParticles.particleSystemTransform;
		playgroundParticles.source = SOURCE.Transform;
		playgroundParticles.particleSystemTransform.position = position;
		playgroundParticles.particleSystemTransform.rotation = rotation;
		if (Playground.reference==null)
			Playground.ResourceInstantiate("Playground Manager");
		if (Playground.reference.autoGroup && playgroundParticles.particleSystemTransform.parent==null)
			playgroundParticles.particleSystemTransform.parent = Playground.referenceTransform;
		
		if (playgroundParticles.particleSystemRenderer.sharedMaterial==null)
			playgroundParticles.particleSystemRenderer.sharedMaterial = material;
		playgroundParticles.shurikenParticleSystem.startSize = particleSize;
		
		return playgroundParticles;
	}
	
	// Create a new WorldObject
	static function NewWorldObject (playgroundParticles : PlaygroundParticles, meshTransform : Transform) : WorldObject {
		var worldObject : WorldObject = new WorldObject();
		if (meshTransform.GetComponentInChildren(MeshFilter)) {
			worldObject.transform = meshTransform;
			worldObject.gameObject = meshTransform.gameObject;
			worldObject.rigidbody = meshTransform.rigidbody;
			worldObject.renderer = meshTransform.GetComponentInChildren(Renderer);
			worldObject.meshFilter = meshTransform.GetComponentInChildren(MeshFilter);
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
	static function NewSkinnedWorldObject (playgroundParticles : PlaygroundParticles, meshTransform : Transform) : SkinnedWorldObject {
		var worldObject : SkinnedWorldObject = new SkinnedWorldObject();
		if (meshTransform.GetComponentInChildren(SkinnedMeshRenderer)) {
			worldObject.transform = meshTransform;
			worldObject.gameObject = meshTransform.gameObject;
			worldObject.rigidbody = meshTransform.rigidbody;
			worldObject.renderer = meshTransform.GetComponentInChildren(SkinnedMeshRenderer);
			worldObject.mesh = worldObject.renderer.sharedMesh;
			worldObject.vertexPositions = new Vector3[worldObject.mesh.vertexCount];
			worldObject.normals = worldObject.mesh.normals;
			worldObject.cachedId = worldObject.gameObject.GetInstanceID();
			
			playgroundParticles.skinnedWorldObject = worldObject;
			
		} else Debug.Log("Could not find a skinned mesh in "+meshTransform.name+".");
		
		return worldObject;
	}
	
	// Create a new PaintObject
	static function NewPaintObject (playgroundParticles : PlaygroundParticles) : PaintObject {
		var paintObject : PaintObject = new PaintObject();
		playgroundParticles.paint = paintObject;
		playgroundParticles.paint.Initialize();
		return paintObject;
	}
	
	// Create a new ParticleProjection object
	static function NewProjectionObject (playgroundParticles : PlaygroundParticles) : ParticleProjection {
		var projectionObject : ParticleProjection = new ParticleProjection();
		playgroundParticles.projection = projectionObject;
		playgroundParticles.projection.Initialize();
		return projectionObject;
	}
	
	// Create a new ManipulatorObject and attach to the Playground Manager
	static function NewManipulatorObject (type : MANIPULATORTYPE, affects : LayerMask, manipulatorTransform : Transform, size : float, strength : float, playgroundParticles : PlaygroundParticles) : ManipulatorObject {
		var manipulatorObject : ManipulatorObject = new ManipulatorObject();
		manipulatorObject.type = type;
		manipulatorObject.affects = affects;
		manipulatorObject.transform = manipulatorTransform;
		manipulatorObject.size = size;
		manipulatorObject.strength = strength;
		manipulatorObject.bounds = new Bounds(Vector3.zero, Vector3(size, size, size));
		manipulatorObject.property = new ManipulatorProperty();
		
		// Add this to Playground Manager or the passed in playgroundParticles
		if (!playgroundParticles)
			Playground.reference.manipulators.Add(manipulatorObject);
		else
			playgroundParticles.manipulators.Add(manipulatorObject);
		
		return manipulatorObject;
	}
	
	// Lerp to specified state in this PlaygroundParticles
	static function Lerp (playgroundParticles : PlaygroundParticles, to : int, time : float, lerpType : LERPTYPE) {
		if (to<0) {to=playgroundParticles.states.Count;} to=to%playgroundParticles.states.Count;
		if(time<0) time = .0;
		var color : Color = new Color();
		for (var i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			if (lerpType==LERPTYPE.PositionColor||lerpType==LERPTYPE.Position) 
				playgroundParticles.particleCache.particles[i].position = Vector3.Lerp(playgroundParticles.particleCache.particles[i].position, playgroundParticles.states[to].GetPosition(i%playgroundParticles.states[to].positionLength), time);
			if (lerpType==LERPTYPE.PositionColor||lerpType==LERPTYPE.Color) {
				color = playgroundParticles.states[to].GetColor(i%playgroundParticles.states[to].colorLength);
				playgroundParticles.particleCache.particles[i].color = Color.Lerp(playgroundParticles.particleCache.particles[i].color, color, time);
			}
		}
		Update(playgroundParticles);
	}
	
	// Lerp to state object
	static function Lerp (playgroundParticles : PlaygroundParticles, state : ParticleState, time : float, lerpType : LERPTYPE) {
		if(time<0) time = .0;
		var color : Color = new Color();
		for (var i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			if (lerpType==LERPTYPE.PositionColor||lerpType==LERPTYPE.Position) 
				playgroundParticles.particleCache.particles[i].position = Vector3.Lerp(playgroundParticles.particleCache.particles[i].position, state.GetPosition(i%state.positionLength), time);
			if (lerpType==LERPTYPE.PositionColor||lerpType==LERPTYPE.Color) {
				color = state.GetColor(i%state.colorLength);
				playgroundParticles.particleCache.particles[i].color = Color.Lerp(playgroundParticles.particleCache.particles[i].color, color, time);
			}
		}
	}
	
	// Lerp to a Skinned World Object
	static function Lerp (playgroundParticles : PlaygroundParticles, particleStateWorldObject : SkinnedWorldObject, time : float) {
		if(time<0) time = .0;
		var vertices : Vector3[] = particleStateWorldObject.mesh.vertices;
		var weights : BoneWeight[] = particleStateWorldObject.mesh.boneWeights;
		var boneMatrices : Matrix4x4[] = new Matrix4x4[particleStateWorldObject.renderer.bones.Length];

		for (var i = 0; i<boneMatrices.Length; i++)
            boneMatrices[i] = particleStateWorldObject.renderer.bones[i].localToWorldMatrix * particleStateWorldObject.mesh.bindposes[i];
		
		var vertexMatrix : Matrix4x4 = new Matrix4x4();
		for (i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			var weight : BoneWeight = weights[i];
			var m0 : Matrix4x4 = boneMatrices[weight.boneIndex0];
			var m1 : Matrix4x4 = boneMatrices[weight.boneIndex1];
			var m2 : Matrix4x4 = boneMatrices[weight.boneIndex2];
			var m3 : Matrix4x4 = boneMatrices[weight.boneIndex3];

			for(var n=0;n<16;n++){
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
	static function SetPosition (playgroundParticles : PlaygroundParticles, to : int, runUpdate : boolean) {
		
		for (var i = 0; i<playgroundParticles.particleCache.particles.Length; i++)
			playgroundParticles.particleCache.particles[i].position = playgroundParticles.states[to].GetPosition(i%playgroundParticles.states[to].positionLength)+GetOverflowOffset(playgroundParticles, i, playgroundParticles.states[to].positionLength);
		if (runUpdate)
			Update(playgroundParticles);
	}
	
	// Set color from PixelParticle object
	static function SetColor (playgroundParticles : PlaygroundParticles, to : int) {
		var color : Color = new Color();
		for (var i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			color = playgroundParticles.states[to].GetColor(i%playgroundParticles.states[to].colorLength);
			playgroundParticles.particleCache.particles[i].color = color;
		}
	}
	
	// Set color from Color
	static function SetColor (playgroundParticles : PlaygroundParticles, color : Color) {
		for (var i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			playgroundParticles.particleCache.particles[i].color = color;
		}
	}
	
	// Set position from Mesh World Object
	static function SetPosition (playgroundParticles : PlaygroundParticles, particleStateWorldObject : WorldObject) {
		if (!playgroundParticles.worldObject || playgroundParticles.worldObject.mesh==null) {
			Debug.Log("There is no mesh assigned to "+playgroundParticles.particleSystemGameObject.name+"'s worlObject.");
			return;
		}
		var meshVertices : Vector3[] = particleStateWorldObject.mesh.vertices;
		for (var i = 0; i<playgroundParticles.particleCache.particles.Length; i++)
			playgroundParticles.particleCache.particles[i].position = particleStateWorldObject.transform.TransformPoint(meshVertices[i%meshVertices.Length])+GetOverflowOffset(playgroundParticles, i, meshVertices.Length);
	}
	
	// Set position from Skinned Mesh World Object
	static function SetPosition (playgroundParticles : PlaygroundParticles, particleStateWorldObject : SkinnedWorldObject) {
		if (!playgroundParticles.skinnedWorldObject || playgroundParticles.skinnedWorldObject.mesh==null) {
			Debug.Log("There is no skinned mesh assigned to "+playgroundParticles.particleSystemGameObject.name+"'s skinnedWorlObject.");
			return;
		}
		var vertices : Vector3[] = particleStateWorldObject.mesh.vertices;
		var weights : BoneWeight[] = particleStateWorldObject.mesh.boneWeights;
		var bindposes : Matrix4x4[] = particleStateWorldObject.mesh.bindposes;
		var boneMatrices : Matrix4x4[] = new Matrix4x4[particleStateWorldObject.renderer.bones.Length];
		
		for (var i = 0; i<boneMatrices.Length; i++)
            boneMatrices[i] = particleStateWorldObject.renderer.bones[i].localToWorldMatrix * bindposes[i];
		
		var vertexMatrix : Matrix4x4 = new Matrix4x4();
		for (i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			var weight : BoneWeight = weights[i];
			var m0 : Matrix4x4 = boneMatrices[weight.boneIndex0];
			var m1 : Matrix4x4 = boneMatrices[weight.boneIndex1];
			var m2 : Matrix4x4 = boneMatrices[weight.boneIndex2];
			var m3 : Matrix4x4 = boneMatrices[weight.boneIndex3];

			for(var n=0;n<16;n++){
				vertexMatrix[n] =
                    m0[n] * weight.weight0 +
                    m1[n] * weight.weight1 +
                    m2[n] * weight.weight2 +
                    m3[n] * weight.weight3;
			}
		    
			playgroundParticles.particleCache.particles[i].position = vertexMatrix.MultiplyPoint3x4(vertices[i%vertices.Length])+GetOverflowOffset(playgroundParticles, i, vertices.Length);
		}
	}
	
	// Get vertices from a skinned world object in a Vector3-array
	static function GetPosition (v3 : Vector3[], norm : Vector3[], particleStateWorldObject : SkinnedWorldObject) {
		var vertices : Vector3[] = particleStateWorldObject.mesh.vertices;
		norm = particleStateWorldObject.mesh.normals;
		var weights : BoneWeight[] = particleStateWorldObject.mesh.boneWeights;
		var bindPoses : Matrix4x4[] = particleStateWorldObject.mesh.bindposes;
		var boneMatrices : Matrix4x4[] = new Matrix4x4[particleStateWorldObject.renderer.bones.Length];
		if (v3.Length!=vertices.Length) v3 = new Vector3[vertices.Length];
		for (var i = 0; i<boneMatrices.Length; i++)
            boneMatrices[i] = particleStateWorldObject.renderer.bones[i].localToWorldMatrix * bindPoses[i];
		
		var vertexMatrix : Matrix4x4 = new Matrix4x4();
		for (i = 0; i<v3.Length; i++) {
			var weight : BoneWeight = weights[i];
			var m0 : Matrix4x4 = boneMatrices[weight.boneIndex0];
			var m1 : Matrix4x4 = boneMatrices[weight.boneIndex1];
			var m2 : Matrix4x4 = boneMatrices[weight.boneIndex2];
			var m3 : Matrix4x4 = boneMatrices[weight.boneIndex3];

			for(var n=0;n<16;n++){
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
	static function GetPosition (v3 : Vector3[], particleStateWorldObject : WorldObject) {
		if (particleStateWorldObject.meshFilter.sharedMesh!=particleStateWorldObject.mesh)
			particleStateWorldObject.mesh = particleStateWorldObject.meshFilter.sharedMesh;
		v3 = particleStateWorldObject.mesh.vertices;
	}
	
	// Get procedural position from Mesh World Object
	static function GetProceduralPosition (v3 : Vector3[], particleStateWorldObject : WorldObject) {
		if (particleStateWorldObject.meshFilter.sharedMesh!=particleStateWorldObject.mesh)
			particleStateWorldObject.mesh = particleStateWorldObject.meshFilter.sharedMesh;
		var vertices : Vector3[] = particleStateWorldObject.mesh.vertices;
		if (v3.Length!=vertices.Length) v3 = new Vector3[vertices.Length];
		for (var i = 0; i<v3.Length; i++) {
			v3[i] = particleStateWorldObject.transform.TransformPoint(vertices[i%vertices.Length]);
		}
	}
	
	// Get normals from Mesh World Object
	static function GetNormals (v3 : Vector3[], particleStateWorldObject : WorldObject) {
		v3 = particleStateWorldObject.mesh.normals;
	}
	
	// Returns the offset as a remainder using the particleCount towards maxVal
	static function GetOverflowOffset (playgroundParticles : PlaygroundParticles, currentVal : int, maxVal : int) : Vector3 {
		var iteration : float = (currentVal/maxVal);
		
		// Trying to use the transform of the current source
		if (playgroundParticles.overflowMode == OVERFLOWMODE.SourceTransform) {
			
			// State
			if (playgroundParticles.source == SOURCE.State && playgroundParticles.states[playgroundParticles.activeState].stateTransform) {
				return playgroundParticles.states[playgroundParticles.activeState].stateTransform.TransformPoint(
					playgroundParticles.overflowOffset.x*iteration,
					playgroundParticles.overflowOffset.y*iteration,
					playgroundParticles.overflowOffset.z*iteration
				)-playgroundParticles.states[playgroundParticles.activeState].stateTransform.position;
			} else
			
			// World Object
			if (playgroundParticles.source == SOURCE.WorldObject && playgroundParticles.worldObject.transform) {
				return playgroundParticles.worldObject.transform.TransformPoint(
					playgroundParticles.overflowOffset.x*iteration,
					playgroundParticles.overflowOffset.y*iteration,
					playgroundParticles.overflowOffset.z*iteration
				)-playgroundParticles.worldObject.transform.position;
			} else
			
			// Skinned World Object
			if (playgroundParticles.source == SOURCE.SkinnedWorldObject && playgroundParticles.skinnedWorldObject.transform) {
				return playgroundParticles.skinnedWorldObject.transform.TransformPoint(
					playgroundParticles.overflowOffset.x*iteration,
					playgroundParticles.overflowOffset.y*iteration,
					playgroundParticles.overflowOffset.z*iteration
				)-playgroundParticles.skinnedWorldObject.transform.position;
			} else
			
			// Transform
			if (playgroundParticles.source == SOURCE.Transform && playgroundParticles.sourceTransform) {
				return playgroundParticles.sourceTransform.TransformPoint(
					playgroundParticles.overflowOffset.x*iteration,
					playgroundParticles.overflowOffset.y*iteration,
					playgroundParticles.overflowOffset.z*iteration
				)-playgroundParticles.sourceTransform.position;
			} else
			
			// Paint
			if (playgroundParticles.source == SOURCE.Paint) {
				var paintParent : Transform = playgroundParticles.paint.GetParent(currentVal);
				return paintParent.TransformPoint(
					playgroundParticles.overflowOffset.x*iteration,
					playgroundParticles.overflowOffset.y*iteration,
					playgroundParticles.overflowOffset.z*iteration
				)-paintParent.position;
			} else
			
			// Projection
			if (playgroundParticles.source == SOURCE.Projection) {
				var projectionParent : Transform = playgroundParticles.projection.GetParent(currentVal);
				return projectionParent.TransformPoint(
					playgroundParticles.overflowOffset.x*iteration,
					playgroundParticles.overflowOffset.y*iteration,
					playgroundParticles.overflowOffset.z*iteration
				)-projectionParent.position;
			}
			
		// Using the transform of the Particle System
		} else if (playgroundParticles.overflowMode == OVERFLOWMODE.ParticleSystemTransform) {
			return playgroundParticles.particleSystemTransform.TransformPoint(
				playgroundParticles.overflowOffset.x*iteration,
				playgroundParticles.overflowOffset.y*iteration,
				playgroundParticles.overflowOffset.z*iteration
			)-playgroundParticles.particleSystemTransform.position;
		
		// Using the source point
		} else if (playgroundParticles.overflowMode == OVERFLOWMODE.SourcePoint) {
			
			// State
			if (playgroundParticles.source == SOURCE.State && playgroundParticles.states[playgroundParticles.activeState].stateTransform) {
				var statePos : Vector3 = playgroundParticles.states[playgroundParticles.activeState].stateTransform.TransformPoint(playgroundParticles.states[playgroundParticles.activeState].GetPosition(currentVal));
				var stateNorm : Vector3 = playgroundParticles.states[playgroundParticles.activeState].stateTransform.TransformDirection(playgroundParticles.states[playgroundParticles.activeState].GetNormal(currentVal));
				
				return Vector3(
					statePos.x+(stateNorm.x*playgroundParticles.overflowOffset.z*iteration),
					statePos.y+(stateNorm.y*playgroundParticles.overflowOffset.z*iteration),
					statePos.z+(stateNorm.z*playgroundParticles.overflowOffset.z*iteration)
				)-statePos;
			} else
			
			// World Object
			if (playgroundParticles.source == SOURCE.WorldObject && playgroundParticles.worldObject.transform) {
				var woPos : Vector3 = playgroundParticles.worldObject.vertexPositions[currentVal%playgroundParticles.worldObject.vertexPositions.Length];
				var woNorm : Vector3 = playgroundParticles.worldObject.transform.TransformDirection(playgroundParticles.worldObject.normals[currentVal%playgroundParticles.worldObject.normals.Length]);
				
				return Vector3(
					woPos.x+(woNorm.x*playgroundParticles.overflowOffset.z*iteration),
					woPos.y+(woNorm.y*playgroundParticles.overflowOffset.z*iteration),
					woPos.z+(woNorm.z*playgroundParticles.overflowOffset.z*iteration)
				)-woPos;
			} else
			
			// Skinned World Object
			if (playgroundParticles.source == SOURCE.SkinnedWorldObject && playgroundParticles.skinnedWorldObject.transform) {
				var swoPos : Vector3 = playgroundParticles.skinnedWorldObject.vertexPositions[currentVal%playgroundParticles.skinnedWorldObject.vertexPositions.Length];
				var swoNorm : Vector3 = playgroundParticles.skinnedWorldObject.normals[currentVal%playgroundParticles.skinnedWorldObject.normals.Length];
				
				return Vector3(
					swoPos.x+(swoNorm.x*playgroundParticles.overflowOffset.z*iteration),
					swoPos.y+(swoNorm.y*playgroundParticles.overflowOffset.z*iteration),
					swoPos.z+(swoNorm.z*playgroundParticles.overflowOffset.z*iteration)
				)-swoPos;
			} else
			
			// Transform
			if (playgroundParticles.source == SOURCE.Transform && playgroundParticles.sourceTransform) {
				return (playgroundParticles.sourceTransform.forward*playgroundParticles.overflowOffset.z*iteration);
			} else
			
			// Paint
			if (playgroundParticles.source == SOURCE.Paint) {
				var paintPos : Vector3 = playgroundParticles.paint.GetPosition(currentVal);
				var paintNorm : Vector3 = playgroundParticles.paint.GetNormal(currentVal);
				
				return Vector3(
					paintPos.x+(paintNorm.x*playgroundParticles.overflowOffset.z*iteration),
					paintPos.y+(paintNorm.y*playgroundParticles.overflowOffset.z*iteration),
					paintPos.z+(paintNorm.z*playgroundParticles.overflowOffset.z*iteration)
				)-paintPos;
			}
			
			// Projection
			if (playgroundParticles.source == SOURCE.Projection) {
				var projectionPos : Vector3 = playgroundParticles.projection.GetPosition(currentVal);
				var projectionNorm : Vector3 = playgroundParticles.projection.GetNormal(currentVal);
				
				return Vector3(
					projectionPos.x+(projectionNorm.x*playgroundParticles.overflowOffset.z*iteration),
					projectionPos.y+(projectionNorm.y*playgroundParticles.overflowOffset.z*iteration),
					projectionPos.z+(projectionNorm.z*playgroundParticles.overflowOffset.z*iteration)
				)-projectionPos;
			}
		}
		
		// Return world position if all else fails
		return Vector3(
			playgroundParticles.overflowOffset.x*iteration,
			playgroundParticles.overflowOffset.y*iteration,
			playgroundParticles.overflowOffset.z*iteration
		);
	}
	
	// Returns the offset as a remainder from a specified transform
	static function GetOverflowOffset (playgroundParticles : PlaygroundParticles, currentVal : int, maxVal : int, overflowTransform : Transform) : Vector3 {
		var iteration : float = (currentVal/maxVal);
		return overflowTransform.TransformPoint(
			playgroundParticles.overflowOffset.x*iteration,
			playgroundParticles.overflowOffset.y*iteration,
			playgroundParticles.overflowOffset.z*iteration
		)-overflowTransform.position;
	}
	
	// Set size for particles
	static function SetSize (playgroundParticles:PlaygroundParticles, size:float) {
		for (var i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			playgroundParticles.playgroundCache.size[i] = size;
			playgroundParticles.particleCache.particles[i].size = size;
		}
	}
	
	// Set random size for particles within sizeMinimum- and sizeMaximum range 
	static function SetSizeRandom (playgroundParticles:PlaygroundParticles, sizeMinimum : float, sizeMaximum : float) {
		playgroundParticles.playgroundCache.size = Playground.RandomFloat(playgroundParticles.particleCache.particles.Length, sizeMinimum, sizeMaximum);
		for (var i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			playgroundParticles.particleCache.particles[i].size = playgroundParticles.playgroundCache.size[i];
		}
		playgroundParticles.sizeMin = sizeMinimum;
		playgroundParticles.sizeMax = sizeMaximum;
		playgroundParticles.previousSizeMin = playgroundParticles.sizeMin;
		playgroundParticles.previousSizeMax = playgroundParticles.sizeMax;
	}
	
	// Set random rotation for particles within rotationMinimum- and rotationMaximum range
	static function SetRotationRandom (playgroundParticles:PlaygroundParticles, rotationMinimum : float, rotationMaximum : float) {
		playgroundParticles.playgroundCache.rotationSpeed = Playground.RandomFloat(playgroundParticles.particleCache.particles.Length, rotationMinimum, rotationMaximum);
		for (var i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			playgroundParticles.particleCache.particles[i].rotation = playgroundParticles.playgroundCache.initialRotation[i];
		}
		playgroundParticles.rotationSpeedMin = rotationMinimum;
		playgroundParticles.rotationSpeedMax = rotationMaximum;
		playgroundParticles.previousRotationSpeedMin = playgroundParticles.rotationSpeedMin;
		playgroundParticles.previousRotationSpeedMax = playgroundParticles.rotationSpeedMax;
	}
	
	// Set random initial rotation for particles within rotationMinimum- and rotationMaximum range
	static function SetInitialRotationRandom (playgroundParticles:PlaygroundParticles, rotationMinimum : float, rotationMaximum : float) {
		playgroundParticles.playgroundCache.initialRotation = Playground.RandomFloat(playgroundParticles.particleCache.particles.Length, rotationMinimum, rotationMaximum);
		for (var i = 0; i<playgroundParticles.particleCache.particles.Length; i++) {
			playgroundParticles.particleCache.particles[i].rotation = playgroundParticles.playgroundCache.initialRotation[i];
		}
		playgroundParticles.initialRotationMin = rotationMinimum;
		playgroundParticles.initialRotationMax = rotationMaximum;
		playgroundParticles.previousInitialRotationMin = playgroundParticles.initialRotationMin;
		playgroundParticles.previousInitialRotationMax = playgroundParticles.initialRotationMax;
	}
	
	// Set initial random velocity for particles within velocityMinimum- and velocityMaximum range
	static function SetVelocityRandom (playgroundParticles:PlaygroundParticles, velocityMinimum : Vector3, velocityMaximum : Vector3) {
		playgroundParticles.playgroundCache.initialVelocity = Playground.RandomVector3(playgroundParticles.particleCache.particles.Length, velocityMinimum, velocityMaximum);
		
		playgroundParticles.initialVelocityMin = velocityMinimum;
		playgroundParticles.initialVelocityMax = velocityMaximum;
		playgroundParticles.previousVelocityMin = playgroundParticles.initialVelocityMin;
		playgroundParticles.previousVelocityMax = playgroundParticles.initialVelocityMax;
	}
	
	// Set initial random local velocity for particles within velocityMinimum- and velocityMaximum range
	static function SetLocalVelocityRandom (playgroundParticles:PlaygroundParticles, velocityMinimum : Vector3, velocityMaximum : Vector3) {
		playgroundParticles.playgroundCache.initialLocalVelocity = Playground.RandomVector3(playgroundParticles.particleCache.particles.Length, velocityMinimum, velocityMaximum);
		
		playgroundParticles.initialLocalVelocityMin = velocityMinimum;
		playgroundParticles.initialLocalVelocityMax = velocityMaximum;
		playgroundParticles.previousLocalVelocityMin = playgroundParticles.initialLocalVelocityMin;
		playgroundParticles.previousLocalVelocityMax = playgroundParticles.initialLocalVelocityMax;
	}
	
	// Set material for particle system
	static function SetMaterial (playgroundParticles : PlaygroundParticles, particleMaterial : Material) {
		playgroundParticles.particleSystemRenderer.sharedMaterial = particleMaterial;
	}
	
	// Set alphas for particles
	static function SetAlpha (playgroundParticles:PlaygroundParticles, alpha : float) {
		for (var i = 0; i<playgroundParticles.particleCache.particles.Length; i++)
			playgroundParticles.particleCache.particles[i].color.a = alpha;
	}
	
	// Set particle random particle positions within min- and max range
	static function Random (playgroundParticles : PlaygroundParticles, min : Vector3, max : Vector3) {
		for (var p : ParticleSystem.Particle in playgroundParticles.particleCache.particles) {
			p.position = Vector3(
				UnityEngine.Random.Range(min.x, max.x),
				UnityEngine.Random.Range(min.y, max.y),
				UnityEngine.Random.Range(min.z, max.z)
			);
		}
		Update(playgroundParticles);
	}
	
	// Move all particles in direction
	static function Translate (playgroundParticles:PlaygroundParticles, direction:Vector3) {
		for (var i = 0; i<playgroundParticles.particleCache.particles.Length; i++)
			playgroundParticles.particleCache.particles[i].position += direction;
	}
	
	// Add new state from state
	static function Add (playgroundParticles : PlaygroundParticles, state : ParticleState) {
		playgroundParticles.states.Add(state);
		state.Initialize();
	}
	
	// Add new state from image
	static function Add (playgroundParticles : PlaygroundParticles, image : Texture2D, scale : float, offset : Vector3, stateName : String, stateTransform : Transform) {
		playgroundParticles.states.Add(new ParticleState());
		playgroundParticles.states[playgroundParticles.states.Count-1].ConstructParticles(image,scale,offset,stateName,stateTransform);
	}
	
	// Add new state from image with depthmap
	static function Add (playgroundParticles:PlaygroundParticles, image:Texture2D, depthmap:Texture2D, depthmapStrength:float, scale:float, offset:Vector3, stateName:String, stateTransform:Transform) {
		playgroundParticles.states.Add(new ParticleState());
		playgroundParticles.states[playgroundParticles.states.Count-1].ConstructParticles(image,scale,offset,stateName,stateTransform);
		playgroundParticles.states[playgroundParticles.states.Count-1].stateDepthmap = depthmap;
		playgroundParticles.states[playgroundParticles.states.Count-1].stateDepthmapStrength = depthmapStrength;
	}
	
	// Destroy a PlaygroundParticles object
	static function Destroy (playgroundParticles : PlaygroundParticles) {
		Clear(playgroundParticles);
		MonoBehaviour.DestroyImmediate(playgroundParticles.particleSystemGameObject);
		playgroundParticles = null;
	}
	
	// Sorts the particles in lifetime
	static function SetLifetime (playgroundParticles : PlaygroundParticles, time : float) {
		if (playgroundParticles.playgroundCache.targetPosition==null) return;
		playgroundParticles.lifetime = time;
		playgroundParticles.playgroundCache.lifetimeOffset = new float[playgroundParticles.particleCount];
		var currentCount : int = playgroundParticles.particleCount;
		
		if (playgroundParticles.source!=SOURCE.Script) {
			switch (playgroundParticles.sorting) {
				case SORTING.Scrambled:
					playgroundParticles.playgroundCache.lifetimeOffset = Playground.RandomFloat(playgroundParticles.particleCount, .0, playgroundParticles.lifetime);
				break;
				case SORTING.ScrambledLinear:
					var slPerc : float;
					for (var sl = 0; sl<playgroundParticles.particleCount; sl++) {
						if (currentCount!=playgroundParticles.particleCount) return;
						slPerc = (sl*1.0)/(playgroundParticles.particleCount*1.0);
						playgroundParticles.playgroundCache.lifetimeOffset[sl] = playgroundParticles.lifetime*slPerc;
					}
					Playground.ShuffleFloat(playgroundParticles.playgroundCache.lifetimeOffset);
				break;
				case SORTING.Burst:
					// No action needed for spawning all particles at once
				break;
				case SORTING.Reversed:
					var lPerc : float;
					for (var l = 0; l<playgroundParticles.particleCount; l++) {
						if (currentCount!=playgroundParticles.particleCount) return;
						lPerc = (l*1.0)/(playgroundParticles.particleCount*1.0);
						playgroundParticles.playgroundCache.lifetimeOffset[l] = playgroundParticles.lifetime*lPerc;
					}
				break;
				case SORTING.Linear:
					var rPerc : float;
					var rInc : int;
					for (var r = playgroundParticles.particleCount-1; r>=0; r--) {
						if (currentCount!=playgroundParticles.particleCount) return;
						rPerc = (rInc*1.0)/(playgroundParticles.particleCount*1.0);
						rInc++;
						playgroundParticles.playgroundCache.lifetimeOffset[r] = playgroundParticles.lifetime*rPerc;
					}
				break;
				case SORTING.NearestNeighbor:
					playgroundParticles.nearestNeighborOrigin = Mathf.Clamp(playgroundParticles.nearestNeighborOrigin, 0, playgroundParticles.particleCount-1);
					var nnDist : float[] = new float[playgroundParticles.particleCount];
					var nnHighest : float;
					for (var nn = 0; nn<playgroundParticles.particleCount; nn++) {
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
				case SORTING.NearestNeighborReversed:
					playgroundParticles.nearestNeighborOrigin = Mathf.Clamp(playgroundParticles.nearestNeighborOrigin, 0, playgroundParticles.particleCount-1);
					var nnrDist : float[] = new float[playgroundParticles.particleCount];
					var nnrHighest : float;
					for (var nnr = 0; nnr<playgroundParticles.particleCount; nnr++) {
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
				case SORTING.Custom:
					for (var cs = playgroundParticles.particleCount-1; cs>=0; cs--) {
						playgroundParticles.playgroundCache.lifetimeOffset[cs] = playgroundParticles.lifetime*playgroundParticles.lifetimeSorting.Evaluate(cs*1.0/playgroundParticles.particleCount*1.0);
					}
				break;
			}
		}
		SetEmissionRate(playgroundParticles);
		SetParticleTimeNow(playgroundParticles);
		playgroundParticles.previousLifetime = time;
	}
	
	// Set emission rate percentage of particle count
	static function SetEmissionRate (playgroundParticles : PlaygroundParticles) {
		var rateCount : float = playgroundParticles.lifetime*playgroundParticles.emissionRate;
		var currentCount : int = playgroundParticles.particleCount;
		for (var p = 0; p<playgroundParticles.particleCount; p++) {
			if (currentCount!=playgroundParticles.particleCount) return;
			if (playgroundParticles.emissionRate!=0 && playgroundParticles.source!=SOURCE.Script) {
				if (playgroundParticles.sorting!=SORTING.Burst || playgroundParticles.sorting==SORTING.NearestNeighbor && playgroundParticles.overflowOffset!=Vector3.zero || playgroundParticles.sorting==SORTING.NearestNeighborReversed && playgroundParticles.overflowOffset!=Vector3.zero) {
					playgroundParticles.playgroundCache.emission[p] = (playgroundParticles.playgroundCache.lifetimeOffset[p]>=playgroundParticles.lifetime-rateCount && playgroundParticles.emit);
				} else {
					playgroundParticles.playgroundCache.emission[p] = (playgroundParticles.emit && playgroundParticles.emissionRate>(p/currentCount));
				}
			} else playgroundParticles.playgroundCache.emission[p] = false;
			if (playgroundParticles.playgroundCache.emission[p])
				playgroundParticles.playgroundCache.rebirth[p] = true;
			else if (playgroundParticles.source==SOURCE.Script)
				playgroundParticles.playgroundCache.rebirth[p] = false;
		}
		playgroundParticles.previousEmissionRate = playgroundParticles.emissionRate;
	}
	
	// Set life and death of particles
	static function SetParticleTimeNow (playgroundParticles : PlaygroundParticles) {
		if (playgroundParticles.playgroundCache.lifetimeOffset==null || playgroundParticles.playgroundCache.lifetimeOffset.Length!=playgroundParticles.particleCount) return;
		playgroundParticles.playgroundCache.birth = new float[playgroundParticles.particleCount];
		playgroundParticles.playgroundCache.birthDelay = new float[playgroundParticles.particleCount];
		playgroundParticles.playgroundCache.death = new float[playgroundParticles.particleCount];  
		playgroundParticles.playgroundCache.life = new float[playgroundParticles.particleCount];
		if (playgroundParticles.source!=SOURCE.Script) {
			var currentTime : float = Playground.globalTime+playgroundParticles.lifetimeOffset;
			var currentCount : int = playgroundParticles.particleCount;
			playgroundParticles.simulationStarted = currentTime;
			var p : int = 0;	
			if (playgroundParticles.sorting!=SORTING.Burst || playgroundParticles.sorting==SORTING.NearestNeighbor && playgroundParticles.overflowOffset!=Vector3.zero || playgroundParticles.sorting==SORTING.NearestNeighborReversed && playgroundParticles.overflowOffset!=Vector3.zero) {
				for (p = 0; p<playgroundParticles.particleCount; p++) {
					if (currentCount!=playgroundParticles.particleCount) return;
					playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime-(playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeOffset[p]);
					playgroundParticles.playgroundCache.birth[p] = currentTime-playgroundParticles.playgroundCache.life[p];
					playgroundParticles.playgroundCache.death[p] = currentTime+(playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeOffset[p]);
					playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].startLifetime = playgroundParticles.lifetime;
					playgroundParticles.playgroundCache.targetPosition[p] = Playground.initialTargetPosition;
					playgroundParticles.particleCache.particles[p].position = Playground.initialTargetPosition;
				}
			} else {
				currentTime = Playground.globalTime;
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
	static function SetParticleTimeNowWithRestEmission (playgroundParticles : PlaygroundParticles) {
		var currentTime : float = Playground.globalTime+playgroundParticles.lifetimeOffset;
		var emissionDelta : float = Playground.globalTime-playgroundParticles.emissionStopped;
		var applyDelta : boolean = false;
		if (emissionDelta<playgroundParticles.lifetime && emissionDelta>0)
			applyDelta = true;
		for (var p = 0; p<playgroundParticles.particleCount; p++) {
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
	static function GetColorAtLifetime (playgroundParticles : PlaygroundParticles, time : float) : Color32 {
		return playgroundParticles.lifetimeColor.Evaluate(time/playgroundParticles.lifetime);
	}
	
	// Color all particles from evaluated lifetime color value where time is normalized
	static function SetColorAtLifetime (playgroundParticles : PlaygroundParticles, time : float) {
		var c : Color32 = playgroundParticles.lifetimeColor.Evaluate(time/playgroundParticles.lifetime);
		for (var p = 0; p<playgroundParticles.particleCount; p++)
			playgroundParticles.particleCache.particles[p].color = c;
		playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache.particles, playgroundParticles.particleCache.particles.Length);
	}
	
	// Color all particles from lifetime span with sorting
	static function SetColorWithLifetimeSorting (playgroundParticles : PlaygroundParticles) {
		SetLifetime(playgroundParticles, playgroundParticles.lifetime);
		var c : Color32;
		for (var p = 0; p<playgroundParticles.particleCount; p++) {
			c = playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime);
			playgroundParticles.particleCache.particles[p].color = c;
		}
		playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache.particles, playgroundParticles.particleCache.particles.Length);
	}
	
	// Set position of emission when using a Transform together with Overflow Offset
	static function SetTransformPositionWithOffset (playgroundParticles : PlaygroundParticles) {
		for (var p = 0; p<playgroundParticles.particleCount; p++)
			playgroundParticles.particleCache.particles[p].position = playgroundParticles.sourceTransform.position+GetOverflowOffset(playgroundParticles, p, 1);
		playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache.particles, playgroundParticles.particleCache.particles.Length);
	}
	
	// Sets the amount of particles and initiates the necessary arrays
	static function SetParticleCount (playgroundParticles : PlaygroundParticles, amount : int) {
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
		playgroundParticles.playgroundCache.rebirth = new boolean[amount];
		playgroundParticles.playgroundCache.emission = new boolean[amount];
		playgroundParticles.playgroundCache.changedByProperty = new boolean[amount];
		playgroundParticles.playgroundCache.changedByPropertyColor = new boolean[amount];
		playgroundParticles.playgroundCache.changedByPropertyColorLerp = new boolean[amount];
		playgroundParticles.playgroundCache.changedByPropertyColorKeepAlpha = new boolean[amount];
		playgroundParticles.playgroundCache.changedByPropertySize = new boolean[amount];
		playgroundParticles.playgroundCache.changedByPropertyTarget = new boolean[amount];
		playgroundParticles.playgroundCache.changedByPropertyDeath = new boolean[amount];
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
		playgroundParticles.playgroundCache.initialRotation = Playground.RandomFloat(amount, playgroundParticles.initialRotationMin, playgroundParticles.initialRotationMax);
		playgroundParticles.playgroundCache.rotationSpeed = Playground.RandomFloat(amount, playgroundParticles.rotationSpeedMin, playgroundParticles.rotationSpeedMax);
		playgroundParticles.previousInitialRotationMin = playgroundParticles.initialRotationMin;
		playgroundParticles.previousInitialRotationMax = playgroundParticles.initialRotationMax;
		playgroundParticles.previousRotationSpeedMin = playgroundParticles.rotationSpeedMin;
		playgroundParticles.previousRotationSpeedMax = playgroundParticles.rotationSpeedMax;
		
		// Set velocities
		SetVelocityRandom(playgroundParticles, playgroundParticles.initialVelocityMin, playgroundParticles.initialVelocityMax);
		SetLocalVelocityRandom(playgroundParticles, playgroundParticles.initialLocalVelocityMin, playgroundParticles.initialLocalVelocityMax);
		
		// Garbage Collect
		if (Playground.reference!=null && Playground.reference.garbageCollectOnResize)
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
			if (playgroundParticles.source==SOURCE.Transform && playgroundParticles.overflowOffset!=Vector3.zero) {
				SetTransformPositionWithOffset(playgroundParticles);
				SetColorWithLifetimeSorting(playgroundParticles);
			} else if (playgroundParticles.source==SOURCE.WorldObject && playgroundParticles.worldObject!=null) {
				SetPosition(playgroundParticles, playgroundParticles.worldObject);
				SetColorWithLifetimeSorting(playgroundParticles);
			} else if (playgroundParticles.source==SOURCE.SkinnedWorldObject && playgroundParticles.skinnedWorldObject!=null) {
				SetPosition(playgroundParticles, playgroundParticles.skinnedWorldObject);
				SetColorWithLifetimeSorting(playgroundParticles);
			}
		}
		
		// Misc
		playgroundParticles.isPainting = false;
	}
	
	// Updates a PlaygroundParticles object (called from Playground)
	static function Update (playgroundParticles : PlaygroundParticles) {
		
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
			if (playgroundParticles.initialVelocityMin!=playgroundParticles.previousVelocityMin || playgroundParticles.initialVelocityMax!=playgroundParticles.previousVelocityMax || !playgroundParticles.playgroundCache.initialVelocity || playgroundParticles.playgroundCache.initialVelocity.Length!=playgroundParticles.particleCount)
				SetVelocityRandom(playgroundParticles, playgroundParticles.initialVelocityMin, playgroundParticles.initialVelocityMax);
			
		// Particle local velocity
		if (playgroundParticles.applyInitialLocalVelocity)
			if (playgroundParticles.initialLocalVelocityMin!=playgroundParticles.previousLocalVelocityMin || playgroundParticles.initialLocalVelocityMax!=playgroundParticles.previousLocalVelocityMax || !playgroundParticles.playgroundCache.initialLocalVelocity || playgroundParticles.playgroundCache.initialLocalVelocity.Length!=playgroundParticles.particleCount)
				SetLocalVelocityRandom(playgroundParticles, playgroundParticles.initialLocalVelocityMin, playgroundParticles.initialLocalVelocityMax);
		
		// Particle life
		if (playgroundParticles.previousLifetime!=playgroundParticles.lifetime)
			SetLifetime(playgroundParticles, playgroundParticles.lifetime);
			
		// Particle emission rate
		if (playgroundParticles.previousEmissionRate!=playgroundParticles.emissionRate)
			SetEmissionRate(playgroundParticles);
		
		// Particle state change transition
		if (playgroundParticles.source==SOURCE.State && playgroundParticles.activeState!=playgroundParticles.previousActiveState) {
			if (playgroundParticles.states[playgroundParticles.activeState].positionLength>playgroundParticles.particleCount)
				SetParticleCount(playgroundParticles, playgroundParticles.states[playgroundParticles.activeState].positionLength);
			playgroundParticles.InitTransition(playgroundParticles);
			playgroundParticles.previousActiveState = playgroundParticles.activeState;
		}
		
		// Particle calculation
		if (Playground.reference.calculate && playgroundParticles.calculate && !playgroundParticles.inTransition)
			Calculate(playgroundParticles);
		else playgroundParticles.cameFromNonCalculatedFrame = true;
		
		// Assign all particles into the particle system
		if (!playgroundParticles.inTransition && playgroundParticles.particleCache!=null && playgroundParticles.particleCache.particles.Length>0)
			playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache.particles, playgroundParticles.particleCache.particles.Length);
	
		// Make sure that variables are reset till next frame
		playgroundParticles.isPainting = false;		
	}
	
	// Initial target position
	static function SetInitialTargetPosition (playgroundParticles : PlaygroundParticles, position : Vector3) {
		for (var p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
			playgroundParticles.playgroundCache.previousTargetPosition[p] = position;
			playgroundParticles.playgroundCache.targetPosition[p] = position;
			playgroundParticles.particleCache.particles[p].position = position;
		}
		playgroundParticles.particleSystem.SetParticles(playgroundParticles.particleCache.particles, playgroundParticles.particleCache.particles.Length);
	}
	
	// Transition event on activeState change
	function InitTransition (playgroundParticles : PlaygroundParticles) {
		playgroundParticles.Transition(playgroundParticles);
	}
	function Transition (playgroundParticles : PlaygroundParticles) {
		
		playgroundParticles.inTransition = true;
		
		var p : int;
		var t : float = .0;
		var timeTransitionStarted : float = Time.realtimeSinceStartup;
		var timeTransitionFinishes : float = timeTransitionStarted+playgroundParticles.transitionTime;
		var thisState : int = playgroundParticles.activeState;
		var currentPosition : Vector3[];
		var currentColor : Color32[];
		var targetColor : Color32;
		var calculatedoverflowOffset : Vector3;
		
		if (playgroundParticles.states[playgroundParticles.activeState].stateMesh!=null && playgroundParticles.states[playgroundParticles.activeState].stateTexture==null)
			targetColor = GetColorAtLifetime(playgroundParticles, 0);
		
		switch (playgroundParticles.transition) {
		
			case TRANSITION.None:
				// Set all particles directly
				for (p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
				
					// overflow offset
					if (playgroundParticles.overflowOffset!=Vector3.zero)
						calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, p, playgroundParticles.states[playgroundParticles.activeState].positionLength);
				
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
			
			case TRANSITION.Lerp:
				currentPosition = new Vector3[playgroundParticles.particleCache.particles.Length];
				currentColor = new Color32[playgroundParticles.particleCache.particles.Length];
				
				// Set target, current positions and colors
				for (p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
					
					// Overflow offset
					if (playgroundParticles.overflowOffset!=Vector3.zero)
						calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, p, playgroundParticles.states[playgroundParticles.activeState].positionLength);
					
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
					
					yield;
				}
			break;
			
			case TRANSITION.Fade: case TRANSITION.Fade2:
				
				var isFade2 : boolean = (playgroundParticles.transition==TRANSITION.Fade2);
				
				// Set colors (and positions if Fade2)
				currentColor = new Color32[playgroundParticles.particleCache.particles.Length];
				if (isFade2)
					currentPosition = new Vector3[playgroundParticles.particleCache.particles.Length];
				for (p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
					currentColor[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].color;
					if (isFade2) {
						currentPosition[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.particleCache.particles[p%playgroundParticles.particleCache.particles.Length].position;
						
						if (playgroundParticles.overflowOffset!=Vector3.zero)
							calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, p, playgroundParticles.states[playgroundParticles.activeState].positionLength);
						if (playgroundParticles.states[playgroundParticles.activeState].stateTransform!=null) 
							playgroundParticles.playgroundCache.targetPosition[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.states[playgroundParticles.activeState].GetParentedPosition(p%playgroundParticles.particleCache.particles.Length)+calculatedoverflowOffset;
						else
							playgroundParticles.playgroundCache.targetPosition[p%playgroundParticles.particleCache.particles.Length] = playgroundParticles.states[playgroundParticles.activeState].GetPosition(p%playgroundParticles.particleCache.particles.Length)+calculatedoverflowOffset;
					}
				}
				
				var transitionColor : Color;
				var transitionAlpha : float;
				var fadeOut : boolean = true;
				var setPositions : boolean = true;
				
				timeTransitionStarted = Time.realtimeSinceStartup;
				
				while (timeTransitionStarted+t<timeTransitionFinishes && playgroundParticles.activeState==thisState && playgroundParticles.inTransition) {
					
					// Update time
					t = (Time.realtimeSinceStartup-timeTransitionStarted);

					for (p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
						
						// Get color and set alpha
						if (fadeOut) {
							transitionColor = currentColor[p%playgroundParticles.particleCache.particles.Length];
							transitionAlpha = Mathf.Lerp(transitionColor.a, 0, t/(playgroundParticles.transitionTime*.5));
							if (t/(playgroundParticles.transitionTime*.5)>=1.0) {
								transitionAlpha = 0;
								fadeOut=false;
							}
						} else {
							if (playgroundParticles.states[playgroundParticles.activeState].stateMesh==null && playgroundParticles.states[playgroundParticles.activeState].stateTexture!=null)
								transitionColor = playgroundParticles.states[playgroundParticles.activeState%playgroundParticles.states.Count].GetColor(p%playgroundParticles.particleCache.particles.Length);
							else
								transitionColor = targetColor;
							transitionAlpha = Mathf.Lerp(0, transitionColor.a, -1.0+(t/(playgroundParticles.transitionTime*.5)));
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
					
					yield;
				}
				
			break;
		}
		
		// No longer in transition
		if (playgroundParticles.activeState==thisState) {
			SetParticleTimeNow(playgroundParticles);
			playgroundParticles.inTransition = false;
		}
	}
	
	// Set emission of PlaygroundParticles object
	static function Emission (playgroundParticles : PlaygroundParticles, emission : boolean, callRestEmission : boolean) {
		playgroundParticles.previousEmission = emission;
		if (emission) {
			for (var p = 0; p<playgroundParticles.playgroundCache.rebirth.Length; p++)
				playgroundParticles.playgroundCache.rebirth[p] = true;
			if (callRestEmission)
				SetParticleTimeNowWithRestEmission(playgroundParticles);
		}
	}
	
	// Returns the angle between a and b with normal direction
	static function SignedAngle (a : Vector3, b : Vector3, n : Vector3) : float {
	    return (Vector3.Angle(a, b)*Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b))));
	}
	
	// Returns a random value between negative- and positive Vector3
	static function RandomVector3 (v1 : Vector3, v2 : Vector3) : Vector3 {
		return Vector3(UnityEngine.Random.Range(v1.x,v2.x), UnityEngine.Random.Range(v1.y,v2.y), UnityEngine.Random.Range(v1.z,v2.z));
	}
	
	// Run all calculations for this PlaygroundParticles object
	static function Calculate (playgroundParticles : PlaygroundParticles) {
		
		if (!playgroundParticles.enabled) return;
		
		var distance : float;
		var prevP : int;
		var spreadP : int;
		var prevSpreadP : int;
		var deltaVelocity : Vector3;
		var localSpace : boolean = (playgroundParticles.shurikenParticleSystem.simulationSpace == ParticleSystemSimulationSpace.Local);
		var hitInfo : RaycastHit;
		var ray : Ray;
		var currentSource : SOURCE = playgroundParticles.source;
		var currentState = playgroundParticles.activeState;
		var lifetimeColor : Color;
		var calculatedoverflowOffset : Vector3;
		var m : int;
		var evaluatedLife : float;
		var t : float = (Playground.globalDeltaTime*playgroundParticles.particleTimescale)*(playgroundParticles.updateRate*1.0);
		
		// Get data from source
		if (playgroundParticles.emit) {
			switch (playgroundParticles.source) {
				case SOURCE.State:
					if (playgroundParticles.states.Count==0)
						return;
				break;
				case SOURCE.Transform:
					if (playgroundParticles.sourceTransform==null)
						return;
				break;
				case SOURCE.WorldObject:
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
							GetProceduralPosition(playgroundParticles.worldObject.vertexPositions, playgroundParticles.worldObject);
						if (playgroundParticles.worldObjectUpdateNormals)
							GetNormals(playgroundParticles.worldObject.normals, playgroundParticles.worldObject);
					} else return;
				break;
				case SOURCE.SkinnedWorldObject:
					// Handle vertex data in active Skinned World Object
					if (playgroundParticles.skinnedWorldObject.gameObject!=null && playgroundParticles.skinnedWorldObject.mesh!=null) {
						if (playgroundParticles.skinnedWorldObject.gameObject.GetInstanceID()!=playgroundParticles.skinnedWorldObject.cachedId)
							NewSkinnedWorldObject(playgroundParticles, playgroundParticles.skinnedWorldObject.gameObject.transform);
						if (Time.frameCount%Playground.skinnedUpdateRate==0) {
							GetPosition(playgroundParticles.skinnedWorldObject.vertexPositions, playgroundParticles.skinnedWorldObject.normals, playgroundParticles.skinnedWorldObject);
						}
					} else return;
				break;
				case SOURCE.Paint:
					if (playgroundParticles.paint==null)
						return;
				break;
				case SOURCE.Projection:
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
		for (var p = 0; p<playgroundParticles.particleCache.particles.Length; p++) {
			if (currentSource!=playgroundParticles.source || playgroundParticles.particleCache.particles.Length!=playgroundParticles.particleCount) return;
			
			// Zero out Shuriken velocity (for the applied stretching)
			playgroundParticles.particleCache.particles[p].velocity = Vector3.zero;
			
			// If this particle is set to rebirth
			if (playgroundParticles.playgroundCache.rebirth[p]) {

				// Calculate lifetime
				evaluatedLife = (Playground.globalTime-playgroundParticles.playgroundCache.birth[p])/playgroundParticles.lifetime;
				 				 
				// Previous particle
				prevP = p>0?p-1:playgroundParticles.particleCache.particles.Length-1;
				
				// Lifetime
				if (playgroundParticles.playgroundCache.life[p]<playgroundParticles.lifetime) {
					playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime*evaluatedLife;
					playgroundParticles.particleCache.particles[p].lifetime = Mathf.Clamp(playgroundParticles.lifetime - playgroundParticles.playgroundCache.life[p], .1, playgroundParticles.lifetime);
				} else {
					
					// Loop exceeded
					if (!playgroundParticles.loop && Playground.globalTime>playgroundParticles.simulationStarted+playgroundParticles.lifetime-.01) {
						playgroundParticles.loopExceeded = true;
						
						if (playgroundParticles.disableOnDone && playgroundParticles.loopExceededOnParticle==p && evaluatedLife>2)
							playgroundParticles.particleSystemGameObject.SetActive(false);
						if (playgroundParticles.loopExceededOnParticle==-1)
							playgroundParticles.loopExceededOnParticle = p;
					}
					
					// New cycle begins
					if (Playground.globalTime>=playgroundParticles.playgroundCache.birth[p]+playgroundParticles.playgroundCache.birthDelay[p] && !playgroundParticles.loopExceeded && playgroundParticles.source!=SOURCE.Script) {
						if (!playgroundParticles.playgroundCache.changedByPropertyDeath[p] || playgroundParticles.playgroundCache.changedByPropertyDeath[p] && Playground.globalTime>playgroundParticles.playgroundCache.death[p])
							Rebirth(playgroundParticles, p);
						else playgroundParticles.particleCache.particles[p].position = Playground.initialTargetPosition;
							
					} else playgroundParticles.particleCache.particles[p].position = Playground.initialTargetPosition;
					
				}
				
				// Particle is alive
				if (playgroundParticles.playgroundCache.life[p]<=Playground.globalTime+playgroundParticles.lifetime) {
				
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
						case SOURCE.Script:
						
							// Set scripted color
							if (playgroundParticles.colorSource==COLORSOURCE.Source)
								playgroundParticles.particleCache.particles[p].color = playgroundParticles.playgroundCache.color[p];
							
						break;
						case SOURCE.State:
						
							// Calculate State
							if (playgroundParticles.activeState!=currentState)
								return;
							if (playgroundParticles.states.Count>0 && playgroundParticles.states[playgroundParticles.activeState]!=null && playgroundParticles.states[playgroundParticles.activeState].initialized) {
								
								// Previous target position (for delta movement, available when having a transform assigned)
								if (playgroundParticles.states[playgroundParticles.activeState].stateTransform) {
									if (playgroundParticles.overflowOffset!=Vector3.zero)
										calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, prevP, playgroundParticles.states[playgroundParticles.activeState].positionLength);
									playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.states[playgroundParticles.activeState].GetParentedPosition(prevP)+calculatedoverflowOffset;
								} else {
									playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.playgroundCache.targetPosition[prevP];
								}
								
								// Overflow offset this particle
								if (playgroundParticles.overflowOffset!=Vector3.zero)
									calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, p, playgroundParticles.states[playgroundParticles.activeState].positionLength);
								
								// Position and color
								if (playgroundParticles.states[playgroundParticles.activeState].stateTransform!=null) 
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.states[playgroundParticles.activeState].GetParentedPosition(p)+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.states[playgroundParticles.activeState].stateTransform.position);
								else
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.states[playgroundParticles.activeState].GetPosition(p)+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.particleSystemTransform.position);

								
								if (playgroundParticles.colorSource==COLORSOURCE.Source) 
									playgroundParticles.particleCache.particles[p].color = playgroundParticles.states[playgroundParticles.activeState].GetColor(p);
								
								// Transform direction velocity
								if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.playgroundCache.life[p]==0 && playgroundParticles.states[playgroundParticles.activeState].stateTransform!=null && !playgroundParticles.onlySourcePositioning) {
									playgroundParticles.playgroundCache.initialLocalVelocity[p] = !localSpace?
										playgroundParticles.states[playgroundParticles.activeState].stateTransform.TransformDirection(
											Vector3(UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
													UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
													UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
											)
										)
									:
										Vector3(UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
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
								playgroundParticles.playgroundCache.previousTargetPosition[p] = Playground.initialTargetPosition;
								playgroundParticles.playgroundCache.targetPosition[p] = Playground.initialTargetPosition;
							}
						break;
						case SOURCE.Transform:
						
							// Calculate Transform
							if (playgroundParticles.sourceTransform!=null) {
								
								// Overflow offset previous particle
								if (playgroundParticles.overflowOffset!=Vector3.zero)
									calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, prevP, 1);
								
								// Previous target position (for delta movement)
								playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.sourceTransform.position+calculatedoverflowOffset;
								
								// Overflow offset this particle
								if (playgroundParticles.overflowOffset!=Vector3.zero)
									calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, p, 1);
								
								// Position on transform
								if (playgroundParticles.overflowOffset!=Vector3.zero)
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.sourceTransform.position+GetOverflowOffset(playgroundParticles, p, 1)+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.sourceTransform.position);
								else
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.sourceTransform.position+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.sourceTransform.position);
								
								// Normal direction velocity
								if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.playgroundCache.life[p]==0 && !playgroundParticles.onlySourcePositioning) {
									playgroundParticles.playgroundCache.initialLocalVelocity[p] = !localSpace?
										playgroundParticles.sourceTransform.TransformDirection(
											Vector3(UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
													UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
													UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
											)
										)
									:
										Vector3(UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
												UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
												UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
										);
									playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.playgroundCache.initialLocalVelocity[p];
									
									// Give this spawning particle its velocity shape
									if (playgroundParticles.applyInitialVelocityShape)
										playgroundParticles.playgroundCache.velocity[p] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((p*1.0)/(playgroundParticles.particleCount*1.0)));
								}
								
								// Set target positions accordingly to local simulation space
								if (localSpace) {
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.sourceTransform.InverseTransformPoint(playgroundParticles.playgroundCache.targetPosition[p]);
									playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.sourceTransform.InverseTransformPoint(playgroundParticles.playgroundCache.previousTargetPosition[p]);
								}
							}
						break;
						case SOURCE.WorldObject:
						
							// Calculate World Object
							if (playgroundParticles.worldObject.gameObject!=null) {
								
								// Overflow offset previous particle
								if (playgroundParticles.overflowOffset!=Vector3.zero)
									calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, prevP, playgroundParticles.worldObject.vertexPositions.Length);
								
								// Previous target position (for delta movement)
								if (!playgroundParticles.worldObjectUpdateVertices)
									playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.worldObject.transform.TransformPoint(playgroundParticles.worldObject.vertexPositions[prevP%playgroundParticles.worldObject.vertexPositions.Length])+calculatedoverflowOffset;
								else
									playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.worldObject.vertexPositions[prevP%playgroundParticles.worldObject.vertexPositions.Length]+calculatedoverflowOffset;
								
								// Overflow offset this particle
								if (playgroundParticles.overflowOffset!=Vector3.zero)
									calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, p, playgroundParticles.worldObject.vertexPositions.Length);
								
								// Position towards vertices
								if (!playgroundParticles.worldObjectUpdateVertices)	
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.worldObject.transform.TransformPoint(playgroundParticles.worldObject.vertexPositions[p%playgroundParticles.worldObject.vertexPositions.Length])+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.worldObject.transform.position);
								else
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.worldObject.vertexPositions[p%playgroundParticles.worldObject.vertexPositions.Length]+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.worldObject.transform.position);
								
								// Normal direction velocity
								if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.playgroundCache.initialLocalVelocity[p]!=Vector3.zero && playgroundParticles.playgroundCache.life[p]==0 && !playgroundParticles.onlySourcePositioning) {
									playgroundParticles.playgroundCache.initialLocalVelocity[p] = !localSpace?
										playgroundParticles.worldObject.transform.TransformDirection(
											Vector3(playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length].x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
													playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length].y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
													playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length].z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
											)
										)
									:
										Vector3(playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length].x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
												playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length].y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
												playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length].z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
										);
									playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.playgroundCache.initialLocalVelocity[p];
									
									// Give this spawning particle its velocity shape
									if (playgroundParticles.applyInitialVelocityShape)
										playgroundParticles.playgroundCache.velocity[p] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((p*1.0)/(playgroundParticles.particleCount*1.0)));
								}
								
								// Set target positions accordingly to local simulation space
								if (localSpace) {
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.targetPosition[p]);
									playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.previousTargetPosition[p]);
								}
							}
						break;
						case SOURCE.SkinnedWorldObject:
						
							// Calculate Skinned World Object
							if (playgroundParticles.skinnedWorldObject.gameObject!=null) {
								
								// Source target spread
								spreadP = Mathf.RoundToInt(p*playgroundParticles.sourceDownResolution)%playgroundParticles.skinnedWorldObject.vertexPositions.Length;
								prevSpreadP = Mathf.RoundToInt(prevP*playgroundParticles.sourceDownResolution)%playgroundParticles.skinnedWorldObject.vertexPositions.Length;
								
								// Overflow offset previous particle
								if (playgroundParticles.overflowOffset!=Vector3.zero)
									calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, prevP, playgroundParticles.sourceDownResolution<=1?playgroundParticles.skinnedWorldObject.vertexPositions.Length:playgroundParticles.skinnedWorldObject.vertexPositions.Length/playgroundParticles.sourceDownResolution);
								
								// Previous target position (for delta movement)
								playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.skinnedWorldObject.vertexPositions[prevSpreadP]+calculatedoverflowOffset;
								
								// Overflow offset this particle
								if (playgroundParticles.overflowOffset!=Vector3.zero)
									calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, p, playgroundParticles.sourceDownResolution<=1?playgroundParticles.skinnedWorldObject.vertexPositions.Length:playgroundParticles.skinnedWorldObject.vertexPositions.Length/playgroundParticles.sourceDownResolution);
								
								// Position towards vertices
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.skinnedWorldObject.vertexPositions[spreadP]+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.skinnedWorldObject.transform.position);
								
								// Normal direction velocity
								if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.playgroundCache.initialLocalVelocity[p]!=Vector3.zero && playgroundParticles.playgroundCache.life[p]==0 && !playgroundParticles.onlySourcePositioning) {
									playgroundParticles.playgroundCache.initialLocalVelocity[p] = !localSpace?
										playgroundParticles.skinnedWorldObject.transform.TransformDirection(
											Vector3(playgroundParticles.skinnedWorldObject.normals[spreadP%playgroundParticles.skinnedWorldObject.normals.Length].x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
													playgroundParticles.skinnedWorldObject.normals[spreadP%playgroundParticles.skinnedWorldObject.normals.Length].y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
													playgroundParticles.skinnedWorldObject.normals[spreadP%playgroundParticles.skinnedWorldObject.normals.Length].z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
											)
										)
									:
										Vector3(playgroundParticles.skinnedWorldObject.normals[spreadP%playgroundParticles.skinnedWorldObject.normals.Length].x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
												playgroundParticles.skinnedWorldObject.normals[spreadP%playgroundParticles.skinnedWorldObject.normals.Length].y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
												playgroundParticles.skinnedWorldObject.normals[spreadP%playgroundParticles.skinnedWorldObject.normals.Length].z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
										);
									playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.playgroundCache.initialLocalVelocity[p];
									
									// Give this spawning particle its velocity shape
									if (playgroundParticles.applyInitialVelocityShape)
										playgroundParticles.playgroundCache.velocity[spreadP] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[spreadP], playgroundParticles.initialVelocityShape.Evaluate((spreadP*1.0)/(playgroundParticles.particleCount*1.0)));
								}
								
								// Set target positions accordingly to local simulation space
								if (localSpace) {
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.targetPosition[p]);
									playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.previousTargetPosition[p]);
								}
							}
						break;
						case SOURCE.Paint:
						
							// Calculate Paint
							if (playgroundParticles.paint!=null && playgroundParticles.paint.positionLength>0) {
								
								// Update current paint position regarding its transform parent
								playgroundParticles.paint.Update(p);
								
								// Overflow offset previous particle
								if (playgroundParticles.overflowOffset!=Vector3.zero)
									calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, prevP, playgroundParticles.paint.positionLength);
								
								// Previous target position (for delta movement)
								playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.paint.GetPosition(prevP)+calculatedoverflowOffset;
								
								// Overflow offset this particle
								if (playgroundParticles.overflowOffset!=Vector3.zero)
									calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, p, playgroundParticles.paint.positionLength);
								
								// Position and color
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.paint.GetPosition(p)+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.paint.GetParent(p).TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.paint.GetParent(p).position);

								if (playgroundParticles.colorSource==COLORSOURCE.Source && !playgroundParticles.playgroundCache.changedByPropertyColor[p] && !playgroundParticles.playgroundCache.changedByPropertyColorLerp[p]) 
									playgroundParticles.particleCache.particles[p].color = playgroundParticles.paint.GetColor(p);
								
								// Normal direction velocity
								if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.playgroundCache.initialLocalVelocity[p]!=Vector3.zero && playgroundParticles.playgroundCache.life[p]==0 && !playgroundParticles.onlySourcePositioning) {
									var normal : Vector3 = playgroundParticles.paint.GetNormal(p);
									playgroundParticles.playgroundCache.initialLocalVelocity[p] = !localSpace?
										playgroundParticles.paint.GetParent(p).TransformDirection(
											Vector3(normal.x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
													normal.y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
													normal.z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
											)
										)
									:
										Vector3(normal.x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
												normal.y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
												normal.z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
										);
									playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.playgroundCache.initialLocalVelocity[p];
									
									// Give this spawning particle its velocity shape
									if (playgroundParticles.applyInitialVelocityShape)
										playgroundParticles.playgroundCache.velocity[p] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((p*1.0)/(playgroundParticles.particleCount*1.0)));
									
								}
							
								// Set target positions accordingly to local simulation space
								if (localSpace) {
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.targetPosition[p]);
									playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.previousTargetPosition[p]);
								}
							} else {
								playgroundParticles.playgroundCache.previousTargetPosition[p] = Playground.initialTargetPosition;
								playgroundParticles.playgroundCache.targetPosition[p] = Playground.initialTargetPosition;
							}
						break;
						case SOURCE.Projection:
							if (playgroundParticles.projection.initialized && playgroundParticles.projection.positionLength>0) {
								
								// Update current projected position regarding its texture and transform
								if (playgroundParticles.projection.liveUpdate)
									playgroundParticles.projection.Update(p);
								
								if (playgroundParticles.projection.GetParent(p)!=null) {
								
									// Previous target position and overflow
									if (playgroundParticles.projection.GetParent(prevP)) {
										if (playgroundParticles.overflowOffset!=Vector3.zero) {
											calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, prevP, playgroundParticles.projection.positionLength);
										}
										
										// Previous target position
										playgroundParticles.playgroundCache.previousTargetPosition[prevP] = playgroundParticles.projection.GetPosition(prevP)+calculatedoverflowOffset;
									}
									
									// Overflow offset this particle
									if (playgroundParticles.overflowOffset!=Vector3.zero)
										calculatedoverflowOffset = GetOverflowOffset(playgroundParticles, p, playgroundParticles.projection.positionLength);
										
									// Position and color
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.projection.GetPosition(p)+calculatedoverflowOffset+(!localSpace?playgroundParticles.playgroundCache.scatterPosition[p]:playgroundParticles.projection.GetParent(p).TransformPoint(playgroundParticles.playgroundCache.scatterPosition[p])-playgroundParticles.projection.GetParent(p).position);
									
									if (playgroundParticles.colorSource==COLORSOURCE.Source && !playgroundParticles.playgroundCache.changedByPropertyColor[p] && !playgroundParticles.playgroundCache.changedByPropertyColorLerp[p]) 
										playgroundParticles.particleCache.particles[p].color = playgroundParticles.projection.GetColor(p);
								
									// Normal direction velocity
									if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.playgroundCache.initialLocalVelocity[p]!=Vector3.zero && playgroundParticles.playgroundCache.life[p]==0 && !playgroundParticles.onlySourcePositioning) {
										var projNormal : Vector3 = playgroundParticles.projection.GetNormal(p);
										playgroundParticles.playgroundCache.initialLocalVelocity[p] = !localSpace?
											playgroundParticles.projection.GetParent(p).TransformDirection(
												Vector3(projNormal.x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
														projNormal.y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
														projNormal.z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
												)
											)
										:
											Vector3(projNormal.x*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x),
													projNormal.y*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.y, playgroundParticles.initialLocalVelocityMax.y),
													projNormal.z*UnityEngine.Random.Range(playgroundParticles.initialLocalVelocityMin.z, playgroundParticles.initialLocalVelocityMax.z)
											);
										playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.playgroundCache.initialLocalVelocity[p];
										
										// Give this spawning particle its velocity shape
										if (playgroundParticles.applyInitialVelocityShape)
											playgroundParticles.playgroundCache.velocity[p] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((p*1.0)/(playgroundParticles.particleCount*1.0)));
									}
									
									// Set target positions accordingly to local simulation space
									if (localSpace) {
										playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.targetPosition[p]);
										playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.playgroundCache.previousTargetPosition[p]);
									}
								} else {
									playgroundParticles.playgroundCache.previousTargetPosition[prevP] = Playground.initialTargetPosition;
									playgroundParticles.playgroundCache.targetPosition[prevP] = Playground.initialTargetPosition;
									playgroundParticles.playgroundCache.previousTargetPosition[p] = Playground.initialTargetPosition;
									playgroundParticles.playgroundCache.targetPosition[p] = Playground.initialTargetPosition;
								}
							} else {
								playgroundParticles.playgroundCache.previousTargetPosition[prevP] = Playground.initialTargetPosition;
								playgroundParticles.playgroundCache.targetPosition[prevP] = Playground.initialTargetPosition;
								playgroundParticles.playgroundCache.previousTargetPosition[p] = Playground.initialTargetPosition;
								playgroundParticles.playgroundCache.targetPosition[p] = Playground.initialTargetPosition;
							}
						break;
					}
					
					// Lifetime coloring
					if (!playgroundParticles.playgroundCache.changedByPropertyColor[p] && !playgroundParticles.playgroundCache.changedByPropertyColorLerp[p]) {
						if (playgroundParticles.colorSource!=COLORSOURCE.Source || playgroundParticles.colorSource==COLORSOURCE.Source && playgroundParticles.source!=SOURCE.State && playgroundParticles.source!=SOURCE.Paint && playgroundParticles.source!=SOURCE.Projection && playgroundParticles.source!=SOURCE.Script || playgroundParticles.source==SOURCE.State && playgroundParticles.states[playgroundParticles.activeState].stateMesh!=null && playgroundParticles.states[playgroundParticles.activeState].stateTexture==null)
							playgroundParticles.particleCache.particles[p].color = playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.lifetime>0?playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime:0);
						else if (playgroundParticles.colorSource==COLORSOURCE.Source && playgroundParticles.sourceUsesLifetimeAlpha) {
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
								Vector3(
									playgroundParticles.playgroundCache.velocity[p].x*playgroundParticles.velocityBending.x,
									playgroundParticles.playgroundCache.velocity[p].y*playgroundParticles.velocityBending.y, 
									playgroundParticles.playgroundCache.velocity[p].z*playgroundParticles.velocityBending.z
								),
								(playgroundParticles.playgroundCache.targetPosition[p]-playgroundParticles.particleCache.particles[p].position).normalized
							)*t;
						}
						
						// Delta velocity
						if (playgroundParticles.calculateDeltaMovement && playgroundParticles.playgroundCache.life[p]==0 && playgroundParticles.source!=SOURCE.Script && !playgroundParticles.isPainting) {
							deltaVelocity = playgroundParticles.playgroundCache.targetPosition[p]-(playgroundParticles.playgroundCache.previousTargetPosition[p]+playgroundParticles.playgroundCache.scatterPosition[p]);
							playgroundParticles.playgroundCache.velocity[p] += deltaVelocity*playgroundParticles.deltaMovementStrength;
							playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.playgroundCache.targetPosition[p];
						}
						
					}
					
					// Local Manipulators
					for (m = 0; m<playgroundParticles.manipulators.Count; m++)
						if (playgroundParticles.manipulators[m].transform!=null)
							CalculateManipulator(playgroundParticles, playgroundParticles.manipulators[m], p, t, playgroundParticles.particleCache.particles[p].position, localSpace?playgroundParticles.particleSystemTransform.InverseTransformPoint(playgroundParticles.manipulators[m].transform.position):playgroundParticles.manipulators[m].transform.position, localSpace);
					// Global Manipulators
					for (m = 0; m<Playground.reference.manipulators.Count; m++)
						if (Playground.reference.manipulators[m].transform!=null)
							CalculateManipulator(playgroundParticles, Playground.reference.manipulators[m], p, t, playgroundParticles.particleCache.particles[p].position, localSpace?playgroundParticles.particleSystemTransform.InverseTransformPoint(Playground.reference.manipulators[m].transform.position):Playground.reference.manipulators[m].transform.position, localSpace);
					
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
							//	playgroundParticles.particleCache.particles[p].position += playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.previousTransformPosition-playgroundParticles.particleSystemTransform.position)-playgroundParticles.particleSystemTransform.position;
						}
						
						// Collision detection 
						if (playgroundParticles.collision && playgroundParticles.collisionRadius>0) {
							
							// Playground Plane colliders (never exceed these)
							for (m = 0; m<playgroundParticles.colliders.Count; m++) {
								if (playgroundParticles.colliders[m].enabled && playgroundParticles.colliders[m].transform && !playgroundParticles.colliders[m].plane.GetSide(!localSpace?playgroundParticles.particleCache.particles[p].position : playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.particleCache.particles[p].position))) {
									
									// Set particle to location
									ray = new Ray(!localSpace?playgroundParticles.particleCache.particles[p].position : playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.particleCache.particles[p].position), playgroundParticles.colliders[m].plane.normal);
									if (playgroundParticles.colliders[m].plane.Raycast(ray, distance))
										playgroundParticles.particleCache.particles[p].position = !localSpace?ray.GetPoint(distance) : playgroundParticles.particleSystemTransform.InverseTransformPoint(ray.GetPoint(distance));
									
									// Reflect particle
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Reflect(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.colliders[m].plane.normal+RandomVector3(playgroundParticles.bounceRandomMin, playgroundParticles.bounceRandomMax))*playgroundParticles.bounciness;
									
									// Apply lifetime loss
									playgroundParticles.playgroundCache.life[p] = playgroundParticles.playgroundCache.life[p]/(1-playgroundParticles.lifetimeLoss);
									
								}
							}
							
							// Colliders in scene
							if (playgroundParticles.playgroundCache.velocity[p].magnitude>Playground.collisionSleepVelocity) {
							
								// Collide by checking for potential passed collider in the direction of this particle's velocity from the previous frame
								// Origin, Direction, OutInfo, Distance, LayerMask
								if (Physics.Raycast(
									playgroundParticles.playgroundCache.previousParticlePosition[p], 
									((!localSpace?playgroundParticles.particleCache.particles[p].position : playgroundParticles.particleSystemTransform.TransformPoint(playgroundParticles.particleCache.particles[p].position))-playgroundParticles.playgroundCache.previousParticlePosition[p]).normalized, 
									hitInfo, 
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
						if (playgroundParticles.source!=SOURCE.Script) {
							playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.playgroundCache.targetPosition[p];
							playgroundParticles.particleCache.particles[p].position = playgroundParticles.playgroundCache.targetPosition[p];
						} else if (playgroundParticles.playgroundCache.parent[p]) {
							playgroundParticles.particleCache.particles[p].position = playgroundParticles.playgroundCache.parent[p].TransformPoint(playgroundParticles.playgroundCache.targetPosition[p]);
						}
					}
					
				
				} else {
					playgroundParticles.playgroundCache.targetPosition[p] = Playground.initialTargetPosition;
					playgroundParticles.particleCache.particles[p].position = Playground.initialTargetPosition;
				}
				
			} else {
			
				// Particle is set to not rebirth
				playgroundParticles.playgroundCache.targetPosition[p] = Playground.initialTargetPosition;
				playgroundParticles.particleCache.particles[p].position = Playground.initialTargetPosition;
			}
		}
		//if (localSpace && playgroundParticles.applyLocalSpaceMovementCompensation)
		//	playgroundParticles.previousTransformPosition = playgroundParticles.particleSystemTransform.position;
	}
	
	// Calculate the effect from a manipulator
	static function CalculateManipulator (playgroundParticles : PlaygroundParticles, thisManipulator : ManipulatorObject, p : int, t : float, particlePosition : Vector3, manipulatorPosition : Vector3, localSpace : boolean) {
		if (thisManipulator.enabled && thisManipulator.transform!=null && thisManipulator.strength!=0 && (thisManipulator.affects.value & 1<<playgroundParticles.particleSystemGameObject.layer)!=0) {
			var manipulatorDistance : float;
			if (!playgroundParticles.onlySourcePositioning) {
				// Attractors
				if (thisManipulator.type==MANIPULATORTYPE.Attractor) {
					manipulatorDistance = Vector3.Distance(manipulatorPosition, particlePosition);
					if (thisManipulator.Contains(particlePosition, manipulatorPosition)) {
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition-particlePosition)*(thisManipulator.strength/manipulatorDistance), t*(thisManipulator.strength/manipulatorDistance));
					}
				} else
									
				// Attractors Gravitational
				if (thisManipulator.type==MANIPULATORTYPE.AttractorGravitational) {
					manipulatorDistance = Vector3.Distance(manipulatorPosition, particlePosition);
					if (thisManipulator.Contains(particlePosition, manipulatorPosition)) {
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition-particlePosition)*thisManipulator.strength/manipulatorDistance, t);
					}
				} else
				
				// Repellents 
				if (thisManipulator.type==MANIPULATORTYPE.Repellent) {
					manipulatorDistance = Vector3.Distance(manipulatorPosition, particlePosition);
					if (thisManipulator.Contains(particlePosition, manipulatorPosition)) {
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (particlePosition-manipulatorPosition)*(thisManipulator.strength/manipulatorDistance), t*(thisManipulator.strength/manipulatorDistance));
					}
				}
			}
			
			// Properties
			if (thisManipulator.type==MANIPULATORTYPE.Property) {
				PropertyManipulator(playgroundParticles, thisManipulator, thisManipulator.property, p, t, particlePosition, manipulatorPosition, localSpace);
			}
			
			// Combined
			if (thisManipulator.type==MANIPULATORTYPE.Combined) {
				for (var i = 0; i<thisManipulator.properties.Count; i++)
					PropertyManipulator(playgroundParticles, thisManipulator, thisManipulator.properties[i], p, t, particlePosition, manipulatorPosition, localSpace);
			}
		}
	}
	
	// Calculate the effect from manipulator properties
	static function PropertyManipulator (playgroundParticles : PlaygroundParticles, thisManipulator : ManipulatorObject, thisManipulatorProperty : ManipulatorProperty, p : int, t : float, particlePosition : Vector3, manipulatorPosition : Vector3, localSpace : boolean) {
		if (thisManipulator.Contains(particlePosition, manipulatorPosition)) {
			switch (thisManipulatorProperty.type) {
				
				// Velocity Property
				case MANIPULATORPROPERTYTYPE.Velocity:
					if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITION.None)
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
				case MANIPULATORPROPERTYTYPE.AdditiveVelocity:
					if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITION.None)
						playgroundParticles.playgroundCache.velocity[p] += thisManipulatorProperty.useLocalRotation?
							thisManipulator.transform.TransformPoint(thisManipulatorProperty.velocity*(t*thisManipulatorProperty.strength*thisManipulator.strength))-manipulatorPosition
						:
							thisManipulatorProperty.velocity*(t*thisManipulatorProperty.strength*thisManipulator.strength);
					else
						playgroundParticles.playgroundCache.velocity[p] += Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], thisManipulatorProperty.useLocalRotation? thisManipulator.transform.TransformPoint(thisManipulatorProperty.velocity)-manipulatorPosition : thisManipulatorProperty.velocity, t*thisManipulatorProperty.strength*thisManipulator.strength)*(t*thisManipulatorProperty.strength*thisManipulator.strength);
				break;
				
				// Color Property
				case MANIPULATORPROPERTYTYPE.Color:
					var staticColor : Color;
					if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITION.None) {
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
				case MANIPULATORPROPERTYTYPE.LifetimeColor:
					if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITION.None) {
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
				case MANIPULATORPROPERTYTYPE.Size:
					if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITION.None)
						playgroundParticles.particleCache.particles[p].size = thisManipulatorProperty.size;
					else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITION.Lerp)
						playgroundParticles.particleCache.particles[p].size = Mathf.Lerp(playgroundParticles.particleCache.particles[p].size, thisManipulatorProperty.size, t*thisManipulatorProperty.strength*thisManipulator.strength);
					else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITION.Linear)
						playgroundParticles.particleCache.particles[p].size = Mathf.MoveTowards(playgroundParticles.particleCache.particles[p].size, thisManipulatorProperty.size, t*thisManipulatorProperty.strength*thisManipulator.strength);
					playgroundParticles.playgroundCache.changedByPropertySize[p] = true;
				break;
				
				// Target Property
				case MANIPULATORPROPERTYTYPE.Target:
					if (thisManipulatorProperty.targets.Count>0 && thisManipulatorProperty.targets[thisManipulatorProperty.targetPointer%thisManipulatorProperty.targets.Count]) {
						
						
						// Set target pointer
						if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.transform.GetInstanceID()) {
							playgroundParticles.playgroundCache.propertyTarget[p] = thisManipulatorProperty.targetPointer;
							thisManipulatorProperty.targetPointer++; thisManipulatorProperty.targetPointer=thisManipulatorProperty.targetPointer%thisManipulatorProperty.targets.Count;
							playgroundParticles.playgroundCache.propertyId[p] = thisManipulator.transform.GetInstanceID();
						}
						
						// Teleport or lerp to position based on transition type
						if (playgroundParticles.playgroundCache.propertyId[p] == thisManipulator.transform.GetInstanceID() && thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count]) {
							if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITION.None)
								playgroundParticles.particleCache.particles[p].position = localSpace?
									playgroundParticles.particleSystemTransform.InverseTransformPoint(thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position) 
								: 
									thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position;
							else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITION.Lerp) {
								playgroundParticles.particleCache.particles[p].position = localSpace?
									Vector3.Lerp(particlePosition, playgroundParticles.particleSystemTransform.InverseTransformPoint(thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position), t*thisManipulatorProperty.strength*thisManipulator.strength)
								:
									Vector3.Lerp(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position, t*thisManipulatorProperty.strength*thisManipulator.strength);
								if (thisManipulatorProperty.zeroVelocityStrength>0)
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
							} else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITION.Linear) {
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
				case MANIPULATORPROPERTYTYPE.Death:
					if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITION.None)
						playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime;
					else
						playgroundParticles.playgroundCache.birth[p] -= t*thisManipulatorProperty.strength*thisManipulator.strength;
					
					// This particle was changed by a death property
					playgroundParticles.playgroundCache.changedByPropertyDeath[p] = true;
				break;
				
				
				// Attractors
				case MANIPULATORPROPERTYTYPE.Attractor:
					if (!playgroundParticles.onlySourcePositioning) {
						var attractorDistance = Vector3.Distance(manipulatorPosition, particlePosition);
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition-particlePosition)*((thisManipulatorProperty.strength*thisManipulator.strength)/attractorDistance), t*((thisManipulatorProperty.strength*thisManipulator.strength)/attractorDistance));
					}
				break;
									
				// Attractors Gravitational
				case MANIPULATORPROPERTYTYPE.Gravitational:
					if (!playgroundParticles.onlySourcePositioning) {
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition-particlePosition)*(thisManipulatorProperty.strength*thisManipulator.strength)/Vector3.Distance(manipulatorPosition, particlePosition), t);
					}
				break;
				
				// Repellents 
				case MANIPULATORPROPERTYTYPE.Repellent:
					if (!playgroundParticles.onlySourcePositioning) {
						var repellentsDistance = Vector3.Distance(manipulatorPosition, particlePosition);
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (particlePosition-manipulatorPosition)*((thisManipulatorProperty.strength*thisManipulator.strength)/repellentsDistance), t*((thisManipulatorProperty.strength*thisManipulator.strength)/repellentsDistance));
					}
				break;
			}
			
			playgroundParticles.playgroundCache.changedByProperty[p] = true;
		
		} else {
		
			// Particle is outside of property range and is set to only color in range (try to lerp back color with previous set key)
			if (playgroundParticles.playgroundCache.propertyColorId[p] == thisManipulator.transform.GetInstanceID() && (thisManipulatorProperty.type == MANIPULATORPROPERTYTYPE.Color || thisManipulatorProperty.type == MANIPULATORPROPERTYTYPE.LifetimeColor) && playgroundParticles.playgroundCache.changedByPropertyColorLerp[p] && thisManipulatorProperty.transition != MANIPULATORPROPERTYTRANSITION.None && thisManipulatorProperty.onlyColorInRange)
				playgroundParticles.particleCache.particles[p].color = Color.Lerp(playgroundParticles.particleCache.particles[p].color, playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime), t*thisManipulatorProperty.strength*thisManipulator.strength);
			
			// Target positioning outside of range
			if (thisManipulatorProperty.type == MANIPULATORPROPERTYTYPE.Target && thisManipulatorProperty.transition != MANIPULATORPROPERTYTRANSITION.None) {
				if (thisManipulatorProperty.targets.Count>0 && thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count]!=null) {
					if (playgroundParticles.playgroundCache.changedByPropertyTarget[p] && !thisManipulatorProperty.onlyPositionInRange && thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count] && playgroundParticles.playgroundCache.propertyId[p] == thisManipulator.transform.GetInstanceID()) {
						if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITION.Lerp)
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
	function RefreshScatter () {
		for (var p = 0; p<particleCount; p++) {
			playgroundCache.scatterPosition[p] = applySourceScatter?Vector3(
				UnityEngine.Random.Range(sourceScatterMin.x, sourceScatterMax.x),
				UnityEngine.Random.Range(sourceScatterMin.y, sourceScatterMax.y),
				UnityEngine.Random.Range(sourceScatterMin.z, sourceScatterMax.z)
			) : Vector3.zero;
		}
	}

	// Rebirth of a specified particle
	static function Rebirth (playgroundParticles : PlaygroundParticles, p : int) {
		playgroundParticles.playgroundCache.birthDelay[p] = 0;
		playgroundParticles.playgroundCache.life[p] = 0;
		playgroundParticles.playgroundCache.birth[p] = playgroundParticles.playgroundCache.death[p]; 
		playgroundParticles.playgroundCache.death[p] += playgroundParticles.lifetime;
		playgroundParticles.playgroundCache.rebirth[p] = playgroundParticles.source==SOURCE.Script?true:(playgroundParticles.emit && playgroundParticles.playgroundCache.emission[p]);
		playgroundParticles.playgroundCache.velocity[p] = Vector3.zero;
		//Debug.Log("rebirth:"+playgroundParticles.playgroundCache.rebirth[p]+" emit:"+playgroundParticles.emit+" emission:"+playgroundParticles.playgroundCache.emission[p]);
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
				playgroundParticles.playgroundCache.velocity[p] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((p*1.0)/(playgroundParticles.particleCount*1.0)));
		}
		if (playgroundParticles.source==SOURCE.Script) {
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
			if (playgroundParticles.applySourceScatter && playgroundParticles.source!=SOURCE.Script) {
				if (playgroundParticles.playgroundCache.scatterPosition[p]==Vector3.zero || playgroundParticles.applyRandomScatterOnRebirth)
					playgroundParticles.playgroundCache.scatterPosition[p] = new Vector3(
						UnityEngine.Random.Range(playgroundParticles.sourceScatterMin.x, playgroundParticles.sourceScatterMax.x),
						UnityEngine.Random.Range(playgroundParticles.sourceScatterMin.y, playgroundParticles.sourceScatterMax.y),
						UnityEngine.Random.Range(playgroundParticles.sourceScatterMin.z, playgroundParticles.sourceScatterMax.z)
					);
			} else playgroundParticles.playgroundCache.scatterPosition[p]=Vector3.zero;
			
			playgroundParticles.particleCache.particles[p].position = playgroundParticles.playgroundCache.targetPosition[p];
			playgroundParticles.playgroundCache.previousParticlePosition[p] = playgroundParticles.particleCache.particles[p].position;
		} else playgroundParticles.particleCache.particles[p].position = Playground.initialTargetPosition;
		
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
	function RemoveState (i : int) {
		var newState : int = this.activeState;
		newState = (newState%this.states.Count)-1;
		if (newState<0) newState = 0;
			
		this.states[newState].Initialize();
		this.activeState = newState;
		this.states.RemoveAt(i);
	}
	
	// Wipe out particles in current PlaygroundParticles object
	static function Clear (playgroundParticles:PlaygroundParticles) {
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
	function CheckReferences () {
		if (Playground.reference==null)
			Playground.reference = FindObjectOfType(Playground);
		if (Playground.reference==null)
			Playground.ResourceInstantiate("Playground Manager");
		if (this.playgroundCache==null)
			this.playgroundCache = new PlaygroundCache();
		if (this.particleCache==null)
			this.particleCache = new ParticleCache();

		if (this.particleSystemGameObject==null) {
			this.particleSystemGameObject = gameObject;
			this.particleSystemTransform = transform;
			this.particleSystemRenderer = renderer;
			this.shurikenParticleSystem = particleSystemGameObject.GetComponent(ParticleSystem);
		}
	}

	// YieldedRefresh makes sure that Playground Manager and simulation time is ready before this particle system
	function YieldedRefresh () {
		yield;
		SetLifetime(this, lifetime);
		#if UNITY_EDITOR
		if (!UnityEditor.EditorApplication.isPlaying && sorting!=SORTING.Burst) {
			Update (this);
		}
		#endif
		if (sorting==SORTING.Burst) {
			Update (this);
		}
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// MonoBehaviours
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	function Awake () {
		// Make sure World Object is initialized
		if (worldObject!=null)
			worldObject.Initialize();
		
		// Make sure that state data is initialized
		for (var x = 0; x<states.Count; x++) {
			states[x].Initialize();
		}
		
		// Make sure projection is initialized
		if (projection!=null && projection.projectionTexture)
			projection.Initialize();
	}

	function OnEnable () {

		// Make sure all references exists
		CheckReferences();

		// Set 0 size to avoid one-frame flash
		shurikenParticleSystem.startSize = 0;
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
			SetInitialTargetPosition(this, Playground.initialTargetPosition);

			if (sorting==SORTING.Burst)
				YieldedRefresh();

			// Make sure that particles have current time
			SetParticleTimeNow(this);
		}
	}
	
	function Start () {
		particleSystemRenderer2 = gameObject.particleSystem.renderer as ParticleSystemRenderer;
		if (Playground.reference!=null) {
			if (!Playground.reference.particleSystems.Contains(this))
				Playground.reference.particleSystems.Add(this);
			if (particleSystemTransform.parent==null && Playground.reference.autoGroup)
				particleSystemTransform.parent = Playground.referenceTransform;
		}

		if (particleSystemGameObject.activeSelf)
			YieldedRefresh();
			
	}
	
	function OnDestroy () {
		// Remove this PlaygroundParticlesC object from Particle Systems list
		if (Playground.reference)
			Playground.reference.particleSystems.Remove(this);
	}
	
}	

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// MeshParticles - Extension class for PlaygroundParticles which creates mesh states. 
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class MeshParticles extends PlaygroundParticles {

	static function CreateMeshParticles (meshes : Mesh[], textures:Texture2D[], heightMap : Texture2D[], name : String, position : Vector3, rotation : Quaternion, particleScale : float, offsets:Vector3[], material : Material) {
		var meshParticles : PlaygroundParticles = PlaygroundParticles.CreateParticleObject(name,position,rotation,particleScale,material);
		meshParticles.states = new List.<ParticleState>();
		
		var quantityList : int[] = new int[meshes.Length];
		for (var i = 0; i<textures.Length; i++)
			quantityList[i] = meshes[i].vertexCount;
		meshParticles.particleCache.particles = new ParticleSystem.Particle[quantityList[Playground.Largest(quantityList)]];
		meshParticles.shurikenParticleSystem.Emit(meshParticles.particleCache.particles.Length);
		meshParticles.shurikenParticleSystem.GetParticles(meshParticles.particleCache.particles);
		for (i = 0; i<meshes.Length; i++) {
			meshParticles.states.Add(new ParticleState());
			meshParticles.states[0].ConstructParticles(meshes[i],textures[i],particleScale,offsets[i], "State "+i,null);
		}
		
		Playground.Update(meshParticles);
		Playground.particlesQuantity++;
		
		return meshParticles;
	}
	
	static function Add (meshParticles : PlaygroundParticles, mesh : Mesh, scale : float, offset : Vector3, stateName : String, stateTransform : Transform) {
		meshParticles.states.Add(new ParticleState());
		meshParticles.states[meshParticles.states.Count-1].ConstructParticles(mesh,scale,offset,stateName,stateTransform);
	}
	
	static function Add (meshParticles : PlaygroundParticles, mesh : Mesh, texture : Texture2D, scale : float, offset : Vector3, stateName : String, stateTransform : Transform) {
		meshParticles.states.Add(new ParticleState());
		meshParticles.states[meshParticles.states.Count-1].ConstructParticles(mesh,texture,scale,offset,stateName,stateTransform);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Cache
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class PlaygroundCache {
	@NonSerialized var size : float[];							// The size of each particle
	@NonSerialized var life : float[];							// The life time of each particle
	@NonSerialized var birth : float[];							// The time of birth for each particle
	@NonSerialized var birthDelay : float[];					// The delayed time of birth when emission has changed
	@NonSerialized var death : float[];							// The time of death for each particle
	@NonSerialized var emission : boolean[];					// The emission for each particle (controlled by emission rate)
	@NonSerialized var rebirth : boolean[];						// The rebirth for each particle
	@NonSerialized var lifetimeOffset : float[];				// The offset in birth-death (sorting)
	@NonSerialized var velocity : Vector3[];					// The velocity of each particle in this PlaygroundParticles
	@NonSerialized var initialVelocity : Vector3[];				// The initial velocity of each particle in this PlaygroundParticles
	@NonSerialized var initialLocalVelocity : Vector3[];		// The initial local velocity of each particle in this PlaygroundParticles
	@NonSerialized var targetPosition : Vector3[];				// The source position for each particle
	@NonSerialized var previousTargetPosition : Vector3[]; 		// The previous source position for each particle (used to calculate delta movement)
	@NonSerialized var previousParticlePosition : Vector3[];	// The previous calculated frame's particle position
	@NonSerialized var scatterPosition : Vector3[];				// The scattered position to apply on each particle birth in this PlaygroundParticles
	@NonSerialized var initialRotation : float[];				// The initial rotation of each particle in this PlaygroundParticles
	@NonSerialized var rotationSpeed : float[];					// The rotation speed of each particle in this PlaygroundParticles
	@NonSerialized var parent : Transform[];					// The transform parent of each particle in this PlaygroundParticles
	@NonSerialized var color : Color32[];						// The color of each particle in this PlaygroundParticles
	
	@NonSerialized var changedByProperty : boolean[];			// The interaction with property manipulators of each particle
	@NonSerialized var changedByPropertyColor : boolean[];		// The interaction with property manipulators that change color of each particle
	@NonSerialized var changedByPropertyColorLerp : boolean[]; 	// The interaction with property manipulators that change color over time of each particle
	@NonSerialized var changedByPropertyColorKeepAlpha : boolean[]; // The interaction with property manipulators that change color and wants to keep alpha
	@NonSerialized var changedByPropertySize : boolean[];		// The interaction with property manipulators that change size of each particle
	@NonSerialized var changedByPropertyTarget : boolean[];		// The interaction with property manipulators that change target of each particle
	@NonSerialized var changedByPropertyDeath : boolean[];		// The interaction with death manipulators that forces a particle to a sooner end
	@NonSerialized var propertyTarget : int[];					// The property target pointer for each particle
	@NonSerialized var propertyId : int[];						// The property target id for each particle (pairing a particle's target to a manipulator)
	@NonSerialized var propertyColorId : int[];					// The property color id for each particles (pairing a particle's color to a manipulator
	
	// Copy this PlaygroundCache struct
	function Clone () : PlaygroundCache {
		var playgroundCache : PlaygroundCache = new PlaygroundCache();
		playgroundCache.size = this.size.Clone() as float[];
		playgroundCache.life = this.life.Clone() as float[];
		playgroundCache.birth = this.birth.Clone() as float[];
		playgroundCache.birthDelay = this.birthDelay.Clone() as float[];
		playgroundCache.death = this.death.Clone() as float[];
		playgroundCache.emission = this.birth.Clone() as boolean[];
		playgroundCache.rebirth = this.rebirth.Clone() as boolean[];
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
		playgroundCache.changedByProperty = this.changedByProperty.Clone() as boolean[];
		playgroundCache.changedByPropertyColor = this.changedByPropertyColor.Clone() as boolean[];
		playgroundCache.changedByPropertyColorLerp = this.changedByPropertyColorLerp.Clone() as boolean[];
		playgroundCache.changedByPropertyColorKeepAlpha = this.changedByPropertyColorKeepAlpha.Clone() as boolean[];
		playgroundCache.changedByPropertySize = this.changedByPropertySize.Clone() as boolean[];
		playgroundCache.changedByPropertyTarget = this.changedByPropertyTarget.Clone() as boolean[];
		playgroundCache.changedByPropertyDeath = this.changedByPropertyDeath.Clone() as boolean[];
		playgroundCache.propertyTarget = this.propertyTarget.Clone() as int[];
		playgroundCache.propertyId = this.propertyId.Clone() as int[];
		playgroundCache.propertyColorId = this.propertyColorId.Clone() as int[];
		return playgroundCache;
	}
}

class ParticleCache {
	@NonSerialized var particles : ParticleSystem.Particle[];	// The particle pool of this PlaygroundParticles object
}