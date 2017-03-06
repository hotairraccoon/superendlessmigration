using UnityEngine;
using System.Collections;

public class StartingBird : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Vector3 cameraPos = Camera.main.WorldToScreenPoint (Camera.main.gameObject.transform.position);
		this.gameObject.transform.position = new Vector3 (cameraPos.x, cameraPos.y, this.gameObject.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
