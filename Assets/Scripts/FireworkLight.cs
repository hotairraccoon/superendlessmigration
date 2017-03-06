using UnityEngine;
using System.Collections;

public class FireworkLight : MonoBehaviour {

	public float fireworkLoopTime = 3f;
	public float lifespan = 3f;
	private int flickerCtr;
	private float lifeCtr;
	private float loopCtr;


	void Awake(){
		lifeCtr = lifespan;
		loopCtr = fireworkLoopTime;
	}
	// Use this for initialization
	void Start () {

		GetComponent<Light>().intensity = 0;
		
	}
	
	// Update is called once per frame
	void Update () {

		lifeCtr -= Time.deltaTime;
		loopCtr -= Time.deltaTime;

		if (flickerCtr <= 0) {
			if (lifeCtr >= lifespan * 0.9) {
				GetComponent<Light>().intensity = 0;
			} else if (lifeCtr >= lifespan * 0.65){
				GetComponent<Light>().intensity = lifespan/6 + Random.value * lifespan/6;
			} else if (lifeCtr >= lifespan * 0.5){
				GetComponent<Light>().intensity = lifespan/3 + Random.value * lifespan/6;
			} else if (lifeCtr >= lifespan * 0.4){
				GetComponent<Light>().intensity = lifespan/2 + Random.value * lifespan/6;
			} else if (lifeCtr >= lifespan * 0.3){
				GetComponent<Light>().intensity = lifespan/6 + Random.value * lifespan/8;
			} else if (lifeCtr >= lifespan * 0.2){
				GetComponent<Light>().intensity = lifespan/8 + Random.value * lifespan/8;
			} else if (lifeCtr > 0) {
				GetComponent<Light>().intensity = 0;
			} else {
				GetComponent<Light>().intensity = 0;
			}

			flickerCtr = Random.Range (1, 5);
		} else {
			flickerCtr--;
		}

		if (loopCtr <= 0){
			lifeCtr = lifespan;
			loopCtr = fireworkLoopTime;
		}

	}
	
}