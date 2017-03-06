using UnityEngine;
using System.Collections;

public class PortalRay : MonoBehaviour {

	public float rotateSpeed_base = 1f;
	public float rotateSpeed;
	
	private float rotateZ;
	
	// Use this for initialization
	void Start () {
		rotateSpeed = rotateSpeed_base * 0.75f + Random.value * rotateSpeed_base * 0.5f;
		rotateSpeed *= Mathf.Pow(-1f, Mathf.Ceil(Random.value * 2f));
		rotateZ = gameObject.transform.rotation.z;
	}
	
	// Update is called once per frame
	void Update () {
		rotateZ = rotateZ + rotateSpeed;
		gameObject.transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, rotateZ);
	}
}
