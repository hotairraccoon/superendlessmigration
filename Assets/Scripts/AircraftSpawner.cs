using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AircraftSpawner : MonoBehaviour {

	//aircraft types
	public List<GameObject> aircraftTypes_city;
	public List<GameObject> aircraftTypes_iceage;
	public List<GameObject> aircraftTypes_farmland;
	public List<GameObject> aircraftTypes_battle;
	public List<GameObject> aircraftTypes_postcity;
	
	public float spawnTime;
	private float randomPositionSpawn;
	
	private float doubleSpawnProbability = 0.1f;
	
	// Use this for initialization
	void Start () 
	{
		Invoke ("SpawnPlane", 3f);
		spawnTime = 1f + 2f * Random.value;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	void SpawnPlane ()
	{
		// instantiate an aircraft
		
		if (Controller.current.allAircraft.Count < Controller.current.maxAircraftCount)
		{
			GameObject aircraft = CreateAircraft(Application.loadedLevelName);
			Controller.current.allAircraft.Add(aircraft);
			
			if (Random.value <= doubleSpawnProbability)
			{ //chance of creating 2 aircraft at once
				aircraft = CreateAircraft(Application.loadedLevelName);
				Controller.current.allAircraft.Add(aircraft);
			}
		}
		
		
		
		Invoke("SpawnPlane", spawnTime);
	}
	
	public GameObject CreateAircraft(string levelName) 
	{
		
		GameObject aircraft;
		int randomSpawnSeed = Random.Range(0,1000);
		Vector3 startPos;
		int spawnSide;
		
		bool noAircraft = false;
		
		if (levelName == "scene_city")
		{
			if (Camera.main.gameObject.transform.position.y < 20)
			{ // before ocean and airport
				if (randomSpawnSeed <= 350){ // prop plane 1
					aircraft = (GameObject) Instantiate(aircraftTypes_city[0]); 
				} else if (randomSpawnSeed <= 700){ // prop plane 2
					aircraft = (GameObject) Instantiate(aircraftTypes_city[1]); 
				} else { // airliner 1
					aircraft = (GameObject) Instantiate(aircraftTypes_city[3]); 
				}
				doubleSpawnProbability = 0.1f;
				spawnTime = 2f + 1.25f * Random.value;
			} 
			else if (Camera.main.gameObject.transform.position.y < 45)
			{ // before ocean and airport part 2
				if (randomSpawnSeed <= 300){ // prop plane 1
					aircraft = (GameObject) Instantiate(aircraftTypes_city[0]); 
				} else if (randomSpawnSeed <= 500){ // prop plane 2
					aircraft = (GameObject) Instantiate(aircraftTypes_city[1]); 
				} else if (randomSpawnSeed <= 700){ // jet
					aircraft = (GameObject) Instantiate(aircraftTypes_city[2]); 
				} else if (randomSpawnSeed <= 850){ // airliner 1
					aircraft = (GameObject) Instantiate(aircraftTypes_city[3]); 
				} else { // helicopter
					aircraft = (GameObject) Instantiate(aircraftTypes_city[6]); 
				} 
				
				doubleSpawnProbability = 0.125f;
				spawnTime = 1.125f + 1.25f * Random.value;
				
			} 
			else if (Camera.main.gameObject.transform.position.y < 75)
			{ // before ocean and airport
				if (randomSpawnSeed <= 150){ // prop plane 1
					aircraft = (GameObject) Instantiate(aircraftTypes_city[0]); 
				} else if (randomSpawnSeed <= 350){ // prop plane 2
					aircraft = (GameObject) Instantiate(aircraftTypes_city[1]); 
				} else if (randomSpawnSeed <= 500){ // jet
					aircraft = (GameObject) Instantiate(aircraftTypes_city[2]); 
				} else if (randomSpawnSeed <= 650){ // airliner 1
					aircraft = (GameObject) Instantiate(aircraftTypes_city[3]); 
				} else if (randomSpawnSeed <= 800){ // airliner 2
					aircraft = (GameObject) Instantiate(aircraftTypes_city[4]); 
				} else { // helicopter
					aircraft = (GameObject) Instantiate(aircraftTypes_city[6]); 
				} 
				doubleSpawnProbability = 0.15f;
				spawnTime = 1f + 1.25f * Random.value;
				
			} else if (Camera.main.gameObject.transform.position.y < 120){ // at airport
				if (randomSpawnSeed <= 200){ // airliner 1
					aircraft = (GameObject) Instantiate(aircraftTypes_city[3]); 
				} else if (randomSpawnSeed <= 400){ // airliner 2
					aircraft = (GameObject) Instantiate(aircraftTypes_city[4]);
				} else if (randomSpawnSeed <= 650){ // jet
					aircraft = (GameObject) Instantiate(aircraftTypes_city[2]);  
				} else if (randomSpawnSeed <= 800) { // prop plane 1
					aircraft = (GameObject) Instantiate(aircraftTypes_city[0]); 
				} else { // prop plane 2
					aircraft = (GameObject) Instantiate(aircraftTypes_city[1]); 
				} 
				doubleSpawnProbability = 0.175f;
				spawnTime = 0.50f + 1.125f * Random.value;
				
			} else if (Camera.main.gameObject.transform.position.y < 150){ // at airport
				if (randomSpawnSeed <= 150){ // airliner 1
					aircraft = (GameObject) Instantiate(aircraftTypes_city[3]); 
				} else if (randomSpawnSeed <= 250){ // airliner 2
					aircraft = (GameObject) Instantiate(aircraftTypes_city[4]);
				} else if (randomSpawnSeed <= 400){ // jet
					aircraft = (GameObject) Instantiate(aircraftTypes_city[2]);  
				} else  if (randomSpawnSeed <= 550){ // prop plane 1
					aircraft = (GameObject) Instantiate(aircraftTypes_city[0]); 
				} else if (randomSpawnSeed <= 700) { // prop plane 2
					aircraft = (GameObject) Instantiate(aircraftTypes_city[1]); 
				} else { //pontoon
					aircraft = (GameObject) Instantiate(aircraftTypes_city[5]); 
				}
				doubleSpawnProbability = 0.2f;
				spawnTime = 0.50f + 1f * Random.value;	
			} else {
				aircraft = (GameObject) Instantiate(aircraftTypes_city[0]); 
				noAircraft = true;
			}	
			
			spawnSide = Random.Range (0,4); // planes spawn from top, left, or right of camera
			randomPositionSpawn = -1.25f + 2.5f * Random.value;
			
			if (noAircraft == false)
			{	
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
				float zRotation = Mathf.Atan2(0.8f * diff.y + (0.4f * diff.y * Random.value), 0.8f * diff.x + (0.4f * diff.x * Random.value)) * Mathf.Rad2Deg;
				aircraft.transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
					
				return aircraft;	
			}
			
		} 
		
		else if (levelName == "scene_iceage")
		{
			if (Camera.main.gameObject.transform.position.y < 20)
			{ // before ocean and airport
				aircraft = (GameObject) Instantiate(aircraftTypes_iceage[0]); 
				doubleSpawnProbability = 0f;
				noAircraft = true;
			}
			else if (Camera.main.gameObject.transform.position.y < 65)
			{ // before ocean and airport
				aircraft = (GameObject) Instantiate(aircraftTypes_iceage[0]); 
				doubleSpawnProbability = 0.1f;
				spawnTime = 2f + 1.25f * Random.value;
			}
			else
			{
				aircraft = (GameObject) Instantiate(aircraftTypes_iceage[0]); 
				doubleSpawnProbability = 0f;
				noAircraft = true;
			}
		}
		else if (levelName == "scene_farmlands")
		{
			
			spawnSide = Random.Range (0,4); // planes spawn from top, left, or right of camera
			randomPositionSpawn = -1.25f + 2.5f * Random.value;
			
			aircraft = (GameObject) Instantiate(aircraftTypes_farmland[0]);
			spawnTime = 0.75f + 1.5f * Random.value;
			
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
		else if (levelName == "scene_battle")
		{		
			spawnSide = Random.Range (0,4); // planes spawn from top, left, or right of camera
			randomPositionSpawn = -1.25f + 2.5f * Random.value;
			if (Camera.main.gameObject.transform.position.y < -65)
			{ 
				
				aircraft = (GameObject) Instantiate(aircraftTypes_battle[0]); 
				noAircraft = true;
				
			} 
			else if (Camera.main.gameObject.transform.position.y < 55)
			{ // before ocean and airport
				if (randomSpawnSeed <= 350){ // german plane 1
					aircraft = (GameObject) Instantiate(aircraftTypes_battle[0]); 
				} else { // british plane 1
					aircraft = (GameObject) Instantiate(aircraftTypes_battle[1]); 
				}
				doubleSpawnProbability = 0.10f;
				spawnTime = 1.25f + 1.5f * Random.value;
			} 
			else if (Camera.main.gameObject.transform.position.y < 120)
			{ // before ocean and airport
				if (randomSpawnSeed <= 350){ // german plane 1
					aircraft = (GameObject) Instantiate(aircraftTypes_battle[0]); 
				} else if (randomSpawnSeed <= 700){ // british plane 1
					aircraft = (GameObject) Instantiate(aircraftTypes_battle[1]); 
				} else { // zeppelin 1
					aircraft = (GameObject) Instantiate(aircraftTypes_battle[2]); 
				}
				doubleSpawnProbability = 0.25f;
				spawnTime = 0.5f + 1f * Random.value;
			} 
			else 
			{
				aircraft = (GameObject) Instantiate(aircraftTypes_battle[0]); 
				noAircraft = true;
			}	
			
			if (noAircraft == false){
				
				if (spawnSide == 0){
					// spawn from left
					
					startPos.x = Camera.main.ViewportToWorldPoint(new Vector3(-0.25f, randomPositionSpawn, 0.0f)).x;
					startPos.y = Camera.main.ViewportToWorldPoint(new Vector3(-0.25f, randomPositionSpawn, 0.0f)).y;
					
				} else if (spawnSide == 1){
					
					// spawn from right
					
					startPos.x = Camera.main.ViewportToWorldPoint(new Vector3(1.25f, randomPositionSpawn, 0.0f)).x;
					startPos.y = Camera.main.ViewportToWorldPoint(new Vector3(1.25f, randomPositionSpawn, 0.0f)).y;
					
				} else { 
					// spawn from top is most likely
					
					startPos.x = Camera.main.ViewportToWorldPoint(new Vector3(randomPositionSpawn, 1.25f, 0.0f)).x;
					startPos.y = Camera.main.ViewportToWorldPoint(new Vector3(randomPositionSpawn, 1.25f, 0.0f)).y;
					
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
