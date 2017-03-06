using UnityEngine;
using System.Collections;

public class BulletMovement : MonoBehaviour {
	
	public float movementSpeed = 100f;
	
	private Rigidbody2D rigid2d;
	
	// Use this for initialization
	void Start () {
		rigid2d = GetComponent<Rigidbody2D>();
		
		movementSpeed = movementSpeed + movementSpeed * 0.25f * Random.value;
	}
	
	// Update is called once per frame
	void Update () {
		rigid2d.velocity = transform.right * movementSpeed;
	}
}	
