//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.IO;
//
//public class PersistentController : MonoBehaviour 
//{
//	
//	public static PersistentController control;
//	
//	public int currentScore;
//	public List<GameObject> flockBirds;
//	public string nextLevelName = "";
//	
//	// Use this for initialization
//	void Awake () 
//	{
//		if (control == null)
//		{
//			DontDestroyOnLoad(gameObject);
//			control = this;
//		} 
//		else if (control != this) 
//		{
//			// we only want one persistent controller, so delete this if one already exists
//			Destroy(gameObject);	
//		}
//		
//	}
//	
//	void Start () 
//	{
//		InvokeRepeating("UpdateScore",0f,0.1f);
//	}
//	
//	// Update is called once per frame
//	void Update () 
//	{
//	
//	}
//	
//	void UpdateScore()
//	{
//		currentScore += Controller.current.flockPointsPerSecond / 10;
//	}
//	
//	public void SaveGame() 
//	{
//		// works on all platforms except web player (you can't save local files on web)
//		
//		BinaryFormatter bf = new BinaryFormatter();
//		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");
//		
//		PlayerData data = new PlayerData();
//		data.currentScore = currentScore;
//		bf.Serialize(file, data);
//		file.Close();
//		
//	}
//	
//	public void LoadGame() 
//	{
//		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat"))
//		{
//			BinaryFormatter bf = new BinaryFormatter();
//			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
//			
//			PlayerData data = (PlayerData)bf.Deserialize(file);
//			file.Close();
//			
//			currentScore = data.currentScore;
//		}
//	}
//
//}
//
//[Serializable]
//class PlayerData 
//{
//	//data container for writing data to file with SaveGame() above
//	//vars below should mirror data to be saved in the main class
//	public int currentScore;
//}
//
//
