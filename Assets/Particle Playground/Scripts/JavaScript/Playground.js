#pragma strict

import System;
import System.Collections.Generic;

@script ExecuteInEditMode()

class Playground extends MonoBehaviour {

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Playground settings
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// Time variables
	static var globalTime : float;												// Time used by particles
	static var lastTimeUpdated : float;											// Time when globalTime last updated
	static var globalDeltaTime : float;											// Delta time for globalTime (globalDeltaTime-lastTimeUpdated)
	static var globalTimescale : float = 1.0;									// Scaling of globalTime
	
	// Standard settings for a created particle system by script
	static var collisionSleepVelocity : float = .01;							// Minimum velocity of a particle before it goes to rest
	
	// Misc settings
	static var initialTargetPosition : Vector3 = Vector3(0,100000,0); 			// Initial spawn position (dirty trick to not make them appear in Vector3.zero...)
	static var skinnedUpdateRate : int = 1;										// Update rate for finding vertices in skinned meshes (1 = Every frame, 2 = Every second frame...)
	static var triggerSceneRepaint : boolean = true;							// Let a PlaygroundParticleWindow repaint the scene
	
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Playground cache
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	static var reference : Playground;
	static var referenceTransform : Transform;
	static var referenceGameObject : GameObject;
	
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Playground public variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	var particleSystems : List.<PlaygroundParticles>;							// List of particle systems handled by Playground Manager
	var manipulators : List.<ManipulatorObject>;								// List of manipulator objects handled by Playground Manager
	@HideInInspector var paintMaxPositions : int = 10000;						// Maximum amount of emission positions in a PaintObject
	@HideInInspector var calculate : boolean = true;							// Calculate forces on PlaygroundParticles objects
	@HideInInspector var pixelFilterMode : PIXELMODE = PIXELMODE.Pixel32;		// Color filtering mode
	@HideInInspector var garbageCollectOnResize : boolean = true;				// Issue a GC.Collect when particle lists are resized
	@HideInInspector var autoGroup : boolean = true;							// Automatically parent a PlaygroundParticles object to Playground if it has no parent
 	@HideInInspector var buildZeroAlphaPixels : boolean = false;				// Turn this on if you want to build particles from 0 alpha pixels into states
 	@HideInInspector var drawGizmos : boolean = true;							// Draw gizmos for manipulators in Scene View
 	@HideInInspector var showShuriken : boolean = false;						// Should the Shuriken particle system component be visible? (This is just a precaution as editing Shuriken can lead to unexpected behavior)
 	@HideInInspector var paintToolbox : boolean = true;							// Show toolbox in Scene View when Source is set to Paint on PlaygroundParticles objects
 	@HideInInspector var collisionPlaneScale : float = .1;						// Scale of collision planes
 	
 	// Limitations (shown in Playground Manager > Advanced)
	@HideInInspector var maximumAllowedLifetime : float = 100;					// The maximum value for Particle Lifetime in Editor
 	@HideInInspector var maximumAllowedParticles : int = 100000;				// The maximum value for Particle Count in Editor
	@HideInInspector var maximumAllowedRotation : float = 360;					// The maximum value for Minimum- and Maximum Particle Rotation Speed in Editor
	@HideInInspector var maximumAllowedSize : float = 10;						// The maximum value for Minimum- and Maximum Particle Size in Editor
	@HideInInspector var maximumAllowedDeltaMovementStrength : float = 100;		// The maximum value for Particle Delta Movement Strength in Editor
	@HideInInspector var maximumAllowedScale : float = 10;						// The maximum value for Particle Scale in Editor
	@HideInInspector var maximumAllowedDamping : float = 10;					// The maximum value for Particle Damping in Editor
	@HideInInspector var maximumAllowedVelocity : float = 100;					// The maximum value for Particle Max Velocity in Editor
	@HideInInspector var maximumAllowedMass : float = 100;						// The maximum value for Particle Mass in Editor
	@HideInInspector var maximumAllowedCollisionRadius : float = 10;			// The maximum value for Particle Collision Radius in Editor
	@HideInInspector var maximumAllowedBounciness : float = 2;					// The maximum value for Particle Bounciness in Editor
	@HideInInspector var minimumAllowedUpdateRate : int = 10;					// The minimum value for Particle Update Rate in Editor
	@HideInInspector var maximumAllowedTransitionTime : float = 10;				// The maximum value for Particle Transition Time in Editor
	@HideInInspector var minimumAllowedTimescale : float = .01;					// The minimum value for Particle Timescale
	@HideInInspector var maximumAllowedTimescale : float = 2;					// The maximum value for Particle Timescale
	@HideInInspector var maximumAllowedPaintPositions : int = 100000;			// The maximum value for Paint Positions
	@HideInInspector var minimumAllowedBrushScale : float = .001;				// The minimum scale of a Brush
	@HideInInspector var maximumAllowedBrushScale : float = 1.0;				// The maximum scale of a Brush
	@HideInInspector var maximumAllowedPaintSpacing : float = 10.0;				// The maximum spacing when painting
	@HideInInspector var maximumAllowedInitialVelocity : float = 100;			// The maximum value for Minimum- and Maximum Initial (+Local) Velocity
	@HideInInspector var minimumEraserRadius : float = .001;					// The minimum value for Eraser radius
	@HideInInspector var maximumEraserRadius : float = 100;						// The maximum value for Eraser radius
	@HideInInspector var maximumRenderSliders : float = 10;						// The minimum- and maximum value for sliders in Rendering
 	@HideInInspector var maximumAllowedManipulatorSize : float = 100;			// The maximum value for Manipulator Size
	@HideInInspector var maximumAllowedManipulatorStrength : float = 100;		// The maximum value for Manipulator Strength
	@HideInInspector var maximumAllowedManipulatorZeroVelocity : float = 10;	// The maximum value for Manipulator Property Zero Velocity Strength
	@HideInInspector var maximumAllowedSourceScatter : float = 10;				// The maximum value for scattering source positions
	
 
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Playground counters
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	static var meshQuantity : int;
	static var particlesQuantity : int;
	static var version : String = "1.20 (JS)";
	

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Playground wrapper
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// Create empty PlaygroundParticles object
	static function Particle () : PlaygroundParticles {
		var playgroundParticles : PlaygroundParticles = PlaygroundParticles.CreateParticleObject("Particle Playground System "+particlesQuantity,Vector3.zero,Quaternion.identity,1.0,new Material(Shader.Find("Playground/Vertex Color")));
		playgroundParticles.particleCache.particles = new ParticleSystem.Particle[playgroundParticles.particleCount];
		PlaygroundParticles.OnCreatePlaygroundParticles(playgroundParticles);
		return playgroundParticles;
	}
	
	// Single image
	static function Particle (image:Texture2D, name:String, position:Vector3, rotation:Quaternion, offset:Vector3, particleSize:float, scale:float, material:Material) : PlaygroundParticles {
		return PlaygroundParticles.CreatePlaygroundParticles([image],name,position,rotation,offset,particleSize,scale,material);
	}
		
	// Single image simple
	static function Particle (image:Texture2D) : PlaygroundParticles {
		return PlaygroundParticles.CreatePlaygroundParticles([image],"Particle Playgrounds "+particlesQuantity,Vector3.zero,Quaternion.identity,Vector3.zero,1.0,1.0,new Material(Shader.Find("Playground/Vertex Color")));
	}
	
	// Multiple images
	static function Particle (images:Texture2D[], name:String, position:Vector3, rotation:Quaternion, offset:Vector3, particleSize:float, scale:float, material:Material) : PlaygroundParticles {
		return PlaygroundParticles.CreatePlaygroundParticles(images,name,position,rotation,offset,particleSize,scale,material);
	}
	
