using UnityEngine;
using System.Collections;

public class MenuEvents : MonoBehaviour {
	
	public string sceneName = "scene_city";
	
	public void LoadLevel(string levelName)
	{
		Highscore.highscoreManager.Off ();
		Application.LoadLevel(levelName);
	}
	
	public void ExitApplication()
	{
		Application.Quit();
	}
	
	
	// Use this for initialization
	void Start () {
		Cursor.visible = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
}
