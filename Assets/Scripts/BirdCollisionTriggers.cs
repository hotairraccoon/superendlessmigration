using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BirdCollisionTriggers : MonoBehaviour {

	public int featherCount = 8;
	public List<string> featherTypes;
	
	public Bird birdComponent;
	
	void Start()
	{	
		//birdComponent = gameObject.GetComponent<Bird>();
		transform.rotation = transform.localRotation;
		
	}
	
	void Update()
	{
		if (birdComponent.poisoned)
		{
			birdComponent.renderer.material.SetColor("_Color", Color.green);
		}
		else
		{
			birdComponent.renderer.material.SetColor("_Color", Color.white);
		}
	}
	
	void OnTriggerEnter2D(Collider2D col) 
	{	
		if (!Controller.current.flockDodging)
		{
			if (col.gameObject.tag == "Aircraft")
			{
				ActivateFeatherPop(gameObject);
			} 
			else if (col.gameObject.tag == "Gunfire" && col.transform.parent.GetComponent<Dogfight>().firing == true)
			{
				ActivateFeatherPop(gameObject);
			} 
			else if (col.gameObject.tag == "Bullet") 
			{
				ActivateFeatherPop(gameObject);
			} 
			else if (col.gameObject.tag == "Wall")
			{
				ActivateFeatherPop(gameObject);
			} 
			else if (col.gameObject.tag == "CaveWall")
			{
				if (col.transform.parent.transform.parent.GetComponent<CaveFadeTrigger>().caveActivated == true)
				{
					ActivateFeatherPop(gameObject);
				}
			}
		}
		
		if (col.gameObject.tag == "Predator" && col.GetComponent<Predator>().attackActivated == false && (birdComponent.inFlock == true))
		{
			col.GetComponent<Predator>().ActivateAttack(gameObject);
		} 
		
		if (col.gameObject.tag == "PredatorBeak" && (birdComponent.inFlock == true ||
		 	(Camera.main.WorldToViewportPoint(gameObject.transform.position).x > 0 
		 	&& Camera.main.WorldToViewportPoint(gameObject.transform.position).x < 1
		 	&& Camera.main.WorldToViewportPoint(gameObject.transform.position).y > 0
		 	&& Camera.main.WorldToViewportPoint(gameObject.transform.position).y < 1)))
		{
			if (!Controller.current.flockDodging)
			{
				col.transform.parent.GetComponent<Predator>().EatBird(gameObject);
				ActivateFeatherPop(gameObject);
			}
		} 
		else if (col.gameObject.tag == "PredatorStrikeRadius" && col.transform.parent.GetComponent<Predator>().strikeBoostReady == true && (birdComponent.inFlock == true))
		{
			col.transform.parent.GetComponent<Predator>().ActivateStrikeBoost();
		}
		
		if (col.gameObject.tag == "CaveEntrance" && birdComponent.inFlock == true)
		{
			col.transform.parent.GetComponent<CaveFadeTrigger>().caveActivated = true;
		} 
		
		if (col.gameObject.tag == "BoulderSpawner")
		{
			//stop boulders
			col.gameObject.GetComponent<BoulderSpawner>().finished = true;
		}
		
	}
	
	void OnTriggerStay2D(Collider2D col) 
	{
		if (col.gameObject.tag == "PoisonGas")
		{
			birdComponent.hitPoints -= 75f * Time.deltaTime;
			birdComponent.poisoned = true;
			
			if (birdComponent.hitPoints <= 0f && !Controller.current.flockDodging)
			{
				ActivateFeatherPop(gameObject);
			}
		}
	}
	
	void OnTriggerExit2D(Collider2D col) 
	{
		if (col.gameObject.tag == "PoisonGas")
		{
			birdComponent.poisoned = false;
		}
	}

	void ActivateFeatherPop(GameObject colObj)
	{	
		
		for (int i = 0; i < featherCount; i++)
		{
			PopFeather(colObj);
		}
		
		if (gameObject.GetComponent<Bird>().inFlock)
		{
			Controller.current.GetComponent<AudioSource>().PlayOneShot(this.gameObject.GetComponent<Bird>().deathSounds_inFlock[Random.Range(0,this.gameObject.GetComponent<Bird>().deathSounds_inFlock.Count)], 0.5f);
			Controller.current.flockBirds.Remove (gameObject);
			gameObject.GetComponent<Bird>().inFlock = false;
		} 
		else if ((Camera.main.WorldToViewportPoint(gameObject.transform.position).x > 0 
			     && Camera.main.WorldToViewportPoint(gameObject.transform.position).x < 1
			     && Camera.main.WorldToViewportPoint(gameObject.transform.position).y > 0
			     && Camera.main.WorldToViewportPoint(gameObject.transform.position).y < 1))
		{ 
			//only play sound for birds that die onscreen
			Controller.current.GetComponent<AudioSource>().PlayOneShot(this.gameObject.GetComponent<Bird>().deathSounds[Random.Range(0,this.gameObject.GetComponent<Bird>().deathSounds.Count)], 0.5f);
		}
		
		Controller.current.UpdateFlockPositions();
		gameObject.SetActive (false);

	}
	
	void PopFeather(GameObject colObj)
	{
		GameObject obj = ObjectPoolerScript.current.GetPooledFeather();
		
		if (obj == null)
		{
			return;
		}
		
		obj.transform.position = this.gameObject.transform.position;
		obj.GetComponent<Feather>().popRotation = colObj.transform.rotation;
		
		int activeFeatherIndex = Random.Range(0,featherTypes.Count);
		obj.GetComponent<Feather>().activeFeatherSprite = obj.transform.Find(featherTypes[activeFeatherIndex]).gameObject;
		obj.GetComponent<Feather>().activeFeatherSprite.transform.rotation = Quaternion.Euler(0,0, 360 * Random.value);
		obj.GetComponent<Feather>().activeFeatherSprite.SetActive(true);
		obj.SetActive(true);

	}
}
