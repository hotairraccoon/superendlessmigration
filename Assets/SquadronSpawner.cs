using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquadronSpawner : MonoBehaviour {
	
	//aircraft types
	public List<GameObject> aircraftTypes_farmland;
	public List<GameObject> aircraftTypes_battle;
	
	private float spawnTime;
	private float randomPositionSpawn;
	
	// Use this for initialization
	void Start () {
		Invoke ("SpawnSquadron", 10);
		spawnTime = 1f + 3f * Random.value;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void SpawnSquadron ()
	{
		// instantiate an aircraft
		
		if (Controller.current.allAircraft.Count < Controller.current.maxAircraftCount){
			GameObject aircraft = CreateAircraft(Application.loadedLevelName);
			Controller.current.allAircraft.Add(aircraft);
			
			//			if (aircraft.GetComponent<Aircraft>().spawnSound != null){
			//				Controller.current.GetComponent<AudioSource>().PlayOneShot(aircraft.GetComponent<Aircraft>().spawnSound, 0.6f);
			//			}
		}
		
		Invoke("SpawnSquadron", spawnTime);
	}
	
	public GameObject CreateAircraft(string levelName) {
		
		GameObject aircraft;
		int randomSpawnSeed = Random.Range(0,1000);
		Vector3 startPos;
		int spawnSide;
		
		bool noAircraft = false;
		
		if (levelName == "scene_battle"){
			
			spawnSide = Random.Range (0,4); // planes spawn from top, left, or right of camera
			randomPositionSpawn = -2 + 4 * Random.value;
			
			if (Camera.main.gameObject.transform.position.y < -65){ 
				
				aircraft = (GameObject) Instantiate(aircraftTypes_battle[0]); 
				noAircraft = true;
				
			} else if (Camera.main.gameObject.transform.position.y < 55){ // before ocean and airport
				if (randomSpawnSeed <= 350){ // german plane 1
					aircraft = (GameObject) Instantiate(aircraftTypes_battle[0]); 
				} else { // british plane 1
					aircraft = (GameObject) Instantiate(aircraftTypes_battle[1]); 
				}
				
				spawnTime = 2f + 3f * Random.value;
				
			} else if (Camera.main.gameObject.transform.position.y < 85){ // before ocean and airport
				if (randomSpawnSeed <= 350){ // german plane 1
					aircraft = (GameObject) Instantiate(aircraftTypes_battle[0]); 
				} else if (randomSpawnSeed <= 700){ // british plane 1
					aircraft = (GameObject) Instantiate(aircraftTypes_battle[1]); 
				} else { // zeppelin 1
					aircraft = (GameObject) Instantiate(aircraftTypes_battle[2]); 
				}
				
				spawnTime = 1f + 2f * Random.value;
				
			} else {
				aircraft = (GameObject) Instantiate(aircraftTypes_battle[0]); 
				noAircraft = true;
			}	
			
			if (noAircraft == false){
				
				if (spawnSide == 0){
					// spawn from left
					
					startPos.x = Camera.main.ViewportToWorldPoint(new Vector3(-0.5f, randomPositionSpawn, 0.0f)).x;
					startPos.y = Camera.main.ViewportToWorldPoint(new Vector3(-0.5f, randomPositionSpawn, 0.0f)).y;
					
				} else if (spawnSide == 1){
					
					// spawn from right
					
					startPos.x = Camera.main.ViewportToWorldPoint(new Vector3(1.5f, randomPositionSpawn, 0.0f)).x;
					startPos.y = Camera.main.ViewportToWorldPoint(new Vector3(1.5f, randomPositionSpawn, 0.0f)).y;
					
				} else { 
					// spawn from top is most likely
					
					startPos.x = Camera.main.ViewportToWorldPoint(new Vector3(randomPositionSpawn, 1.5f, 0.0f)).x;
					startPos.y = Camera.main.ViewportToWorldPoint(new Vector3(randomPositionSpawn, 1.5f, 0.0f)).y;
					
				}
				
				startPos.z = transform.position.z;
				
				aircraft.transform.position = startPos; 
				
				Vector3 diff = Camera.main.gameObject.transform.position - aircraft.transform.position;
				float zRotation = Mathf.Atan2(0.8f * diff.y + (0.4f * diff.y)*Random.value, 0.8f * diff.x + (0.4f * diff.x)*Random.value ) * Mathf.Rad2Deg;
				aircraft.transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
				
				return aircraft;	
			}
		}
		
		return null;
		
	}
	
}
