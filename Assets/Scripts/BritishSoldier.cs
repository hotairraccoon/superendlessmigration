using UnityEngine;
using System.Collections;

public class BritishSoldier : MonoBehaviour {
	
	public float shootTimerSeed = 7.0f;	
	public float deathTimerSeed = 4.0f;
	
	public float leftBounds = -100f;
	
	private float shootTimer;
	private float deathTimer;
	
	private bool dead = false;
	private HorizontalGroundMovement movementComponent;
	private Animator animator;
	
	public bool triggered = false;
	public TriggerListener triggerListener;
	
	// Use this for initialization
	void Start () {
		
		triggerListener = gameObject.GetComponent<TriggerListener>();
		
		shootTimer = shootTimerSeed + shootTimerSeed * 3.0f * Random.value;
		deathTimer = deathTimerSeed + deathTimerSeed * 3.0f * Random.value;
		
		movementComponent = GetComponent<HorizontalGroundMovement>();

		animator = GetComponent<Animator>();
		
		if (triggered == true){
			Invoke ("Shoot", shootTimer);
			Invoke ("Death", deathTimer);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if (triggerListener != null) {
			if (triggered == false && triggerListener.triggered == true){
				triggered = true;
				Invoke ("Shoot", shootTimer);
				Invoke ("Death", deathTimer);
			}
		}
		
		if (gameObject.transform.position.x <= leftBounds && dead == false){
			
			Death ();
			
		}
	}
	
	void Shoot () {
	
		movementComponent.disableMovement = true;
		
		if (dead == false){
			animator.Play("BritishSoldier_shooting");
			
			if (gameObject.transform.position.x > leftBounds){
				Invoke ("Run", shootTimer/5f);
			}
		}
		
	}
	
	void Run ()	{
	
		if (dead == false){
			movementComponent.disableMovement = false;
			animator.Play("BritishSoldier_running");
			Invoke ("Shoot", shootTimer);
		}
		
	}
	
	void Death () {
	
		if (dead == false){
			dead = true;
			movementComponent.disableMovement = true;
			
			if (Random.value <= 0.5f) {
				animator.Play("BritishSoldier_death");
			} else {	
				animator.Play("BritishSoldier_death2");
			}
		}
	}
	
}
