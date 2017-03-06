using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class Controller : MonoBehaviour {
	
	public static Controller current;
	public double currentScore;
	public string nextLevelName = "scene_city"; //should change before used
	
	public int flockCount;
	
	public List<GameObject> allBirds;
	public List<GameObject> flockBirds;
	public GameObject leadBird;

	//for spawning birds
	public List<GameObject> birdPrefabs;
	
	public List<GameObject> allAircraft;
	public int maxAircraftCount = 6;
	
	private float alphaFadeValue = 1.0f;
	
	private bool gameOver = false;
	
	// upgrades and abilities
	public float flockBoostDuration = 0.75f;
	public float flockBoostTimeElapsed = 0f;
	public bool flockBoosting = false;
	public bool flockBoostReady = false;
	public float flockBoostCooldownElapsed = 0f;
	public float flockBoostCooldownDuration = 3f;
	
	public float flockSingDuration = 0.5f;
	public float flockSingTimeElapsed = 0f;
	public bool flockSinging = false;
	public bool flockSingReady = false;
	public float flockSingCooldownElapsed = 0f;
	public float flockSingCooldownDuration = 8f;
	
	public float flockDodgeDuration = 3f;
	public float flockDodgeTimeElapsed = 0f;
	public bool flockDodging = false;
	public bool flockDodgeReady = false;
	public float flockDodgeCooldownElapsed = 0f;
	public float flockDodgeCooldownDuration = 20f;
	
	public AudioSource audioSrc;
	public AudioClip whooshSound;
	public AudioClip flapSound;
	public AudioClip gunSound;
	public AudioClip songSound;
	public AudioClip dodgeSound;
	public AudioClip newHighscoreSound;
	
	public GameObject mouseCursor;	
	public Vector3 flockLeaderDestPosition;
	public int flockPointsPerSecond = 1;
	
	public List<string> discoveredBirds;
	public GameObject defaultBird;
	public bool freeFloating = false;
	
	private Animator mouseCursorAnim;
	
	public GameObject boostIcon;
	public GameObject songIcon;
	public GameObject dodgeIcon;
	public float defaultSpeed = 2f;
	
	public GameObject boostCooldownObj;
	public GameObject songCooldownObj;
	public GameObject dodgeCooldownObj;
	
	private Image boostCooldown;
	private Image songCooldown;
	private Image dodgeCooldown;
	
	public Text currentScoreText;
	public Text highScoreText;
	private bool achievedHighscore = false; 
	
	private double highscore;
	
	public SongCircle songCircle;
	public GameObject cameraPos;

	void Awake()
	{
		
		if (current == null)
		{
			DontDestroyOnLoad(gameObject);
			current = this;
		} 
		else if (current != this) 
		{
			// we only want one persistent controller, so delete this if one already exists
			Destroy(gameObject);	
		}
		
		audioSrc = GetComponent<AudioSource>();

		foreach (GameObject gObj in GameObject.FindGameObjectsWithTag ("Bird")){
			allBirds.Add (gObj);
		}
		
		discoveredBirds.Add ("Canada Goose");

		mouseCursor = GameObject.FindGameObjectWithTag("Cursor");
		mouseCursor.SetActive(false);
		boostCooldown = boostCooldownObj.GetComponent<Image>();
		songCooldown = songCooldownObj.GetComponent<Image>();
		dodgeCooldown = dodgeCooldownObj.GetComponent<Image>();
		
		Vector3 tempPos;
		
		if (flockBirds.Count > 0)
		{
			for (int i = 0; i < flockBirds.Count; i++)
			{
				Vector3 randomStartPosition = new Vector3(0.2f + 0.6f * UnityEngine.Random.value, 0.2f + 0.6f * UnityEngine.Random.value, flockBirds[i].transform.position.z);
				tempPos = Camera.main.ViewportToWorldPoint(randomStartPosition);
				tempPos.z = flockBirds[i].transform.position.z;
				flockBirds[i].transform.position = tempPos;
				flockBirds[i].GetComponent<Bird>().ResetForNewLevel();
				flockBirds[i].GetComponent<Bird>().flockPosition = i;
			}
		}
		else
		{
			GameObject tempBird = Instantiate(defaultBird);
			tempBird.GetComponent<Bird>().inFlock = true;
			tempPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1f));
			tempPos.z = tempBird.transform.position.z;
			tempBird.transform.position = tempPos;	
			tempBird.GetComponent<Bird>().flockPosition = 0;
			allBirds.Add (tempBird);
			flockBirds.Add (tempBird);
		}
		
		achievedHighscore = false;
		
	}

	void Start ()
	{
		
		// bird spawning
		Invoke("SpawnBird", 1f);
		InvokeRepeating("UpdateScore",0f,0.1f);

		mouseCursorAnim = mouseCursor.GetComponent<Animator>();
		
		highscore = Highscore.highscoreManager.GetHighscore();
		highScoreText.text = (Convert.ToInt32(Math.Round(highscore))).ToString("D8");
		
		
	}

	void UpdateScore()
	{
		currentScore += flockPointsPerSecond / 10f;
		currentScoreText.text = (Convert.ToInt32(Math.Round(currentScore))).ToString("D8");
		
		if (currentScore >= highscore)
		{
			highscore = currentScore;
			highScoreText.text = (highscore).ToString("D8");
			
			if (achievedHighscore == false)
			{
				achievedHighscore = true;
				audioSrc.PlayOneShot(newHighscoreSound, 1f);
			}
		}
	}

	void Update()
	{	
		if(Input.GetKeyDown (KeyCode.Q)) {
			Invoke ("GameOver", 2f);
		}
		if (flockBirds.Count <= 0 && gameOver == false)
		{	
			//lose game
			gameOver = true;
			Invoke ("GameOver", 2f);
		} else {
		
			if (flockBirds.Count > 0)
			{
				leadBird = flockBirds[0];
			}
			
//			for (int i = 0; i < flockBirds.Count; ++i)
//			{
//				for (int j = 0; j < allBirds.Count; ++j)
//				{
//					if (allBirds[j] != null)
//					{
//						if (!allBirds[j].GetComponent<Bird>().inFlock 
//						    && Vector3.Distance(flockBirds[i].transform.position, allBirds[j].transform.position) <= flockBirds[i].GetComponent<Bird>().songRadius)
//						{
//							Bird addedBird = allBirds[j].gameObject.GetComponent<Bird>();
//							addedBird.inFlock = true;
//							flockBirds.Add(allBirds[j].gameObject);
//							UpdateFlockPositions();	
//							audioSrc.PlayOneShot(addedBird.flockSounds[UnityEngine.Random.Range(0,addedBird.flockSounds.Count)], 0.1f);
//							addedBird.BirdDiscovered(addedBird.birdName);
////							
////							Bird bird = allBirds[j].GetComponent<Bird>();
////							
////							flockBirds.Add (allBirds[j]);
////							bird.inFlock = true;
////							bird.BirdDiscovered(bird.birdName);
////							audioSrc.PlayOneShot(bird.flockSounds[UnityEngine.Random.Range(0,bird.flockSounds.Count)], 0.1f);
////							UpdateFlockPositions();
////							return;
//						}	
//					}
//				}	
//			}
					
			if (Input.GetButton("BoostButton")) {
				
				if (flockBirds.Count > 0 && flockBoosting == false && flockBoostReady == true){
					flockBoosting = true;
					flockBoostReady = false;
					
					for (int i = 0; i < flockBirds.Count; ++i)
					{
						flockBirds[i].GetComponent<Bird>().flockBoosting = true;
						flockBirds[i].GetComponent<Bird>().trailRenderer.enabled = true;
					}
					
					audioSrc.PlayOneShot(whooshSound, 0.75f);	
					audioSrc.PlayOneShot(flapSound, 0.75f);	
				}
				
				Debug.Log ("BoostButton");
				boostCooldown.fillAmount = 1f;
			}
			
			if (Input.GetButton("SongButton")) {
				
				if (flockBirds.Count > 0 && flockSinging == false && flockSingReady == true){
					flockSinging = true;
					flockSingReady = false;
					
					audioSrc.PlayOneShot(songSound, 0.75f);
				}
				
				Debug.Log ("SongButton");
				songCooldown.fillAmount = 1f;	
			}
			
			if (Input.GetButton("DodgeButton")) {
				Debug.Log ("DodgeButton");
				if (flockBirds.Count > 0 && flockDodging == false && flockDodgeReady == true){
					flockDodging = true;
					flockDodgeReady = false;
					InvokeRepeating("DodgeBlink", 0f, 0.1f);
					audioSrc.PlayOneShot(dodgeSound, 0.75f);
				}
				dodgeCooldown.fillAmount = 1f;
			}
		}
		
		if (flockBoosting == true)
		{
			flockBoostTimeElapsed += 1f * Time.deltaTime;
			if (flockBoostTimeElapsed >= flockBoostDuration)
			{
				flockBoostCooldownElapsed = 0f;
				flockBoosting = false;
				flockBoostTimeElapsed = 0f;
			}
		} 
		else 
		{
			if (flockBoostCooldownElapsed + 1f * Time.deltaTime < flockBoostCooldownDuration)
			{
				flockBoostCooldownElapsed += 1f * Time.deltaTime;
				boostCooldown.fillAmount = (flockBoostCooldownDuration - flockBoostCooldownElapsed) / flockBoostCooldownDuration;
			}
			else
			{
				flockBoostReady = true;
				boostCooldown.fillAmount = 0f;
			}
			
			
		}
		
		if (flockSinging == true)
		{
			songCircle.circleRadius += 0.5f;
			for (int i = 0; i < allBirds.Count; i++)
			{	
				if (allBirds[i] != null)
				{
					if (allBirds[i].activeInHierarchy 
						&& !allBirds[i].GetComponent<Bird>().inFlock 
						&& Vector3.Distance(allBirds[i].transform.position, flockBirds[0].transform.position) <= songCircle.circleRadius) 
					{
						Bird bird = allBirds[i].GetComponent<Bird>();
						bird.inFlock = true;
						bird.BirdDiscovered(bird.birdName);
						flockBirds.Add (allBirds[i]);
						audioSrc.PlayOneShot(bird.flockSounds[UnityEngine.Random.Range(0,bird.flockSounds.Count)], 0.1f);
						UpdateFlockPositions();
					}
				}
			}
			flockSingTimeElapsed += 1f * Time.deltaTime;
			
			if (flockSingTimeElapsed >= flockSingDuration)
			{
				flockSingCooldownElapsed = 0f;
				flockSinging = false;
				flockSingTimeElapsed = 0f;
				songCircle.circleRadius = 0f;
			}
		}
		else 
		{
			if (flockSingCooldownElapsed + 1f * Time.deltaTime < flockSingCooldownDuration)
			{
				flockSingCooldownElapsed += 1f * Time.deltaTime;
				songCooldown.fillAmount = (flockSingCooldownDuration - flockSingCooldownElapsed) / flockSingCooldownDuration;
			}
			else
			{
				flockSingReady = true;
				songCooldown.fillAmount = 0f;
			}
		}
		
		if (flockDodging == true)
		{
			flockDodgeTimeElapsed += 1f * Time.deltaTime;	
			if (flockDodgeTimeElapsed >= flockDodgeDuration)
			{
				flockDodgeCooldownElapsed = 0f;
				flockDodging = false;
				flockDodgeTimeElapsed = 0f;
				CancelInvoke("DodgeBlink");
				
				for (int i = 0; i < flockBirds.Count; i++)
				{
					SpriteRenderer renderer = flockBirds[i].GetComponent<SpriteRenderer>();
					renderer.enabled = true;
				}
			}
		}
		else 
		{
			if (flockDodgeCooldownElapsed + 1f * Time.deltaTime < flockDodgeCooldownDuration)
			{
				flockDodgeCooldownElapsed += 1f * Time.deltaTime;
				dodgeCooldown.fillAmount = (flockDodgeCooldownDuration - flockDodgeCooldownElapsed) / flockDodgeCooldownDuration;
			}
			else
			{
				flockDodgeReady = true;
				dodgeCooldown.fillAmount = 0f;
			}	
		}
	}
	
	void DodgeBlink () 
	{
		for (int i = 0; i < flockBirds.Count; i++)
		{
			SpriteRenderer renderer = flockBirds[i].GetComponent<SpriteRenderer>();
			if (renderer.enabled)
			{
				renderer.enabled = false;	
			}
			else
			{
				renderer.enabled = true;
			}
		}
	}

	void SpawnBird ()
	{
		
		Vector3 startPos;
		
		if (Application.loadedLevelName == "scene_city" || Application.loadedLevelName == "scene_city_MOBILE" || Application.loadedLevelName == "scene_postcity" || Application.loadedLevelName == "scene_battle" || (Application.loadedLevelName == "scene_iceage" && Camera.main.transform.position.y < 45f)){

			startPos.x = UnityEngine.Random.Range (Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 1.0f, 0.0f)).x, Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f)).x);
			startPos.y = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 1.0f, 0.0f)).y;
			startPos.z = transform.position.z;
			
			// instantiate a bird
			int randomSpawnSeed = UnityEngine.Random.Range(0,1000);
			GameObject gObj;
			
			if (randomSpawnSeed <= 600){
				gObj = (GameObject) Instantiate(birdPrefabs[0]);
			} else if (randomSpawnSeed <= 750){
				gObj = (GameObject) Instantiate(birdPrefabs[1]);
			} else if (randomSpawnSeed <= 850){
				gObj = (GameObject) Instantiate(birdPrefabs[2]);
			} else if (randomSpawnSeed <= 980){
				gObj = (GameObject) Instantiate(birdPrefabs[3]);
			} else {
				gObj = (GameObject) Instantiate(birdPrefabs[4]);
			}
			
			gObj.transform.position = startPos;
	
			allBirds.Add (gObj);
	
			float spawnTime = 1.5f + UnityEngine.Random.value * 1.5f;
			Invoke("SpawnBird", spawnTime);
		}
	}
	
	public void UpdateFlockPositions() 
	{
		flockPointsPerSecond = 0;
		
		for (int i = 0; i < flockBirds.Count; ++i)
		{
			flockBirds[i].GetComponent<Bird>().flockPosition = i;
			flockPointsPerSecond += flockBirds[i].GetComponent<Bird>().pointsPerSecond;
		}
	}

	public void GameOver()
	{
//		if (Highscore.highscoreManager != null)
//		{
//			Highscore.highscoreManager.Score (Convert.ToInt32(Math.Round(currentScore)));
//		}
		Application.LoadLevel("menu_chapterSelect");		
		Destroy (gameObject);
	}
	
	public void SaveGame() 
	{
		// works on all platforms except web player (you can't save local files on web)
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");
		
		PlayerData data = new PlayerData();
		data.currentScore = currentScore;
		bf.Serialize(file, data);
		file.Close();
	}
	
	public void LoadGame() 
	{
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			
			PlayerData data = (PlayerData)bf.Deserialize(file);
			file.Close();
			
			currentScore = data.currentScore;
		}
	}
	
	void OnLevelWasLoaded(int level) 
	{
		gameOver = false;
		mouseCursor = GameObject.FindGameObjectWithTag("Cursor");
		
		Vector3 tempPos;
		GetComponent<AircraftSpawner>().Invoke ("SpawnPlane", 3f);
		GetComponent<AircraftSpawner>().spawnTime = 1f + 2f * UnityEngine.Random.value;
		
		if (flockBirds.Count > 0)
		{
			for (int i = 0; i < flockBirds.Count; i++)
			{
				Vector3 randomStartPosition = new Vector3(0.2f + 0.6f * UnityEngine.Random.value, 0.2f + 0.6f * UnityEngine.Random.value, flockBirds[i].transform.position.z);
				tempPos = Camera.main.ViewportToWorldPoint(randomStartPosition);
				tempPos.z = flockBirds[i].transform.position.z;
				flockBirds[i].transform.position = tempPos;
				flockBirds[i].GetComponent<Bird>().ResetForNewLevel();
				flockBirds[i].GetComponent<Bird>().flockPosition = i;
			}
		}
		else
		{
			GameObject tempBird = Instantiate(defaultBird);
			tempBird.GetComponent<Bird>().inFlock = true;
			tempPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1f));
			tempPos.z = tempBird.transform.position.z;
			tempBird.transform.position = tempPos;	
			tempBird.GetComponent<Bird>().flockPosition = 0;
			allBirds.Add (tempBird);
			flockBirds.Add (tempBird);
		}
		
		if(Application.loadedLevelName == "scene_wormhole")
		{
			freeFloating = true;
		}
		else 
		{
			freeFloating = false;
		}
		
	}

	public Transform getFlockLeader(){
		if (flockBirds.Count > 0){
			return flockBirds[0].GetComponent<Transform>();
		} else {
			Debug.LogError("No flock burds");
			return null;
		}
	}
	
}

[Serializable]
class PlayerData 
{
	//data container for writing data to file with SaveGame() above
	//vars below should mirror data to be saved in the main class
	public double currentScore;
}

