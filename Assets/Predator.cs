using UnityEngine;
using System.Collections;

public class Predator : MonoBehaviour {
	
	
	public bool attackActivated = false;
	public bool strikeActivated = false;
	public Color attackColor;
	public AudioClip attackSound;
	public AudioClip finalAttackSound;
	
	public float strikeBoostCooldown = 2f;
	public bool strikeBoostReady = true;
	
	public float huntSpeed_base = 6f;
	public float huntRotateSpeed_base = 60f; //degrees per second
	public float huntRotateSpeed_max;
	public float huntRotateSpeed_reset;
	public bool huntBoosting = false;
	public int appetite = 5;
	public int totalStrikeAttempts = 0;
	
	public float freeFlightSpeed = 2f;
	
	private AudioSource audioSrc;
	private SpriteRenderer spriteRenderer;
	
	public GameObject preyTarget;
	
	private float huntBoostMultiplier;
	private float huntSpeed;
	private float huntRotateSpeed;
	
	private float rotDir = 0f;
	
	private Vector3 huntDirection;
	
	private GameObject aggroCircle;
	
	// Use this for initialization
	void Start () {
		huntBoostMultiplier = 2.5f;
		attackColor = new Color(255f,60f,60f,1f);
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		audioSrc = Controller.current.GetComponent<AudioSource>();
		
		huntSpeed = huntSpeed_base;
		huntRotateSpeed = huntRotateSpeed_base;
		huntRotateSpeed_max = huntRotateSpeed_base * 3f;
		huntRotateSpeed_reset = huntRotateSpeed_base;
		
		aggroCircle = transform.Find ("CircleRadius").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
			
		if (attackActivated && Camera.main.transform.position.y < 50f){
			
			if (preyTarget) {
				
				huntDirection = preyTarget.transform.position - transform.position;
				float currentAngle = transform.localRotation.eulerAngles.z;
				float destAngle = Mathf.Atan2(huntDirection.y, huntDirection.x) * Mathf.Rad2Deg;
				
				if (huntRotateSpeed_base < huntRotateSpeed_max){
					huntRotateSpeed_base += 1f;
					huntRotateSpeed += 1f;
				}
				
				if (destAngle < 0){
					destAngle += 360f;
				}
				
				if (strikeBoostReady){
					if (currentAngle + huntRotateSpeed * Time.deltaTime < destAngle && rotDir == 0f){
						rotDir = 1f;
						transform.rotation = Quaternion.AngleAxis(currentAngle + huntRotateSpeed * rotDir * Time.deltaTime, Vector3.forward);
					} else if (currentAngle - huntRotateSpeed * Time.deltaTime > destAngle && rotDir == 0f){
						rotDir = -1f;
						transform.rotation = Quaternion.AngleAxis(currentAngle + huntRotateSpeed * rotDir * Time.deltaTime, Vector3.forward);
					} else {
						transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(destAngle, Vector3.forward), 0.1f);
						rotDir = 0f;
					}
				}
				
				if (transform.position.x < -25f || transform.position.x > 25f){
					transform.rotation = Quaternion.AngleAxis(destAngle, Vector3.forward);
					rotDir = 0f;
				}
					
			} else {
				rotDir = 0f;
			}
			
			transform.Translate(Vector3.right * huntSpeed * Time.deltaTime);
			
		}
		else {
		
			transform.Translate(Vector3.left * freeFlightSpeed * Time.deltaTime);
				
		}
	}
	
	public void ActivateAttack(GameObject target) {
		
		if (attackActivated == false){
			rotDir = 1f;
			preyTarget = target;
			spriteRenderer.material.SetColor("_Color", Color.red);
			audioSrc.PlayOneShot(attackSound, 1f);
			
			aggroCircle.SetActive(false);
			attackActivated = true;
			
		}
		
	}
	
	public void EatBird (GameObject target) 
	{
		//predators always target last bird in flock next!
		preyTarget = Controller.current.flockBirds[Controller.current.flockBirds.Count-1];
	}
	
	public void ActivateStrikeBoost() 
	{
		strikeBoostReady = false;
		strikeActivated = true;
		huntSpeed = huntBoostMultiplier * huntSpeed_base;
		huntRotateSpeed = huntBoostMultiplier * huntRotateSpeed_base;
		Invoke ("EndStrikeBoost", 0.25f);
		
		if (totalStrikeAttempts + 1 >= appetite){
			audioSrc.PlayOneShot(finalAttackSound, 1f);
		} else {
			audioSrc.PlayOneShot(attackSound, 1f);
		}
	}
	
	void EndStrikeBoost () {
		strikeActivated = false;
		huntSpeed = huntSpeed_base;
		
		huntRotateSpeed_base = huntRotateSpeed_reset;
		huntRotateSpeed = huntRotateSpeed_base;
		Invoke ("ResetStrikeBoost", strikeBoostCooldown);
	}
	
	void ResetStrikeBoost () {
		totalStrikeAttempts++;
		
		if (totalStrikeAttempts < appetite){
			strikeBoostReady = true;
		} else {
			preyTarget = null;
		}	
		
	}
	
}
