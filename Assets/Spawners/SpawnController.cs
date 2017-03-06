using UnityEngine;
using System.Collections.Generic;


public abstract class SpawnController : MonoBehaviour {

	public List<GameObject> spawnables;
	public int possibleActive = 100;
	private int spawn = 0;
	void Start () {
		
	}
	
	void Update () {
		if(spawn < possibleActive) {
			int spawnIndex = Mathf.FloorToInt((spawnables.Count * Random.value));
			Spawnable spawnableData = (Spawnable)spawnables[spawnIndex].GetComponent(typeof(Spawnable));
			if(Random.value < spawnableData.chanceToSpawn) {
				Spawn(spawnables[spawnIndex]);
				spawn++;
			}
		} 
	}

	virtual public Vector3 getStartPosition() {
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	virtual public Vector3 getTarget() {
		return Vector3.zero;
	}

	virtual public void destroySpawned() {
		spawn = spawn - 1;
	}

	virtual public void Spawn(GameObject who) {
		Vector3 pos = getStartPosition();
		Quaternion rot = Quaternion.identity;
		pos.z = 0;
		Instantiate(who, pos, rot);
	}
}
