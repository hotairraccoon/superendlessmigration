using UnityEngine;
using System.Collections;

public class AlienInvaderMovement : MonoBehaviour 
{
	
	public float moveDirection = 1f;
	public float maxSpeed = 0.5f;
	public float baseAccel = 0.5f;
	public float randomSpeedFactor = 0.125f;
	
	private float moveSpeed;
	
	// Use this for initialization
	void Start () 
	{
		maxSpeed = maxSpeed + randomSpeedFactor * Random.value;
		baseAccel = baseAccel + baseAccel * 0.25f * Random.value;
		float switchTime = 4f;
		InvokeRepeating("SwitchDirection", switchTime, switchTime);
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 movePos;
		
		movePos.x = transform.position.x + maxSpeed * moveDirection * Time.deltaTime;
		movePos.y = transform.position.y;
		movePos.z = transform.position.z;
		
		transform.position = movePos;
		
	}
	
	void SwitchDirection() 
	{
		moveDirection *= -1f;
	}
}
