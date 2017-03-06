using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class Highscore : MonoBehaviour {
	public static Highscore highscoreManager;

	private List<PlayerScoreData> highscores = new List<PlayerScoreData>();
	public List<Text> places;
	public List<Text> scores;
	public Canvas highscoreCanvas;
	public Canvas enterNameCanvas;
	public List<Text> letters;

	private string name = "";
	private int score = 0;
	private PlayerScoreData tmp;

	public void Score(int score) {
		if (score > highscores [highscores.Count - 1].score) {
			highscoreCanvas.enabled = false;
			enterNameCanvas.enabled = true;
			tmp = new PlayerScoreData("", score);
		} else {
			highscoreCanvas.enabled = true;
			enterNameCanvas.enabled = false;
			Invoke("MainMenu", 5f);
		}
			
	}

	public void MainMenu() {
		this.Off ();
		Application.LoadLevel("menu_chapterSelect");	
	}

	public int GetHighscore() {
		return highscores [0].score;
	}

	public void Off() {
		highscoreCanvas.enabled = false;
		enterNameCanvas.enabled = false;
	}

	void Awake () {
		if (highscoreManager == null) {
			DontDestroyOnLoad (gameObject);
			highscoreManager = this;
		} else if(highscoreManager != this) {
			Destroy(gameObject);
		}
		if (File.Exists (Application.persistentDataPath + "/highscore.dat")) {
			Load ();
		} else {
			//need to build a default list of highscores
			highscores.Add(new PlayerScoreData("AAA", 1000));
			highscores.Add(new PlayerScoreData("BBB", 900));
			highscores.Add(new PlayerScoreData("CCC", 800));
			highscores.Add(new PlayerScoreData("DDD", 700));
			highscores.Add(new PlayerScoreData("EEE", 600));
			highscores.Add(new PlayerScoreData("FFF", 500));
			highscores.Add(new PlayerScoreData("GGG", 400));
			highscores.Add(new PlayerScoreData("HHH", 300));
			highscores.Add(new PlayerScoreData("III", 200));
			highscores.Add(new PlayerScoreData("JJJ", 100));
			highscores.Add(new PlayerScoreData("KKK", 90));
			highscores.Add(new PlayerScoreData("LLL", 80));
			highscores.Add(new PlayerScoreData("MMM", 70));
			highscores.Add(new PlayerScoreData("NNN", 60));
			highscores.Add(new PlayerScoreData("OOO", 50));
			highscores.Add(new PlayerScoreData("PPP", 40));
			highscores.Add(new PlayerScoreData("QQQ", 30));
			highscores.Add(new PlayerScoreData("RRR", 20));
			highscores.Add(new PlayerScoreData("SSS", 10));
			highscores.Add(new PlayerScoreData("TTT", 5));
			highscores.Add(new PlayerScoreData("UUU", 2));
		}
	}

	void OnApplicationQuit() {
		Save ();
	}

	public void Save() {
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream fs = File.Create (Application.persistentDataPath + "/highscore.dat");

		HighscoreData data = new HighscoreData ();
		data.highscores = highscores;

		bf.Serialize (fs, data);
		fs.Close ();
	}

	public void Load() {
		if (File.Exists (Application.persistentDataPath + "/highscore.dat")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fs = File.Open(Application.persistentDataPath + "/highscore.dat", FileMode.Open);
			HighscoreData data = (HighscoreData)bf.Deserialize(fs);
			fs.Close ();

			highscores = data.highscores;
		}
	}
	
	void Update () {
		int index = 0;
		foreach (PlayerScoreData data in highscores) {
			places [index].text = data.name;
			scores[index].text = data.score.ToString();
			index = index + 1;
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			highscoreCanvas.enabled = !highscoreCanvas.enabled;
			enterNameCanvas.enabled = !enterNameCanvas.enabled;
			tmp = new PlayerScoreData("tes", 10101);
		}

		if (enterNameCanvas.enabled) {
			if(Input.GetKeyDown (KeyCode.Backspace)) {
				name = name.Substring(0,name.Length - 1);
			} else if(Input.GetKeyDown(KeyCode.Return)) {
				if(name.Length == 3) {
					highscoreCanvas.enabled = !highscoreCanvas.enabled;
					enterNameCanvas.enabled = !enterNameCanvas.enabled;
					tmp.name = name;
					name = "";
					highscores.Add(tmp);
					highscores.Sort();
					highscores.Reverse();
					highscores.RemoveAt(highscores.Count - 1);
					Invoke("MainMenu", 5f);
				} 
			} else {
				if(name.Length < 3) {
					if(Input.anyKey && !Input.GetKeyDown (KeyCode.Space))
					{
						name = name + Input.inputString;
					}
				}
			}
			int charIndex = 0;
			foreach (char c in name) {
				letters[charIndex].text = c.ToString();
				charIndex = charIndex + 1;
			}
			for(int i = charIndex; i < 3; i++) {
				letters[i].text = "_";
			}
		}


	}	
}

[Serializable]
class PlayerScoreData :  IComparable<PlayerScoreData> {
	public String name = "";
	public int score = 0;
	
	public PlayerScoreData(string name, int score) {
		this.name = name;
		this.score = score;
	}

	public int CompareTo(PlayerScoreData comparePart)
	{
		if (comparePart == null) {
			return 1;
		} else {
			return this.score.CompareTo (comparePart.score);
		}
	}
}

[Serializable]
class HighscoreData {
	public List<PlayerScoreData> highscores;
}


