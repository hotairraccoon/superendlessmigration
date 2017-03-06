using UnityEngine;
using System.Collections;

public class AircraftTriggerSound : MonoBehaviour {
	
	public bool triggeredSound = false;
	public AudioClip spawnSound;
	public float spawnSoundVolume = 0.6f;
	
	// Use this for initialization
	void Start () { 
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (triggeredSound == false){
			
			Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
			
			if ((screenPos.x > -Camera.main.pixelWidth * 0.5f && screenPos.x < Camera.main.pixelWidth * 1.5f) 
			    && (screenPos.y > -Camera.main.pixelHeight * 0.5f || screenPos.y < Camera.main.pixelHeight * 1.5f))
			    {
				triggeredSound = true; 
				if (spawnSound){
					Controller.current.GetComponent<AudioSource>().PlayOneShot(spawnSound, spawnSoundVolume);
				} else {
					//Debug.Log ("This aircraft has no spawn sound");
				}
			}
	
		}
	}
}
