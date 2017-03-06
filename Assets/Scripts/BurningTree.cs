using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BurningTree : MonoBehaviour {
	
	public BurnState burnState = BurnState.Unburned;
	
	//the distance the heat from this tree can reach to spread fire
	public float burnSpreadRadius = 0.75f;
	//how long this tree burns before it's charred
	public float burnDuration = 10f;
	//the rate this tree "heats up" and potentially ignites neighbor trees
	public float burnHeatRatePerSecond = 1f;
	//the amount of "heat" required to light this tree on fire
	public float burnResilience = 1f;
	//the current "temperature" of this tree when its burning
	private float burnHeatIntensity = 0;

	
	public List<GameObject> neighbors;

	private bool smoking = false;

	private Animator animator;
	
	// Use this for initialization
	void Start () {
		animator = this.gameObject.GetComponent<Animator> ();
		//add a little randomness to resilience based on seed value from default/component
		float baseResilience = burnResilience;
		burnResilience = baseResilience * 0.5f + (Random.value * baseResilience * 2f);
		
		foreach (GameObject gObj in GameObject.FindGameObjectsWithTag ("BurningTree")){
			if (Vector3.Distance(this.gameObject.transform.position, gObj.transform.position) <= burnSpreadRadius){
				neighbors.Add (gObj);
			}
		}
	
		if (burnState == BurnState.Charred){
			animator.SetBool ("charred", true);
		} else if (burnState == BurnState.Catching){
		
		} else if (burnState == BurnState.Burning){
			

			//gameObject.SendMessageUpwards("OnSmoke", this.gameObject);
			
			animator.SetBool ("burning", true);
			Invoke ("BurnOut", burnDuration);
			InvokeRepeating ("HeatUp", 0.1f, 0.1f);
		} else if (burnState == BurnState.Smouldering){
			
		} else {
			animator.StopPlayback();
		}
		
	}
	
	// Update is called once per frame
	void Update () {

		if (animator.GetBool ("burning") && !smoking) {
			gameObject.SendMessageUpwards("OnSmoke", this.gameObject);
			smoking = true;
		}
	}
	
	void BurnOut(){
		//the tree stops burning and becomes charred
		burnState = BurnState.Charred;
		animator.SetBool ("burning", false);
		animator.SetBool ("charred", true);
		gameObject.SendMessageUpwards("OffSmoke", this.gameObject);

	}


	void HeatUp(){
		
		foreach(GameObject neighbor in neighbors){
			neighbor.GetComponent<BurningTree>().burnHeatIntensity += burnHeatRatePerSecond/10;
			
			if (neighbor.GetComponent<BurningTree>().burnState == BurnState.Unburned){
				if (neighbor.GetComponent<BurningTree>().burnHeatIntensity > neighbor.GetComponent<BurningTree>().burnResilience){
					//unburned trees that reach their "heat" threshold light on fire
					neighbor.GetComponent<BurningTree>().burnState = BurnState.Burning;
					neighbor.GetComponent<Animator> ().SetBool ("burning", true);
					

					neighbor.GetComponent<BurningTree>().Invoke ("BurnOut", neighbor.GetComponent<BurningTree>().burnDuration);
					neighbor.GetComponent<BurningTree>().InvokeRepeating ("HeatUp", 0.1f, 0.1f);
					
				}
			}
		}
	}
	
}

public enum BurnState {
	Unburned, Charred, Catching, Burning, Smouldering
}
