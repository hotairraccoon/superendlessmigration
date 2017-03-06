using UnityEngine;
using System.Collections;

public class FallingBoulder : MonoBehaviour {
	
	private GameObject boulder;
	private GameObject shadow;
	private GameObject boulderSmash;
	private float shadowScale;
	
	public int fallState = 0;
	
	public AudioClip smashSound;
	
	// Use this for initialization
	void Start () {
		boulder = transform.Find("Boulder").gameObject;
		boulder.transform.localScale = new Vector3(1.25f,1.25f,1.25f);
		shadow = transform.Find("BoulderShadow").gameObject;
		boulderSmash = transform.Find("BoulderSmash").gameObject;
		shadowScale = shadow.transform.localScale.x;
		
		boulder.SetActive(false);
		boulderSmash.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
		if (fallState == 0){
			shadowScale += 0.5f * Time.deltaTime;
			shadow.transform.localScale = new Vector3(shadowScale, shadowScale, shadowScale);
			
			if (shadowScale > 1f){
				boulder.SetActive(true);
				fallState = 1;
			} 
		} else if (fallState == 1) {
			boulder.transform.position = new Vector3(boulder.transform.position.x, boulder.transform.position.y - 8f * Time.deltaTime, boulder.transform.position.z);
			boulder.transform.localScale = new Vector3(boulder.transform.localScale.x * 0.99f,
			                                           boulder.transform.localScale.y * 0.99f,
			                                           boulder.transform.localScale.z * 0.99f);
			
			if (boulder.transform.localPosition.y <= 0f){
				boulder.SetActive(false);
				boulderSmash.SetActive(true);
				shadow.SetActive(false);
				fallState = 2;
				Controller.current.GetComponent<AudioSource>().PlayOneShot(smashSound, 0.25f + Random.value * 0.25f);
				
			}
		}
		
	}
}
