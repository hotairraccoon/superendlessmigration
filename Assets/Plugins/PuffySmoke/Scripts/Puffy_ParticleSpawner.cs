using UnityEngine;
using System.Collections;

public class Puffy_ParticleSpawner : MonoBehaviour {

	protected Transform _transform = null;
	
	[HideInInspector]
	public Puffy_Emitter emitter;
	
	[HideInInspector]
	public Vector3 spawnPosition = Vector3.zero;
	
	[HideInInspector]
	public Vector3 particleDirection = Vector3.zero;
	
	/*
	[HideInInspector]
	public float particleSpeed = 0f;
	
	[HideInInspector]
	public float lastParticleSpeed = 0f;
	*/
	
	[HideInInspector]
	public Vector3 localParticleDirection = Vector3.zero;
	
	
	[HideInInspector]
	public Vector3 lastPosition = Vector3.zero;
	
	[HideInInspector]
	public Vector3 lastParticleDirection = Vector3.zero;
					
	void Awake(){
		_transform = transform;
		spawnPosition = _transform.position;
		lastPosition = _transform.position;
		lastParticleDirection = particleDirection;
		
		
		if(emitter != null) localParticleDirection = emitter.startDirection;
	}
	
	public void InitSpawnPoint(Vector3 pos, Vector3 dir){ //, float spd = 0f){
		
		if(emitter != null) localParticleDirection = emitter.startDirection;
		
		spawnPosition = pos;
		lastPosition = pos;
		
		particleDirection = dir;
		lastParticleDirection = dir;
		/*
		particleSpeed = spd;
		lastParticleSpeed = spd;
		*/
	}
	
	public void UpdateSpawnPoint(Vector3 pos){
		spawnPosition = pos;
	}
	
	public void UpdateSpawnPoint(Vector3 pos, Vector3 dir){
		spawnPosition = pos;
		particleDirection = dir;
	}
	/*
	public void UpdateSpawnPoint(Vector3 pos, Vector3 dir, float spd){
		spawnPosition = pos;
		particleDirection = dir;
		particleSpeed = spd;
	}
	*/
	void Update(){
		if(emitter != null) localParticleDirection = emitter.startDirection;
		spawnPosition = _transform.position;
		particleDirection = _transform.TransformDirection(localParticleDirection);
	}
	
}
