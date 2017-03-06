using UnityEngine;
using System.Collections;

public class FadeInOut : MonoBehaviour {

	public void FadeOut (){
		
		StopCoroutine(DoFadeInFromBlack());
		StartCoroutine(DoFadeToBlack());
		
	}
	
	public void FadeIn (){
		
		StopCoroutine(DoFadeToBlack());
		StartCoroutine(DoFadeInFromBlack());
	
	}
	
	IEnumerator DoFadeToBlack() {
		
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.interactable = true;
		while (canvasGroup.alpha < 1){
			canvasGroup.alpha += Time.unscaledDeltaTime / 2;
			yield return null;
		}
		
		yield return null;
		
	}
	
	IEnumerator DoFadeInFromBlack() {
		
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		while (canvasGroup.alpha > 0){
			canvasGroup.alpha -= Time.unscaledDeltaTime / 2;
			yield return null;
		}
		
		canvasGroup.interactable = false;
		yield return null;
		
	}
	
}
