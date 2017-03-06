using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class testingtrigger : MonoBehaviour {

	void OnScreenTriggerEvent (string name) {
		Debug.Log ("On screen: " + name);
	}

	void OnPlayerTriggerEvent (string name) {
		Debug.Log ("Player event: " + name);
	}

	void OnNPCTriggerEvent (Dictionary<string, string> name) {
		Debug.Log ("NPC: " + name["name"]);
		Debug.Log ("NPC: " + name["who"]);
	}
}
