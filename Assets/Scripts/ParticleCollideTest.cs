using UnityEngine;
using System.Collections;

public class ParticleCollideTest : MonoBehaviour {
	
	public GameObject prefab;
	public float test = 0;
	private int ctr;

	private Vector3 pos;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		pos.x = prefab.transform.position.x;
		pos.y = prefab.transform.position.y;
		pos.z = -10f;

		this.transform.position = pos;
	}
	
	void OnParticleCollision(GameObject other) {
		ctr++;
	}
}
