using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OceanGenerator : MonoBehaviour {

	public int oceanHeight = 200;

	public GameObject prefab1;
	public GameObject prefab2;
	public GameObject prefab3;

	void Awake()
	{

		float totalHeight = 0f;
		int waveAlternator = 0;
		GameObject gObj;

		while (totalHeight < oceanHeight) {
		
			if (waveAlternator == 0){
				gObj = (GameObject) Instantiate(prefab1);
				waveAlternator = 1;
				totalHeight += 0.75f + Random.value * 0.5f;
			} else if (waveAlternator == 1) {
				gObj = (GameObject) Instantiate(prefab2);
				waveAlternator = 2;
				totalHeight += 0.4f + Random.value * 0.2f;
			} else {
				gObj = (GameObject) Instantiate(prefab3);
				waveAlternator = 0;
				totalHeight += 0.1f + Random.value * 1f;
			}

			Vector3 startPos;
			startPos.x = -3f + Random.value * 3;
			startPos.y = this.gameObject.transform.position.y + totalHeight;
			startPos.z = 0;
			gObj.transform.position = startPos;

			gObj.transform.localScale = new Vector3(Mathf.Pow(-1f, Random.Range(1,3)), 1, 1);

		}

	}
	// Use this for initialization
	void Start () {



	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
