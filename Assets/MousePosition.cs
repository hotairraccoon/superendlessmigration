using UnityEngine;
using System.Collections;

public class MousePosition : MonoBehaviour {

	public Vector3 currentPosition;

	void Start () {
		currentPosition = transform.position;
	}

	// Update is called once per frame
	void Update () {

		currentPosition.x = Input.mousePosition.x;
		currentPosition.y = Input.mousePosition.y;
		currentPosition.z = 10f;
		transform.position = Camera.main.ScreenToWorldPoint(currentPosition);

	}
}