	// Multiple images simple
	static function Particle (images:Texture2D[]) : PlaygroundParticles {
		return PlaygroundParticles.CreatePlaygroundParticles(images,"Particle Playgrounds "+particlesQuantity,Vector3.zero,Quaternion.identity,Vector3.zero,1.0,1.0,new Material(Shader.Find("Playground/Vertex Color")));
	}
	
	// Single mesh
	static function Particle (mesh : Mesh, texture:Texture2D, name : String, position : Vector3, rotation : Quaternion, particleScale : float, offset : Vector3, material : Material) : PlaygroundParticles {
		return MeshParticles.CreateMeshParticles([mesh],[texture],null,name,position,rotation,particleScale,[offset],material);
	}
	
	// Single mesh simple
	static function Particle (mesh:Mesh, texture:Texture2D) : PlaygroundParticles {
		return MeshParticles.CreateMeshParticles([mesh],[texture],null,"Particle Playgrounds "+particlesQuantity,Vector3.zero,Quaternion.identity,1.0,[Vector3.zero],new Material(Shader.Find("Playground/Vertex Color")));
	}
	
	// Multiple meshes
	static function Particle (meshes:Mesh[], textures:Texture2D[], name:String, position:Vector3, rotation:Quaternion, particleScale:float, offsets:Vector3[], material:Material) : PlaygroundParticles {
		return MeshParticles.CreateMeshParticles(meshes,textures,null,name,position,rotation,particleScale,offsets,material);
	}
	
	// Multiple meshes simple
	static function Particle (meshes:Mesh[], textures:Texture2D[]) : PlaygroundParticles {
		return MeshParticles.CreateMeshParticles(meshes,textures,null,"Particle Playgrounds "+particlesQuantity,Vector3.zero,Quaternion.identity,1.0,new Vector3[meshes.Length],new Material(Shader.Find("Playground/Vertex Color")));
	}
	
	// Emit next particle - using the particle system as a pool (note that you need to set scriptedEmission-variables on beforehand). Returns emitted particle number.
	static function Emit (playgroundParticles:PlaygroundParticles) : int {
		return playgroundParticles.Emit(playgroundParticles.scriptedEmissionPosition,playgroundParticles.scriptedEmissionVelocity,playgroundParticles.scriptedEmissionColor,playgroundParticles.scriptedEmissionParent);
	}
	
	// Emit next particle while setting scriptedEmission data - using the particle system as a pool. Returns emitted particle number.
	static function Emit (playgroundParticles:PlaygroundParticles, position:Vector3, normal:Vector3, color:Color, parent:Transform) : int {
		return playgroundParticles.Emit(position,normal,color,parent);
	}
	
	// Set emission on/off
	static function Emit (playgroundParticles:PlaygroundParticles, setEmission : boolean) {
		playgroundParticles.Emit(setEmission);
	}
	
	// Random position
	static function Random (playgroundParticles:PlaygroundParticles, min:Vector3, max:Vector3) {
		PlaygroundParticles.Random(playgroundParticles,min,max);
	}
	
	// Linear interpolation position and color
	static function Lerp (playgroundParticles : PlaygroundParticles, to : int, time : float) {
		PlaygroundParticles.Lerp(playgroundParticles,to,time,LERPTYPE.PositionColor);
	}
	
	// Linear interpolation to state
	static function Lerp (playgroundParticles : PlaygroundParticles, state : ParticleState, time : float) {
		PlaygroundParticles.Lerp(playgroundParticles,state,time,LERPTYPE.PositionColor);
	}
	
	// Linear interpolation color
	static function ColorLerp (playgroundParticles : PlaygroundParticles, to : int, time : float) {
		PlaygroundParticles.Lerp(playgroundParticles,to,time,LERPTYPE.Color);
	}
	
	// Linear interpolation position
	static function PositionLerp (playgroundParticles : PlaygroundParticles, to : int, time : float) {
		PlaygroundParticles.Lerp(playgroundParticles,to,time,LERPTYPE.Position);
	}
	
	// Linear interpolation to object in world space
	static function Lerp (playgroundParticles : PlaygroundParticles, particleStateWorldObject : SkinnedWorldObject, time : float) {
		PlaygroundParticles.Lerp(playgroundParticles,particleStateWorldObject,time);
	} 
	
	// Set new image/mesh position from state instantly
	static function SetPosition (playgroundParticles:PlaygroundParticles, to:int, runUpdate:boolean) {
		PlaygroundParticles.SetPosition(playgroundParticles,to,runUpdate);
	}
	
	// Set new mesh position from world object state instantly
	static function SetPosition (playgroundParticles : PlaygroundParticles, particleStateWorldObject:WorldObject) {
		PlaygroundParticles.SetPosition(playgroundParticles,particleStateWorldObject);
	}
	
	// Set new mesh position from mesh world object state instantly
	static function SetPosition (playgroundParticles : PlaygroundParticles, particleStateWorldObject:SkinnedWorldObject) {
		PlaygroundParticles.SetPosition(playgroundParticles,particleStateWorldObject);
	}
	
	// Get vertices from a skinned world object in Vector3[] format (notice that the array is modified by reference)
	static function GetPosition (vertices : Vector3[], particleStateWorldObject : SkinnedWorldObject) {
		PlaygroundParticles.GetPosition(vertices,[Vector3.zero],particleStateWorldObject);
	}
	
	// Get vertices and normals from a skinned world object in Vector3[] format (notice that the array is modified by reference)
	static function GetPosition (vertices : Vector3[], normals : Vector3[], particleStateWorldObject : SkinnedWorldObject) {
		PlaygroundParticles.GetPosition(vertices,normals,particleStateWorldObject);
	}
	
	// Get vertices from a world object in Vector3[] format (notice that the array is modified by reference)
	static function GetPosition (vertices : Vector3[], particleStateWorldObject : WorldObject) {
		PlaygroundParticles.GetPosition(vertices,particleStateWorldObject);
	}
	
	// Get normals from a world object in Vector3[] format (notice that the array is modified by reference)
	static function GetNormals (normals : Vector3[], particleStateWorldObject : WorldObject) {
		PlaygroundParticles.GetNormals(normals,particleStateWorldObject);
	}
	
	// Set new color to State instantly
	static function SetColor (playgroundParticles:PlaygroundParticles, to:int) {
		PlaygroundParticles.SetColor(playgroundParticles,to);
	}
	
	// Set new color to Color instantly 
	static function SetColor (playgroundParticles:PlaygroundParticles, color:Color) {
		PlaygroundParticles.SetColor(playgroundParticles,color);
	}
	
	// Set alpha of particles instantly
	static function SetAlpha (playgroundParticles:PlaygroundParticles, alpha : float) {
		PlaygroundParticles.SetAlpha(playgroundParticles,alpha);
	}
	
	// Set particle size
	static function SetSize (playgroundParticles:PlaygroundParticles, size:float) {
		PlaygroundParticles.SetSize(playgroundParticles,size);
	}
	
	// Translate all particles in Particle System
	static function Translate (playgroundParticles:PlaygroundParticles, direction:Vector3) {
		PlaygroundParticles.Translate(playgroundParticles,direction);
	}
	
	// Refresh and calculate particles
	static function Update (playgroundParticles:PlaygroundParticles) {
		PlaygroundParticles.Update(playgroundParticles);
	}
	
	// Add single state
	static function Add (playgroundParticles:PlaygroundParticles, state : ParticleState) {
		PlaygroundParticles.Add(playgroundParticles,state);
	}
	
	// Add single state image
	static function Add (playgroundParticles:PlaygroundParticles, image:Texture2D, scale:float, offset:Vector3, stateName:String) {
		PlaygroundParticles.Add(playgroundParticles,image,scale,offset,stateName,null);
	}
	
	// Add single state image with transform
	static function Add (playgroundParticles:PlaygroundParticles, image:Texture2D, scale:float, offset:Vector3, stateName:String, stateTransform:Transform) {
		PlaygroundParticles.Add(playgroundParticles,image,scale,offset,stateName,stateTransform);
	}
	
