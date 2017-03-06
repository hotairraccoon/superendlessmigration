using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoulderSpawner : MonoBehaviour {

	public bool triggered;
	
	
	public GameObject boulderPrefab;
	public float caveInRate = 1.25f;
	
	public AudioClip smashSound;
	
	private float baseX;
	private float shakeCtr = 0.05f;
	
	public bool finished = false;
	
	// Use this for initialization
	void Start () {
		baseX = transform.parent.transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		if (triggered == false && Camera.main.gameObject.transform.position.y > transform.position.y){
			//the camera has "passed" the spawner, triggering boulders
			triggered = true;
			Invoke("SpawnBoulder", 0.5f);
			InvokeRepeating ("ShakeyCam", 0.05f, 0.05f);
			Controller.current.GetComponent<AudioSource>().PlayOneShot(smashSound, 0.25f + Random.value * 0.25f);
		} 
	}
	
	void SpawnBoulder(){
	
		GameObject boulder = (GameObject) Instantiate(boulderPrefab);
		Vector3 startPos;
		float randScale = 0.65f + Random.value * 0.35f;
		startPos.x = -4.5f + Random.value * 9f; 
		startPos.y = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.1f + Random.value * 0.9f, 0f)).y;
		startPos.z = 0;		
		boulder.transform.position = startPos;
		boulder.transform.localScale = new Vector3(randScale,randScale,randScale);
		boulder.transform.parent = transform.parent.transform;
		
		if (caveInRate > 0.3f){
			caveInRate -= 0.1f;
		}
		
		if (finished == false){
			Invoke("SpawnBoulder", caveInRate + Random.value * 0.5f);
			
			if (Random.value < 0.5f){
				Invoke("SpawnBoulderExtra", caveInRate + Random.value * 0.5f);
			}
		}
	}
	
	void SpawnBoulderExtra(){
		
		GameObject boulder = (GameObject) Instantiate(boulderPrefab);
		Vector3 startPos;
		float randScale = 0.65f + Random.value * 0.35f;
		startPos.x = -4.5f + Random.value * 9f; 
		startPos.y = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.1f + Random.value * 0.9f, 0f)).y;
		startPos.z = 0;		
		boulder.transform.position = startPos;
		boulder.transform.localScale = new Vector3(randScale,randScale,randScale);
		boulder.transform.parent = transform.parent.transform;
		
	}
	
	void ShakeyCam(){
		if (finished == false){
			transform.parent.transform.position = new Vector3(baseX + shakeCtr, transform.parent.transform.position.y, transform.parent.transform.position.z);
			shakeCtr *= -1f;
		}
	}
}
