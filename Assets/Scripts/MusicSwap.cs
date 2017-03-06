using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicSwap : MonoBehaviour {
	
	public Vector3 cameraPosition;
	public float musicFadeSpeed = 1f;
	public bool triggered = false;
	
	private Transform primaryMusic;
	private Transform secondaryMusic;
	
	void Awake () {
	}
	
	// Use this for initialization
	void Start () {
		primaryMusic = transform.Find("PrimaryMusic");
		secondaryMusic = transform.Find("SecondaryMusic");
	}
	
	// Update is called once per frame
	void Update () {
		
		if (triggered == true)
		{
			MusicFading ();
		} else {
			if (Camera.main.transform.position.y > secondaryMusic.gameObject.transform.position.y){
				//the camera has "passed" the object, triggering it
				secondaryMusic.GetComponent<AudioSource>().PlayDelayed(4f);
				triggered = true;
			}
		}
		
	}
	
	void MusicFading(){
		//Debug.Log ("fading");
		primaryMusic.GetComponent<AudioSource>().volume = Mathf.Lerp(primaryMusic.GetComponent<AudioSource>().volume, 0f, musicFadeSpeed);
		//secondaryMusic.GetComponent<AudioSource>().volume = Mathf.Lerp(secondaryMusic.GetComponent<AudioSource>().volume, 0.9f, 2 * musicFadeSpeed * Time.deltaTime);
	}
}
