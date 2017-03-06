using UnityEngine;
using System.Collections;

public class TriggerContainer : MonoBehaviour {
	
	public TriggerListener[] childrenTriggers;
	public bool triggered = false;
	public bool hideUntilTriggered = false;
	
	// Use this for initialization
	void Awake () {
	
		childrenTriggers = GetComponentsInChildren<TriggerListener>();
		if (hideUntilTriggered)
		{
			for (int i = childrenTriggers.Length - 1; i >= 0; i--){
				childrenTriggers[i].gameObject.SetActive (false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnScreenTriggerEvent () {
		triggered = true;
		for (int i = childrenTriggers.Length - 1; i >= 0; i--){
			childrenTriggers[i].gameObject.SetActive (true);
			childrenTriggers[i].triggered = true;
		}
	}
}
