using UnityEngine;
using System.Collections;

public class AircraftMovement : MonoBehaviour {
	
	public float baseFlightSpeed = 1f;
	public float flightSpeed;
	public bool activated = true;
	
	public Rigidbody2D rigid2d;
	public TriggerListener triggerListener;
	
	// Use this for initialization
	void Start () {
		flightSpeed = (baseFlightSpeed * 1.125f) - (baseFlightSpeed * 0.25f * Random.value);
		//rigid2d = GetComponent<Rigidbody2D>();
		
//		if (GetComponent<TriggerListener>()) {
//			triggerListener = GetComponent<TriggerListener>();
//		}
	}
	
	// Update is called once per frame
	void Update () {
		if (activated) {
			MoveAircraft(flightSpeed);
		} else if (triggerListener.triggered){
			activated = true;
		}
	}
	
	public void MoveAircraft(float flightSpeed){
		rigid2d.velocity = transform.right * flightSpeed;
	}
	
}
