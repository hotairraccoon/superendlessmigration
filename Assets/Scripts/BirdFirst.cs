using UnityEngine;
using System.Collections;

public class BirdFirst : MonoBehaviour {

	public bool inFlock;
	public int flockPosition;

	public float currentSpeed = 0.05f;
	public float screenWidth;

	Vector3 mousePos;

	// Use this for initialization
	void Start () {
		this.gameObject.tag = "BirdFirst";
		screenWidth = Screen.width;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter2D(Collision2D col) {

		if (col.gameObject.tag == "BirdFirst" && !col.gameObject.GetComponent<BirdFirst>().inFlock) {
			col.gameObject.GetComponent<BirdFirst>().inFlock = true;
			Controller.current.flockBirds.Add (col.gameObject);
			Controller.current.UpdateFlockPositions();
			col.gameObject.GetComponent<BirdFirst>().flockPosition = Controller.current.flockBirds.Count-1;
		}
		
	}

	public void BirdMovement(float freeFlightSpeed,
	                         float maxSpeed_flock,
	                         float acceleration_flock
	                         ){

		Vector3 birdPos;

		if (inFlock) {
			
			if (currentSpeed < maxSpeed_flock - acceleration_flock) 
			{
				if (flockPosition > 0)
				{
					currentSpeed += acceleration_flock;		
				}
				else 
				{
					currentSpeed += acceleration_flock * 2;
				}
			} 
			else {
				currentSpeed = maxSpeed_flock;
			}


			birdPos = transform.position;
			mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			mousePos.z = transform.position.z;

			Vector3 destPos;

			if(flockPosition == 0) { //lead bird
				destPos.x = mousePos.x;
				destPos.y = mousePos.y;
			}
			else if (flockPosition%2 == 1)
			{ //flockPosition is odd
				destPos.x = mousePos.x - 0.12f * (flockPosition+1);
				destPos.y = mousePos.y - 0.18f * (flockPosition+1);
			}
			else 
			{ //flockPosition is even
				destPos.x = mousePos.x + 0.12f * (flockPosition);
				destPos.y = mousePos.y - 0.18f * (flockPosition);
			}

			destPos.z = mousePos.z;

			if (Vector3.Distance (birdPos, destPos) < currentSpeed) 
			{
				if (flockPosition > 0){
					currentSpeed = 0f;
				}
				else {
					currentSpeed = 0.05f;
				}

				transform.position = destPos;
			}
			else 
			{
				if(currentSpeed > 0)
				{

					float bankAngle = 60 * (birdPos.x - destPos.x) / (screenWidth * 0.01f);
					transform.position = Vector3.MoveTowards (birdPos, destPos, currentSpeed);
					transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, 90 + bankAngle);

				} 
				else 
				{

				}
			}

		}
		else 
		{
			//move birds down the screen if not part of the flock
			
			birdPos = transform.position;
			birdPos.y -= freeFlightSpeed;
			transform.position = birdPos;
		}

	}

}
