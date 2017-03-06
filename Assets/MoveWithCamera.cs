using UnityEngine;
using System.Collections;

public class MoveWithCamera : MonoBehaviour {

	Camera camera;

	// Use this for initialization
	void Start () {
		camera = Camera.main;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.position = new Vector3(transform.position.x, camera.transform.position.y, transform.position.z);
	}
}