	// Add single state image with depthmap
	static function Add (playgroundParticles:PlaygroundParticles, image:Texture2D, depthmap:Texture2D, depthmapStrength:float, scale:float, offset:Vector3, stateName:String) {
		PlaygroundParticles.Add(playgroundParticles,image,depthmap,depthmapStrength,scale,offset,stateName,null);
	}
	
	// Add single state image with depthmap and transform
	static function Add (playgroundParticles:PlaygroundParticles, image:Texture2D, depthmap:Texture2D, depthmapStrength:float, scale:float, offset:Vector3, stateName:String, stateTransform:Transform) {
		PlaygroundParticles.Add(playgroundParticles,image,depthmap,depthmapStrength,scale,offset,stateName,stateTransform);
	}
	
	// Add single state mesh
	static function Add (playgroundParticles:PlaygroundParticles, mesh:Mesh, scale:float, offset:Vector3, stateName:String) {
		MeshParticles.Add(playgroundParticles,mesh,scale,offset,stateName,null);
	}
	
	// Add single state mesh with transform
	static function Add (playgroundParticles:PlaygroundParticles, mesh:Mesh, scale:float, offset:Vector3, stateName:String, stateTransform:Transform) {
		MeshParticles.Add(playgroundParticles,mesh,scale,offset,stateName,stateTransform);
	}
	
	// Add single state mesh with texture
	static function Add (playgroundParticles:PlaygroundParticles, mesh:Mesh, texture:Texture2D, scale:float, offset:Vector3, stateName:String) {
		MeshParticles.Add(playgroundParticles,mesh,texture,scale,offset,stateName,null);
	}
	
	// Add single state mesh with texture and transform
	static function Add (playgroundParticles:PlaygroundParticles, mesh:Mesh, texture:Texture2D, scale:float, offset:Vector3, stateName:String, stateTransform:Transform) {
		MeshParticles.Add(playgroundParticles,mesh,texture,scale,offset,stateName,stateTransform);
	}
	
	// Add a plane collider
	static function AddCollider (playgroundParticles:PlaygroundParticles) : PlaygroundCollider {
		var pCollider : PlaygroundCollider = new PlaygroundCollider();
		playgroundParticles.colliders.Add(pCollider);
		return pCollider;
	}
	
	// Add a plane collider and assign a transform
	static function AddCollider (playgroundParticles:PlaygroundParticles, transform : Transform) : PlaygroundCollider {
		var pCollider : PlaygroundCollider = new PlaygroundCollider();
		pCollider.transform = transform;
		playgroundParticles.colliders.Add(pCollider);
		return pCollider;
	}
	
	// Set amount of particles for this Particle System
	static function SetParticleCount (playgroundParticles:PlaygroundParticles, amount : int) {
		PlaygroundParticles.SetParticleCount(playgroundParticles,amount);
	}
	
	// Set lifetime for this Particle System
	static function SetLifetime (playgroundParticles:PlaygroundParticles, time : float) {
		PlaygroundParticles.SetLifetime(playgroundParticles,time);
	}
	
	// Set material for this Particle System
	static function SetMaterial (playgroundParticles:PlaygroundParticles, particleMaterial:Material) {
		PlaygroundParticles.SetMaterial(playgroundParticles, particleMaterial);
	}
	
	// Destroy this Particle System
	static function Destroy (playgroundParticles:PlaygroundParticles) {
		PlaygroundParticles.Destroy(playgroundParticles);
	}
	
	// Create a world object reference (used for live world positioning of particles towards a mesh)
	static function WorldObject (playgroundParticles:PlaygroundParticles, meshTransform : Transform) : WorldObject {
		return PlaygroundParticles.NewWorldObject(playgroundParticles,meshTransform);
	}
	
	// Create a skinned world object reference (used for live world positioning of particles towards a mesh)
	static function SkinnedWorldObject (playgroundParticles:PlaygroundParticles, meshTransform:Transform) : SkinnedWorldObject {
		return PlaygroundParticles.NewSkinnedWorldObject(playgroundParticles,meshTransform);
	}
	
	// Create a manipulator object
	static function ManipulatorObject (type:MANIPULATORTYPE, affects:LayerMask, manipulatorTransform:Transform, size:float, strength:float) : ManipulatorObject {
		return PlaygroundParticles.NewManipulatorObject(type,affects,manipulatorTransform,size,strength,null);
	}
	
	// Create a manipulator object by transform
	static function ManipulatorObject (manipulatorTransform:Transform) : ManipulatorObject {
		var layerMask : LayerMask = -1;
		return PlaygroundParticles.NewManipulatorObject(MANIPULATORTYPE.Attractor,layerMask,manipulatorTransform,1.0,1.0,null);
	}
	
	// Create a manipulator in a PlaygroundParticles object
	static function ManipulatorObject (type:MANIPULATORTYPE, affects:LayerMask, manipulatorTransform:Transform, size:float, strength:float, playgroundParticles:PlaygroundParticles) : ManipulatorObject {
		return PlaygroundParticles.NewManipulatorObject(type,affects,manipulatorTransform,size,strength,playgroundParticles);
	}
	
	// Create a manipulator in a PlaygroundParticles object by transform
	static function ManipulatorObject (manipulatorTransform:Transform, playgroundParticles:PlaygroundParticles) : ManipulatorObject {
		var layerMask : LayerMask = -1;
		return PlaygroundParticles.NewManipulatorObject(MANIPULATORTYPE.Attractor,layerMask,manipulatorTransform,1.0,1.0,playgroundParticles);
	}
	
	// Return a manipulator in array position
	static function GetManipulator (i : int) : ManipulatorObject {
		if (reference.manipulators.Count>0 && reference.manipulators[i%reference.manipulators.Count]!=null)
			return reference.manipulators[i%reference.manipulators.Count];
		else return null;
	}
	
	// Return a manipulator in a PlaygroundParticles object in array position
	static function GetManipulator (i : int, playgroundParticles:PlaygroundParticles) : ManipulatorObject {
		if (playgroundParticles.manipulators.Count>0 && playgroundParticles.manipulators[i%playgroundParticles.manipulators.Count]!=null)
			return playgroundParticles.manipulators[i%playgroundParticles.manipulators.Count];
		else return null;
	}
	
	// Return a Particle Playground System in array position
	static function GetParticles (i : int) : PlaygroundParticles {
		if (reference.particleSystems.Count>0 && reference.particleSystems[i%reference.particleSystems.Count]!=null)
			return reference.particleSystems[i%reference.particleSystems.Count];
		else return null;
	}
	
	// Create a projection object reference
	static function ParticleProjection (playgroundParticles:PlaygroundParticles) : ParticleProjection {
		return PlaygroundParticles.NewProjectionObject(playgroundParticles);
	}
	
	// Create a paint object reference
	static function PaintObject (playgroundParticles:PlaygroundParticles) : PaintObject {
		return PlaygroundParticles.NewPaintObject(playgroundParticles);
	}
	
	// Live paint into a PlaygroundParticles PaintObject's positions
	static function Paint (playgroundParticles:PlaygroundParticles, position:Vector3, normal:Vector3, parent:Transform, color:Color32) : int {
		playgroundParticles.isPainting = true;
		return playgroundParticles.paint.Paint(position,normal,parent,color);
	}
	
	// Live paint into a PaintObject's positions directly
	static function Paint (paintObject:PaintObject, position:Vector3, normal:Vector3, parent:Transform, color:Color32) {
		paintObject.Paint(position,normal,parent,color);
	}
	
	// Live erase into a PlaygroundParticles PaintObject's positions, returns true if position was erased
	static function Erase (playgroundParticles:PlaygroundParticles, position:Vector3, radius:float) : boolean {
		playgroundParticles.isPainting = true;
		return playgroundParticles.paint.Erase(position,radius);
	}
	
	// Live erase into a PaintObject's positions directly, returns true if position was erased
	static function Erase (paintObject:PaintObject, position:Vector3, radius:float) : boolean {
		return paintObject.Erase(position,radius);
	}
	
