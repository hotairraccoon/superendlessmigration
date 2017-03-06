using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerTitleCard : MonoBehaviour {
	
	public bool triggered;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnPlayerTriggerEvent() {
		triggered = true;
		TitleCard.current.Message("A cave!");
		
	}
	
}
