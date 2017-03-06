using UnityEngine;
using System.Collections;

public class Soar : MonoBehaviour {
	
	public float soarTime_trigger = 3f;
	public float soarTime_duration = 1f;
	private Animator animator;
	
	// Use this for initialization
	void Start () {
		float soarTime = soarTime_trigger + Random.value * soarTime_trigger * 2f;
		
		animator = GetComponent<Animator>();
		Invoke ("StartSoaring", soarTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	
	}
	
	void StartSoaring () {
	
		animator.Play ("teratorn_soar");
		animator.speed = 0f;
		
		float soarTime = soarTime_duration + Random.value * soarTime_duration * 0.5f;
		Invoke ("StopSoaring", soarTime);
	}
	
	void StopSoaring () {
		
		animator.Play ("teratorn");
		animator.speed = 1f;
		
		float soarTime = soarTime_trigger + Random.value * soarTime_trigger * 2f;
		Invoke ("StartSoaring", soarTime);
	}
}
