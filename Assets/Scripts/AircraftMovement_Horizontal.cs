using UnityEngine;
using System.Collections;

public class AircraftMovement_Horizontal : MonoBehaviour {
	
	public float flightSpeed = 0.1f;
	public float flightDirectionX = 1f;
	private Quaternion flightDirection;
	private float flightPositionY;
	
	public bool setPositionToCamera = true;
	
	// Use this for initialization
	void Start () {
		if (setPositionToCamera){
			flightPositionY = Camera.main.transform.position.y + Random.value * 6f;
		} else {
			flightPositionY = transform.position.y;
		}
		flightDirection = Quaternion.Euler(this.gameObject.transform.rotation.x, this.gameObject.transform.rotation.y, 0);
		this.gameObject.transform.rotation = flightDirection;
	}
	
	// Update is called once per frame
	void Update () {
		float newX = this.gameObject.transform.position.x + flightSpeed * flightDirectionX;
		this.gameObject.transform.position = new Vector3(newX, flightPositionY, this.gameObject.transform.position.z);
		
		if ( this.gameObject.transform.position.x > 30f && flightDirectionX > 0){
			flightDirectionX = -1f;
			
			if (setPositionToCamera){
				flightPositionY = Camera.main.transform.position.y + Random.value * 6f;
			}
			
			flightDirection = Quaternion.Euler(this.gameObject.transform.rotation.x, this.gameObject.transform.rotation.y, 180);
			this.gameObject.transform.rotation = flightDirection;
			
		} else if ( this.gameObject.transform.position.x < -30f && flightDirectionX < 0){
			flightDirectionX = 1f;
			
			if (setPositionToCamera){
				flightPositionY = Camera.main.transform.position.y + Random.value * 6f;
			}
			
			flightDirection = Quaternion.Euler(this.gameObject.transform.rotation.x, this.gameObject.transform.rotation.y, 0);
			this.gameObject.transform.rotation = flightDirection;
			
		}
		
	}
}
