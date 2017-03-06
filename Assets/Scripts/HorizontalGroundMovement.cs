using UnityEngine;
using System.Collections;

public class HorizontalGroundMovement : MonoBehaviour {
	
	public bool	looping = true;
	public float speed = 0.05f;
	public bool disableMovement = false;
	
	public bool useSceneStartingPoint = false;
	public float startingPointX = -20f;
	public float startingDirection = 1f;
	
	public float rightLimit = 21f;
	public float leftLimit = -21f;
	
	public bool randomizeSpeed = false;
	public float randomSpeedFactor = 0f;
	
	public bool triggered = true;
	public TriggerListener triggerListener;
	
	// Use this for initialization
	void Start () {
		
		triggerListener = gameObject.GetComponent<TriggerListener>();
		
		if (useSceneStartingPoint == false) {
			this.gameObject.transform.position = new Vector3(startingPointX, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
		}
		
		Vector3 moveDirection = new Vector3(this.gameObject.transform.localScale.x * startingDirection, this.gameObject.transform.localScale.y, this.gameObject.transform.localScale.z);
		this.gameObject.transform.localScale = moveDirection;
		
		if (randomizeSpeed == true){
			speed = speed + (2f * randomSpeedFactor * Random.value);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (triggerListener != null){
			triggered = triggerListener.triggered;
		}
		
		if (triggered == true && disableMovement == false){
			if (this.gameObject.transform.position.x > rightLimit) {
				if (looping){
					this.gameObject.transform.position = new Vector3(startingPointX, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
				} else {
					Destroy (this.gameObject);
				}
			} else if (this.gameObject.transform.position.x < leftLimit && startingDirection == -1f) {
				if (looping){
					this.gameObject.transform.position = new Vector3(startingPointX, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
				} else {
					Destroy (this.gameObject);
				}
			} else {
				this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x + speed * this.gameObject.transform.localScale.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
			}
		}

	}
	
	public void OnScreenTriggerEvent () {
		triggered = true;
	}
}
