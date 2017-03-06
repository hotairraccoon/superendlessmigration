using UnityEngine;
using System.Collections;

public class LanternSpawner : MonoBehaviour {

	//for spawning lanters
	public GameObject prefab;

	// Use this for initialization
	void Start () {
		Invoke("SpawnLantern", 0.1f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void SpawnLantern ()
	{
		
		Vector3 startPos;
		
		startPos.x = Random.Range (-4, 15);
		startPos.y = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 1.0f, 0.0f)).y + 5.0f;
		startPos.z = -15;
		
		// instantiate a bird
		GameObject gObj = (GameObject) Instantiate(prefab);
		gObj.transform.position = startPos;

		float spawnTime = 1.5f * Random.value;
		Invoke("SpawnLantern", spawnTime);
	}
}
