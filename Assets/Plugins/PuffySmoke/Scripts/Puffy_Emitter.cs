using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Puffy_Emitter : MonoBehaviour {
	public static bool globalFreeze = false;
	
	public Puffy_Renderer puffyRenderer = null;
	
	public bool autoEmit = true;
	
	public bool autoAssign = true;
	public string autoRendererName = "";
	
	public bool freezed = false;
	
	public float lifeTime = 5f;
	
	public float lifeTimeVariation = 0f;
	
	public Vector3 positionVariation = Vector3.zero;
	
	public Vector3 startDirection = Vector3.up;
	
	public Vector3 startDirectionVariation = new Vector3(0.01f,0.01f,0.01f);
	
	public float startSize = 0.5f;
	
	public float endSize = 2f;
	
	public float startSizeVariation = 0f;
	
	public float endSizeVariation = 0f;
	
	public float startSpeed = 1f;
	
	public float startSpeedVariation = 0f;
	
	public Color startColor = Color.white;
	
	public Color endColor = Color.white;
	
	public Color startColorVariation = Color.black;
	
	public Color endColorVariation = Color.black;
	
	public Puffy_Gradient colorGradient = null;
	public float colorGradientEndTime = 1f;
	
	public float maxParticlesDistance = 0.0f;

	public enum colorModes{
		Basic,
		Mesh,
		Gradient
	}
	
	public colorModes colorMode = colorModes.Basic;
	
	public bool useLuminosity = true;
	public AnimationCurve luminosityCurve = new AnimationCurve(new Keyframe(0, 0),new Keyframe(1, 0));
	
	public bool autoResize = true;
	
	public int chunkSize = 512;
	
	public float spawnRate = 25;
	
	public bool useThread = true;
	
	public bool debugIntermediate = false;
	
	public bool trailMode = true;
	public float trailStepDistance = 0.1f;
	
	public bool autoTrailStep = true;
	public float autoTrailStepFactor = 0.3f;
	public float autoTrailStepRatio = 0.5f;
	
	[System.NonSerialized]
	public Puffy_ParticleData[] particles;
		
	[System.NonSerialized]
	public double debugTime = 0;
	
	[System.NonSerialized]
	public int particleTotal = 0;
	
	[System.NonSerialized]
	public int particleLive = 0;

	[System.NonSerialized]
	public int[] particlesPointers;
	
	private int liveParticleCount = 0;
	private int _particleCount = 512;

	private Vector3 lastSpawnPosition = Vector3.zero;
	private Vector3 lastSpawnDirection = Vector3.zero;
	private float lastStartSpeed = 0f;
	
	private float _deltaTime;
	private int _coresCount = 1;
	private List<PointerGroup> deadGroups = new List<PointerGroup>();
	private Transform _transform;

	private float elapsedAccumulation = 0;

	public int subParticlesCount = 0;
	public int subParticlesCounter = 0;
	public float subParticlesRatio = 0.5f;
	
	private bool wasStopped = false;
	
	public Puffy_ShapeSpawner shapeSpawner = null;
	public Puffy_MultiSpawner multiSpawner = null;
	
	public bool hasGradient = false;
	
	void Start(){
				
		_transform = transform;

		lastSpawnPosition = _transform.position;
		
		_coresCount = SystemInfo.processorCount;
		
		_particleCount = chunkSize;
		
		particleTotal = _particleCount;
		particles = new Puffy_ParticleData[_particleCount];
		particlesPointers = new int[_particleCount];
		
		Clear();
		
		if(colorMode == colorModes.Gradient && GetComponent<Puffy_Gradient>() == null){
			colorGradient = (gameObject.AddComponent<Puffy_Gradient>() as Puffy_Gradient);
			hasGradient = colorGradient.enabled;
		}
		
		if(autoAssign){
			if(puffyRenderer == null){
				Puffy_Renderer renderer = null;
				
				if(autoRendererName == ""){
					renderer = Puffy_Renderer.GetRenderer();
				}else{
					renderer = Puffy_Renderer.GetRenderer(autoRendererName);
				}
				
				if(renderer == null){
					
					renderer = Puffy_Renderer.GetRenderer();
					if(renderer){
						Debug.LogWarning("Can't find a PuffyRenderer gameobject with the name '"+autoRendererName+"' , the first renderer found is picked instead");
					}else{
						Debug.LogWarning("Can't find any PuffyRenderer");
					}
					
				}
				if(renderer) renderer.AddEmitter(this);
				puffyRenderer = renderer;
			}
		}
		
	}
	
	public void Clear(bool reset = false){
		int i;
		if(reset) Resize (chunkSize*2);
		for(i=0; i<_particleCount; i++){
			particlesPointers[i] = i;
			if(particles[i] == null) particles[i] = new Puffy_ParticleData();
			particles[i].startLifetime = 0f;
			particles[i].lifetime = 0f;
			particles[i].dead = true;
		}
				
		liveParticleCount = 0;
		particleLive = 0;
	}
	
	bool waitingForLastParticle = false;
	public void Kill(){
		waitingForLastParticle = true;
	}
	
	public void Resurrect(){
		waitingForLastParticle = false;
		enabled = true;
	}
	
	bool doFreeze = false;
	
	void LateUpdate () {
		
		if(colorGradient != null) hasGradient = colorGradient.enabled;
		
		if(!doFreeze){
			debugTime = Time.realtimeSinceStartup;

			if(!waitingForLastParticle){
				
				
				if(autoEmit && Time.timeScale > 0){	
					float elapsed = Time.deltaTime;
					float step = 1f / spawnRate;
					
					elapsedAccumulation += elapsed;
					
					if(elapsedAccumulation >= step){
						
						float ratio = elapsedAccumulation/step;
						int spawnCount = Mathf.FloorToInt(ratio);
						
						float difference = step * spawnCount;
						
						if(multiSpawner){
							if(multiSpawner.enabled){
								multiSpawner.DoUpdate(elapsedAccumulation);
							}
						}
					
						if(shapeSpawner){
							if(shapeSpawner.enabled){

								if(wasStopped) shapeSpawner.Init();
								
								shapeSpawner.DoUpdate(elapsedAccumulation);
							}
						}
						
						if(!shapeSpawner && !multiSpawner){
							Vector3 spawnPosition,offset,offsetStep = Vector3.zero;
							Vector3 spawnDirection,dir,dirStep = Vector3.zero;
							float spawnSpeed,spd,spdStep = 0f;
							
							int j,index = 0;
							
							Vector3 transformedPosition = _transform.position;
							Vector3 transformedDirection = _transform.TransformDirection(startDirection);
							
							Puffy_ParticleData particle;
							
							if(wasStopped){
								lastSpawnPosition = transformedPosition;
								lastSpawnDirection = transformedDirection;
								lastStartSpeed = startSpeed;
							}
							
							float localRatio,localStep;
							int localSpawnCount;
							
							float age = elapsedAccumulation;
							
							localRatio = ratio;
							localSpawnCount = spawnCount;
							localStep = step;
							
							offset = transformedPosition - lastSpawnPosition;
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
							
							spawnPosition = lastSpawnPosition;
							
							dir = transformedDirection - lastSpawnDirection;
							dirStep = dir / localRatio;
							spawnDirection = lastSpawnDirection;
							
							spd = startSpeed - lastStartSpeed;
							spdStep = spd / localRatio;
							spawnSpeed = lastStartSpeed;
							
							for(j=0;j<localSpawnCount;j++){
								spawnPosition += offsetStep;
								spawnDirection += dirStep;
								spawnSpeed += spdStep;
								
								index = SpawnParticle(spawnPosition, spawnDirection, spawnSpeed, lifeTime, startSize, endSize, startColor, endColor, age);
			
								age -= localStep;
								
								if(index>=0 && subParticlesCount>0){
									
									particle = particles[index];
									
									if(subParticlesCounter < subParticlesCount){

										particle.startLifetime *= subParticlesRatio;
										particle.endSize *= subParticlesRatio;
									
										if(debugIntermediate){
											particle.startColor = Color.yellow;
											particle.endColor = Color.yellow;
										}
										
									}else{
										if(debugIntermediate){
											particle.startColor = Color.magenta;
											particle.endColor = Color.magenta;
										}
										subParticlesCounter = 0;
									}
									
									subParticlesCounter++;
								}
								
							}
						
							lastSpawnPosition = spawnPosition;
							lastSpawnDirection = spawnDirection;
							lastStartSpeed = spawnSpeed;
						}
					
						elapsedAccumulation -= difference;
						
					}
		
					wasStopped = false;
				}else{
										
					wasStopped = true;
				}
				
			}else if(waitingForLastParticle && liveParticleCount == 0){
				// kill only when no particle is left
				Clear();
				enabled = false;
				debugTime = Time.realtimeSinceStartup - debugTime;
				return;
			}
			
			UpdateParticles();
			
			debugTime = Time.realtimeSinceStartup - debugTime;
		}else{
			
			wasStopped = true;
		}
		
		doFreeze = (freezed || globalFreeze);
	}
		
	void UpdateParticles(){
		
		if(liveParticleCount > 0){
			
			while(deadGroups.Count<_coresCount){
				deadGroups.Add(new PointerGroup());					
			}
			
			_deltaTime = Time.deltaTime;
			
			int count;
			int i;
			int j;
			if(liveParticleCount < _coresCount*10 || _coresCount == 1 || !useThread){
				UpdateParticlesTask(0,0,liveParticleCount);
			}else{
				
				int stp = Mathf.CeilToInt((float)liveParticleCount / _coresCount);
				List<UnityThreading.Task> taskList = new List<UnityThreading.Task>();
				
				j = 0;
				for(i=0;i<liveParticleCount;i+=stp){
					var end = Mathf.Min (i+stp,liveParticleCount);
					var start = i;
					var grp = j;
					taskList.Add(UnityThreadHelper.TaskDistributor.Dispatch(() => UpdateParticlesTask(grp,start,end)));
					j++;
				}
				count = taskList.Count;
				for(i=0;i<count;i++){
					taskList[i].Wait();
					taskList[i].Dispose();
				}
		
				taskList.Clear();
			}
			
			
			int groupCount = deadGroups.Count;
			int g;
			count = 0;
			for(g = 0 ; g < groupCount ; g++){
				count += deadGroups[g].count;
			}
			
			
			if(count > 0){
				
				if(count != liveParticleCount){
					int index;
					int pointer;
					
					for(g = 0; g < groupCount; g++){
						count = deadGroups[g].count;
						
						for(i = 0 ; i < count ; i++){
							pointer = deadGroups[g].pointers[i];
							index = particlesPointers[pointer];
							
							liveParticleCount--;
						
							particlesPointers[pointer] = particlesPointers[liveParticleCount];
							particlesPointers[liveParticleCount] = index;
						}
						
						deadGroups[g].count = 0;
					}
				}
				particleLive = liveParticleCount;
				
			}
		}
	}
	
	//void OnGUI(){
		/*
		string str = "";
		for(int i=0;i < particlesPointers.Length;i++){
			if(particles[particlesPointers[i]].dead){
				str += ", ("+particlesPointers[i]+")";
			}else{
				str += ", "+particlesPointers[i];
			}
		}
		
		GUI.Label(new Rect(5,400,800,50),str);
		*/
		/*
		Vector3 pos,screenPos;
		Rect r = new Rect(0,0,50,20);
		Camera cam = Camera.main.camera;
		float age;
		for(int i=0;i<liveParticleCount;i++){
			
			age = particles[particlesPointers[i]].lifetime;
			
			if(age < 0.1f){
				pos = particles[particlesPointers[i]].position;
				
				screenPos = cam.WorldToScreenPoint(pos);
				r.y = (Screen.height - screenPos.y);
				r.x = screenPos.x;
				
				GUI.Label(r,age.ToString("f3"));
			}
		}
		*/
	//}
	
	
	
	void UpdateParticlesTask(int groupIndex, int start, int end){
		
		if(end > start && end > 0){
			
			Puffy_ParticleData p = null;
			int total = deadGroups[groupIndex].pointers.Count;
			int index = 0;
			
			for(int pointer = start; pointer < end; pointer++){
	
				p = particles[ particlesPointers[pointer] ];
				
				// call particle update
				p.Update(_deltaTime);
				
				if(p.dead){
					if(index >= total){
						deadGroups[groupIndex].pointers.Add(pointer);
						total ++;
					}else{
						deadGroups[groupIndex].pointers[index] = pointer;
					}
					index ++;
				}
			
			}
			
			deadGroups[groupIndex].count = index;
		}
	}
	
	public void KillParticle(int index){
		particles[index].dead = true;
	}
	
	private class PointerComparer : IComparer<int>
	{
		public int Compare(int a, int b)
		{
			return (a > b)?1:-1;
		}
	}

	public int SpawnParticle(Vector3 start_position, Vector3 start_direction,float age = 0f){
		return SpawnParticle(start_position, start_direction, startSpeed, lifeTime, startSize, endSize, startColor, endColor, age);
	}
	
	public int SpawnParticle(Vector3 start_position, Vector3 start_direction,float age = 0f,float start_lifeTime = 1f){
		return SpawnParticle(start_position, start_direction, startSpeed, start_lifeTime, startSize, endSize, startColor, endColor, age);
	}
	
	// create one particle
	public int SpawnParticle(Vector3 start_position, Vector3 start_direction, float start_speed, float start_lifetime, float start_size, float end_size, Color start_color, Color end_color, float age = 0){
		
		if(autoResize && liveParticleCount >= _particleCount){
			if(!Resize (_particleCount + chunkSize)) return -1;
		}
				
		if(liveParticleCount < _particleCount && particlesPointers != null){
			int index = particlesPointers[liveParticleCount];
	
			liveParticleCount++;
			
			particleLive = liveParticleCount;
						
			if(particles[index] == null) particles[index] = new Puffy_ParticleData();
			
			if(lifeTimeVariation != 0) start_lifetime += Random.Range(-lifeTimeVariation , lifeTimeVariation);
			if(startSizeVariation != 0) start_size += Random.Range(-startSizeVariation , startSizeVariation);
			if(endSizeVariation != 0) end_size += Random.Range(-endSizeVariation , endSizeVariation);
			if(startColorVariation != Color.black){
				start_color.r += Random.Range(-startColorVariation.r,startColorVariation.r);
				start_color.g += Random.Range(-startColorVariation.g,startColorVariation.g);
				start_color.b += Random.Range(-startColorVariation.b,startColorVariation.b);
				
				start_color.r = Mathf.Clamp01(start_color.r);
				start_color.g = Mathf.Clamp01(start_color.g);
				start_color.b = Mathf.Clamp01(start_color.b);
				
			}
			if(endColorVariation != Color.black){
				end_color.r += Random.Range(-endColorVariation.r,endColorVariation.r);
				end_color.g += Random.Range(-endColorVariation.g,endColorVariation.g);
				end_color.b += Random.Range(-endColorVariation.b,endColorVariation.b);
				
				end_color.r = Mathf.Clamp01(end_color.r);
				end_color.g = Mathf.Clamp01(end_color.g);
				end_color.b = Mathf.Clamp01(end_color.b);
			}
			
			if(startSpeedVariation != 0) start_speed += Random.Range(-startSpeedVariation , startSpeedVariation);
			
			if(startDirectionVariation != Vector3.zero){
				start_direction.x += Random.Range(-startDirectionVariation.x,startDirectionVariation.x);
				start_direction.y += Random.Range(-startDirectionVariation.y,startDirectionVariation.y);
				start_direction.z += Random.Range(-startDirectionVariation.z,startDirectionVariation.z);
			}
			
			if(positionVariation != Vector3.zero) {
				
				start_position.x += Random.Range(-positionVariation.x,positionVariation.x);
				start_position.y += Random.Range(-positionVariation.y,positionVariation.y);
				start_position.z += Random.Range(-positionVariation.z,positionVariation.z);
				
			}
			
			if(colorMode == colorModes.Gradient){
				particles[index].Spawn(start_position,start_direction,start_speed,start_lifetime,start_size,end_size,age);
			}else{
				particles[index].Spawn(start_position,start_direction,start_speed,start_lifetime,start_size,end_size,start_color,end_color,age);
			}
						
			return index;
		}
		
		return -1;
	}
	
	public void SpawnRow(Vector3 row_start_position, Vector3 row_end_position, Vector3 velocityOffset, float intermediateRatio = -1f){
		if(intermediateRatio<0) intermediateRatio = autoTrailStepRatio;
		SpawnRow(row_start_position, row_end_position, trailStepDistance , startDirection + velocityOffset,startSpeed,lifeTime,startSize,endSize,startColor,endColor,intermediateRatio);
	}
	
	// create a row of particles
	public void SpawnRow(Vector3 row_start_position, Vector3 row_end_position, float stepDistance, Vector3 start_direction, float start_speed, float start_lifetime, float start_size, float end_size, Color start_color, Color end_color, float intermediateRatio = -1f){
		
		if(intermediateRatio<0) intermediateRatio = autoTrailStepRatio;
		
		Vector3 dir = row_end_position - row_start_position;
		float distance = dir.magnitude;
		int count;
		
		if(autoTrailStep){
			count = Mathf.FloorToInt(distance / (start_size*autoTrailStepFactor));
		}else{
			count = Mathf.FloorToInt(distance / stepDistance);
		}
		
		if(count < 2 || intermediateRatio==0){
			SpawnParticle(row_end_position, start_direction, start_speed, start_lifetime, start_size , end_size, start_color,end_color);
		}else{
			int i;
			float stepDist = distance / count;
			dir = dir.normalized * stepDist;
			int index;
			float age = 0;
			float stepTime = _deltaTime / count;
			
			for(i=0;i<count;i++){
				
				index = SpawnParticle(row_end_position, start_direction, start_speed, start_lifetime, start_size , end_size, start_color,end_color,age);
			
				if(index > -1){
					if(i>0 && count>1){
						//intermediate particles are smaller and die sooner, to keep performances
						particles[index].startLifetime *= intermediateRatio;
						particles[index].endSize *= intermediateRatio;
						
						if(debugIntermediate){
							particles[index].startColor = Color.yellow;
							particles[index].endColor = Color.yellow;
						}
					}else{
						if(debugIntermediate){
							particles[index].startColor = Color.magenta;
							particles[index].endColor = Color.magenta;
						}
					}
				}
				age += stepTime;
				row_end_position -= dir;
			}
			
		}
	
	}

	
	
	// resize particles array
	public bool Resize(int newParticleCount){		
		
		if(_particleCount != newParticleCount){
			double tmp = Time.realtimeSinceStartup;
			_particleCount = newParticleCount;
			Puffy_ParticleData[] tmp_particles = new Puffy_ParticleData[_particleCount];
			int[] tmp_particlesPointers = new int[_particleCount];
	
			int i=0;
			int j=0;
		
			for(i=0;i<liveParticleCount;i++){
				j = particlesPointers[i];
				tmp_particles[i] = particles[j];				
				tmp_particlesPointers[i] = i;
			}
			
			for(i=liveParticleCount;i<_particleCount;i++){
				tmp_particlesPointers[i] = i;
			}
			
			particles = tmp_particles;
			particlesPointers = tmp_particlesPointers;
			
			particleTotal = _particleCount;
			particleLive = liveParticleCount;
			
			tmp = Time.realtimeSinceStartup - tmp;

			return true;
		}
		return false;
	}
		
	class PointerGroup{
		public List<int> pointers = new List<int>();
		public int count = 0;
	}
	
	
}

