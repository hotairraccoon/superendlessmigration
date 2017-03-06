using UnityEngine;
using System.Collections;

public class CaveRoofFade : MonoBehaviour {
	
	private bool caveActivated;
	private CaveFadeTrigger fadeTrigger;
	private SpriteRenderer spriteRenderer;
	
	// Use this for initialization
	void Start () {
		fadeTrigger = transform.parent.transform.parent.GetComponent<CaveFadeTrigger>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (fadeTrigger.caveActivated){
			spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - 1f * Time.deltaTime);
		}	
	
	}
}
