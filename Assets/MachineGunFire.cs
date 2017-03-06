using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MachineGunFire : MonoBehaviour {
	
	public bool firing = false;
	public bool disableGuns = false;
	
	public bool onscreen = false;
	public float fireRate = 0.5f;
	
	public float chanceToFireOnscreen = 1f;
	public float initialFireDelay = 0.5f;
	
	public bool fireRightAway = false;
	
	public AudioClip gunSound;
	public WeaponType weaponType;
	
	public float xOffset;
	public float yOffset;
	
	public float fireDuration = 0.5f;
	public float pauseFireDuration = 0.5f;
	
	
	// Use this for initialization
	void Start () 
	{
		
		if ( (firing || fireRightAway) && !disableGuns && gameObject.activeInHierarchy == true)
		{
			StartFiring();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
		Vector3 screenPoint = Camera.main.WorldToViewportPoint (transform.position);
		
		if (!onscreen)
		{
			if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1) {
				
				onscreen = true;
				
				if (Random.value <= chanceToFireOnscreen && !disableGuns && !firing){
					//subset of fighters start shooting onscreen
					StartFiring();
				}
				
			}
		}
	}
	
	public void StartFiring () 
	{
		disableGuns = false;
		Vector3 screenPoint = Camera.main.WorldToViewportPoint (transform.position);
		if (screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
		{
			if (gunSound != null)
			{
				Controller.current.audioSrc.PlayOneShot(gunSound, 0.25f);
			}
		}
		InvokeRepeating ("FireMachineGuns", initialFireDelay + initialFireDelay * Random.value, fireRate);
		Invoke ("PauseFiring", fireDuration);
	}
	
	public void StopFiring () 
	{
		firing = false;
		CancelInvoke ("FireMachineGuns");
	}
	
	public void PauseFiring () 
	{
		firing = false;
		CancelInvoke ("FireMachineGuns");
		Invoke ("StartFiring", pauseFireDuration);
	}
	
	void FireMachineGuns()
	{
		firing = true;
		GameObject obj;
		
		if (weaponType == WeaponType.Minigun)
		{
			obj = ObjectPoolerScript.current.GetPooledBullet();
		}
		else if (weaponType == WeaponType.Blaster)
		{
			obj = ObjectPoolerScript.current.GetPooledBullet_B();
		}
		else 
		{
			return;
		}
		
		float spawnX = this.gameObject.transform.position.x + xOffset + 0.4f * Random.value - 0.2f;
		float spawnY = this.gameObject.transform.position.y + yOffset + 0.4f * Random.value - 0.2f;	
		
		obj.transform.position = new Vector3(spawnX, spawnY, this.gameObject.transform.position.z);
		obj.transform.rotation = this.gameObject.transform.rotation;
		obj.SetActive(true);
		
	}
}

public enum WeaponType {
	Minigun, Blaster
}