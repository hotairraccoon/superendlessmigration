using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InitiateWormhole : MonoBehaviour 
{
	
	public List<GameObject> starObjects;
	private List<GameObject> stars;
	
	// Use this for initialization
	void Start () 
	{
		int starCount = 50 + (int)Mathf.Ceil(20f * Random.value);
		stars = new List<GameObject>();
		
		for (int i = 0; i < starCount; i++)
		{
			int starType = (int)Mathf.Floor (starObjects.Count * Random.value);
			GameObject obj = Instantiate(starObjects[starType]);
			Vector3 starPos = new Vector3(1.0f * Random.value, 1.0f * Random.value, obj.transform.position.z);
			float starScaleFactor = 0.25f + 0.125f * Random.value;
			float starBrightnessFactor = 0.25f + 0.5f * Random.value;
			SpriteRenderer starRenderer = obj.GetComponent<SpriteRenderer>();
			starRenderer.material.color = new Color(starRenderer.material.color.r, starRenderer.material.color.g, starRenderer.material.color.b, starBrightnessFactor);
			obj.transform.position = Camera.main.ViewportToWorldPoint(starPos);
			obj.transform.localScale *= starScaleFactor;
			
			stars.Add(obj);
		}
		
		Invoke ("TransitionLevel", 3f);
		
	}
	
	private AsyncOperation async;
	private List<Bird> flockBirdComponents;
	
	void TransitionLevel () {
	
		List<Bird> flockBirdComponents = new List<Bird>();
		
		for (int i = 0; i < Controller.current.flockBirds.Count; i++)
		{
			flockBirdComponents.Add (Controller.current.flockBirds[i].GetComponent<Bird>());
		}
		StartCoroutine(TransitionLevelAsync());
	}
	
	IEnumerator TransitionLevelAsync(){
		async = Application.LoadLevelAsync(Controller.current.nextLevelName);
//		while (! async.isDone) {
//			for (int i = 0; i < flockBirdComponents.Count; i++)
//			{
//				flockBirdComponents[i].BirdFloat();
//			}
//			yield return null;
//		}
		yield return async;
	}
}
