using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Bird : MonoBehaviour {

	public bool inFlock;
	public int flockPosition;

	public float currentSpeed = 0.05f;
	public float screenWidth;

	Vector3 mousePos;
	GameObject controller;
	private Vector3 mousePosition = Vector3.zero;

	public List<AudioClip> flockSounds;
	public List<AudioClip> deathSounds;
	public List<AudioClip> deathSounds_inFlock;	
	
	public bool flockBoosting;
	public float flockBoostAccel = 0.25f;// multiply by Time.deltaTime when using
	public float flockBoostVelocityMax = 0.25f;
	public float flockBoostVelocity = 0f; //this velocity gets added to normal movement speed when boosting
	
	private Animator anim;
	public TrailRenderer trailRenderer;
	
	private AudioSource audioSrc;
	
	public float flockMovementReactionTimeRate = 3.0f; // multiply by Time.deltaTime when using
	private float flockMovementReactionTimeMax = 1.0f;
	private float flockMovementReactionTimeTotal = 0.0f;
	
	private Vector3 destPos = Vector3.zero;
	
	public int pointsPerSecond = 1;
	
	public string birdName = "";
	
	private CameraFollow cameraFollow;
	
	private float floatRotationSpeed;
	private Vector3 floatDestPos;
	
	public float hitPoints = 100f;
	public Color poisonColor;
	public Color defaultColor;
	public bool poisoned = false;
	public SpriteRenderer renderer;
	
	public float boostDelayTotal = 0f;
	public float boostDelayMax = 0.15f;

	private float birdSpeed = 2;
	private float birdMaxSpeed = 500;
	private float birdAcceleration = 100;
	private float birdDeceleration = 500;
	private Vector3 birdMovement = Vector3.zero;
	
	public float songRadius = 0.5f; 
	
	public void init() {
		
		poisonColor = new Color(170f,202f,96f,0.25f);
		defaultColor = new Color(255f,255f,255f,1f);
		this.gameObject.tag = "Bird";
		screenWidth = Screen.width;
		
		anim = GetComponent<Animator>();
		trailRenderer = transform.Find("TrailRenderer").gameObject.GetComponent<TrailRenderer>();
		trailRenderer.enabled = false;
		
		audioSrc = Controller.current.GetComponent<AudioSource>();
		cameraFollow = Camera.main.GetComponent<CameraFollow>();
		
		floatRotationSpeed = -1f + 2f * Random.value;
		floatDestPos = new Vector3(1.0f * Random.value, 1.0f * Random.value, transform.position.z);
		
//		renderer = GetComponent<SpriteRenderer>();
		
		boostDelayMax = boostDelayMax * 0.75f + boostDelayMax * 0.25f * Random.value;
	}
	
	void OnCollisionEnter2D(Collision2D col) {

		if (inFlock && col.gameObject.tag == "Bird" && !col.gameObject.GetComponent<Bird>().inFlock)
		{
			Bird addedBird = col.gameObject.GetComponent<Bird>();
			Controller.current.flockBirds.Add (col.gameObject);
			Controller.current.UpdateFlockPositions();
			addedBird.inFlock = true;	
			audioSrc.PlayOneShot(addedBird.flockSounds[Random.Range(0,addedBird.flockSounds.Count)], 0.1f);
			BirdDiscovered(addedBird.birdName);
		}

	}
	
	public void BirdDiscovered(string birdName) {
		bool alreadyDiscovered = false;
		for (int i = 0; i < Controller.current.discoveredBirds.Count; i++){
			if (Controller.current.discoveredBirds[i] == birdName){
				alreadyDiscovered = true;
				break;
			}
		}
		
		if (!alreadyDiscovered) {
			if (TitleCard.current != null)
			{
				TitleCard.current.Message(birdName + " discovered!");
			}
			
			Controller.current.discoveredBirds.Add(birdName);
		}
	}
	
	public void BirdFloat() {
		anim.speed = 0f;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + floatRotationSpeed);
	}
	
	public void BirdMovement(float freeFlightSpeed,
	                         float maxSpeed_flock,
	                         float acceleration_flock,
	                         float maxSpeed_leader) {

		Vector3 birdPos;
		
		if (Controller.current.freeFloating) {
			BirdFloat();
			return;
		}
		
		//move birds down the screen if not part of the flock
		if (inFlock) {
			
			birdPos = transform.position;
			mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			mousePosition.z = transform.position.z;	
			
			int leader = flockPosition - 2;
			GameObject followBird;
			
			float maxSpeed = maxSpeed_flock;
		
			if(flockPosition == 0) { // lead bird
				//this should be elsewhere but let start here
				if(1==1) {
					Vector3 stayWithCamera = Vector3.up * Controller.current.defaultSpeed * Time.deltaTime;
					//birdMaxSpeed = maxSpeed;
					birdMovement = stayWithCamera/Camera.main.orthographicSize;

					float inputVerticalAxis;
					float inputHorizontalAxis;

					transform.position += birdMovement;

				} else {
					float speed = 10.0f;
					Vector3 move;
					move = new Vector3(Input.GetAxis("Horizontalj"), Input.GetAxis("Verticalj"), 0);
					if( move == Vector3.zero) {
						move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
					}
					if( move == Vector3.zero) {
						Debug.Log ("no input, need to move camera speed");
						move = new Vector3(0, 1, 0);
						transform.position += move * 2 * Time.deltaTime;
					} else {
						transform.position += move * speed * Time.deltaTime;
					}
				}

				destPos =  transform.position;
				destPos.z = transform.position.z;
				Controller.current.flockLeaderDestPosition = destPos;
				maxSpeed = maxSpeed_leader; 
			} else { // not the lead bird
					
				if(leader <= 0) 
				{
					leader = 0;
					followBird = Controller.current.flockBirds[leader];
				} 
				else 
				{ 
					followBird = Controller.current.flockBirds[leader];
				}
				
				//set destination position for non-lead flock birds
				if (flockPosition != 0 && flockPosition%2 == 1) 
				{ //flockPosition is odd
					destPos.x = Controller.current.flockLeaderDestPosition.x - 0.30f * (flockPosition - Mathf.Floor (flockPosition/2));
					destPos.y = Controller.current.flockLeaderDestPosition.y - 0.28f * (flockPosition - Mathf.Floor (flockPosition/2));
				} 
				else if (flockPosition != 0) 
				{ //flockPosition is even
					destPos.x = Controller.current.flockLeaderDestPosition.x + 0.30f * (flockPosition - Mathf.Floor (flockPosition/2));
					destPos.y = Controller.current.flockLeaderDestPosition.y - 0.28f * (flockPosition - Mathf.Floor (flockPosition/2)); 
				}
			}
				
			if (flockBoosting == true) {
				
				if (boostDelayTotal >= boostDelayMax){
					anim.speed = 3f; 
					float totalAccelerationThisFrame = acceleration_flock;
					
					if (flockBoostVelocity < flockBoostVelocityMax - flockBoostAccel){
						flockBoostVelocity += flockBoostAccel;
						totalAccelerationThisFrame += flockBoostAccel;
					} else {
						flockBoostVelocity = flockBoostVelocityMax;
					}
								
					if (currentSpeed < maxSpeed * 3f - totalAccelerationThisFrame) {
						if (flockPosition > 0){
							currentSpeed += totalAccelerationThisFrame * 2;
						} else {
							currentSpeed += totalAccelerationThisFrame * 2;
						}
					} else {
						currentSpeed = maxSpeed * 3f;
					}
					if (Controller.current.flockBoosting == false){
						flockBoosting = false;
						trailRenderer.enabled = false;
					} 
				} else {
					if (flockPosition == 0)
					{
						boostDelayTotal += 1f;
					}
					else
					{
						boostDelayTotal += 1f * Time.deltaTime;
					}
				}
			} else {
				anim.speed = 1f;
				boostDelayTotal = 0f;
				flockBoostVelocity = 0f;
				if (currentSpeed < maxSpeed - acceleration_flock) {
					if (flockPosition > 0){
						currentSpeed += acceleration_flock;
					}
					else {
						currentSpeed += acceleration_flock;
					}
				} else {
					currentSpeed = maxSpeed;
				}
			}
			
			destPos.z = birdPos.z;
			
			transform.position = Vector3.MoveTowards (birdPos, destPos, currentSpeed * Time.deltaTime);
			
			if (cameraFollow != null) {
				float bankAngle = 60 * (birdPos.x - destPos.x) / (screenWidth * 0.01f);
				transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, 90 + cameraFollow.cameraRotation + bankAngle);
			}
			
			if (Vector3.Distance (birdPos, destPos) <= currentSpeed * Time.deltaTime) {
				transform.position = destPos;
			} 
			
			
		} else {

			//not in flock

			birdPos = transform.position;
			birdPos.y -= freeFlightSpeed * Time.deltaTime;
			transform.position = birdPos;
		}

	}
	
	public void ResetForNewLevel() {
		cameraFollow = Camera.main.GetComponent<CameraFollow>();
	}

}
