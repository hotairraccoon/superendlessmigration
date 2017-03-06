using UnityEngine;
using System.Collections;

public class MouseCursor : MonoBehaviour {
	
	
	public bool hideMouse = true;
	
	public Vector3 target = Vector3.zero;
	private Vector3 lastMousePosition = Vector3.zero;
	
	// Use this for initialization
	void Start () {
		
		if (hideMouse){
			Cursor.visible = false;	
		}	
	
	}
	
	// Update is called once per frame
	void Update () {
		
		//if (lastMousePosition != Input.mousePosition)
		//{
			target = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			lastMousePosition = Input.mousePosition;
			
			target.z = 100f; // reset z to something not crazy
			gameObject.transform.position = target;
		//}
	}
	
}
