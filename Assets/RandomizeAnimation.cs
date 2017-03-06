using UnityEngine;
using System.Collections;

public class RandomizeAnimation : MonoBehaviour {
	
	public Animator anim;
	
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		anim.Play(0, -1, Random.value);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
