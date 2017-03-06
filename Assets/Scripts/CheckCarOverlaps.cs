using UnityEngine;
using System.Collections;

public class CheckCarOverlaps : MonoBehaviour {
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter2D(Collider2D col) 
	{

		if (col.gameObject.tag == "Car") 
		{
			this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x + 1f, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
		}

	}


}
