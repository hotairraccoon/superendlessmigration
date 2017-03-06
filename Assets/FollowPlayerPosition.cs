using UnityEngine;
using System.Collections;

public class FollowPlayerPosition : MonoBehaviour {

	public bool isFlockLeader;
	public GameObject playerPosition;
	public Rigidbody2D playerPosition_rb2d;

	// Use this for initialization
	void Start () {
		playerPosition = GameObject.Find("/PlayerPosition");
		playerPosition_rb2d = playerPosition.GetComponent<Rigidbody2D>();

		isFlockLeader = (transform == Controller.current.getFlockLeader());
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (isFlockLeader && playerPosition != null){
			transform.position = playerPosition.transform.position;
		}
	}

	void OnCollisionEnter2D(Collision2D collision2d) {
		playerPosition_rb2d.velocity = new Vector2(-playerPosition_rb2d.velocity.x/2, playerPosition_rb2d.velocity.y/2);
	}
}
