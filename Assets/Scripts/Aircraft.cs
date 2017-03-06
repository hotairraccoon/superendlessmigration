using UnityEngine;
using System.Collections;

public class Aircraft : MonoBehaviour {
	
	public float scaleFactor = 0.85f;
	
	// Use this for initialization
	void Start () {
		this.gameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
