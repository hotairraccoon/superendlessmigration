using UnityEngine;
using System.Collections;

public class Puffy_ShapeSpawner : MonoBehaviour {
	
	public Puffy_Emitter emitter;

	protected Transform _transform;
	
	[HideInInspector]
	public Vector3[] vertices;
	
	[HideInInspector]
	public Color[] colors;
	
	[HideInInspector]
	public Vector3[] lastSpawnPosition;
	
	[HideInInspector]
	public Vector3[] lastSpawnDirection;
	
	[HideInInspector]
	public	Vector3[] normals;
	
	public float particleSpeed = 0f;
	private float lastParticleSpeed = 0f;
	
	public int emitEveryNthPoint = 1;
			
	void Awake() {
		_transform = transform;
		if(emitter == null) emitter = gameObject.GetComponent<Puffy_Emitter>();
		if(emitter != null) emitter.shapeSpawner = this;
		
		vertices = new Vector3[1];
		normals = new Vector3[1];
		colors = new Color[1];
		lastSpawnPosition = new Vector3[1];
		lastSpawnDirection = new Vector3[1];
		
		vertices[0] = Vector3.zero;
		normals[0]=Vector3.zero;
		colors[0]=Color.white;
		lastSpawnPosition[0] = Vector3.zero;
		lastSpawnDirection[0] = Vector3.zero;
	}
	
	public virtual void Init(){
		
	}
	
	public void DoUpdate(float deltaTime){
		
		int count = vertices.Length;
		
		Vector3 transformedPosition = Vector3.zero;
		Vector3 transformedDirection = Vector3.zero;
		
		Vector3 spawnPosition,offset,offsetStep = Vector3.zero;
		Vector3 spawnDirection,dir,dirStep = Vector3.zero;
		float spawnSpeed,spd,spdStep,age = 0f;
		int i,j,index,localSubCounter = 0;
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
		Puffy_Emitter.colorModes colorMode = emitter.colorMode;
		Puffy_ParticleData particle;
		float maxParticlesDistance = emitter.maxParticlesDistance;
		
		float localRatio,localStep;
		int localSpawnCount;
				
		for(i=0; i < count; i+= emitEveryNthPoint){
			localSubCounter = subParticlesCounter;
			localRatio = ratio;
			localSpawnCount = spawnCount;
			localStep = step;
			if(true){
				
				transformedPosition = _transform.TransformPoint(vertices[i]);
				transformedDirection = _transform.TransformDirection(normals[i]);
				
				age = deltaTime;
				
				offset = transformedPosition - lastSpawnPosition[i];
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
				
				spawnPosition = lastSpawnPosition[i];
				
				dir = transformedDirection - lastSpawnDirection[i];
				dirStep = dir / localRatio;
				spawnDirection = lastSpawnDirection[i];
				
				spd = particleSpeed - lastParticleSpeed;
				spdStep = spd / localRatio;
				spawnSpeed = lastParticleSpeed;
															
				for(j = 0; j < localSpawnCount; j++){
					
					spawnPosition += offsetStep;
					spawnDirection += dirStep;
					spawnSpeed += spdStep;
					
					index = -1;
					switch(colorMode){
						case Puffy_Emitter.colorModes.Basic:	
						case Puffy_Emitter.colorModes.Gradient:
							index = emitter.SpawnParticle(spawnPosition, spawnDirection, spawnSpeed, lifeTime, startSize, endSize, startColor, endColor, age);
						break;
						
						case Puffy_Emitter.colorModes.Mesh:
							index = emitter.SpawnParticle(spawnPosition, spawnDirection, spawnSpeed, lifeTime, startSize, endSize, colors[i], colors[i], age);
						break;
					}
					
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
				
				lastSpawnPosition[i] = spawnPosition;
				lastSpawnDirection[i] = spawnDirection;
				lastParticleSpeed = spawnSpeed;
				
			}
		}
		emitter.subParticlesCounter = localSubCounter;
		particle = null;
		
	}
}
