using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Puffy_Cloud : MonoBehaviour {
	
	protected Transform _transform;
	private Vector3 _forward;
	private Vector3 _right;
	private Vector3 _position;
	private float _scale;
	
	public Puffy_Emitter emitter;
	
	public int randomSeed = 0;
	public int baseCount = 10;
	public int subCount = 10;
	
	private List<int> particlesList;
	private List<Vector3> particlesPositions;
	private List<float> particlesSizes;
	
	private int _randomSeed;
	private int _baseCount;
	private int _subCount;
	
	private bool built = false;
	
	// Use this for initialization
	void Awake() {
		_transform = transform;
		if(!emitter) emitter = gameObject.GetComponent<Puffy_Emitter>();
		particlesList = new List<int>();
		particlesPositions = new List<Vector3>();
		particlesSizes = new List<float>();
	}
	
	// Use this for initialization
	void Build() {
		
		if(subCount<0) subCount = 0;
		if(baseCount<0) baseCount = 0;
		
		_randomSeed = randomSeed;
		_subCount = subCount;
		_baseCount = baseCount;
		
		int i,j;
		Vector3 pos,pos2;
		float size,size2;
		
		List<int> existingParticles = new List<int>(); 
		
		for(i=0; i<particlesList.Count;i++){
			existingParticles.Add (particlesList[i]);
		}
		
		particlesList.Clear();
		particlesPositions.Clear();
		particlesSizes.Clear();
		_position += Vector3.one;
		
		int index = 0;
		
		for(i=0;i < baseCount;i++){
			
			//Random.seed = randomSeed + i;
			
			pos = Random.insideUnitSphere*emitter.startSize*0.2f;
			size = Random.Range(emitter.startSize,emitter.endSize);
			
			if(existingParticles.Count > 0){
				index = existingParticles[0];
				existingParticles.RemoveAt(0);
				emitter.particles[index].position = pos;
				emitter.particles[index].size = size;
				emitter.particles[index].startSize = size;
				emitter.particles[index].endSize = size;
				emitter.particles[index].startLifetime = -1;
				emitter.particles[index].lifetime = 0;
				emitter.particles[index].speed = 0;
				emitter.particles[index].startColor = emitter.startColor;
				emitter.particles[index].endColor = emitter.endColor;
			}else{
				index = emitter.SpawnParticle(pos,Vector3.zero,0,-1,size,size,emitter.startColor,emitter.endColor,0);
			}
			
			if(index >= 0){
				particlesList.Add (index);
				particlesPositions.Add (pos);
				particlesSizes.Add (size);
				
				for(j=0;j<subCount;j++){
					
					//Random.seed = randomSeed + 100 + j*5;
					
					pos2 = pos + Random.onUnitSphere * size * 0.2f;
					size2 = Random.Range(emitter.startSize*0.5f,emitter.endSize*0.3f);
					
					if(existingParticles.Count > 0){
						index = existingParticles[0];
						existingParticles.RemoveAt(0);
						emitter.particles[index].position = pos2;
						emitter.particles[index].size = size2;
						emitter.particles[index].startSize = size2;
						emitter.particles[index].endSize = size2;
						emitter.particles[index].startLifetime = -1;
						emitter.particles[index].lifetime = 0;
						emitter.particles[index].speed = 0;
						emitter.particles[index].startColor = emitter.startColor;
						emitter.particles[index].endColor = emitter.endColor;
					}else{
						index = emitter.SpawnParticle(pos2,Vector3.zero,0,-1,size2,size2,emitter.startColor,emitter.endColor,0);
					}
					
					if(index >=0){
						particlesList.Add (index);
						particlesPositions.Add (pos2);
						particlesSizes.Add (size2);
					}
				}
			}
		}
		
		for(i=0; i < existingParticles.Count;i++){
			emitter.KillParticle(existingParticles[i]);
		}
		
		built = true;
	}
	
	
	void Update(){
		if(!built || _randomSeed!=randomSeed || _baseCount!=baseCount || _subCount!=subCount) Build();
		
		if(_transform.position != _position || _transform.forward != _forward || _transform.right != _right || _transform.localScale.x != _scale){
			_position = _transform.position;
			_forward = _transform.forward;
			_right = _transform.right;
			_scale = _transform.localScale.x;
			_transform.localScale = Vector3.one * _scale;
			
			int i,index;
			
			for(i = 0; i < particlesList.Count ; i++){
				index = particlesList[i];
				emitter.particles[index].position = _transform.TransformPoint(particlesPositions[i]);
				emitter.particles[index].size = particlesSizes[i] * _scale;
			}
		}
	}
	
	#if UNITY_EDITOR
	void OnDrawGizmos(){
		if(emitter != null){
			int i,j;
			Vector3 pos,pos2;
			float size,size2;
			
			_transform = transform;
			
			_scale = _transform.localScale.x;
			_transform.localScale = Vector3.one * _scale;
		
			for(i=0;i < baseCount;i++){
				
				//Random.seed = randomSeed + i;
				
				pos = Random.insideUnitSphere*emitter.startSize*0.2f;
				size = Random.Range(emitter.startSize,emitter.endSize);
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(_transform.TransformPoint(pos),size * 0.3f * _scale);
				
				Gizmos.color = Color.gray;	
				for(j=0;j<subCount;j++){
					
					//Random.seed = randomSeed + 100 + j*5;
					
					pos2 = pos + Random.onUnitSphere * size * 0.2f;
					size2 = Random.Range(emitter.startSize*0.5f,emitter.endSize*0.3f);
					
					Gizmos.DrawWireSphere(_transform.TransformPoint(pos2),size2 * 0.3f * _scale);
				}
			}
		}else{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position,1);
		}
	}
	#endif
}