	// Live erase into a PlaygroundParticles PaintObject's using a specified index, returns true if position was erased
	static function Erase (playgroundParticles:PlaygroundParticles, index:int) : boolean {
		return playgroundParticles.paint.Erase(index);
	}
	
	// Clear out paint in a PlaygroundParticles object
	static function ClearPaint (playgroundParticles:PlaygroundParticles) {
		if (playgroundParticles.paint && playgroundParticles.paint!=null)
			playgroundParticles.paint.ClearPaint();
	}
	
	// Get the amount of paint positions in this PlaygroundParticles PaintObject
	static function GetPaintPositionLength (playgroundParticles:PlaygroundParticles) : int {
		return playgroundParticles.paint.positionLength;
	}
	
	// Set initial target position for this Particle System
	static function SetInitialTargetPosition (playgroundParticles:PlaygroundParticles, position:Vector3) {
		PlaygroundParticles.SetInitialTargetPosition(playgroundParticles,position);
	}
	
	// Set emission for this Particle System
	static function Emission (playgroundParticles:PlaygroundParticles, emit:boolean) {
		PlaygroundParticles.Emission(playgroundParticles,emit,false);
	}
	
	// Set emission for this Particle System controlling to run rest emission
	static function Emission (playgroundParticles:PlaygroundParticles, emit:boolean, restEmission:boolean) {
		PlaygroundParticles.Emission(playgroundParticles,emit,restEmission);
	}
	
	// Clear out this Particle System
	static function Clear (playgroundParticles:PlaygroundParticles) {
		PlaygroundParticles.Clear(playgroundParticles);
	}
	
	// Refresh source scatter for this Particle System
	static function RefreshScatter (playgroundParticles:PlaygroundParticles) {
		playgroundParticles.RefreshScatter();
	}
	
	// Instantiate a preset by name reference
	static function InstantiatePreset (presetName : String) : PlaygroundParticles {
		var presetGo : GameObject = ResourceInstantiate("Presets/"+presetName);
		var presetParticles : PlaygroundParticles = presetGo.GetComponent(PlaygroundParticles);
		if (presetParticles!=null) {
			if (reference==null)
				Playground.ResourceInstantiate("Playground Manager");
			if (reference) {
				if (reference.autoGroup && presetParticles.particleSystemTransform.parent==null)
					presetParticles.particleSystemTransform.parent = referenceTransform;
				particlesQuantity++;
				reference.particleSystems.Add(presetParticles);
				presetParticles.particleSystemId = particlesQuantity;
			}
			presetGo.name = presetName;
			return presetParticles;
		} else return null;
	}
	
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Shared functions
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// Return pixels from a texture
	static function GetPixels (image : Texture2D) : Color32[] {
		if (reference && reference.pixelFilterMode==PIXELMODE.Bilinear) {
			var pixels : Color32[] = new Color32[image.width*image.height];
			for (var y = 0; y<image.height; y++) {
				for (var x = 0; x<image.width; x++) {
					pixels[(y*image.width)+x] = image.GetPixelBilinear((x*1.0)/image.width, (y*1.0)/image.height); 
				}
			}
			return pixels;
		} else return image.GetPixels32();
	}
	
	// Return offset based on image size
	static function Offset (origin : PLAYGROUNDORIGIN, imageWidth : int, imageHeight : int, meshScale : float) : Vector3 {
		var offset : Vector3 = Vector3.zero;
		switch (origin) {
			case PLAYGROUNDORIGIN.TopLeft: 		offset.y = -imageHeight*meshScale; break;
			case PLAYGROUNDORIGIN.TopCenter: 		offset.y = -imageHeight*meshScale; offset.x = (-imageWidth*meshScale)/2; break;
			case PLAYGROUNDORIGIN.TopRight: 		offset.y = -imageHeight*meshScale; offset.x = (-imageWidth*meshScale); break;
			case PLAYGROUNDORIGIN.MiddleLeft: 	offset.y = -imageHeight*meshScale/2; break;
			case PLAYGROUNDORIGIN.MiddleCenter:	offset.y = -imageHeight*meshScale/2; offset.x = (-imageWidth*meshScale)/2; break;
			case PLAYGROUNDORIGIN.MiddleRight:	offset.y = -imageHeight*meshScale/2; offset.x = (-imageWidth*meshScale); break;
			case PLAYGROUNDORIGIN.BottomCenter:	offset.x = (-imageWidth*meshScale)/2; break;
			case PLAYGROUNDORIGIN.BottomRight:	offset.x = (-imageWidth*meshScale); break;
		}
		return offset;
	}
	
	// Return random vector3-array
	static function RandomVector3 (length : int, min : Vector3, max : Vector3) : Vector3[] {
		var v3 : Vector3[] = new Vector3[length];
		for (var i = 0; i<length; i++) {
			v3[i] = Vector3(
				UnityEngine.Random.Range(min.x, max.x),
				UnityEngine.Random.Range(min.y, max.y),
				UnityEngine.Random.Range(min.z, max.z)
			);
		}
		return v3;
	}
	
	// Returns a float array by random values
	static function RandomFloat (length : int, min : float, max : float) : float[] {
		var f : float[] = new float[length];
		for (var i = 0; i<length; i++) {
			f[i] = UnityEngine.Random.Range(min, max);
		}
		return f;
	}
	
	// Shuffle an existing float array
	static function ShuffleFloat (arr : float[]) {
		var r : int;
		var tmp : float;
		for (var i : int = arr.length - 1; i >= 0; i--) {
			r = UnityEngine.Random.Range(0,i);
			tmp = arr[i];
			arr[i] = arr[r];
			arr[r] = tmp;
		}
	}
	
	// Compare and return largest array
	static function Largest (compare : int[]) : int {
		var largest : int;
		for (var i = 0; i<compare.Length; i++)
			if (compare[i]>largest) largest = i;
		return largest;
	}
	
	// Count the completely transparent pixels in a Texture2D
	static function CountZeroAlphasInTexture (image : Texture2D) : int {
		var alphaCount : int;
		var pixels : Color32[] = image.GetPixels32();
		for (var x = 0; x<pixels.Length; x++)
			if (pixels[x].a==0)
				alphaCount++;
		return alphaCount;
	}
	
	// Instantiate from Resources
	static function ResourceInstantiate (n : String) : GameObject {
		var res : GameObject = UnityEngine.Resources.Load("JavaScript/"+n) as GameObject;
		if (!res)
			return null;
		var go : GameObject = Instantiate(res);
		go.name = n;
		return go;
	}
	
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Playground Manager
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	static function TimeReset () {
		globalDeltaTime = .0;
		#if UNITY_EDITOR
		if (!UnityEditor.EditorApplication.isPlaying) {
			globalTime = 0;
		} else
			globalTime = Time.timeSinceLevelLoad;
		#else
			globalTime = Time.timeSinceLevelLoad;
		#endif
		lastTimeUpdated = globalTime;
	}
	
	// Initializes the manager and particle systems
	function InitializePlayground () {

		// Check for duplicates of the Playground Manager
		if (reference!=null && reference!=this) {
			yield;
			Debug.Log("There can only be one instance of the Playground Manager in the scene.");
			DestroyImmediate(this.gameObject);
			return;
		}

		// Reset time
		TimeReset();
		
		// Cache objects
		for (var i = 0; i<particleSystems.Count; i++) {
			if (particleSystems[i]!=null) {
				particleSystems[i].particleSystemGameObject = particleSystems[i].gameObject;
				particleSystems[i].particleSystemTransform = particleSystems[i].transform;
				particleSystems[i].particleSystemRenderer = particleSystems[i].renderer;
				particleSystems[i].particleSystemRenderer2 = particleSystems[i].gameObject.particleSystem.renderer as ParticleSystemRenderer;
				particleSystems[i].shurikenParticleSystem = particleSystems[i].particleSystemGameObject.GetComponent(ParticleSystem);
				particleSystems[i].shurikenParticleSystem.Clear();
				particleSystems[i].particleSystemId = i;
				
				if (particleSystems[i].sourceTransform==null)
					particleSystems[i].sourceTransform = particleSystems[i].particleSystemTransform;
			} else {
				particleSystems.RemoveAt(i);
				i--;
			}
		}
		
		// Set quantity counter
		particlesQuantity = particleSystems.Count;
	}
	
