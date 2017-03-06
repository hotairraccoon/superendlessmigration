using UnityEngine;
using System.Collections;

public class AlienFighterMovement : MonoBehaviour 
{
	
	public float moveDirection = 1f;
	public float maxSpeed = 0.5f;
	public float baseAccel = 0.5f;
	public float randomSpeedFactor = 0.125f;
	
	private float moveSpeed;
	
	private float bankAngle = 0f;
	
	// Use this for initialization
	void Start () 
	{
		maxSpeed = maxSpeed + randomSpeedFactor * Random.value;
		baseAccel = baseAccel + baseAccel * 0.25f * Random.value;
		float switchTime = 3.75f + 0.5f * Random.value;
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
		
		
		if (moveDirection > 0 && bankAngle > -5f)
		{
			bankAngle -= 5f * Time.deltaTime;
		}
		else if (moveDirection < 0 && bankAngle < 5f)
		{	
			bankAngle += 5f * Time.deltaTime;
		}
		
		transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, 270 + bankAngle);
		
	}
	
	void SwitchDirection() 
	{
		moveDirection *= -1f;
	}
}

