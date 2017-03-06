using UnityEngine;
using System.Collections;

public class Puffy_Gradient : MonoBehaviour {
	
	public Gradient gradient;

	void Start () {
		Puffy_Emitter emitter = GetComponent<Puffy_Emitter>();
		if(!emitter){
			enabled = false;
		}else{
			emitter.colorGradient = this;
		}
	}
	
}