	function SetTime () {
		
		// Set time
		#if UNITY_EDITOR
		if (!UnityEditor.EditorApplication.isPlaying)
			globalTime += (Time.realtimeSinceStartup-lastTimeUpdated)*globalTimescale;
		else
			globalTime += (Time.timeSinceLevelLoad-lastTimeUpdated)*globalTimescale;
		#else
			globalTime += (Time.timeSinceLevelLoad-lastTimeUpdated)*globalTimescale;
		#endif
		
		
		// Set delta time
		globalDeltaTime = globalTime-lastTimeUpdated;
		
		// Set interval stamp
		lastTimeUpdated = globalTime;
	}


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// MonoBehaviours
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// OnEnable is called in both Edit- and Play Mode
	// Initializes all particle systems
	function OnEnable () {

		// Cache the Playground reference
		reference = this;
		referenceTransform = transform;
		referenceGameObject = gameObject;

		// Initialize
		InitializePlayground();
	}

	// Set initial time
	function Start () {SetTime();}
	
	// PlaygroundUpdate is called both in Edit- and Play Mode
	// Updates all particle systems
	function Update () {
		SetTime();
		
		for (var i = 0; i<particleSystems.Count; i++) {
			if (particleSystems[i]!=null &&
			    particleSystems[i].shurikenParticleSystem!=null &&
			    particleSystems[i].particleSystemGameObject.activeInHierarchy && 
			    Time.frameCount%particleSystems[i].updateRate==0) {
				PlaygroundParticles.Update(particleSystems[i]);
			}
		}
		
	}
	
	// Reset time when turning back to the editor
	#if UNITY_EDITOR
	function OnApplicationPause (pauseStatus : boolean) {
		if (!pauseStatus) {
			TimeReset();
			yield;
			for(var p : PlaygroundParticles in particleSystems)
				PlaygroundParticles.SetParticleTimeNow(p);
		}
	}
	#endif
}

// Holds information about a Paint source
class PaintObject {
	@HideInInspector var paintPositions : List.<PaintPosition>; // Position data for each origin point
	@HideInInspector var positionLength : int;			// The current length of position array
	@HideInInspector var lastPaintPosition : Vector3;	// The last painted position
	var spacing : float;								// The required space between the last and current paint position 
	var layerMask : LayerMask = -1;						// The layers this PaintObject sees when painting
	var brush : PlaygroundBrush;						// The brush data for this PaintObject
	var exceedMaxStopsPaint : boolean;					// Should painting stop when paintPositions is equal to maxPositions (if false paint positions will be removed from list when painting new ones)
	var initialized : boolean = false;					// Is this PaintObject initialized yet?
	
	// Initializes a PaintObject for painting
	function Initialize () {
		paintPositions = new List.<PaintPosition>();
		brush = new PlaygroundBrush();
		lastPaintPosition = Playground.initialTargetPosition;
		initialized = true;
	}
	
	// Live paint into this PaintObject using a Ray and color information
	function Paint (ray : Ray, color : Color32) : boolean {		
		var hit : RaycastHit;
		
		if (Physics.Raycast(ray, hit, brush.distance, layerMask)) {
			//if (Vector3.Distance(hit.point, lastPaintPosition)<=spacing) return;
			Paint(hit.point, hit.normal, hit.transform, color);
		}
		
		if (hit!=null) {
			lastPaintPosition = hit.point;
			return true;
		}
		return false;
	}
	
	// Live paint into this PaintObject (single point with information about position, normal, parent and color), returns current painted index
	function Paint (pos : Vector3, norm : Vector3, parent : Transform, color : Color32) : int {		
		var pPos : PaintPosition = new PaintPosition();
		
		if (parent) {
			pPos.parent = parent;
			pPos.initialPosition = parent.InverseTransformPoint(pos);
			pPos.normal = parent.InverseTransformDirection(norm);
		} else {
			pPos.initialPosition = pos;
			pPos.normal = norm;
		}
		
		pPos.color = color;
		pPos.position = pPos.initialPosition;
		
		pPos.initialNormal = pPos.normal;
		paintPositions.Add(pPos);
				
		positionLength = paintPositions.Count;
		if (!exceedMaxStopsPaint && positionLength>Playground.reference.paintMaxPositions) {
			paintPositions.RemoveAt(0);
			positionLength = paintPositions.Count;
		}
		
		return positionLength-1;
	}
	
	// Erase in this PaintObject using a position and radius, returns true if position was erased
	function Erase (pos : Vector3, radius : float) : boolean {
		var erased : boolean = false;
		if (!paintPositions || paintPositions.Count==0) return false;
		for (var i = 0; i<paintPositions.Count; i++) {
			if (Vector3.Distance(paintPositions[i].position, pos)<radius) {
				paintPositions.RemoveAt(i);
				erased = true;
			}
		}
		positionLength = paintPositions.Count;
		return erased;
	}
	
	// Erase in this PaintObject using a specified index, returns true if position at index was found and removed
	function Erase (index : int) {
		if (index>=0 && index<positionLength) {
			paintPositions.RemoveAt(index);
			positionLength = paintPositions.Count;
			return true;
		} else return false;
	}
	
	// Return position at index of PaintObject's PaintPosition
	function GetPosition (index : int) : Vector3 {
		index=index%positionLength;
		return paintPositions[index].position;
	}
	
	// Return color at index of PaintObject's PaintPosition
	function GetColor (index : int) : Color32 {
		index=index%positionLength;
		return paintPositions[index].color;
	}
	
	// Return normal at index of PaintObject's PaintPosition
	function GetNormal (index : int) : Vector3 {
		index=index%positionLength;
		return paintPositions[index].normal;
	}
	
	// Return parent at index of PaintObject's PaintPosition
	function GetParent (index : int) : Transform {
		index=index%positionLength;
		return paintPositions[index].parent;
	}
	
	// Live positioning of paintPositions regarding their parent
	// This happens automatically in PlaygroundParticles.Update() - only use this if you need to set all positions at once
	function Update () {
		for (var i = 0; i<paintPositions.Count; i++) {
			if (paintPositions[i].parent==null)
				continue;
			else
				Update(i);
		}
	}
	
	// Update specific position
	function Update (thisPosition : int) {
		thisPosition = thisPosition%positionLength;
		if (!paintPositions[thisPosition].parent) {
			RemoveNonParented(thisPosition);
			return;
		}
		paintPositions[thisPosition].position = paintPositions[thisPosition].parent.TransformPoint(paintPositions[thisPosition].initialPosition);
		paintPositions[thisPosition].normal = paintPositions[thisPosition].parent.TransformDirection(paintPositions[thisPosition].initialNormal);
	}
	
	// Clear out all emission positions where the parent transform has been removed
	function RemoveNonParented () {
		for (var i = 0; i<paintPositions.Count; i++)
			if (paintPositions[i].parent==null) {
				paintPositions.RemoveAt(i);
				i--;
			}
	}
	
	// Clear out emission position where the parent transform has been removed
	// Returns true if this position didn't have a parent
	function RemoveNonParented (thisPosition : int) : boolean {
		thisPosition = thisPosition%positionLength;
		if (paintPositions[thisPosition].parent==null) {
			paintPositions.RemoveAt(thisPosition);
			return true;
		}
		return false;
	}
	
	// Clear out the painted positions
	function ClearPaint () {
		paintPositions = new List.<PaintPosition>();
		positionLength = 0;
	}
	
