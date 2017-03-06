using UnityEngine;
using System.Collections;

public class GasExplosion : MonoBehaviour {
	
	public float scaleFactor = 0.1f;
	public float maxScale = 1f;
	private Vector3 currentScale = new Vector3(0f, 0f, 0f);
	// Use this for initialization
	void Start () {
		InvokeRepeating("Explode",0f,0.1f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Explode () {
		if (currentScale.x < maxScale){
			currentScale.x += scaleFactor;
			currentScale.y += scaleFactor;
			currentScale.z += scaleFactor;
		}
		
		transform.localScale = currentScale;
	}
}
