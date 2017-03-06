using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StreetTraffic : MonoBehaviour {

	public List<GameObject> cars;
	public GameObject prefab;
	public float trafficDensityMultiplier = 1f;
	public int maxCarCount = 24;

	void Awake()
	{

		
	}
	
	void Start ()
	{

		// repeat bird spawning
		if (trafficDensityMultiplier > 0) 
		{
			maxCarCount = (int)(3 + 21 * Random.value);
			SpawnStartingCars();
			//Invoke ("SpawnCar", 1f/trafficDensityMultiplier);
		}
	}

	void Update () 
	{
		int randomizeCar = (int)(Random.value * 20);

		for (int i = cars.Count-1; i >= 0; i--) {

			if (cars [i].transform.position.x > 21f) {
				
				cars [i].transform.position = new Vector3 (-20f, cars [i].transform.position.y, cars [i].transform.position.z);

				randomizeCar = (int)(Random.value * 20);

				if (cars [i].GetComponent<Animator>()){
					if (randomizeCar <= 6) {
							cars [i].GetComponent<Animator> ().Play ("car1");
					} else if (randomizeCar <= 9) {
							cars [i].GetComponent<Animator> ().Play ("car2");
					} else if (randomizeCar <= 12) {
							cars [i].GetComponent<Animator> ().Play ("car3");
					} else if (randomizeCar <= 14) {
							cars [i].GetComponent<Animator> ().Play ("car4");
					} else if (randomizeCar <= 16) {
							cars [i].GetComponent<Animator> ().Play ("car5");
					} else if (randomizeCar <= 18) {
							cars [i].GetComponent<Animator> ().Play ("car6");
					} else {
							cars [i].GetComponent<Animator> ().Play ("bus1");
					}
				}

			} else if (cars [i].transform.position.x < -21f) {
			
				cars [i].transform.position = new Vector3 (20f, cars [i].transform.position.y, cars [i].transform.position.z);

				randomizeCar = (int)(Random.value * 20);
	
				if (cars [i].GetComponent<Animator>()){
					if (randomizeCar <= 6) {
						cars [i].GetComponent<Animator> ().Play ("car1");
					} else if (randomizeCar <= 9) {
						cars [i].GetComponent<Animator> ().Play ("car2");
					} else if (randomizeCar <= 12) {
						cars [i].GetComponent<Animator> ().Play ("car3");
					} else if (randomizeCar <= 14) {
						cars [i].GetComponent<Animator> ().Play ("car4");
					} else if (randomizeCar <= 16) {
						cars [i].GetComponent<Animator> ().Play ("car5");
					} else if (randomizeCar <= 18) {
						cars [i].GetComponent<Animator> ().Play ("car6");
					} else {
						cars [i].GetComponent<Animator> ().Play ("bus1");
					}
				}

			} else {
				cars [i].transform.position = new Vector3 (cars [i].transform.position.x + 0.05f * cars [i].transform.localScale.x, cars [i].transform.position.y, cars [i].transform.position.z);
			}

		}

	}

	void SpawnStartingCars ()
	{
		if (prefab) {

			int startingCarCount = (int)(maxCarCount * Random.value);

			for (int i = 0; i < startingCarCount; i++){
				GameObject gObj = (GameObject)Instantiate (prefab);
		
				Vector3 startPos;
				startPos.x = Random.Range (-20, 20);
				
				float startDirection = 1f;
				float zPosition = 0f;
				
				if (Random.value <= 0.5) {
					startPos.y = this.gameObject.transform.position.y + 0.25f;
					startDirection = -1f; // moving left

				} else {
					startPos.y = this.gameObject.transform.position.y - 0.25f;
					zPosition = 1;
				} 
				startPos.z = transform.position.z;
				
				int randomizeCar = (int)(Random.value * 20);
				
				if (gObj.GetComponent<Animator>()){
					if (randomizeCar <= 6){
						gObj.GetComponent<Animator>().Play ("car1");
					} else if (randomizeCar <= 9){
						gObj.GetComponent<Animator>().Play ("car2");
					} else if (randomizeCar <= 12){
						gObj.GetComponent<Animator>().Play ("car3");
					} else if (randomizeCar <= 14){
						gObj.GetComponent<Animator>().Play ("car4");
					} else if (randomizeCar <= 16){
						gObj.GetComponent<Animator>().Play ("car5");
					} else if (randomizeCar <= 18){
						gObj.GetComponent<Animator>().Play ("car6");
					} else {
						gObj.GetComponent<Animator>().Play ("bus1");
					}
				}
				
				Vector3 driveDirection = new Vector3(gObj.transform.localScale.x * startDirection, gObj.transform.localScale.y, zPosition);
				gObj.transform.localScale = driveDirection;
				gObj.transform.position = startPos;	
				cars.Add (gObj);

			}
		}
	}

	void SpawnCar ()
	{

		// instantiate a car
		if (prefab) {
			GameObject gObj = (GameObject)Instantiate (prefab);

			Vector3 startPos;
			float startDirection = 1f;
			float zPosition = 0f;
			
			if (Random.value <= 0.5) {
				startDirection = -1f; // moving left
				zPosition = 1;
				startPos.x = 20f;
				startPos.y = this.gameObject.transform.position.y - 0.25f;
			} else {
				startPos.x = -20f;
				startPos.y = this.gameObject.transform.position.y + 0.25f;
			} 
			startPos.z = transform.position.z;

			int randomizeCar = (int)(Random.value * 20);

			if (gObj.GetComponent<Animator>()){
				if (randomizeCar <= 6){
					gObj.GetComponent<Animator>().Play ("car1");
				} else if (randomizeCar <= 9){
					gObj.GetComponent<Animator>().Play ("car2");
				} else if (randomizeCar <= 12){
					gObj.GetComponent<Animator>().Play ("car3");
				} else if (randomizeCar <= 14){
					gObj.GetComponent<Animator>().Play ("car4");
				} else if (randomizeCar <= 16){
					gObj.GetComponent<Animator>().Play ("car5");
				} else if (randomizeCar <= 18){
					gObj.GetComponent<Animator>().Play ("car6");
				} else {
					gObj.GetComponent<Animator>().Play ("bus1");
				}
			}

			Vector3 driveDirection = new Vector3(gObj.transform.localScale.x * startDirection, gObj.transform.localScale.y, zPosition);
			gObj.transform.localScale = driveDirection;
			gObj.transform.position = startPos;	
			cars.Add (gObj);
		}
	}

}