	// Clone this PaintObject
	function Clone () : PaintObject {
		var paintObject : PaintObject = new PaintObject();
		if (this.paintPositions)
			paintObject.paintPositions.AddRange(this.paintPositions);
		paintObject.positionLength = this.positionLength;
		paintObject.lastPaintPosition = this.lastPaintPosition;
		paintObject.spacing = this.spacing;
		paintObject.layerMask = this.layerMask;
		if (this.brush)
			paintObject.brush = this.brush.Clone();
		else
			paintObject.brush = new PlaygroundBrush();
		paintObject.exceedMaxStopsPaint = this.exceedMaxStopsPaint;
		paintObject.initialized = this.initialized;
		return paintObject;
	}
}

// Constructor for a painted position
class PaintPosition {
	var position : Vector3;				// Emission spot in local position of parent
	var color : Color32;				// Color of emission spot
	var normal : Vector3;				// Direction to emit from the paint position
	var parent : Transform;				// The parent transform
	var initialPosition : Vector3;		// The first position where this originally were painted
	var initialNormal : Vector3;		// The first normal direction when painted
}

// Holds information about a brush used for source painting
class PlaygroundBrush {
	var texture : Texture2D;						// The texture to construct this Brush from
	var scale : float = 1.0;						// The scale of this Brush (measured in Units)
	var detail : BRUSHDETAIL = BRUSHDETAIL.High;	// The detail level of this brush
	var distance : float = 10000;					// The distance the brush reaches
	@HideInInspector var color : Color32[];			// Color data of this brush
	@HideInInspector var colorLength : int;			// The length of color array
	
	// Set the texture of this brush
	function SetTexture (newTexture : Texture2D) {
		texture = newTexture;
		Construct();
	}
	
	// Cache the color information from this brush
	function Construct () {
		color = Playground.GetPixels(texture);
		colorLength = color.Length;
	}
	
	// Return color at index of Brush
	function GetColor (index : int) : Color32 {
		index=index%colorLength;
		return color[index];
	}
	
	// Set color at index of Brush
	function SetColor (c : Color32, index : int) {
		color[index] = c;
	}
	
	// Clone this PlaygroundBrush
	function Clone () : PlaygroundBrush {
		var playgroundBrush : PlaygroundBrush = new PlaygroundBrush();
		playgroundBrush.texture = this.texture;
		playgroundBrush.scale = this.scale;
		playgroundBrush.detail = this.detail;
		playgroundBrush.distance = this.distance;
		playgroundBrush.color = this.color.Clone() as Color32[];
		playgroundBrush.colorLength = this.colorLength;
		return playgroundBrush;
	}
}

// Holds information about a State source
class ParticleState {
	@HideInInspector private var color : Color32[]; // Color data
	@HideInInspector private var position : Vector3[]; // Position data
	@HideInInspector private var normals : Vector3[]; // Normal data
	var stateTexture : Texture2D;				// The texture to construct this state from (used to color each vertex if mesh is used)
	var stateDepthmap : Texture2D;				// The texture to use as depthmap for this state. A grayscale image of the same size as stateTexture is required
	var stateDepthmapStrength : float = 1.0;	// How much the grayscale from stateDepthmap will affect z-value
	var stateMesh : Mesh;						// The mesh used to set this state's positions. Positions will be calculated per vertex.
	var stateName : String;						// The name of this state
	var stateScale : float;						// The scale of this state (measured in units)
	var stateOffset : Vector3;					// The offset of this state in world position (measured in units)
	var stateTransform : Transform;				// The transform that act as parent to this state
	@HideInInspector var initialized : boolean = false;
	@HideInInspector var colorLength : int;		// The length of color array
	@HideInInspector var positionLength : int;	// The length of position array
	
	// Initializes a ParticleState for construction
	function Initialize () {
		if (stateMesh==null && stateTexture!=null) {
			ConstructParticles(stateTexture, stateScale, stateOffset, stateName, stateTransform);
			if (stateDepthmap!=null)
				SetDepthmap(); 
		} else if (stateMesh!=null && stateTexture!=null) {
			ConstructParticles(stateMesh, stateTexture, stateScale, stateOffset, stateName, stateTransform);
		} else if (stateMesh!=null && stateTexture==null) {
			ConstructParticles(stateMesh, stateScale, stateOffset, stateName, stateTransform);
		} else Debug.Log("State Texture or State Mesh returned null. Please assign an object to State: "+stateName+".");
		initialized = true;
	}
	
	// Construct Image data
	function ConstructParticles (image : Texture2D, scale : float, offset : Vector3, newStateName : String, newStateTransform : Transform) {
		var tmpColor : List.<Color32> = new List.<Color32>();
		var tmpPosition : List.<Vector3> = new List.<Vector3>();
		color = Playground.GetPixels(image);
		var readAlpha : boolean = false;
		if (Playground.reference)
			readAlpha = Playground.reference.buildZeroAlphaPixels;
		
		var x : int = 0;
		var y : int = 0;
		for (var i = 0; i<color.Length; i++) {
			if (readAlpha || color[i].a!=0) {
				tmpColor.Add(color[i]);
				tmpPosition.Add(Vector3(x,y,0));			
			}
			x++; x=x%image.width;
			if (x==0 && i!=0) y++;
		}
		color = tmpColor.ToArray() as Color32[];
		position = tmpPosition.ToArray() as Vector3[];
		tmpColor = null;
		tmpPosition = null;
		normals = new Vector3[position.Length];
		for (var n : Vector3 in normals) n = Vector3.forward;
		
		stateTransform = newStateTransform;
		stateTexture = image;
		colorLength = color.Length;
		positionLength = position.Length;
		stateScale = scale;
		stateOffset = offset;
		stateName = newStateName;
		initialized = true;
	}
	
	// Construct Mesh data with texture
	function ConstructParticles (mesh : Mesh, texture : Texture2D, scale : float, offset : Vector3, newStateName : String, newStateTransform : Transform) {
		position = mesh.vertices.Clone() as Vector3[];
		normals = mesh.normals.Clone() as Vector3[];
		var uvs : Vector2[] = mesh.uv.Clone() as Vector2[];
		color = new Color32[uvs.Length];
		for (var i = 0; i<position.Length; i++) {
			color[i] = texture.GetPixelBilinear(uvs[i].x, uvs[i].y);
		}
		stateMesh = mesh;
		stateTransform = newStateTransform;
		colorLength = color.Length;
		positionLength = position.Length;
		stateScale = scale;
		stateOffset = offset;
		stateName = newStateName;
		initialized = true;
	}
	
	// Construct Mesh data
	function ConstructParticles (mesh : Mesh, scale : float, offset : Vector3, newStateName : String, newStateTransform : Transform) {
		position = mesh.vertices.Clone() as Vector3[];
		normals = mesh.normals.Clone() as Vector3[];
		var uvs : Vector2[] = mesh.uv.Clone() as Vector2[];
		color = new Color32[uvs.Length];
		stateMesh = mesh;
		stateTransform = newStateTransform;
		colorLength = color.Length;
		positionLength = position.Length;
		stateScale = scale;
		stateOffset = offset;
		stateName = newStateName;
		initialized = true;
	}
	
	// Set depth map to ParticleState
	function SetDepthmap () {
		var depthMapPixels : Color32[] = Playground.GetPixels(stateDepthmap);
		var z : float;
		for (var x = 0; x<depthMapPixels.Length; x++) {
			z = ((depthMapPixels[x].r+depthMapPixels[x].g+depthMapPixels[x].b)/3);
			position[x%position.Length].z -= (z*stateDepthmapStrength/255);
		}
	}
	
	// Return color at index of ParticleState
	function GetColor (index : int) : Color32 {
		index=index%colorLength;
		return color[index];
	}
	
	// Return position at index of ParticleState
	function GetPosition (index : int) : Vector3 {
		index=index%positionLength;
		return (position[index]+stateOffset)*stateScale;
	}
	
