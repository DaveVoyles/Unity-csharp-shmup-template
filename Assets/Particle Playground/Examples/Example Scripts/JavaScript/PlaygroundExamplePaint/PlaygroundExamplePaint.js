#pragma strict

static var selectedColor : Color32 = Color.white;	// The color to send into our PaintObject (set by colored cubes in the scene)
var eraseRadius : float = 1.0;						// The radius of eraser in units
var paintMask : LayerMask;							// The layers we're able to paint on

var interactionObjects : Rigidbody[];				// The objects to interact with
var clampRect : Rect;								// World boundaries

private var isLiftingRigidbody : boolean = false;	// Are we currently lifting?

// Particle Playground System
private var particles : PlaygroundParticles;		// Reference to our Particle Playground System

// Cached components
private var thisTransform : Transform;				// The cached Transform of this GameObject
private var cam : Camera;							// The cached Camera of Main Camera
private var camTransform : Transform;				// The cached Transform of Main Camera

function Awake () {
	
	// Cache components
	thisTransform = transform;
	cam = Camera.main;
	camTransform = cam.transform;
}

function Start () {

	// Assign the Particle Playground System in the scene, as there only are one we know that it's number 0
	particles = Playground.GetParticles(0);
	
	yield;
	for (var rb : Rigidbody in interactionObjects) {
		rb.isKinematic = false;
		rb.WakeUp();
	}
}

function Update () {
	
	//////////////////////////////////////////
	// Camera control
	//////////////////////////////////////////
	
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
	
	
	//////////////////////////////////////////
	// Paint
	//////////////////////////////////////////
	
	// Paint on left mouse input
	if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.Space))
		Paint();
	// Erase on right mouse input
	if (Input.GetMouseButton(1))
		Erase();
	
			
	//////////////////////////////////////////
	// Rigidbody interaction
	//////////////////////////////////////////
	
	if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
		LiftRigidbody();
	
}

function FixedUpdate () {
	
	for (var i = 0; i<interactionObjects.Length; i++) {
	
		// Set strength of manipulators to the rigidbodies velocity magnitude, this will interact with nearby particles
		Playground.GetManipulator(i).strength = Mathf.Clamp(interactionObjects[i].velocity.magnitude*.5, 0, 10);
		
		// Clamp rigidbody into Rect
		ClampObject(interactionObjects[i]);
	}
}


//////////////////////////////////////////
// Example of live painting in the scene
//////////////////////////////////////////

function Paint () {
	
	// Abort if we're lifting a rigidbody
	if (isLiftingRigidbody) return;
	
	// Send a Raycast from mouse position into the screen
	var ray : Ray = cam.ScreenPointToRay(Input.mousePosition);
	var hit : RaycastHit;
	if (Physics.Raycast(ray, hit, 100, paintMask)) {
		// Paint at position of RaycastHit
		Playground.Paint(particles, hit.point, hit.normal, hit.transform, selectedColor);
	} 
}

function Erase () {
	// Send a Raycast from mouse position into the screen
	var ray : Ray = cam.ScreenPointToRay(Input.mousePosition);
	var hit : RaycastHit;
	if (Physics.Raycast(ray, hit, 100)) {
		// Paint at position of RaycastHit
		Playground.Erase(particles, hit.point, eraseRadius);
	} 
}

//////////////////////////////////////////
// Misc.
//////////////////////////////////////////

function LiftRigidbody () {

	// Send a Raycast
	var hit : RaycastHit;
	var ray : Ray = cam.ScreenPointToRay(Input.mousePosition);
	if (Physics.Raycast(ray, hit, 100) && hit.rigidbody) {
		
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
		
		isLiftingRigidbody = true;
		
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
	
	isLiftingRigidbody = false;
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