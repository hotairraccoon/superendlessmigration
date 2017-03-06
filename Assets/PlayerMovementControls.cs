using UnityEngine;
using System.Collections;

public class PlayerMovementControls : MonoBehaviour {

	public Rigidbody2D rb2d;

	public bool isSwiping;

	public Vector3 gameObjectSreenPoint;
    public Vector3 mousePreviousLocation;
	public Vector3 mouseCursorLocation;

	public Vector3 force;
    public Vector3 objectCurrentPosition;
    public Vector3 objectTargetPosition;
    public float topSpeed = 10f;
	public float forceMultiplier = 1f;

    public GameObject touchPosition;
	private TrailRenderer trailRenderer;

	public float xMin = -19f;
	public float xMax = 19f;


	public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;

	public float globalFlyOverSpeed = 5f; //reset in Start() by GameController
//	public GameObject globalController;

	void Start() 
    {
		rb2d = GetComponent<Rigidbody2D>();
		Debug.Log(gameObject.name);
		trailRenderer = touchPosition.GetComponent<TrailRenderer>();
    }

	void FixedUpdate () {

		if (Input.GetMouseButtonDown(0)) {

			//This grabs the position of the object in the world and turns it into the position on the screen
         	gameObjectSreenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
         	//Sets the mouse pointers vector3
         	mousePreviousLocation = new Vector3(Input.mousePosition.x, Input.mousePosition.y, gameObjectSreenPoint.z);

			isSwiping = true;

			trailRenderer.time = 0.25f;

		} else if (Input.GetMouseButton(0)) {
			mouseCursorLocation = new Vector3(Input.mousePosition.x, Input.mousePosition.y, gameObjectSreenPoint.z);
			force = forceMultiplier * (mouseCursorLocation - mousePreviousLocation);//Changes the force to be applied

			if (rb2d.velocity.magnitude > topSpeed) {
				force = rb2d.velocity.normalized * topSpeed;
            }

            mousePreviousLocation = mouseCursorLocation;
			rb2d.AddForce(force, ForceMode2D.Force);
		} else if (Input.GetMouseButtonUp(0)) {

			//Makes sure there isn't a ludicrous speed
			if (rb2d.velocity.magnitude > topSpeed) {
				force = rb2d.velocity.normalized * topSpeed;
            }

			rb2d.AddForce(force, ForceMode2D.Force);

			isSwiping = false;
		}

		if (isSwiping == false) {
			if (trailRenderer.time > 0f) {
				trailRenderer.time -= 0.05f;
			}
		}

		//set destination of flock leader based on swipes and level movement
		Vector3 destination = new Vector3(
										transform.position.x,
										transform.position.y + globalFlyOverSpeed * Time.deltaTime,
										transform.position.z
									);

		//clamp coordinates to keep flock leader within camera viewport
		float destX = Mathf.Clamp(Camera.main.WorldToViewportPoint(destination).x,0.05f,0.95f);
		float destY = Mathf.Clamp(Camera.main.WorldToViewportPoint(destination).y,0.05f,0.95f);
		float destZ = Camera.main.WorldToViewportPoint(destination).z;

		destination = new Vector3(destX,destY,destZ);

		transform.position = Vector3.SmoothDamp(transform.position, Camera.main.ViewportToWorldPoint(destination), ref velocity, dampTime);
	}	
}
