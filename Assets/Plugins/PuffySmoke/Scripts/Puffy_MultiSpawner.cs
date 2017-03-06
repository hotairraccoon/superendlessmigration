using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Puffy_MultiSpawner : MonoBehaviour {
	
	
	protected Transform _transform;
	
	public Puffy_Emitter emitter;
	public List<Puffy_ParticleSpawner> spawnerList = new List<Puffy_ParticleSpawner>();
	
	
	// Use this for initialization
	void Awake() {
		_transform = transform;
		if(!emitter) emitter = gameObject.GetComponent<Puffy_Emitter>();
		if(emitter) emitter.multiSpawner = this;
	}
	
	public virtual void Init(){
		
	}
	
	public Puffy_ParticleSpawner MakeSpawner(GameObject gameObject, Vector3 position , Vector3 direction = default(Vector3)){
		
		Puffy_ParticleSpawner spawner = gameObject.GetComponent<Puffy_ParticleSpawner>();
		if(!spawner) spawner = gameObject.AddComponent<Puffy_ParticleSpawner>() as Puffy_ParticleSpawner;
		
		gameObject.transform.position = position;
		spawner.emitter = emitter;
		spawner.InitSpawnPoint(gameObject.transform.position,direction);
		
		spawnerList.Add(spawner);
		
		return spawner;
	} 
	
	public Puffy_ParticleSpawner CreateSpawner(Vector3 position,Vector3 direction = default(Vector3)){
		GameObject go = new GameObject();
		return MakeSpawner(go,position,direction);
	} 
	
	public void DoUpdate(float deltaTime){
	
		Vector3 spawnPosition,offset,offsetStep = Vector3.zero;
		Vector3 spawnDirection,dir,dirStep = Vector3.zero;
		float spawnSpeed,age = 0f;
		//float spd,spdStep = 0f;
		
		int j,index,localSubCounter = 0;
		float step = 1f / emitter.spawnRate;
		float subParticlesRatio = emitter.subParticlesRatio;
		int subParticlesCounter = emitter.subParticlesCounter;
		int subParticlesCount = emitter.subParticlesCount;
		
		
		float ratio = deltaTime / step;
		
		int spawnCount = Mathf.FloorToInt(ratio);
		
		float lifeTime = emitter.lifeTime;
		float startSize = emitter.startSize;
		float endSize = emitter.endSize;
		Color startColor = emitter.startColor;
		Color endColor = emitter.endColor;
		Puffy_ParticleData particle;
		float maxParticlesDistance = emitter.maxParticlesDistance;
		
		float localRatio,localStep;
		int localSpawnCount;
		
		spawnSpeed = emitter.startSpeed;
		
		foreach(Puffy_ParticleSpawner spawner in spawnerList){
			localSubCounter = subParticlesCounter;
			
			if(spawner.enabled){
				
				localRatio = ratio;
				localSpawnCount = spawnCount;
				localStep = step;
				
				age = deltaTime;
				
				offset = spawner.spawnPosition - spawner.lastPosition;
				offsetStep = offset / localRatio;
				
				if(maxParticlesDistance > 0){
					float magnitude = offsetStep.magnitude;
					if(magnitude > maxParticlesDistance){
						localRatio = offset.magnitude / maxParticlesDistance;
						localSpawnCount = Mathf.FloorToInt(localRatio);
						if(localSpawnCount < spawnCount * 10){
							offsetStep = offset.normalized * maxParticlesDistance;
							localStep = (spawnCount * step) / localSpawnCount;
						}else{
							localRatio = ratio;
							localSpawnCount = spawnCount;
						}
					}
				}
				
				spawnPosition = spawner.lastPosition;
				
				dir = spawner.particleDirection - spawner.lastParticleDirection;
				dirStep = dir / localRatio;
				spawnDirection = spawner.lastParticleDirection;
				
				/*
				spd = spawner.particleSpeed - spawner.lastParticleSpeed;
				spdStep = spd / localRatio;
				spawnSpeed = spawner.lastParticleSpeed;
				*/
				
				for(j = 0; j < localSpawnCount; j++){
					
					spawnPosition += offsetStep;
					spawnDirection += dirStep;
					//spawnSpeed += spdStep;
					
					index = emitter.SpawnParticle(spawnPosition, spawnDirection, spawnSpeed, lifeTime, startSize, endSize, startColor, endColor, age);
					
					age -= localStep;
					
					
					
					if(index>=0 && subParticlesCount>0){
							
						particle = emitter.particles[index];
						
						if(localSubCounter < subParticlesCount){
							
							particle.startLifetime *= subParticlesRatio;
							particle.endSize *= subParticlesRatio;
					
							if(emitter.debugIntermediate){
								particle.startColor = Color.yellow;
								particle.endColor = Color.yellow;
							}
							
						}else{
							if(emitter.debugIntermediate){
								particle.startColor = Color.magenta;
								particle.endColor = Color.magenta;
							}
							localSubCounter = 0;
						}
						
						localSubCounter++;
					}
									
				}
				
				spawner.lastPosition = spawnPosition;
				spawner.lastParticleDirection = spawnDirection;
//				spawner.lastParticleSpeed = spawnSpeed;
				
			}
		}
		emitter.subParticlesCounter = localSubCounter;
		particle = null;
	}
	
}