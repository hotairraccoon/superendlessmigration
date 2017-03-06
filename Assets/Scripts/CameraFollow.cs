
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CameraFollow : MonoBehaviour {

	public float dampTime = 1f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;
	public float leftBounds = -15f;
	public float rightBounds = 15f;
	public int smallFlockSize = 6;
	public int largeFlockSize = 10;
	public float minZoom = 4.5f;
	public float midZoom = 5.5f;
	public float maxZoom = 6.5f;
	private float halfScreen;
	private float initialZoom;
	private float newZoom = 5;
	private float zoomTimePassed;
	public int speed = 10;
	public bool paused = false;
	private float degree;
	public float cameraRotation;
	public Vector3 destLocation;
	
	public float rotationSpeed = 1.0f;

	void Start() {
		Vector3 stageDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,0));
		halfScreen = stageDimensions.x;
		initialZoom = GetComponent<Camera>().orthographicSize;
		newZoom = GetComponent<Camera>().orthographicSize;
	}

	void OnTriggerStay2D(Collider2D other) {
		Debug.Log ("in trigger entered 2d");
	}

	void OnPlayerTriggerEvent(Dictionary<string, string> data) {
		Debug.Log ("rotate");
		paused = true;
		degree = float.Parse(data["data"]);
		cameraRotation = degree;
		destLocation = new Vector3(Controller.current.leadBird.transform.position.x, Controller.current.leadBird.transform.position.y, transform.position.z);
	}

	// Update is called once per frame
	void Update () {

		if(paused) {
			
			//camera is rotating to new angle
			float angle = Mathf.LerpAngle(transform.rotation.z, degree, Time.deltaTime * rotationSpeed);
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, degree), Time.deltaTime * rotationSpeed);
			float v = Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 0, degree));
			
			transform.position = Vector3.Slerp(transform.position, destLocation, Time.deltaTime * rotationSpeed);
			
			if(v < 1) {
				paused = false;
			}
			
		} else {
			
			Vector3 movement = transform.up * (speed * Camera.main.orthographicSize/minZoom);
			movement *= Time.deltaTime;
			transform.Translate(movement/Camera.main.orthographicSize, Space.World);
			
			if (Controller.current.flockBirds.Count > 0){
				halfScreen = ((GetComponent<Camera>().orthographicSize * 2) * (GetComponent<Camera>().aspect))/2;
				float rightCameraBoundry = GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.65f,1f,0f)).x;
				float leftCameraBoundry = GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.35f, 0f, 0f)).x;

				target = Controller.current.flockBirds [0].transform;
				Vector3 tg = GetComponent<Camera>().WorldToViewportPoint(target.position);
				//Debug.Log (tg);
				/*if (target && (target.position.x > rightCameraBoundry || target.position.x < leftCameraBoundry)) {
					Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
					Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, point.y, point.z)); //(new Vector3(0.5, 0.5, point.z));
					Vector3 destination = transform.position + delta;
					if(destination.x < (leftBounds + halfScreen)) {
						destination.x = leftBounds + halfScreen;
					} else if(destination.x > (rightBounds - halfScreen)) {
						destination.x = rightBounds - halfScreen;
					}
					transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
					if(Controller.current.flockBirds.Count < smallFlockSize && newZoom != minZoom) {
						newZoom = minZoom;
						zoomTimePassed = 0;
					} else if(Controller.current.flockBirds.Count > smallFlockSize && Controller.current.flockBirds.Count < largeFlockSize && midZoom != newZoom) {
						newZoom = midZoom;
						zoomTimePassed = 0;
					} else if(Controller.current.flockBirds.Count >= largeFlockSize && maxZoom != newZoom) {
						newZoom = maxZoom;
						zoomTimePassed = 0;
					}
		
					if(GetComponent<Camera>().orthographicSize != newZoom) {
						zoomTimePassed += Time.deltaTime;
						float t = zoomTimePassed / 5;
						GetComponent<Camera>().orthographicSize = Mathf.SmoothStep(GetComponent<Camera>().orthographicSize, newZoom, t);
					}
				}*/
			}
		}
	}

}
