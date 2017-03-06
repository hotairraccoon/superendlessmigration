using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForestFireController : MonoBehaviour {
	public GameObject smokePrefab;
	public int fireSize = 10;

	private Dictionary<string, GameObject> onFire = new Dictionary<string, GameObject>();
	private ArrayList emitters = new ArrayList();
	private int counter = 0;
	void Start() {
		for (int i = 0; i < fireSize; i++) {
			GameObject smokeObj;
			smokeObj = (GameObject) Instantiate(smokePrefab);
			smokeObj.GetComponent<Puffy_Emitter>().autoEmit = false;
			emitters.Add (smokeObj);
		}
	}
	void OnSmoke(GameObject who) {
		foreach (GameObject neighbor in who.GetComponent<BurningTree>().neighbors) {
			if(onFire.ContainsKey(neighbor.name)) {
				return;
			}
		}
		counter++;
		GameObject emitter = null;
		foreach(GameObject emit in emitters) {

			if(!emit.GetComponent<Puffy_Emitter>().autoEmit) {
				emitter = emit;
				break;
			}
		}
		if (emitter != null) {
			emitter.transform.position = new Vector3(who.transform.position.x, who.transform.position.y + 1.25f, who.transform.position.z);	
			emitter.GetComponent<Puffy_Emitter>().autoEmit = true;
			onFire.Add(who.name, emitter);
		}
	}

	void OffSmoke(GameObject who) {
		if (onFire.ContainsKey (who.name)) {
			onFire [who.name].GetComponent<Puffy_Emitter> ().autoEmit = false;
			onFire.Remove (who.name);
		}
	}
}