	// Return normal at index of ParticleState
	function GetNormal (index : int) : Vector3 {
		index=index%positionLength;
		return normals[index];
	}
	
	// Return colors in ParticleState
	function GetColors () : Color32[] {
		return color.Clone() as Color32[];
	}
	
	// Return positions in ParticleState
	function GetPositions () : Vector3[] {
		return position.Clone() as Vector3[];
	}
	
	// Return normals in ParticleState
	function GetNormals () : Vector3[] {
		return normals.Clone() as Vector3[];
	}
	
	// Set color at index of ParticleState
	function SetColor (index : int, c : Color32) {
		color[index] = c;
	}
	
	// Set position at index of ParticleState
	function SetPosition (index : int, v : Vector3) {
		position[index] = v;
	}
	
	// Set normal at index of ParticleState
	function SetNormal (index : int, v : Vector3) {
		normals[index] = v;
	}
	
	// Return position from parent's TransformPoint
	function GetParentedPosition (thisPosition : int) {
		thisPosition = thisPosition%positionLength;
		return stateTransform.TransformPoint((position[thisPosition]+stateOffset)*stateScale);
	}
	
	// Return a copy of this ParticleState
	function Clone () : ParticleState {
		var particleState : ParticleState = new ParticleState();
		particleState.color = new Color32[this.color.Length];
		particleState.position = new Vector3[this.position.Length];
		particleState.normals = new Vector3[this.normals.Length];
		particleState.color = this.color.Clone() as Color32[];
		particleState.position = this.position.Clone() as Vector3[];
		particleState.normals = this.normals.Clone() as Vector3[];
		particleState.stateName = this.stateName;
		particleState.stateScale = this.stateScale;
		particleState.stateTexture = this.stateTexture;
		particleState.stateDepthmap = this.stateDepthmap;
		particleState.stateDepthmapStrength = this.stateDepthmapStrength;
		particleState.stateMesh = this.stateMesh;
		particleState.stateOffset = this.stateOffset;
		particleState.colorLength = this.colorLength;
		particleState.positionLength = this.positionLength;
		particleState.stateTransform = this.stateTransform;
		return particleState;
	}
}

class ParticleProjection {
	@HideInInspector private var sourceColors : Color32[];		// Color data
	@HideInInspector private var sourcePositions : Vector3[];	// Position data
	@HideInInspector private var targetPositions : Vector3[];	// Projected position data
	@HideInInspector private var targetNormals : Vector3[];		// Projected normal data
	@HideInInspector private var targetParents : Transform[];	// Projected parent data
	var projectionTexture : Texture2D;							// The texture to project
	var projectionOrigin : Vector2; 							// The origin offset in Units
	var projectionTransform : Transform;						// Transform to project from
	var projectionDistance : float = 1000;						// The distance in Units the projection travels
	var projectionScale : float = .1;							// The scale of projection in Units
	var projectionMask : LayerMask = -1;						// Layers seen by projection
	var surfaceOffset : float = .0;								// The offset from projected surface
	var liveUpdate : boolean = true;							// Update this projector each frame
	@HideInInspector var initialized : boolean = false;			// Is this projector ready?
	@HideInInspector var colorLength : int;						// The length of color array
	@HideInInspector var positionLength : int;					// The length of position array
	
	// Initialize this ParticleProjection object
	function Initialize () {
		if (!projectionTexture) return;
		Construct(projectionTexture, projectionTransform);
		if (!liveUpdate)
			Update();
		initialized = true;
	}
	
	// Build source data
	function Construct (image : Texture2D, transform : Transform) {
		var tmpColor : List.<Color32> = new List.<Color32>();
		var tmpPosition : List.<Vector3> = new List.<Vector3>();
		sourceColors = Playground.GetPixels(image);
		var readAlpha : boolean = false;
		if (Playground.reference)
			readAlpha = Playground.reference.buildZeroAlphaPixels;
		
		var x : int = 0;
		var y : int = 0;
		for (var i = 0; i<sourceColors.Length; i++) {
			if (readAlpha || sourceColors[i].a!=0) {
				tmpColor.Add(sourceColors[i]);
				tmpPosition.Add(Vector3(x*projectionScale,y*projectionScale,0));			
			}
			x++; x=x%image.width;
			if (x==0 && i!=0) y++;
		}
		sourceColors = tmpColor.ToArray() as Color32[];
		sourcePositions = tmpPosition.ToArray() as Vector3[];
		tmpColor = null;
		tmpPosition = null;
		targetPositions = new Vector3[sourcePositions.Length];
		targetNormals = new Vector3[sourcePositions.Length];
		targetParents = new Transform[sourcePositions.Length];
		positionLength = sourcePositions.Length;
		colorLength = sourceColors.Length;
		projectionTexture = image;
		projectionTransform = transform;
	}
	
	// Project all particle sources (only call this if you need to set all particles at once)
	function Update () {
		for (var i = 0; i<positionLength; i++)
			Update(i);
	}
	
	// Project a single particle source
	function Update (index : int) {
		index=index%positionLength;
		var hit : RaycastHit;
		var sourcePosition : Vector3 = projectionTransform.TransformPoint(sourcePositions[index]+projectionOrigin);
	    if (Physics.Raycast(sourcePosition, projectionTransform.forward, hit, projectionDistance, projectionMask)) {
			targetPositions[index] = hit.point+(hit.normal*surfaceOffset);
			targetNormals[index] = hit.normal;
			targetParents[index] = hit.transform;
	    } else {
	    	targetPositions[index] = Playground.initialTargetPosition;
	    	targetNormals[index] = Vector3.forward;
	    	targetParents[index] = null;
	    }
	}
	
	// Return color at index of ParticleProjection
	function GetColor (index : int) : Color32 {
		index=index%colorLength;
		return sourceColors[index];
	}
	
	// Return position at index of ParticleProjection
	function GetPosition (index : int) : Vector3 {
		index=index%positionLength;
		return (targetPositions[index]);
	}
	
	// Return normal at index of ParticleProjection
	function GetNormal (index : int) : Vector3 {
		index=index%positionLength;
		return targetNormals[index];
	}
	
	// Return parent at index of ParticleProjection's projected position
	function GetParent (index : int) : Transform {
		index=index%positionLength;
		return targetParents[index];
	}
}

// Holds AnimationCurves in X, Y and Z variables
class Vector3AnimationCurve {
	var x : AnimationCurve;	// AnimationCurve for X-axis
	var y : AnimationCurve;	// AnimationCurve for Y-axis
	var z : AnimationCurve;	// AnimationCurve for Z-axis
	
	// Return a vector3 at time
	function Evaluate (time : float) : Vector3 {
		return Vector3(this.x.Evaluate(time), this.y.Evaluate(time), this.z.Evaluate(time));
	}
	
	// Return a copy of this Vector3AnimationCurve
	function Clone () : Vector3AnimationCurve {
		var vector3AnimationCurveClone : Vector3AnimationCurve = new Vector3AnimationCurve();
		vector3AnimationCurveClone.x = new AnimationCurve(this.x.keys);
		vector3AnimationCurveClone.y = new AnimationCurve(this.y.keys);
		vector3AnimationCurveClone.z = new AnimationCurve(this.z.keys);
		return vector3AnimationCurveClone;
	}
}

// Holds information about a World Object
class WorldObjectBase {
	var gameObject : GameObject;						// The GameObject of this World Object
	@HideInInspector var transform : Transform;			// The Transform of this World Object
	@HideInInspector var rigidbody : Rigidbody;			// The Rigidbody of this World Object
	@HideInInspector var cachedId : int;				// The id of this World Object (used to keep track when this object changes)
	@HideInInspector var meshFilter : MeshFilter;		// The mesh filter of this World Object (will be null for skinned meshes)
	@HideInInspector var mesh : Mesh;					// The mesh of this World Object
	@HideInInspector var vertexPositions : Vector3[];	// The vertices of this World Object
	@HideInInspector var normals : Vector3[];			// The normals of this World Object
}

