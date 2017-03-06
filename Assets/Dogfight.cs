using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dogfight : MonoBehaviour {
	
	public List<GameObject> gunFire;
	public List<GameObject> muzzleFlashes;
	public bool firing = false;
	public bool disableGuns = false;
	
	public List<GameObject> shotDownFire;
	public bool shotDown = false;
	public bool randomlyShotDown = true;
	
	public bool onscreen = false;
	
	public string country = "";
	
	public List<GameObject> objectsToHideOnShotDown;
	
	public bool destroyAfterShotDown = false;
	public float destroyTime = 0.5f;
	
	public float fireRate = 0.125f;
	
	public GunFireType gunFireType;
	
	public bool immuneToBullets = false;
	
	public AudioClip gunSound;
	
	void Start () 
	{

		if (shotDownFire.Count > 0){
			for (int i = 0; i < shotDownFire.Count; i++){
				shotDownFire[i].SetActive(false);
			}
		}
		
		if (gunFire.Count > 0){
			for (int i = 0; i < gunFire.Count; i++){
				gunFire[i].SetActive(false);
			}
		}
		
		destroyTime = destroyTime + Random.value * destroyTime;
		
		for (int i = muzzleFlashes.Count - 1; i >= 0; i--){
			muzzleFlashes[i].SetActive (false);
		}
		
		if (randomlyShotDown){
			if (Random.value <= 0.25f){
				GotShotDown();
			}
		}	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 screenPoint = Camera.main.WorldToViewportPoint (transform.position);
		if (!onscreen && !shotDown)
		{
			if (screenPoint.z > 0f && screenPoint.x > 0f && screenPoint.x < 1f && screenPoint.y > 0f && screenPoint.y < 1f)
			{
				onscreen = true;
				if (Random.value <= 0.65f && disableGuns == false)
				{
					//subset of fighters start shooting onscreen
					StartFiring();
				}
			}
		}
	}
		
	void StartFiring ()
	{
		InvokeRepeating ("FireGuns", 0.25f + 0.75f * Random.value, fireRate);
	}
	
	void GotShotDown ()
	{
	
		shotDown = true;
		firing = false;
		
		if (shotDownFire.Count > 0){
			for (int i = 0; i < shotDownFire.Count; i++){
				shotDownFire[i].SetActive(true);
			}
		}
		if (gunFire.Count > 0){
			for (int i = 0; i < gunFire.Count; i++){
				gunFire[i].SetActive(false);
			}
		}
		
		if (objectsToHideOnShotDown.Count > 0){
			for (int i = 0; i < objectsToHideOnShotDown.Count; i++){
				objectsToHideOnShotDown[i].SetActive(false);
			}
		}
		
		if (destroyAfterShotDown == true){
			Invoke ("DestroyAfterShotDown", destroyTime);
		}
	}
	
	void FireGuns () 
	{
		
		switch (gunFireType) 
		{
			case GunFireType.LaserLine:
				// laser line guns hit everything they touch
				if (shotDown == false && gunFire.Count > 0){
					if (firing == true) {
						firing = false;
						
						for (int i = muzzleFlashes.Count - 1; i >= 0; i--){
							muzzleFlashes[i].SetActive (false);
						}
						
						if (gunFire.Count > 0){
							for (int i = 0; i < gunFire.Count; i++){
								gunFire[i].SetActive(false);
							}
						}
						
					} else {
						firing = true;
						Vector3 screenPoint = Camera.main.WorldToViewportPoint (transform.position);
						if (screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1) {
							if (gunSound != null)
							{
								Controller.current.audioSrc.PlayOneShot(gunSound, 0.125f);
							}
						}
						if (gunFire.Count > 0){
							for (int i = 0; i < gunFire.Count; i++){
								gunFire[i].SetActive(true);
							}
						}
						int randomFlash = (int)((muzzleFlashes.Count - 1) * Random.value);
						muzzleFlashes[randomFlash].SetActive(true);
					}
				}
				break;
			case GunFireType.BulletBurst:
				// bullet burst guns fire lots of projectiles
				FireBurstBullet();
				break;
			default:
				break;
		}
	}
	
	void FireBurstBullet() {
		// handled by new MachineGunFire component
	}
	
	void OnTriggerEnter2D(Collider2D col) 
	{
		
		if (col.gameObject.tag == "Gunfire" && country != col.transform.parent.GetComponent<Dogfight>().country && col.transform.parent.GetComponent<Dogfight>().firing == true){
			GotShotDown ();
		} else if (col.gameObject.tag == "Bullet" && !immuneToBullets){
//			GameObject bullet = col.gameObject;
//			Invoke ("RemoveBullet", 0.1f);
			GotShotDown ();
		}
		
	}
	
	void DestroyAfterShotDown() {
		if (this.gameObject.GetComponent<Aircraft> () != null){
			Controller.current.allAircraft.Remove (this.gameObject);
		}
		
		this.gameObject.SetActive (false);
		Destroy (this.gameObject);
	}

}

public enum GunFireType {
	LaserLine, BulletBurst
}
