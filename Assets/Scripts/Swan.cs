using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Swan : Bird {
	
	//movement variables for free flight 
	public float freeFlightSpeed;
	
	//movement variables while part of flock
	public float maxSpeed_flock;
	public float acceleration_flock;
	public float awareness_flock;	
	public float speedFactor;
	public float maxSpeed_leader;
	
	void Awake () {
		birdName = "Mute Swan";	
	}
	
	// Use this for initialization
	void Start () {
		base.init ();
		speedFactor = Random.value;
		freeFlightSpeed = 0.75f + 0.25f * speedFactor;
		maxSpeed_flock = 4.3f + 1f * speedFactor;
		acceleration_flock = 0.5f + 0.25f * speedFactor;
		flockBoostAccel = 0.5f;
		maxSpeed_leader = 5.3f;
		pointsPerSecond = 2;
		
		hitPoints = 120f;
	}
	
	
	// Update is called once per frame
	void Update () {
		this.BirdMovement (freeFlightSpeed,
		                   maxSpeed_flock,
		                   acceleration_flock,
		                   maxSpeed_leader
		                   );
	}
	
}

