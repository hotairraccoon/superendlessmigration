using UnityEngine;
using System.Collections;

public class OceanWaves : MonoBehaviour {

	public float speed = 2f;
	private int direction = 1;

	// Use this for initialization
	void Start () {

		speed = 6f * Random.Range (-1f, 1f);

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 movement = new Vector3(speed * direction, 0, 0);
		movement *= Time.deltaTime;
		transform.Translate(movement);

		if (this.transform.position.x <= -2) {
			if (speed > 0) {
				direction = 1;
			} else {
				direction = -1;
			}
		} else if (this.transform.position.x >= 2){
			if (speed > 0) {
				direction = -1;
			} else {
				direction = 1;
			}
		}
	}
}
