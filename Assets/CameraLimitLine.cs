using UnityEngine;
using System.Collections;

public class CameraLimitLine : MonoBehaviour {
	
	public Side side = Side.Left;
	public SpriteRenderer renderer;
	// Use this for initialization
	void Start () {
		renderer = gameObject.GetComponent<SpriteRenderer>();
		renderer.color = new Color(1f,1f,1f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		
		// x limits should relate to CameraFollow x limits
		if (transform.position.x < -13f && side == Side.Left){
			renderer.color = new Color(1f,1f,1f, 0.5f);
		} else if (transform.position.x > 13f && side == Side.Right){
			renderer.color = new Color(1f,1f,1f, 0.5f);
		} else {
			renderer.color = new Color(1f,1f,1f, 0f);
		}
	}
}

public enum Side {
	Left, Right
}

