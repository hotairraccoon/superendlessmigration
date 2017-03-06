using UnityEngine;
using System.Collections;

public class Feather : MonoBehaviour {
	
	public float speed = 4f;
	public float rotationVariance = 90f;
	public float featherSpinSpeed = 15f;
	
	public Vector3 popMovement;
	public Quaternion popRotation;
	
	public GameObject activeFeatherSprite;
	private float fadeAlpha = 1f;
	
	void Awake ()
	{
		speed = speed * 0.5f + Random.value * speed;
		rotationVariance = ((Random.value * rotationVariance) - rotationVariance/2f)*Mathf.PI/180f;
		featherSpinSpeed = (Random.value * featherSpinSpeed) - featherSpinSpeed/2f;
		
		float randScale = 0.5f + Random.value * 0.4f;
		transform.localScale = new Vector3(randScale,randScale,randScale);
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (activeFeatherSprite){
			popMovement = new Vector3(
				speed * Mathf.Cos (popRotation.z * Mathf.PI + rotationVariance),
				speed * Mathf.Sin (popRotation.z * Mathf.PI + rotationVariance),
				0);
			transform.Translate (popMovement * Time.deltaTime);
			
			activeFeatherSprite.transform.Rotate (Vector3.forward * featherSpinSpeed);
			
			//fade feather out
			fadeAlpha -= 0.01f;
			activeFeatherSprite.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,fadeAlpha);
			
			if (fadeAlpha <= 0){
				
				foreach (Transform child in transform)
				{
					child.gameObject.SetActive(false);
				}
			
				gameObject.SetActive(false);
				fadeAlpha = 1f;
			}
		}
		
	}
}
