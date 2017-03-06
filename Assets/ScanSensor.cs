using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScanSensor : MonoBehaviour {
	
	public AlienDrone parentDrone;
	
	// Use this for initialization
	void Start () {
		parentDrone = transform.parent.GetComponent<AlienDrone>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Bird"){
			parentDrone.scanning = false;
			parentDrone.activated = true;
			parentDrone.activeSensorCone.transform.localScale = parentDrone.inactiveSensorCone.transform.localScale;
			parentDrone.activeSensorCone.SetActive(true);
			parentDrone.inactiveSensorCone.SetActive(false);
			
			Vector3 dir = col.transform.position - parentDrone.transform.position;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			parentDrone.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			
			
			Controller.current.audioSrc.PlayOneShot(parentDrone.activateSound);
			parentDrone.targetObj = col.gameObject;
			parentDrone.targeting = true;
			parentDrone.Invoke ("FireGuns", parentDrone.fireGunsDelay);
			
		}
	}
}
