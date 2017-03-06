using UnityEngine;
using System.Collections;

public class RotorSpin : MonoBehaviour {
	
	public float rotationalVelocity = 50f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.transform.Rotate( new Vector3(0f,0f,rotationalVelocity) );
	}
}
