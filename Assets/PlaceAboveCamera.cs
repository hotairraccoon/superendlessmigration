using UnityEngine;
using System.Collections;

public class PlaceAboveCamera : MonoBehaviour {
	
	public bool placed = false;
	
	private Vector3 startPos;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (placed == false){
			Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
			
			if (screenPos.y < Camera.main.pixelHeight * 1.25f)
			{
				placed = true; 
				
				float randX = 0.375f + Random.value * 0.25f;
				startPos.x = Camera.main.ViewportToWorldPoint(new Vector3(randX, screenPos.y, 0.0f)).x;
				startPos.y = Camera.main.ViewportToWorldPoint(new Vector3(randX, screenPos.y, 0.0f)).y;
				startPos.z = transform.position.z;
				
				transform.position = startPos;
			}
		}
	
	}
}
