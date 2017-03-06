using UnityEngine;
using System.Collections;

public class MoveCloud : MonoBehaviour {
	
	public float windSpeed = 0.10f;
	
	private float startX;
	private float startY;
	
	// Use this for initialization
	void Start () {
		float randX = -10f + Random.value * 40;
		transform.position = new Vector3(randX, transform.position.y, transform.position.z);
		
		startX = transform.position.x;
		startY = transform.position.y;
		
		windSpeed = 0.95f * windSpeed + Random.value * 0.1f;
		
		float randScale = 0.75f + Random.value * 0.5f;
		transform.localScale = new Vector3(randScale, randScale, randScale);
		
	}
	
	// Update is called once per frame
	void Update () { 
		transform.position = new Vector3(transform.position.x - windSpeed * Time.deltaTime, transform.position.y  - windSpeed/3 * Time.deltaTime, transform.position.z);
		
		if (transform.position.x < -25f){
			transform.position = new Vector3(24f, startY, transform.position.z);
		} else if (transform.position.x > 25f){
			transform.position = new Vector3(-24f, startY, transform.position.z);
		}
	}
}
