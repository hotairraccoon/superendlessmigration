using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SEMTrigger : MonoBehaviour {

	public string name;
	public What what;
	public GameObject[] listeners;
	public string data;
	private bool screenFired = false;
	
	void Update() {
		float screenAspect = (float)Screen.width / (float)Screen.height;
		float cameraHeight = Camera.main.orthographicSize * 2;
		
		if (what == What.Screen){
			Bounds bounds = new Bounds(
				Camera.main.transform.position,
				new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
			if (bounds.max.y > gameObject.GetComponent<Collider2D> ().bounds.min.y && !screenFired) {
				screenFired = true;
				foreach (GameObject listen in listeners) {
					listen.SendMessage ("OnScreenTriggerEvent", name);
				}
			}
		}
	}

	void OnTriggerEnter2D (Collider2D collider) {
		
		if (what == What.Player && collider.gameObject == Controller.current.leadBird) {
			
			foreach (GameObject listen in listeners) {
				Dictionary<string, string> val = 
					new Dictionary<string, string>();
				val.Add("name",name);
				val.Add("data", data);
				
				listen.SendMessage ("OnPlayerTriggerEvent", val);
			}
			
		} else if(what == What.NPC) {
		
			foreach (GameObject listen in listeners) {
				Dictionary<string, string> data = 
					new Dictionary<string, string>();
				data.Add("name",name);
				data.Add("who", collider.gameObject.name);
				listen.SendMessage ("OnNPCTriggerEvent", data);
			}
			
		}
		
	}

	void TriggerEvent () {
		Debug.Log ("event was triggered here");
	}
	
	#if UNITY_EDITOR
	[MenuItem("GameObject/Super Endless Migration/Trigger")]
	static void CreateSEMTrigger()
	{
		GameObject newObject =(GameObject) AssetDatabase.LoadAssetAtPath ("Assets/Prefabs/SEMTrigger.prefab", typeof(GameObject));
		Instantiate (newObject, new Vector3 (0, 0, 0), Quaternion.identity);
	}
	#endif
}

public enum What {
	Player, Screen, NPC
}