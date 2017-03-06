using UnityEngine;
using System.Collections;

public class HelicopterMovement : MonoBehaviour {
	
	public float baseFlightSpeed = 4.5f;
	public float flightSpeed;
	
	public float turnDelay = 2f; // time before helicopter begins to turn
	public float angleToRotate; // the angle the helicopter will turn to
	public float turnSpeed = 2.5f;
	public float totalTurn;
	public bool turning = false;
	
	private Rigidbody2D rigid2d;
	
	// Use this for initialization
	void Start () {
		flightSpeed = (baseFlightSpeed * 1.125f) - (baseFlightSpeed * 0.25f * Random.value);
		turnDelay = (turnDelay * 3f) - (turnDelay * 2.5f * Random.value);
		
		turnSpeed = (turnSpeed) - (turnSpeed * 2f * Random.value);
		
		angleToRotate = 20f + Random.value * 100f;
		
		rigid2d = GetComponent<Rigidbody2D>();
		
		Invoke("TurnChopper", turnDelay);
	}
	
	// Update is called once per frame
	void Update () {
		MoveAircraft(flightSpeed);
	}
	
	public void MoveAircraft(float flightSpeed){
	
		rigid2d.velocity = transform.right*flightSpeed;
		
		if (turning == true){
		
			transform.Rotate (Vector3.forward * turnSpeed);
			totalTurn += Mathf.Abs(turnSpeed);
			
			if (totalTurn >= angleToRotate){
				turning = false;
			}
		}
	}
	
	void TurnChopper (){
		turning = true;
	}
}
