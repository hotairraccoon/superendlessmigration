using UnityEngine;
using System.Collections;

public class DestroyOffscreen : MonoBehaviour {
	
	public bool totallyDestroy = true;
	
	public bool checkOffscreenTop = false;
	public bool checkOffscreenBottom = false;
	public bool checkOffscreenRight = false;
	public bool checkOffscreenLeft = false;

	void Update (){
		if (gameObject.transform.position.x > 30f || gameObject.transform.position.x < -30f){
			RemoveThisObject();
		} else if (gameObject.transform.position.y < Camera.main.ViewportToWorldPoint(new Vector3(0.0f, -2.0f, 0.0f)).y){
			RemoveThisObject();
		} 
		
		if (checkOffscreenTop && Camera.main.WorldToViewportPoint(transform.position).y > 1.1f && transform.eulerAngles.z > 0f) {
			RemoveThisObject();
		}
		if (checkOffscreenBottom && Camera.main.WorldToViewportPoint(transform.position).y < -0.1f && transform.eulerAngles.z < 0f) {
			RemoveThisObject();
		}
		if (checkOffscreenRight && Camera.main.WorldToViewportPoint(transform.position).x > 1.1f) {
			RemoveThisObject();
		}
		if (checkOffscreenLeft && Camera.main.WorldToViewportPoint(transform.position).x < -0.1f) {
			RemoveThisObject();
		}
	}
	
	void RemoveThisObject () {
		if (gameObject.GetComponent<Bird> () != null)
		{
			if (gameObject.GetComponent<Bird> ().inFlock)
			{
				gameObject.GetComponent<Bird>().inFlock = false;
				Controller.current.flockBirds.Remove (gameObject);
				Controller.current.UpdateFlockPositions();
				Controller.current.allBirds.Remove(gameObject);
			}	
		} else if (this.gameObject.GetComponent<Aircraft> () != null){
			Controller.current.allAircraft.Remove (this.gameObject);
		}
		
		this.gameObject.SetActive (false);
		
		if (totallyDestroy){
			Destroy (this.gameObject);
		}
	}
}
