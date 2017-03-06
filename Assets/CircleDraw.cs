using UnityEngine;
using System.Collections;

public class CircleDraw : MonoBehaviour {
	
	public float radius = 4f;
	public Color lineColor = new Color(0,0,0,0.15f);
	
	float theta_scale = 0.01f;        //Set lower to add more points
	int size; //Total number of points in circle
	
	LineRenderer lineRenderer;
	
	// Use this for initialization
	void Awake () {
		
		float sizeValue = (2.0f * Mathf.PI) / theta_scale; 
		size = (int)sizeValue;
		size++;
		
		Color col1 = lineColor;
		Color col2 = lineColor;
		
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Particles/Multiply"));
		lineRenderer.SetColors(col1, col2);
		lineRenderer.SetWidth(0.02f, 0.02f); //thickness of line
		lineRenderer.SetVertexCount(size);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		Vector3 pos;
		float theta = 0f;
		
		for(int i = 0; i < size; i++){
			
			theta += (2.0f * Mathf.PI * theta_scale);
			
			float x = radius * Mathf.Cos(theta);
			float y = radius * Mathf.Sin(theta);
			
			x += gameObject.transform.position.x;
			y += gameObject.transform.position.y;
			pos = new Vector3(x, y, 0);
			lineRenderer.SetPosition(i, pos);

		}
	}
}