class WorldObject extends WorldObjectBase {
	@HideInInspector var renderer : Renderer;
	
	// Initialize this WorldObject
	function Initialize () {
		if (meshFilter!=null) {
			mesh = meshFilter.sharedMesh;
			if (mesh!=null) {
				vertexPositions = mesh.vertices;
				normals = mesh.normals;
			}
		}
	}
}

class SkinnedWorldObject extends WorldObjectBase {
	@HideInInspector var renderer : SkinnedMeshRenderer;
}

// Holds information about a Manipulator Object
class ManipulatorObject {
	var type : MANIPULATORTYPE;							// The type of this manipulator
	var property : ManipulatorProperty; 				// The property settings (if type is property)
	var properties : List.<ManipulatorProperty> = new List.<ManipulatorProperty>(); // The combined properties (if type is combined)
	var affects : LayerMask;							// The layers this manipulator will affect
	var transform : Transform;							// The transform of this manipulator
	var shape : MANIPULATORSHAPE;						// The shape of this manipulator
	var size : float;									// The size of this manipulator (if shape is sphere)
	var bounds : Bounds;								// The bounds of this manipulator (if shape is box)
	var strength : float;								// The strength of this manipulator
	var enabled : boolean = true;						// Is this manipulator enabled?
	var inverseBounds : boolean = false;				// Should this manipulator be checking for particles inside or outside the shape's bounds?
	
	// Check if manipulator contains position
	function Contains (position : Vector3, mPosition : Vector3) : boolean {
		if (shape==MANIPULATORSHAPE.Box) {
			if (!inverseBounds)
				return bounds.Contains(position-mPosition);
			else
				return !bounds.Contains(position-mPosition);
		} else {
			if (!inverseBounds)
				return (Vector3.Distance(position, mPosition)<=size);
			else
				return (Vector3.Distance(position, mPosition)>=size);
		}
		return false;
	}
	
	// Return a copy of this ManipulatorObject
	function Clone () : ManipulatorObject {
		var manipulatorObject : ManipulatorObject = new ManipulatorObject();
		manipulatorObject.type = this.type;
		manipulatorObject.property = this.property.Clone();
		manipulatorObject.affects = this.affects;
		manipulatorObject.transform = this.transform;
		manipulatorObject.size = this.size;
		manipulatorObject.shape = this.shape;
		manipulatorObject.bounds = this.bounds;
		manipulatorObject.strength = this.strength;
		manipulatorObject.enabled = this.enabled;
		manipulatorObject.inverseBounds = this.inverseBounds;
		manipulatorObject.properties = new List.<ManipulatorProperty>();
		for (var i = 0; i<properties.Count; i++)
			manipulatorObject.properties.Add(properties[i].Clone());
		return manipulatorObject;
	}
}

class ManipulatorProperty {
	var type : MANIPULATORPROPERTYTYPE;					// The type of this manipulator property
	var transition : MANIPULATORPROPERTYTRANSITION;		// The transition of this manipulator property
	var velocity : Vector3;								// The velocity of this manipulator property
	var color : Color = new Color(1,1,1,1);				// The color of this manipulator property
	var lifetimeColor : Gradient;						// The lifetime color of this manipulator property
	var size : float = 1.0;								// The size of this manipulator property
	var targets : List.<Transform> = new List.<Transform>(); // The target transforms to position towards of this manipulator property
	var targetPointer : int;							// The pointer of targets (used for calculation)
	var useLocalRotation : boolean = false;				// Should the manipulator's transform direction be used to apply velocity?
	var onlyColorInRange : boolean = true;				// Should the particles go back to original color when out of range?
	var keepColorAlphas : boolean = true;				// Should the particles keep their original color alpha?
	var onlyPositionInRange : boolean = true;			// Should the particles stop positioning towards target when out of range?
	var zeroVelocityStrength : float = 1.0;				// The strength to zero velocity on target positioning when using transitions
	var strength : float = 1.0;
	var unfolded : boolean = true;
	
	function Clone () : ManipulatorProperty {
		var manipulatorProperty : ManipulatorProperty = new ManipulatorProperty();
		manipulatorProperty.type = this.type;
		manipulatorProperty.transition = this.transition;
		manipulatorProperty.velocity = this.velocity;
		manipulatorProperty.color = this.color;
		manipulatorProperty.lifetimeColor = this.lifetimeColor;
		manipulatorProperty.size = this.size;
		manipulatorProperty.useLocalRotation = this.useLocalRotation;
		manipulatorProperty.onlyColorInRange = this.onlyColorInRange;
		manipulatorProperty.keepColorAlphas = keepColorAlphas;
		manipulatorProperty.onlyPositionInRange = onlyPositionInRange;
		manipulatorProperty.zeroVelocityStrength = zeroVelocityStrength;
		manipulatorProperty.strength = strength;
		manipulatorProperty.targetPointer = targetPointer;
		manipulatorProperty.targets = new List.<Transform>();
		for (var i = 0; i<targets.Count; i++)
			manipulatorProperty.targets.Add(targets[i]);
		return manipulatorProperty;
	}
}

class PlaygroundCollider {
	var enabled : boolean = true;						// Is this PlaygroundCollider enabled?
	var transform : Transform;							// The transform that makes this PlaygroundCollider
	var plane : Plane = new Plane();					// The plane of this PlaygroundCollider
	
	// Update this PlaygroundCollider's plane
	function UpdatePlane (delta : boolean) {
		if (!transform) return;
		plane.SetNormalAndPosition(transform.up, transform.position);
	}
	
	function Clone () : PlaygroundCollider {
		var playgroundCollider : PlaygroundCollider = new PlaygroundCollider();
		playgroundCollider.enabled = this.enabled;
		playgroundCollider.transform = this.transform;
		playgroundCollider.plane = new Plane(this.plane.normal, this.plane.distance);
		return playgroundCollider;
	}
}

class PlaygroundAxisConstraints {
	var x : boolean = false;
	var y : boolean = false;
	var z : boolean = false;
}

enum MANIPULATORTYPE {
	None,
	Attractor,
	AttractorGravitational,
	Repellent,
	Property,
	Combined
}

enum MANIPULATORPROPERTYTYPE {
	None,
	Color,
	Velocity,
	AdditiveVelocity,
	Size,
	Target,
	Death,
	Attractor,
	Gravitational,
	Repellent,
	LifetimeColor
}

enum MANIPULATORPROPERTYTRANSITION {
	None,
	Lerp,
	Linear
}

enum MANIPULATORSHAPE {
	Sphere,
	Box
}

enum PLAYGROUNDORIGIN {
	TopLeft, TopCenter, TopRight,
	MiddleLeft, MiddleCenter, MiddleRight,
	BottomLeft, BottomCenter, BottomRight
}

enum PIXELMODE {
	Bilinear,
	Pixel32
}

enum COLORSOURCE {
	Source,
	LifetimeColor
}

enum OVERFLOWMODE {
	SourceTransform,
	ParticleSystemTransform,
	World,
	SourcePoint
}

enum SOURCESCATTERSPACE {
	Local=0,
	Global=1
}

enum EMISSIONMODE {
	Time,
	Scripted
}

enum LERPTYPE {
	PositionColor,
	Position,
	Color,
}

enum MESHMODE {
	Uv,
	Vertex
}

enum SOURCE {
	State,
	Transform,
	WorldObject,
	SkinnedWorldObject,
	Script,
	Paint,
	Projection
}

enum TRANSITION {
	None,
	Lerp,
	Fade,
	Fade2
}

enum SORTING {
	Scrambled,
	ScrambledLinear,
	Burst,
	Linear,
	Reversed,
	NearestNeighbor,
	NearestNeighborReversed,
	Custom
}

enum BRUSHDETAIL {
	Perfect,
	High,
	Medium,
	Low
}