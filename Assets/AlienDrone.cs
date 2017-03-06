using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlienDrone : MonoBehaviour {
	
	public bool onscreen = false;
	public bool scanning = false;
	public bool activated = false;
	public bool targeting = false;
	public bool firing = false;
	
	public bool disableGuns = false;

	public List<GameObject> guns;
	public float fireRate = 0.5f;
	public float chanceToFireOnscreen = 1f;
	
	public GameObject activeSensorCone;
	public GameObject inactiveSensorCone;
	
	public float baseScanRate = 3f;
	public int totalScans = 8;
	private int spentScans;
	private float scanRate;
	
	public float fireGunsDelay = 0.125f;
	
	public float baseMoveSpeed = 1.5f;
	private float activeMoveSpeed;
	private float moveSpeed;
	
	public float destScanRotation;
	
	public AudioClip activateSound;
	public AudioClip fireLasersSound;
	
	public GameObject targetObj;
	
	// Use this for initialization
	void Start () 
	{
		activeSensorCone.transform.localScale = new Vector3(activeSensorCone.transform.localScale.x, 0, activeSensorCone.transform.localScale.z);
		inactiveSensorCone.transform.localScale = new Vector3(inactiveSensorCone.transform.localScale.x, 0, inactiveSensorCone.transform.localScale.z);
		
		activeSensorCone.SetActive(false);
		inactiveSensorCone.SetActive(false);
		
		scanRate = baseScanRate + Random.value * baseScanRate * 0.4f;
		
		moveSpeed = baseMoveSpeed * 1f + Random.value * 0.2f * baseMoveSpeed;
		activeMoveSpeed = moveSpeed * 5f;
		
		if (Random.value > 0.5) 
		{
			destScanRotation = 60f;
		} 
		else 
		{
			destScanRotation = -60f;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		Vector3 glidePos = new Vector3(transform.position.x, transform.position.y + 2f * Time.deltaTime, transform.position.z);
		transform.position = glidePos;
		
		Vector3 screenPoint = Camera.main.WorldToViewportPoint (transform.position);
		
		if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1) 
		{
			if (!onscreen && !scanning && !firing && !activated)
			{
				InvokeRepeating ("ScanForEnemies", scanRate, scanRate * 4f);
			}
			onscreen = true;
		}
		else 
		{
			onscreen = false;
		}	
		
		if (scanning)
		{
			inactiveSensorCone.transform.localScale = Vector3.Lerp(inactiveSensorCone.transform.localScale, new Vector3(inactiveSensorCone.transform.localScale.x, 1f, inactiveSensorCone.transform.localScale.z), 1f * Time.deltaTime);
			
			if (inactiveSensorCone.transform.localScale.y >= 0.25f)
			{
				float scanRot = transform.eulerAngles.z + destScanRotation;
				transform.rotation = Quaternion.AngleAxis(Mathf.LerpAngle(transform.eulerAngles.z, scanRot, 1f * Time.deltaTime), Vector3.forward);
			}
		}
		else if (activated)
		{
			activeSensorCone.transform.localScale = Vector3.Lerp(activeSensorCone.transform.localScale, new Vector3(activeSensorCone.transform.localScale.x, 1f, activeSensorCone.transform.localScale.z), 3f * Time.deltaTime);
			transform.Translate (Vector3.right * activeMoveSpeed * Time.deltaTime);
			
			if (targeting)
			{
				
				Vector3 dir = targetObj.transform.position - transform.position;
				float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}
		}
		else 
		{
			transform.Translate (Vector3.right * moveSpeed * Time.deltaTime);
		}
		
		if (Camera.main.WorldToViewportPoint(transform.position).y < -0.1f || 
		    Camera.main.WorldToViewportPoint(transform.position).y > 1.1f ||
		    Camera.main.WorldToViewportPoint(transform.position).x < -0.1f ||
		    Camera.main.WorldToViewportPoint(transform.position).x > 1.1f)
		    { 
		    
		    activated = false;
			StopScanForEnemies();
			StopFiring ();
			CancelInvoke ("ScanForEnemies");
			
			if (spentScans < totalScans)
			{
				
				Vector3 dir = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.45f, transform.position.z)) - transform.position;
				float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			
			}
		}
	}
	
	void ScanForEnemies ()
	{
		scanning = true;
		scanRate = baseScanRate + Random.value * baseScanRate * 0.4f;
		inactiveSensorCone.SetActive(true);
		
		if (Random.value > 0.5) 
		{
			destScanRotation = 60f;
		} 
		else 
		{
			destScanRotation = -60f;
		}
		Invoke ("StopScanForEnemies", scanRate);
	}
	
	void StopScanForEnemies()
	{
		if (scanning)
		{	
			spentScans++;
			inactiveSensorCone.transform.localScale = new Vector3(inactiveSensorCone.transform.localScale.x, 0f, inactiveSensorCone.transform.localScale.z);
			inactiveSensorCone.SetActive(false);
			scanning = false;
		}
	}
	
	public void FireGuns () 
	{
		CancelInvoke ("ScanForEnemies");
		Controller.current.audioSrc.PlayOneShot(fireLasersSound);
		targeting = false;
		
		if (guns.Count > 0)
		{
			for (int i = 0; i < guns.Count; i++)
			{
				guns[i].GetComponent<MachineGunFire>().StartFiring();
			}
		}
	}
	
	public void StopFiring () 
	{
		firing = false;
		activeSensorCone.SetActive(false);
		if (guns.Count > 0)
		{
			for (int i = 0; i < guns.Count; i++)
			{
				guns[i].GetComponent<MachineGunFire>().StopFiring();
			}
		}
	}
	
}