public class Puffy_ParticleData{
	public Vector3 position = Vector3.zero;
	public Vector3 direction = Vector3.up;
	
	public float randomSeed = 0;
	
	public float startLifetime = 0f;
	public float lifetime = 0f;
	
	public Color startColor = Color.white;
	public Color color = Color.white;
	public Color endColor = Color.white;
	
	public float startSize = 0f;
	public float size = 0f;
	public float endSize = 1;
	
	public float alphaMultiplier = 1f;
	
	public float ageRatio = 0f;
	
	public float speed = 0f;
	
	public bool useEmitterGradient = false;
	
	private bool justEmitted = true;
	
	public bool dead = true;
	
	public void Spawn(Vector3 start_position, Vector3 start_direction, float start_speed, float start_lifetime, float start_size, float end_size, Color start_color, Color end_color, float start_age = 0f){
		
		speed = start_speed;
		
		position = start_position;
		direction = start_direction;
		
		startSize = start_size;
		endSize = end_size;
		
		startColor = start_color;
		endColor = end_color;
		
		useEmitterGradient = false;
		
		lifetime = start_age;
		
		startLifetime = start_lifetime;
		
		color = start_color;

		randomSeed = Random.Range(0f,0.5f);
		
		dead = false;
		
		justEmitted = true;
	}
	
	public void Spawn(Vector3 start_position, Vector3 start_direction, float start_speed, float start_lifetime, float start_size, float end_size, float start_age = 0f){
		
		speed = start_speed;
		
		position = start_position;
		direction = start_direction;
		
		startSize = start_size;
		endSize = end_size;
		
		useEmitterGradient = true;
		
		lifetime = start_age;
		startLifetime = start_lifetime;
		
		color = startColor;
		alphaMultiplier = 1f;
		
		randomSeed = Random.Range(0f,0.5f);
		
		dead = false;
		
		justEmitted = true;
	}
	
	public void Update(float deltaTime){
	
		if(!justEmitted) lifetime += deltaTime;
		
		if(startLifetime > 0){
			
			
			dead = lifetime > startLifetime;
			if(!dead){		
				ageRatio = lifetime / startLifetime;
				size = Mathf.Lerp(startSize,endSize,ageRatio);
				position += direction * speed * deltaTime;
				if(!useEmitterGradient) color = Color.Lerp(startColor,endColor,ageRatio);
			}
		}else{
			ageRatio = 0;
		}
			
		justEmitted = false;
		
	}
}