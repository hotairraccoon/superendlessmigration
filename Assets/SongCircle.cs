using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SongCircle : MonoBehaviour {
	
	public float theta_scale = 0.5f;  //Set lower to add more points
	public int size;
	public LineRenderer lineRenderer;
	public float circleRadius = 0f;
	
	// Use this for initialization
	void Start () {
		float sizeValue = (2.0f * Mathf.PI) / theta_scale;
		size = (int)sizeValue; //Total number of points in circle.
		size++; 
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetColors(Color.white, Color.white);
		lineRenderer.SetWidth(0.025f, 0.025f);
		lineRenderer.SetVertexCount(size);
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Controller.current.flockBirds.Count > 0)
		{
			float x;
			float y;
			Vector3 pos;
			float theta = 0f;
			
			for(int i = 0; i < size; i++){          
				theta += (2.0f * Mathf.PI * theta_scale);
				x = circleRadius * Mathf.Cos(theta);
				y = circleRadius * Mathf.Sin(theta);
				x += Controller.current.flockBirds[0].transform.position.x;
				y += Controller.current.flockBirds[0].transform.position.y;
				
				pos = new Vector3(x, y, 0f);
				lineRenderer.SetPosition(i, pos);
			}
		
		}
	}
}