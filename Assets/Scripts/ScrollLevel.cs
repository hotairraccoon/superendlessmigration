using UnityEngine;
using System.Collections;

public class ScrollLevel : MonoBehaviour {

	public float baseSpeed = 2;
	public float speed;

	public float cameraMinZoom;
	// Use this for initialization
	void Start () {
		cameraMinZoom = GetComponent<CameraFollow>().minZoom;
	}
	
	// Update is called once per frame
	void Update () {

		// Movement
		Controller.current.defaultSpeed = (baseSpeed + (0.1f * Mathf.Pow(2, Controller.current.flockBirds.Count))) * Camera.main.orthographicSize/cameraMinZoom;
		speed = Controller.current.defaultSpeed;

		Vector3 movement = new Vector3(0, speed, 0);
		movement *= Time.deltaTime;
		transform.Translate(movement/Camera.main.orthographicSize);
		
	}
}
