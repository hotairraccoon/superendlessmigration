using UnityEngine;
using System.Collections;

public class Test_FireworkMover : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if ( !GetComponent<ParticleSystem>().IsAlive(true) ) {
				
			this.transform.position = new Vector3(this.transform.position.x + (-50 + 100 * Random.value), this.transform.position.y + (-50 + 100 * Random.value), this.transform.position.z); 
			GetComponent<ParticleSystem>().Play();
		}
	}

}
