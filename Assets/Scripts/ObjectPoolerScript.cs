using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolerScript : MonoBehaviour {
	
	public static ObjectPoolerScript current;
	public GameObject pooledObject;
	public int pooledMaxCount = 25;
	
	public GameObject pooledFeather;
	public int pooledFeatherMaxCount = 25;
	
	public GameObject pooledBullet;
	public GameObject pooledBullet_B;
	public int pooledBulletMaxCount = 25;
	
	public bool growPooledObjects = true;
	public int growAbsoluteMaxCount = 100;
	
	List<GameObject> pooledObjects;
	List<GameObject> pooledFeathers;
	List<GameObject> pooledBullets;
	
	List<GameObject> pooledBullets_B;
	
	
	void Awake ()
	{
		current = this;
	}
	
	// Use this for initialization
	void Start () {
		pooledObjects = new List<GameObject>();
		pooledFeathers = new List<GameObject>();
		pooledBullets = new List<GameObject>();
		pooledBullets_B = new List<GameObject>();
		
		for (int i = 0; i < pooledMaxCount; i++)
		{
			GameObject obj = (GameObject)Instantiate(pooledObject);
			obj.SetActive(false);
			pooledObjects.Add(obj);
		}
		
		for (int i = 0; i < pooledFeatherMaxCount; i++)
		{
			GameObject obj = (GameObject)Instantiate(pooledFeather);
			obj.SetActive(false);
			pooledFeathers.Add(obj);
		}
		
		for (int i = 0; i < pooledBulletMaxCount; i++)
		{
			GameObject obj = (GameObject)Instantiate(pooledBullet);
			obj.SetActive(false);
			pooledBullets.Add(obj);
		}
		
		for (int i = 0; i < pooledBulletMaxCount; i++)
		{
			GameObject obj = (GameObject)Instantiate(pooledBullet_B);
			obj.SetActive(false);
			pooledBullets_B.Add(obj);
		}
	}
	
	public GameObject GetPooledObject()
	{
		for (int i = 0; i < pooledObjects.Count; i++)
		{
			if(!pooledObjects[i].activeInHierarchy)
			{
				return pooledObjects[i];
			}
		} 
		
		if (growPooledObjects && pooledObjects.Count < growAbsoluteMaxCount)
		{
			GameObject obj = (GameObject)Instantiate(pooledObject);
			pooledObjects.Add(obj);
			//let the object that requests this object handle activation
			return obj;
		}
		
		return null;
	}
	
	public GameObject GetPooledFeather()
	{
		for (int i = 0; i < pooledFeathers.Count; i++)
		{
			if(!pooledFeathers[i].activeInHierarchy)
			{
				return pooledFeathers[i];
			}
		} 
		
		if (growPooledObjects && pooledFeathers.Count < growAbsoluteMaxCount)
		{
			GameObject obj = (GameObject)Instantiate(pooledFeather);
			pooledFeathers.Add(obj);
			//let the object that requests this object handle activation
			return obj;
		}
		
		return null;
	}
	
	public GameObject GetPooledBullet()
	{
		for (int i = 0; i < pooledBullets.Count; i++)
		{
			if(!pooledBullets[i].activeInHierarchy)
			{
				return pooledBullets[i];
			}
		} 
		
		if (growPooledObjects && pooledBullets.Count < growAbsoluteMaxCount)
		{
			GameObject obj = (GameObject)Instantiate(pooledBullet);
			pooledBullets.Add(obj);
			//let the object that requests this object handle activation
			return obj;
		}
		
		return null;
	}
	
	public GameObject GetPooledBullet_B()
	{
		for (int i = 0; i < pooledBullets_B.Count; i++)
		{
			if(!pooledBullets_B[i].activeInHierarchy)
			{
				return pooledBullets_B[i];
			}
		} 
		
		if (growPooledObjects && pooledBullets_B.Count < growAbsoluteMaxCount)
		{
			GameObject obj = (GameObject)Instantiate(pooledBullet_B);
			pooledBullets_B.Add(obj);
			//let the object that requests this object handle activation
			return obj;
		}
		
		return null;
	}
	
	
}
