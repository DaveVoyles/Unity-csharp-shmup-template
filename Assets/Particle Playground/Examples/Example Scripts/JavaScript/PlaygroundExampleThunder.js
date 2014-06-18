#pragma strict

var sceneryLight : Transform;
private var thunderManipulator : ManipulatorObject;
private var sceneryManipulator : ManipulatorObject;
private var thunderLight : Light;
private var thisTransform : Transform;
private var cam : Camera;

function Start () {

	// Cache references of the thunder- and scenery manipulators in scene
	thunderManipulator = Playground.GetManipulator(0);
	sceneryManipulator = Playground.GetManipulator(1);
	
	// Cache a reference to the thunder light (we shortcut through the manipulator)
	thunderLight = thunderManipulator.transform.GetComponent(Light);
	
	// Cache this transform
	thisTransform = transform;
	
	// Cache the camera
	cam = Camera.main;
	
	// Start the thunder action
	while (true) {
		Thunder();
		yield;
	}
}

function Update () {
	
	//////////////////////////////////////////
	// Scenery Light
	//////////////////////////////////////////
	
	var ray : Ray = cam.ScreenPointToRay(Input.mousePosition);
	var hit : RaycastHit;
	if (Physics.Raycast(ray, hit)) {
		sceneryLight.position = hit.point;
	} else sceneryLight.position = Vector3(0,1000,0);
	
	//////////////////////////////////////////
	// Scenery Interaction
	//////////////////////////////////////////
	
	sceneryManipulator.enabled = Input.GetMouseButton(0);
		
	//////////////////////////////////////////
	// Camera control
	//////////////////////////////////////////
	
	// Set camera rotation from input
	var newRotation : Vector3 = thisTransform.rotation.eulerAngles;
	newRotation += Vector3(
		Input.GetAxis("Mouse Y")*100*Time.deltaTime, 
		-Input.GetAxis("Mouse X")*100*Time.deltaTime%360, 
		0
	);
	newRotation.x = Mathf.Clamp(newRotation.x, 0, 35);
	thisTransform.rotation = Quaternion.Euler(newRotation);
	
	// Set camera zoom from input
	var fov : float = cam.fieldOfView;
	if (Input.GetKey(KeyCode.Q)) fov += 100*Time.deltaTime;
	if (Input.GetKey(KeyCode.E)) fov -= 100*Time.deltaTime;
	cam.fieldOfView = Mathf.Clamp(fov, 26, 80);
}

private var nextThunder : float;
function Thunder () {
	if (Time.time>nextThunder) {
		
		// Enable manipulator and light
		thunderManipulator.enabled = true;
		thunderLight.enabled = true;
		
		// Set manipulator position
		thunderManipulator.transform.position.x = Random.Range(-12.0, 12.0);
		thunderManipulator.transform.position.y = Random.Range(-5.0, 5.0);
		thunderManipulator.transform.position.z = Random.Range(-10.0, 10.0);
		
		// Wait for next thunder
		yield WaitForSeconds(Random.Range(.1, 1.0));
		
		// Set time for next thunder
		nextThunder = Time.time+Random.Range(.001, 2.0);
		
		// Disable manipulator and light
		thunderManipulator.enabled = false;
		thunderLight.enabled = false;
		
	}
}