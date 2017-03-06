using UnityEngine;
using System.Collections;

public class Spawnable : MonoBehaviour {
	public float chanceToSpawn = 1.0f;
	public bool destroyOfScreen = false;

	private SpawnController spawnC = null;
	void Update () {
		if(destroyOfScreen) {
			Vector3 locToCamera = Camera.main.WorldToViewportPoint(this.gameObject.transform.position);
			if(locToCamera.y < 0) {
				if(spawnC != null) {
					spawnC.destroySpawned();
				}
				Destroy(gameObject);
			}
		}
	}

	public void setController(SpawnController controller) {
		spawnC = controller;
	}


}
