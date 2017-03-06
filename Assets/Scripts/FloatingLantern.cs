using UnityEngine;
using System.Collections;

public class FloatingLantern : MonoBehaviour {
	
	public Vector3 movement;
	private int delayCtr;
	private int flickerCtr;
	private float blowOutThreshold;
	private float blowOutRate;
	private Animator animator;
	private Collider2D lanternCollider;

	private bool sinking;

	void Awake(){
		
		animator = GetComponent<Animator> ();
		lanternCollider = GetComponent<BoxCollider2D> ();
	
	}
		// Use this for initialization
	void Start () {
		movement = new Vector3(-0.001f - Random.value * 0.001f,-0.005f - Random.value * 0.005f,0f);
		blowOutRate = 1f;
		blowOutThreshold = 30f;

	}
	
	// Update is called once per frame
	void Update () {

		transform.position += movement;
	
		if (transform.rotation.z > 0.30f) {
			transform.rotation = Quaternion.Euler (0, 0, 0.30f);
		} else if (transform.rotation.z < -0.30f) {
			transform.rotation = Quaternion.Euler (0, 0, -0.30f);
		}

		if (flickerCtr <= 0) {
			GetComponent<Light>().intensity = 1.375f + Random.value * 0.125f;
			flickerCtr = Random.Range (10,30);
		} else {
			flickerCtr--;
		}

		Quaternion fromRotation = transform.rotation; 
		Quaternion toRotation = Quaternion.Euler(0,0,0);
		transform.rotation = Quaternion.Lerp(fromRotation,toRotation,Time.deltaTime * 5f);

		if (sinking) {

			if (transform.localScale.x > 0.5){
				transform.localScale -= new Vector3(0.0005f,0.0005f,0.0005f);
			}
		}
	}

	void OnCollisionStay2D(Collision2D col) {
		if (col.gameObject.tag == "Bird") {
				blowOutThreshold -= blowOutRate;
				
				GetComponent<Light>().intensity = 0.375f + Random.value * 0.75f;
				flickerCtr = Random.Range (2,4);

				if (blowOutThreshold <= 0) {
					
					//lantern burns out after collided with too much
					
					sinking = true;

					lanternCollider.GetComponent<Collider2D>().enabled = false;
					GetComponent<Light>().enabled = false;	
					animator.Play ("lantern_dark");
					
					//move below birds
					transform.position = new Vector3(transform.position.x,transform.position.y,-1); 
					
				}
		}
	}
	
}
