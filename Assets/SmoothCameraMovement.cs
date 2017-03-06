using UnityEngine;
using System.Collections;

public class SmoothCameraMovement : MonoBehaviour {

	public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    public Transform flockLeader; //target is the lead bird in the flock that the camera follows

    public Camera camera;

	public float globalFlyOverSpeed = 2f; //reset in Start() by GameController

	void Start ()
    {
     	camera = Camera.main;
     	Controller controller = Controller.current.GetComponent<Controller>();
		flockLeader = controller.getFlockLeader();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		//Vector3 point = camera.WorldToViewportPoint(flockLeader.position);
	 	//Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(0.5f, point.y, point.z)); //(new Vector3(0.5, 0.5, point.z));
	 		
//		Vector3 destination = new Vector3(
//										Mathf.Clamp(camera.transform.position.x + delta.x, -10f, 10f),
//										camera.transform.position.y+40f*Time.deltaTime,
//										camera.transform.position.z
//									);

		//camera.transform.position = Vector3.SmoothDamp(camera.transform.position, destination, ref velocity, dampTime);

		Vector3 destination = new Vector3(
			camera.transform.position.x,
			camera.transform.position.y + globalFlyOverSpeed * Time.deltaTime,
			camera.transform.position.z);


		camera.transform.position = destination;
	}
}
