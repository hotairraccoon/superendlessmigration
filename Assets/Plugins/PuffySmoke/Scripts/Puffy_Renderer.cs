using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Puffy_Renderer : MonoBehaviour {
	
	public static List<Puffy_Renderer> instances = new List<Puffy_Renderer>();
	public static int particlesPerCore_ChunkSize = 16384;
	
	public static void ToggleDebug(){
		for(int i=0; i<instances.Count; i++){
			instances[i].debug = !instances[i].debug;
		}
	}
	
	public enum PassModeType{Auto, One, Multiple};
	
	public enum coreslevels{
		Minimum = 1024,
		Low = 2048,
		Medium = 4096,
		High = 8192,
		Maximum = 16384
	}
	
	public enum meshlevels{
		Minimum = 1024,
		Low = 2048,
		Medium = 4096,
		High = 8192,
		Maximum = 16384
	}
	
	public enum debugModes{
		Simple = 1,
		Advanced = 2
	}
	
	[HideInInspector]
	public bool perf_foldout = true;
	
	[HideInInspector]
	public bool aspect_foldout = true;
	
	[HideInInspector]
	public bool visibility_foldout = true;
	
	public coreslevels coresSetup = coreslevels.Maximum;
	public meshlevels meshSetup = meshlevels.Medium;
	
	private string debugString = "";
	
	public bool debug = false;
	public bool render = true;
	
	public debugModes debugMode = debugModes.Advanced;
	
	public PassModeType PassMode = PassModeType.Auto;
	
	[Range(0f,0.1f)]
	public float updateThreshold = 0.01f;
	public Light _light;
	public bool useAmbientColor = true;
	public bool cameraBackgroundAsAmbientColor = true;
	
	public Material particlesMaterial;
	public int TextureColCount = 8;
	public int TextureRowCount = 8;
	public float detailsScaling = 0.55f;
	
	public int MaxRenderDistance = 1000;
	public bool AutoLOD = false;
	
	[Range(0f,2f)]
	public float ScreensizeClipping = 1;
	
	[Range(0f,1f)]
	public float ScreensizeClippingFade = 0.2f;
	
	public List<Puffy_Emitter> Emitters;
	
	public bool useThread = true;
	
	private List<SortIndex> mergedList = new List<SortIndex>();
	
	private int live = 0;
	private int _coresCount;
	private int _virtualCoresCount;
	private int activeGroups = 0;
	private int TextureFrameCount = 1;
	private int meshUpdateCount = 0;
	private int visibleMeshesCount = 0;
	private int renderStep = 0;
	private int visibleParticles = 0;
	
	private float pDirectionalTextureFrameWidth = 1f;
	
	private double updateMeshTime = 0;
	private double updateMeshTime_Step1 = 0;
	private double updateMeshTime_Step2 = 0;
	private double buildTime = 0;
	private double renderTime = 0;
	private double mergeTime = 0;
	
	private DebugTimer frustumTime = new DebugTimer();
	private DebugTimer sortTime = new DebugTimer();
	
	private List<SortGroup> _sortGroups = new List<SortGroup>();
	private List<SortGroup> _sortIndexStock = new List<SortGroup>();
	private List<UnityThreading.Task> taskList = new List<UnityThreading.Task>();
	private List<VariableMesh> meshList = new List<VariableMesh>();
	
	private OrderComparer comp = new OrderComparer();
	
	private Vector2 _UVrotationVector = Vector2.zero;
	
	private Vector3[] _billboardShape = new Vector3[4];
	
	private Camera _camera;
	private Transform _cameraTransform;
	private Transform _lightTransform;
	
	private bool allMeshCleared = false;
	
	private int renderPasses = 0;
	
	public static Puffy_Renderer GetRenderer(){
		if(instances.Count == 0) return null;
		return instances[0];
	}
	
	public static Puffy_Renderer GetRenderer(string rendererName){
		for(int i=0; i<instances.Count; i++){
			if(instances[i].name == rendererName) return instances[i];
		}
		return null;
	}
	
	public static Puffy_Renderer GetRenderer(int index){
		if(index < 0 || index >= instances.Count) return null;
		return instances[index];
	}
	
	void OnDestroy(){
		instances.Remove(this);
	}
	
	void Awake () {
		instances.Add (this);
		
		// number of real cores available in the cpu
		_coresCount = SystemInfo.processorCount;
		
		// virtual cores are used to split the sorting process when too many particles are created
		_virtualCoresCount = _coresCount;
		
		// create initial sort groups
		int i;
		for(i=0; i < _coresCount;i++){
			_sortGroups.Add (new SortGroup());
			_sortIndexStock.Add (new SortGroup());
		}
		
		if(!_light){
			_light = Light.FindObjectOfType(typeof(Light)) as Light;
		}
		
		// identify the main camera
		_camera = Camera.main;
		_cameraTransform = _camera.transform;
		
		if(_light){
			_lightTransform = _light.transform;
		}else{
			_lightTransform = transform;
			Debug.LogWarning("Warning ! No light has been found ! Use Puffy_Renderer transform instead !");
		}

		TextureFrameCount = (TextureColCount*TextureRowCount)-1;
		
		pDirectionalTextureFrameWidth = 1f/TextureColCount;

	}
	
	
	

	void OnGUI() {
		
		if(debug){
			
			double cpuTime = 0;
				
			foreach(Puffy_Emitter e in Emitters){
				cpuTime += (e.debugTime*1000);
			}

			cpuTime += frustumTime.Average();
			cpuTime += sortTime.Average();
			cpuTime += mergeTime;
			cpuTime += buildTime;
			cpuTime += updateMeshTime;
			
			if(debugMode == debugModes.Advanced){
				float updateTime = 0f;
				live = 0;
				float total = 0;
				
				foreach(Puffy_Emitter e in Emitters){
					total += e.particleTotal;
					live += e.particleLive;
					updateTime += (float)(e.debugTime*1000);
				}
				
				GUILayout.Space(30);
				
				GUILayout.Label ("CPU : " + cpuTime.ToString("f2")+"ms");
				
				GUILayout.Label("Live : "+live.ToString()+"/"+total.ToString());
				
				if(debugString!="") GUILayout.Label ("debug "+debugString);
				
				if(live > 0){
					
					
					GUILayout.Label ("Visible : " + visibleParticles.ToString ());
						
					GUI.color = Color.white;
					if(useThread){
						GUILayout.Label("Cores : "+_virtualCoresCount);
					}else{
						GUILayout.Label("Threading OFF");
					}
					
					
					GUILayout.Label("Move : "+updateTime.ToString("f2")+"ms");
							
					GUILayout.Label("Frustum check : " + frustumTime.Average().ToString ("f2") + "ms");
					//GUILayout.Label(frustumTime.minimum.ToString("f2")+" - "+frustumTime.maximum.ToString("f2"));
					
					
					
					GUILayout.Label("Z Sort : " + sortTime.Average().ToString ("f2") + "ms");
					//GUILayout.Label(sortTime.minimum.ToString("f2")+" - "+sortTime.maximum.ToString("f2"));
					
					GUILayout.Label ("Merge sort : " + (mergeTime*1000).ToString ("f2") + "ms");
					
					GUILayout.Label ("Rebuild mesh : " + (buildTime*1000).ToString ("f2") + "ms");
					
					GUILayout.Label ("Update mesh : " + (updateMeshTime*1000).ToString ("f2") + "ms");
					if(useThread){
						GUILayout.Label ("Update mesh A: " + (updateMeshTime_Step1*1000).ToString ("f2") + "ms");
						GUILayout.Label ("Update mesh B: " + (updateMeshTime_Step2*1000).ToString ("f2") + "ms");
					}
					
					GUILayout.Label ("Passes : " + renderPasses.ToString());
					
					
					GUILayout.Label ("TimeScale : " + Time.timeScale.ToString());
					
					/*
					string str = "sort groups " +activeGroups+"/"+_sortGroups.Count+" :\n";
					for(int i=0;i<_sortGroups.Count;i++){
						str += "group "+i+" : "+_sortGroups[i].count+" "+(_sortGroups[i].sortTime>0?(_sortGroups[i].sortTime*1000).ToString("f2")+"ms":"")+"\n";
					}
					
					GUILayout.Label (str);
					*/
				}
			
			}else{
				
				
				
				GUILayout.Space(30);
				GUILayout.Label ("CPU : " + cpuTime.ToString("f2")+"ms");
				
			}
		}
	}
	
	void OnDrawGizmos(){
		if(debug){
			int i=0;
						
			Color[] cols = new Color[16];
			
			cols[0] = new Color(1f,0f,0f,1f);
			cols[1] = new Color(1f,0.2f,0f,1f);
			cols[2] = new Color(1f,0.4f,0f,1f);
			cols[3] = new Color(1f,0.6f,0f,1f);
			cols[4] = new Color(1f,0.8f,0f,1f);
			cols[5] = new Color(1f,1f,0f,1f);
			cols[6] = new Color(0.8f,1f,0f,1f);
			cols[7] = new Color(0.6f,1f,0f,1f);
			cols[8] = new Color(0.4f,1f,0f,1f);
			cols[9] = new Color(0.2f,1f,0f,1f);
			cols[10] = new Color(0.0f,1f,0f,1f);
			cols[11] = new Color(0.0f,1f,0.2f,1f);
			cols[12] = new Color(0.0f,1f,0.4f,1f);
			cols[13] = new Color(0.0f,1f,0.6f,1f);
			cols[14] = new Color(0.0f,1f,0.8f,1f);
			cols[15] = new Color(0.0f,1f,1f,1f);
			
			for(i=0; i<meshList.Count;i++){
				if(meshList[i].particleIndex > 0){
					if(i<=15) Gizmos.color = cols[i];
					Gizmos.DrawWireCube(meshList[i].mesh.bounds.center + meshList[i]._transform.position,meshList[i].mesh.bounds.size);
				}
			}
		}
	}
			
	public bool AddEmitter(Puffy_Emitter emitter){

		if(emitter.puffyRenderer != this && Emitters.IndexOf(emitter)==-1){
			Emitters.Add (emitter);
			return true;
		}
		
		return false;
	}
	
	public bool RemoveEmitter(Puffy_Emitter emitter){
		return Emitters.Remove(emitter);
	}
	
	public bool RemoveEmitter(int index){
		if(index>-1 && index < Emitters.Count){
			Emitters.RemoveAt(index);
			return true;
		}
		
		return false;
	}
	
	public bool RemoveEmitter(string emitterName){
		for(int i=0; i<Emitters.Count; i++){
			if(Emitters[i].name == emitterName){
				Emitters.RemoveAt(i);
				return true;
			}
		}
		
		return false;
	}
	
	
	void LateUpdate(){
		Render();
	}
	
	void Render(){

		if(renderStep == 0){
			particlesPerCore_ChunkSize = (int)coresSetup;
			
			// total number of alive particles
			live = 0;
			
			// number of visible particles
			visibleParticles = 0;
			
			// get the total of alive particles
			for(int i=0; i < Emitters.Count; i++){
				if(Emitters[i] == null){
					Emitters.RemoveAt(i);
					i--;
				}else{
					live += Emitters[i].particleLive;
					if(Emitters[i].puffyRenderer == null) Emitters[i].puffyRenderer = this;
				}
			}
		}
		
		if(live > 0 || renderStep > 0 || visibleMeshesCount > 0){
				
			if(live == 0){
				
				if(!allMeshCleared){
					meshUpdateCount = 0;
					UpdateMeshes();
				}
				renderStep = 0;
			}else{
			
				double time = 0;
				double totalTime = 0;
				
//				Debug.Log (Time.renderedFrameCount+" RENDER");
				
				if(renderStep == 0){
					renderPasses = 0;
					time = Time.realtimeSinceStartup;
					
					// identify visible particles
					frustumTime.Start();
					FrustumCheck();
					frustumTime.Stop();
					
					if(visibleParticles == 0){
						if(!allMeshCleared){
							meshUpdateCount = 0;
							UpdateMeshes();
						}
						renderStep = 0;
					}else{
						
						// update the billboard shape
						updateBillboardsData();
						
						// sort visible particles
						sortTime.Start ();
						SortParticles();
						sortTime.Stop();
						
						totalTime += Time.realtimeSinceStartup - time;
					
						if((totalTime < updateThreshold || PassMode == PassModeType.One) && PassMode != PassModeType.Multiple) renderStep = 1;
						
					}
				}
				
				if(renderStep == 1){
					
					// merge sorted groups
					time = Time.realtimeSinceStartup;
					MergeGroups();
					mergeTime = Time.realtimeSinceStartup - time;
					totalTime += mergeTime;
					
					// rebuild all meshes data
					time = Time.realtimeSinceStartup;
					
					BuildMeshesThreads();
					
					renderTime = Time.realtimeSinceStartup - time;
					totalTime += renderTime;
				
					if((totalTime < updateThreshold || PassMode == PassModeType.One) && PassMode != PassModeType.Multiple) renderStep = 2;
					
				}
				
				if(renderStep == 2){
					// send updated data to the meshes
					UpdateMeshes();
					//Debug.Log (Time.renderedFrameCount+" RENDER MESHES");
			
					renderStep = 0;
					
				}else{
					renderStep++;
				}
				
				renderPasses++;
			}
				
		}
		
	}
	
	private void updateBillboardsData(){
		
		// light direction in camera space
		Vector3 lDir;
		lDir = _cameraTransform.InverseTransformDirection(_lightTransform.forward);
		
		// particles Roll
		if (Mathf.Approximately(1f,lDir.z)){
			lDir.x = 0f;
			lDir.y = 1f;
		}else{
			lDir.z = 0f;
			lDir.Normalize();
		}
		
		// billboard shape
		float lAng = Mathf.Atan2(lDir.y,lDir.x) + 0.7853982f; // + 3.141592f * 0.25f;
		lDir.x = Mathf.Cos(lAng) * 0.5f;
		lDir.y = Mathf.Sin(lAng) * 0.5f;
	
		_billboardShape[0] = _cameraTransform.TransformDirection(new Vector3(lDir.y,-lDir.x,0f));
		_billboardShape[1] = _cameraTransform.TransformDirection(new Vector3(-lDir.x,-lDir.y,0f));
		_billboardShape[2] = _cameraTransform.TransformDirection(new Vector3(-lDir.y,lDir.x,0f));
		_billboardShape[3] = _cameraTransform.TransformDirection(new Vector3(lDir.x,lDir.y,0f));
		
		// details roll
		_UVrotationVector.x = lDir.x;
		_UVrotationVector.y = lDir.y;
	}
	
	// ***********************************************************************************************************************
	// STEP 1) Fill the sort groups with the visible particles indices
	// ***********************************************************************************************************************
	
	private void FrustumCheck(){
	
		Vector3 camDir = _cameraTransform.forward;
		Vector3 camPos = _cameraTransform.position;
		float FOV = _camera.fieldOfView;
		float angle = Mathf.Cos(_camera.fieldOfView * Mathf.Deg2Rad * 0.9f);
		
		int i;
		int j;

		int groupsCount;
		int count;

		// init the sort groups
		groupsCount = _sortGroups.Count;
		for(i=0;i < groupsCount; i++){
			_sortGroups[i].index = 0;
			_sortGroups[i].count = 0;
		}
		
		// update the number of virtual cores according to the number of particles
		_virtualCoresCount = Mathf.Max (_coresCount , Mathf.CeilToInt (live / particlesPerCore_ChunkSize)+1 );
		Matrix4x4 lightMatrix = _lightTransform.worldToLocalMatrix;
		Matrix4x4 cameraMatrix = _cameraTransform.worldToLocalMatrix;
		
		// too few particles or only one core is available, do the computing on a single thread
		if(_coresCount==1 || live < _coresCount * 512 || !useThread){
		//if(_coresCount==1 || !useThread){
			for(i = 0 ; i < _virtualCoresCount ; i++){
				if(i >= groupsCount){
					_sortGroups.Add (new SortGroup());
					groupsCount++;
				}
				
				FrustumTask(i,_virtualCoresCount,camDir,camPos,angle,lightMatrix,cameraMatrix,FOV);
			}
			
		}else{
			if(taskList == null) taskList = new List<UnityThreading.Task>();
			
			taskList.Clear();
			int threadCount = 0;
			
			// split the computing accross all virtual cores
			for(i = 0 ; i < _virtualCoresCount ; i++){
				var grp = i;
				
				if(i >= groupsCount){
					_sortGroups.Add (new SortGroup());
					groupsCount++;
				}
				
				_sortGroups[i].index = 0;
				_sortGroups[i].count = 0;
				taskList.Add(UnityThreadHelper.TaskDistributor.Dispatch(() => FrustumTask(grp,_virtualCoresCount,camDir,camPos,angle,lightMatrix,cameraMatrix,FOV)));
				
				threadCount ++;
				// when all real cores are busy, wait for the end of each process before creating new ones
				if(threadCount == _coresCount){
					for(j = 0; j < threadCount ; j ++){
						taskList[j].Wait();
						taskList[j].Dispose();
					}
					// get ready for the next row of threads
					threadCount = 0;
					taskList.Clear();
				}
			}
			// wait for the last threads
			count = taskList.Count;
			for(i = 0; i < count ; i ++){
				taskList[i].Wait();
				taskList[i].Dispose();
			}
			taskList.Clear();		
			
		}
		
		// total of visible particles
		visibleParticles = 0;
		activeGroups = 0;
		for(i = 0 ; i < groupsCount; i++){
			_sortGroups[i].index = 0;
			if(_sortGroups[i].count > 0) activeGroups++;
			visibleParticles += _sortGroups[i].count;
		}
	}
		
	private void FrustumTask(int groupIndex, int split, Vector3 camDir,Vector3 camPos,float angle, Matrix4x4 lightMatrix, Matrix4x4 cameraMatrix, float FOV){
		

			int index;
			int pointer;

			int count = _sortGroups[groupIndex].index;
			Vector3 direction;
			float distance;
		
			SortIndex s;
			Puffy_Emitter emitter;
		
			int start = 0;
			int end = 0;
			int emitterIndex;
			int stp;
		
			split = Mathf.Min (Mathf.Max(1,split),_virtualCoresCount);
			
			float rowHeight = 1f / TextureRowCount;
			
			int tile_index;
			float tile_U = 0f;
			float tile_V = 0f;
		
			bool excluded = false;
			float excludeDistance4 = MaxRenderDistance / 4;
			float excludeDistance3 = MaxRenderDistance / 3;
			float excludeDistance2 = MaxRenderDistance / 2;
			
			float size = 0;
			Vector3 screenPoint;
			Vector3 localPosition;
		
			// loop on all emitters		
			for(emitterIndex = 0; emitterIndex < Emitters.Count ; emitterIndex++){
				
				emitter = Emitters[emitterIndex];
				
				// define the chunk of particles to process for this emitter on this thread
				stp = Mathf.CeilToInt((float)emitter.particleLive / split);
				start = groupIndex * stp;
				end = Mathf.Min (start + stp,emitter.particleLive);
				
				if(end > start && end > 0){
					// loop on particles
					for(pointer = start; pointer < end; pointer++){
						index = emitter.particlesPointers[pointer];

						direction = (emitter.particles[index].position - camPos).normalized;

						// check if the particle is in the camera view cone
						if( Vector3.Dot(camDir, (emitter.particles[index].position - camPos).normalized ) > angle ){
							
							localPosition = cameraMatrix.MultiplyPoint3x4(emitter.particles[index].position);
							
							distance = localPosition.z;
						
							if(distance > 0){
								
								// remove particles based on distance
								excluded = distance > MaxRenderDistance;	
								
								if(!excluded){
									if(AutoLOD){
										excluded = excluded || distance > excludeDistance4 && (index % 4) == 0;
										excluded = excluded || distance > excludeDistance3 && (index % 3) == 0;
										excluded = excluded || distance > excludeDistance2 && (index % 2) == 0;
									}
								
									// reset fading value
									emitter.particles[index].alphaMultiplier = 1;
								
									if(!excluded){
									
										if(emitter.particles[index].size > 0){
											// compute particle screen size
											screenPoint = Vector3.forward * localPosition.z + Vector3.up * emitter.particles[index].size* 0.5f;
											size = screenPoint.magnitude;
											if(size > 0) size = ((Mathf.Acos(localPosition.z / size ) * Mathf.Rad2Deg) / FOV) * 2;
										
											if(size > ScreensizeClipping - ScreensizeClippingFade){

												emitter.particles[index].alphaMultiplier = Mathf.Clamp01((ScreensizeClipping - size)/ScreensizeClippingFade);
											
												excluded = excluded || emitter.particles[index].alphaMultiplier == 0;
											}
										}
									}

								}
					
								if(!excluded){
									s = _sortGroups[groupIndex].items[count];
									
									s.particleIndex = index;
									s.emitterIndex = emitterIndex;
									s.order = distance;
								
									tile_index = Mathf.FloorToInt( Mathf.Max(0f,(1f-((lightMatrix.MultiplyVector(direction*-1).z + 1.0f)*0.5f))) * TextureFrameCount );
								
									tile_U = pDirectionalTextureFrameWidth * (tile_index % TextureColCount);
									tile_V = rowHeight * Mathf.FloorToInt((float)tile_index / TextureColCount);	
									
									s.uvs[0].x = tile_U;
									s.uvs[0].y = tile_V;
									
									s.uvs[1].x = tile_U;
									s.uvs[1].y = tile_V + rowHeight;
									
									s.uvs[2].x = tile_U + pDirectionalTextureFrameWidth;
									s.uvs[2].y = tile_V + rowHeight;
									
									s.uvs[3].x = tile_U + pDirectionalTextureFrameWidth;
									s.uvs[3].y = tile_V;
									
									count++;
								}
							}
						}
					}
				}
			
			}
			
			_sortGroups[groupIndex].count = count;
	}
	
	// ***********************************************************************************************************************
	// STEP 2) Sort the visible particles
	// ***********************************************************************************************************************
	
	private void SortParticles(){
		
		int i,j;
		int count = _sortGroups.Count;
		int threadAdded = 0;
		double tmp;
		
		if(!useThread){
			// sort everything on one thread
			for(i = 0 ; i < count ; i++){
				if(_sortGroups[i].count > 0){
					tmp = Time.realtimeSinceStartup;

					System.Array.Sort(_sortGroups[i].items,0, _sortGroups[i].count, comp);

					_sortGroups[i].sortTime = Time.realtimeSinceStartup - tmp;
				}else{
					_sortGroups[i].sortTime = 0;
				}
			}
		}else{
		
			taskList.Clear();
			
			// loop on all used sort groups
			tmp = Time.realtimeSinceStartup;
			for(i = 0 ; i < count ; i++){
				
				_sortGroups[i].sortTime = 0;
				if(_sortGroups[i].count > 0){	
					var grp = i;
				
					taskList.Add(UnityThreadHelper.TaskDistributor.Dispatch(() => System.Array.Sort(_sortGroups[grp].items,0, _sortGroups[grp].count, comp) ));
					
					threadAdded++;
				}
				
				// wait for threads to finish
				if(threadAdded == _coresCount){
					for(j = 0 ; j < threadAdded ; j++){
						taskList[j].Wait();
						taskList[j].Dispose();
					}
					_sortGroups[i].sortTime = Time.realtimeSinceStartup - tmp;
					threadAdded = 0;
					taskList.Clear();
					tmp = Time.realtimeSinceStartup;
				}
				
			}

			if(threadAdded > 0){
				for(j=0 ; j < threadAdded ; j++){
					taskList[j].Wait();
					taskList[j].Dispose();
				}
				taskList.Clear();
				_sortGroups[i-1].sortTime = Time.realtimeSinceStartup - tmp;
			}
		}
		
	}
	
	// ***********************************************************************************************************************
	
	
	private VariableMesh AddMesh(){
		VariableMesh vm = new VariableMesh(meshList.Count,particlesMaterial,(int)meshSetup);
		meshList.Add(vm);
		return vm;
	}
	
		
	void MergeGroups(){
		int groupCount = _sortGroups.Count;
		int groupIndex = -1;
		int g,i,index = 0;
		
		float maxValue = -1f;
		float o = 0f;
		
		SortGroup[] local = _sortGroups.ToArray();
		
		for(i = 0; i < visibleParticles ; i++){
				
			groupIndex = -1;
			maxValue = -1f;
			
			// loop on all sortgroups -> TODO : find an optimisation to not loop on all groups all the time
			for(g = 0; g < groupCount; g++){
				index = local[g].index;
				if(index < local[g].count){
					
					// find the group with the next farthest particle
					o = local[g].items[index].order;
					if(o >= maxValue){
						groupIndex = g;
						maxValue = o;
					}
					
				}
			}
			
			if(groupIndex > -1){
				if(i >= mergedList.Count){
					mergedList.Add(local[groupIndex].items[ local[groupIndex].index ]);
				}else{
					mergedList[i] = local[groupIndex].items[ local[groupIndex].index ];
				}
				local[groupIndex].index++;
			}
			
		}

	}
	
	private bool useDetails = false;
	void BuildMeshesThreads(){
		buildTime = Time.realtimeSinceStartup;
		
		if(visibleParticles > 0 && render){
			
			// update shader parameters
			
			if(particlesMaterial != null){
				
				// if the shader doesn't use a details texture then don't compute and send uv1 coordinates
				useDetails = particlesMaterial.HasProperty("_DetailTex");
				
				if(_light != null){
					particlesMaterial.SetFloat("_LightIntensity",_light.intensity*2);
					particlesMaterial.SetColor("_LightColor",_light.enabled?_light.color:Color.black);
				}else{
					particlesMaterial.SetFloat("_LightIntensity",1);
					particlesMaterial.SetColor("_LightColor",Color.white);
				}
				
				if(useAmbientColor){
					if(cameraBackgroundAsAmbientColor){
						particlesMaterial.SetColor("_AmbientColor",_camera.backgroundColor);
					}else{
						particlesMaterial.SetColor("_AmbientColor",RenderSettings.ambientLight);
					}
				}else{
					particlesMaterial.SetColor("_AmbientColor",Color.black);
				}
				
				
			}else{
				//Debug.LogError("No material as been assigned to the Puffy_Renderer : "+name);
			}
			
			Vector3 lCameraPos = _cameraTransform.position;
			Vector3 lCameraForward = _cameraTransform.forward;
			
			int meshCount = Mathf.CeilToInt((float)visibleParticles / VariableMesh.maxParticlesCount);
			int i,j;
			
			meshUpdateCount = meshCount;
			
			
			
			if(meshCount < 2 || !useThread || _coresCount == 1){
				if(meshList.Count == 0) AddMesh();
				BuildMeshes_task(0,0,visibleParticles,lCameraPos,lCameraForward);
				
			}else{
				int[] counts = new int[meshCount];
				int total = 0;
				
				for(i = 0; i < meshCount; i++){
					counts[i] = VariableMesh.maxParticlesCount;
					total += VariableMesh.maxParticlesCount;
					if(i >= meshList.Count) AddMesh();
					
				}
				
				if(total > visibleParticles){
					counts[meshCount-1] = visibleParticles % VariableMesh.maxParticlesCount;
				}
					
				taskList.Clear();
				int start = 0;
				int end = 0;
				int threadAdded = 0;
			
				for(i=0;i < meshCount;i++){
					end += counts[i];
					
					var grp = i;
					var strt = start;
					var nd = end;
					
					//if(true){
						taskList.Add(UnityThreadHelper.TaskDistributor.Dispatch(() => BuildMeshes_task(grp,strt,nd,lCameraPos,lCameraForward) ));
						threadAdded++;
						
						// wait for threads to finish
						if(threadAdded == _coresCount){
							for(j = 0 ; j < threadAdded ; j++){
								taskList[j].Wait();
								taskList[j].Dispose();
							}
							threadAdded = 0;
							taskList.Clear();
						}
					/*
					}else{
						BuildMeshes_task(grp,strt,nd,lCameraPos,lCameraForward);	
					}
					*/
					start += VariableMesh.maxParticlesCount;
				}
				
				
				for(j = 0 ; j < threadAdded ; j++){
					taskList[j].Wait();
					taskList[j].Dispose();
				}
				threadAdded = 0;
				taskList.Clear();
				
			}
			
		}
		
		buildTime = Time.realtimeSinceStartup - buildTime;
	}
	
	
	void BuildMeshes_task(int meshIndex,int startIndex,int endIndex,Vector3 cameraPosition,Vector3 cameraForward) {
		
			SortIndex particlesData;
			
			Puffy_ParticleData p;
				
			VariableMesh currentMesh = meshList[meshIndex];
		
			VariableMeshData currentMeshData = currentMesh.Init(endIndex - startIndex);
		
			float size;
			float uvScale = 1f;
			
			float bs0x = _billboardShape[0].x;
			float bs0y = _billboardShape[0].y;
			float bs0z = _billboardShape[0].z;
			
			float bs1x = _billboardShape[1].x;
			float bs1y = _billboardShape[1].y;
			float bs1z = _billboardShape[1].z;
			
			float bs2x = _billboardShape[2].x;
			float bs2y = _billboardShape[2].y;
			float bs2z = _billboardShape[2].z;
			
			float bs3x = _billboardShape[3].x;
			float bs3y = _billboardShape[3].y;
			float bs3z = _billboardShape[3].z;
			
			byte colorR = 0;
			byte colorG = 0;
			byte colorB = 0;
			byte colorA = 0;
			
			float ageRatio;
			
			float uvOffsetX = 0;
			float uvOffsetY = 0;
			
			float uvRotationVectorX = 0f;
			float uvRotationVectorY = 0f;
			
			float _UVrotationVectorX = _UVrotationVector.x;
			float _UVrotationVectorY = _UVrotationVector.y;
			
			float posX = 0f;
			float posY = 0f;
			float posZ = 0f;
			
			float lCurveEvaluateValue = 0f;
			
			int i;
			int lv0,lv1,lv2,lv3;

			int firstVertex = 0;
					
			Vector3 lCameraPos = cameraPosition;
			Vector3 lCameraForward = cameraForward;
		
			Vector3[] lVerts = currentMeshData.vertices;
				
			Vector2[] lUVs = currentMeshData.uvs;
			Vector2[] lUVs1 = null;
			if(useDetails) lUVs1 = currentMeshData.uvs1;
		
			Vector3[] lExtra = currentMeshData.extraData;
			
			Color32[] lColors = currentMeshData.colors;
			
			Puffy_Emitter emitter = null;
			Color particleColor;
			
			float lLuminosityValue = 0f;
			AnimationCurve lLuminosityCurve = new AnimationCurve();
			Gradient lGradient = null;
		
			int prevEmitterIndex = -1;
			float lifeTime = 0;
			for(i = startIndex; i < endIndex ; i++){
				
				particlesData = mergedList[i];
				
				if( prevEmitterIndex != particlesData.emitterIndex){
					prevEmitterIndex = particlesData.emitterIndex;
					emitter = Emitters[ prevEmitterIndex ];
				
					lLuminosityCurve.keys = emitter.luminosityCurve.keys;
					lGradient = null;
					if(emitter.hasGradient){
						lGradient = emitter.colorGradient.gradient;
					}
					
				}
				
				p = emitter.particles[ particlesData.particleIndex ];
				
				posX = p.position.x;
				posY = p.position.y;
				posZ = p.position.z;
				
				size = p.size;
				
				ageRatio = p.ageRatio;
				
				firstVertex = currentMesh.particleIndex * 4;
				
				lv0 = firstVertex;
				lv1 = firstVertex+1;
				lv2 = firstVertex+2;
				lv3 = firstVertex+3;

				// extra data
				if(p.startLifetime > 0){
					lExtra[lv0].x = ageRatio;
					lExtra[lv1].x = ageRatio;
					lExtra[lv2].x = ageRatio;
					lExtra[lv3].x = ageRatio;
				}else{
					// immortal particle
					lifeTime = p.lifetime;
					lExtra[lv0].x = lifeTime;
					lExtra[lv1].x = lifeTime;
					lExtra[lv2].x = lifeTime;
					lExtra[lv3].x = lifeTime;
				}
			
				if(p.useEmitterGradient && !emitter.debugIntermediate && lGradient!=null){
				
					lCurveEvaluateValue = Mathf.Min (1f,p.lifetime/emitter.colorGradientEndTime);
				
					particleColor = lGradient.Evaluate(lCurveEvaluateValue);
				
					colorR = (byte)(particleColor.r * 255);
					colorG = (byte)(particleColor.g * 255);
					colorB = (byte)(particleColor.b * 255);
					colorA = (byte)(particleColor.a * p.alphaMultiplier *  255);
					
					if(emitter.useLuminosity){
						lLuminosityValue = lLuminosityCurve.Evaluate(lCurveEvaluateValue);
					}else{
						lLuminosityValue = 0;
					}
				
					lExtra[lv0].y = lLuminosityValue;
					lExtra[lv1].y = lLuminosityValue;
					lExtra[lv2].y = lLuminosityValue;
					lExtra[lv3].y = lLuminosityValue;
					
				}else{
					if(emitter.debugIntermediate){
						colorR = (byte)(p.startColor.r * 255);
						colorG = (byte)(p.startColor.g * 255);
						colorB = (byte)(p.startColor.b * 255);
						colorA = (byte)(p.startColor.a * p.alphaMultiplier *  255);
					}else{
						colorR = (byte)(p.color.r * 255);
						colorG = (byte)(p.color.g * 255);
						colorB = (byte)(p.color.b * 255);
						colorA = (byte)(p.color.a * p.alphaMultiplier * 255);
					}
				
					if(emitter.useLuminosity){
						lCurveEvaluateValue = Mathf.Min (1f,p.lifetime/emitter.colorGradientEndTime);
						lLuminosityValue = lLuminosityCurve.Evaluate(lCurveEvaluateValue);
					}else{
						lLuminosityValue = 0;
					}
				
					lExtra[lv0].y = lLuminosityValue;
					lExtra[lv1].y = lLuminosityValue;
					lExtra[lv2].y = lLuminosityValue;
					lExtra[lv3].y = lLuminosityValue;
				}
				
				
				// mesh center accumulation
				currentMesh.center.x += posX;
				currentMesh.center.y += posY;
				currentMesh.center.z += posZ;		
				
				// position
				lVerts[lv0].x = posX + bs0x * size;
				lVerts[lv0].y = posY + bs0y * size;
				lVerts[lv0].z = posZ + bs0z * size;
				
				lVerts[lv1].x = posX + bs1x * size;
				lVerts[lv1].y = posY + bs1y * size;
				lVerts[lv1].z = posZ + bs1z * size;
				
				lVerts[lv2].x = posX + bs2x * size;
				lVerts[lv2].y = posY + bs2y * size;
				lVerts[lv2].z = posZ + bs2z * size;
				
				lVerts[lv3].x = posX + bs3x * size;
				lVerts[lv3].y = posY + bs3y * size;
				lVerts[lv3].z = posZ + bs3z * size;
				
				// color
				lColors[lv0].r   = colorR;
				lColors[lv0].g   = colorG;
				lColors[lv0].b   = colorB;
				lColors[lv0].a   = colorA;
				
				lColors[lv1].r   = colorR;
				lColors[lv1].g   = colorG;
				lColors[lv1].b   = colorB;
				lColors[lv1].a   = colorA;
				
				lColors[lv2].r   = colorR;
				lColors[lv2].g   = colorG;
				lColors[lv2].b   = colorB;
				lColors[lv2].a   = colorA;
				
				lColors[lv3].r   = colorR;
				lColors[lv3].g   = colorG;
				lColors[lv3].b   = colorB;
				lColors[lv3].a   = colorA;
							
				// uv tile
				lUVs[lv0].x = particlesData.uvs[0].x;
				lUVs[lv0].y = particlesData.uvs[0].y;
				
				lUVs[lv1].x = particlesData.uvs[1].x;
				lUVs[lv1].y = particlesData.uvs[1].y;
				
				lUVs[lv2].x = particlesData.uvs[2].x;
				lUVs[lv2].y = particlesData.uvs[2].y;
				
				lUVs[lv3].x = particlesData.uvs[3].x;
				lUVs[lv3].y = particlesData.uvs[3].y;	
			
				// uv smoke details
				if(useDetails){
					if(p.startLifetime > 0){
						uvScale = detailsScaling + 0.5f * ageRatio; // scaling anim
						uvOffsetX = uvOffsetY = p.randomSeed;
					}else{
						// immortal particle
						uvScale = detailsScaling;
						uvOffsetX = uvOffsetY = p.randomSeed; // offset anim
					}
										
					uvRotationVectorX = _UVrotationVectorX * uvScale;
					uvRotationVectorY = _UVrotationVectorY * uvScale;
					
					lUVs1[lv0].x = uvOffsetX + uvRotationVectorY;
					lUVs1[lv0].y = uvOffsetY - uvRotationVectorX;
					
					lUVs1[lv1].x = uvOffsetX - uvRotationVectorX;
					lUVs1[lv1].y = uvOffsetY - uvRotationVectorY;
					
					lUVs1[lv2].x = uvOffsetX - uvRotationVectorY;
					lUVs1[lv2].y = uvOffsetY + uvRotationVectorX;
	
					lUVs1[lv3].x = uvOffsetX + uvRotationVectorX;
					lUVs1[lv3].y = uvOffsetY + uvRotationVectorY;
				}
				
				currentMesh.particleIndex ++;
				
			}
			
			// compute Z-Depth center for the bounding box
			currentMesh.boundMin = lCameraPos + lCameraForward * mergedList[startIndex].order;
			currentMesh.boundMax = lCameraPos + lCameraForward * mergedList[endIndex-1].order;
		
			lVerts = null;
			lUVs = null;
			lUVs1 = null;
			lColors = null;

	}
		
	private void UpdateMeshes(){
		updateMeshTime = Time.realtimeSinceStartup;
		visibleMeshesCount = 0;
		
		if(useThread){
			UpdateMeshesThreads(useDetails);
		}else{
			
			if(meshList.Count > 0){
				int i;
				for(i=0;i < meshUpdateCount;i++){
					meshList[i].UpdateMesh(useDetails);
					
					visibleMeshesCount ++;
				}
				
				for(i = meshUpdateCount ; i < meshList.Count ; i++){
					meshList[i].ClearMesh();
				}
			}
			
			
		}
		updateMeshTime = Time.realtimeSinceStartup - updateMeshTime;
		allMeshCleared = meshUpdateCount == 0;
	}
	
	private void UpdateMeshesThreads(bool sendUV1 = true){
		visibleMeshesCount = 0;
		updateMeshTime_Step1 = 0;
		updateMeshTime_Step2 = 0;
		
		if(meshList.Count > 0){
			updateMeshTime_Step1 = Time.realtimeSinceStartup;
			int i,j;
			int threadAdded = 0;
			
			taskList.Clear();
			
			for(i=0;i < meshUpdateCount;i++){
				
				var grp = i;
				taskList.Add(UnityThreadHelper.TaskDistributor.Dispatch(() => meshList[grp].UpdateMesh_Step1() ));
				threadAdded++;
				visibleMeshesCount ++;
				
				// wait for threads to finish
				if(threadAdded == _coresCount){
					for(j = 0 ; j < threadAdded ; j++){
						taskList[j].Wait();
						taskList[j].Dispose();
					}
					threadAdded = 0;
					taskList.Clear();
				}
			}
			
			
			for(j = 0 ; j < threadAdded ; j++){
				taskList[j].Wait();
				taskList[j].Dispose();
			}
			threadAdded = 0;
			taskList.Clear();
			updateMeshTime_Step1 = Time.realtimeSinceStartup - updateMeshTime_Step1;
			
			
			updateMeshTime_Step2 = Time.realtimeSinceStartup;
			for(i=0;i < meshUpdateCount;i++){
				meshList[i].UpdateMesh_Step2(sendUV1);
			}
			updateMeshTime_Step2 = Time.realtimeSinceStartup - updateMeshTime_Step2;
			
			for(i = meshUpdateCount ; i < meshList.Count ; i++){
				meshList[i].ClearMesh();
			}
		}

		allMeshCleared = meshUpdateCount == 0;
		
	}
		
	
	private class SortGroup{
		public int index;
		public int count;
		public double sortTime;
		public SortIndex[] items;
		
		public SortGroup(){
			int total = Puffy_Renderer.particlesPerCore_ChunkSize;
			items = new SortIndex[total];
			
			for(int i=0; i< total;i++){
				items[i] = new SortIndex();	
			}
		}
	}
	
	private class SortIndex{
		public int emitterIndex;
		public int particleIndex;
		//public int textureTileIndex = 0;
		public Vector2[] uvs = new Vector2[4];
		public float order;
		
		
	}
	
	private class OrderComparer : IComparer<SortIndex>
	{
		public int Compare(SortIndex a, SortIndex b)
		{
			// sort back to front
			return (a.order > b.order)?-1:1;
		}
	}
			
	private class VariableMesh{
		
		public static int maxParticlesCount = 4096;
				
		private List<VariableMeshData> meshData = new List<VariableMeshData>();
		
		public Vector3 boundMin = Vector3.zero;
		public Vector3 boundMax = Vector3.zero;
		
		public GameObject gameObject;
		public Mesh mesh;
		public Vector3 center = Vector3.zero;		
		

		public int meshDataIndex = 0;
		public int particleIndex = 0;
		
		public double updateTime = 0f;

		public Transform _transform;
		private bool needUpdate = true;
		
		public VariableMesh(int n=0 , Material mat = null, int maxCount = -1){
			
			if(maxCount > 0) maxParticlesCount = maxCount;
			
			// create multiple mesh buffers with different vertex counts
			int step = Mathf.FloorToInt(maxParticlesCount / 8);
			int i = step;
			
			while( i <= maxParticlesCount){
				meshData.Add(new VariableMeshData(Mathf.Min(i,maxParticlesCount)));
				i += step; 
			}
			
			mesh = new Mesh();
			
			mesh.MarkDynamic();
			
			gameObject = new GameObject();
			gameObject.name = "Puffy_MESH_" + n.ToString();
			MeshFilter mf = gameObject.AddComponent<MeshFilter>() as MeshFilter;
			mf.sharedMesh = mesh;
			
			MeshRenderer mr = gameObject.AddComponent<MeshRenderer>() as MeshRenderer;
			mr.sortingLayerName = "foreground";
			mr.sharedMaterial = mat;
			
			gameObject.GetComponent<Renderer>().receiveShadows = false;
			gameObject.GetComponent<Renderer>().castShadows = true;
			gameObject.GetComponent<Renderer>().sortingLayerName = "foreground";
			
			_transform = gameObject.transform;
		}
		
		// get the meshdata index closest to the current particle count
		public int getMeshDataIndex(int particleCount){
			meshDataIndex = 0;
			while(meshData[meshDataIndex].particleCount < particleCount){
				meshDataIndex++;
				if(meshDataIndex >= meshData.Count){
					meshDataIndex = meshData.Count-1;
					break;
				}
			}
			return meshDataIndex;
		}
		
		public VariableMeshData getMeshData(int particleCount){
			int i = getMeshDataIndex(particleCount);
			
			if(i < 0) return null;
			//maxCount = meshData[i].particleCount;
			return meshData[i];
		}
		
		public VariableMeshData Init(int particleCount){
			particleIndex = 0;
			center = Vector3.zero;
			
			needUpdate = true;
			return getMeshData(particleCount);
		}
			
		
		public double ClearMesh(){
			updateTime = Time.realtimeSinceStartup;
			
			mesh.Clear(false);
			mesh.RecalculateBounds();
			particleIndex = 0;
			
			updateTime = Time.realtimeSinceStartup - updateTime;
			return updateTime;
		}
		
		public void UpdateMesh(bool sendUV1 = true){
			
			if(needUpdate && particleIndex > 0){
								
				needUpdate = false;
				
				bool sameMesh = mesh.triangles.Length == meshData[meshDataIndex].triangles.Length;
				
				if(!sameMesh) mesh.Clear(false);
				
				if(center.sqrMagnitude > 0 && particleIndex>0){
					center /= particleIndex;
				}
				
				int i;
				int end = particleIndex * 4;
				int cnt = meshData[meshDataIndex].vertexCount;
				Vector3[] lVerts=null;
				bool test = true;
				if(test){
					// local variable for faster access
					lVerts = meshData[meshDataIndex].vertices;
					
					float posX = center.x;
					float posY = center.y;
					float posZ = center.z;
									
					// update all used vertices to reflect the center offset
					for(i = 0 ; i < end ; i++){
						lVerts[i].x -= posX;
						lVerts[i].y -= posY;
						lVerts[i].z -= posZ;
					}
					
					// unused vertices are moved to the position of the first vertex
					posX = lVerts[0].x;
					posY = lVerts[0].y;
					posZ = lVerts[0].z;
					
					for(i = end ; i < cnt ; i++){
						lVerts[i].x = posX;
						lVerts[i].y = posY;
						lVerts[i].z = posZ;
					}
				}else{
					//**********************************************************************************
					
					float posX = center.x;
					float posY = center.y;
					float posZ = center.z;
									
					// update all used vertices to reflect the center offset
					for(i = 0 ; i < end ; i++){
						meshData[meshDataIndex].vertices[i].x -= posX;
						meshData[meshDataIndex].vertices[i].y -= posY;
						meshData[meshDataIndex].vertices[i].z -= posZ;
					}
					
					// unused vertices are moved to the position of the first vertex
					posX = meshData[meshDataIndex].vertices[0].x;
					posY = meshData[meshDataIndex].vertices[0].y;
					posZ = meshData[meshDataIndex].vertices[0].z;
					
					for(i = end ; i < cnt ; i++){
						meshData[meshDataIndex].vertices[i].x = posX;
						meshData[meshDataIndex].vertices[i].y = posY;
						meshData[meshDataIndex].vertices[i].z = posZ;
					}
				}
				//**********************************************************************************
				
				
				// update mesh data
				mesh.MarkDynamic();
				if(test){
					mesh.vertices = lVerts;
				}else{
					mesh.vertices = meshData[meshDataIndex].vertices;
				}
				
				mesh.uv = meshData[meshDataIndex].uvs;
				if(sendUV1) mesh.uv2 = meshData[meshDataIndex].uvs1;
				mesh.colors32 = meshData[meshDataIndex].colors;
				mesh.normals = meshData[meshDataIndex].extraData;
				 
				if(!sameMesh) mesh.triangles = meshData[meshDataIndex].triangles;
				
				// move the gameobject
				_transform.position = center;
				
				// force custom bounds to fix the Z fighting between meshes
				mesh.bounds = new Bounds((boundMax+boundMin)*.5f - center,Vector3.one);
								
				lVerts = null;
			}
		}
		
		public void UpdateMesh_Step1(){
			
			if(needUpdate && particleIndex > 0){
					
				if(center.sqrMagnitude > 0 && particleIndex>0){
					center /= particleIndex;
				}
				
				int i;
				int end = particleIndex * 4;
				int cnt = meshData[meshDataIndex].vertexCount;
				
				float posX = center.x;
				float posY = center.y;
				float posZ = center.z;
				
				bool test = true;
				
				if(test){
					// local variable for faster access
					Vector3[] lVerts = meshData[meshDataIndex].vertices;
				
					// update all used vertices to reflect the center offset
					for(i = 0 ; i < end ; i++){
						lVerts[i].x -= posX;
						lVerts[i].y -= posY;
						lVerts[i].z -= posZ;
					}
					
					// unused vertices are moved to the position of the first vertex
					posX = lVerts[0].x;
					posY = lVerts[0].y;
					posZ = lVerts[0].z;
					
					for(i = end ; i < cnt ; i++){
						lVerts[i].x = posX;
						lVerts[i].y = posY;
						lVerts[i].z = posZ;
					}
					lVerts = null;
				}else{
				
					// update all used vertices to reflect the center offset
					for(i = 0 ; i < end ; i++){
						meshData[meshDataIndex].vertices[i].x -= posX;
						meshData[meshDataIndex].vertices[i].y -= posY;
						meshData[meshDataIndex].vertices[i].z -= posZ;
					}
					
					// unused vertices are moved to the position of the first vertex
					posX = meshData[meshDataIndex].vertices[0].x;
					posY = meshData[meshDataIndex].vertices[0].y;
					posZ = meshData[meshDataIndex].vertices[0].z;
					
					for(i = end ; i < cnt ; i++){
						meshData[meshDataIndex].vertices[i].x = posX;
						meshData[meshDataIndex].vertices[i].y = posY;
						meshData[meshDataIndex].vertices[i].z = posZ;
					}
					
				}
				
			}
		}
		
		public void UpdateMesh_Step2(bool sendUV1 = true){
			if(needUpdate && particleIndex > 0){
				needUpdate = false;
				
				bool sameMesh = mesh.triangles.Length == meshData[meshDataIndex].triangles.Length;
				
				if(!sameMesh) mesh.Clear(false);
				
				//mesh.MarkDynamic();
				mesh.vertices = meshData[meshDataIndex].vertices;
				mesh.uv = meshData[meshDataIndex].uvs;
				if(sendUV1) mesh.uv2 = meshData[meshDataIndex].uvs1;
				mesh.colors32 = meshData[meshDataIndex].colors;
				mesh.normals = meshData[meshDataIndex].extraData;
	
				if(!sameMesh) mesh.triangles = meshData[meshDataIndex].triangles;
				
				// move the gameobject
				_transform.position = center;
				
				// force custom bounds to fix the Z fighting between meshes
				mesh.bounds = new Bounds( (boundMax+boundMin) * 0.5f - center , Vector3.one);
			}
		}
			
	}
	
	private class VariableMeshData{
		
		public int particleCount = 0;
		public int vertexCount = 0;
		public int[] triangles;
		public Vector2[] uvs;
		public Vector2[] uvs1;
		public Vector3[] vertices;
		public Vector3[] extraData;
		public Color32[] colors;
		
		public VariableMeshData(int _particleCount){
			
			particleCount = _particleCount;
			
			vertexCount = particleCount * 4;
			
			vertices = new Vector3[vertexCount];
			uvs = new Vector2[vertexCount];
			uvs1 = new Vector2[vertexCount];
			colors = new Color32[vertexCount];
			extraData = new Vector3[vertexCount];
			
			triangles = new int[particleCount * 6];
			
			int v,j,k;
			
			for(v = 0 ; v < particleCount ; v++){
				
				j = v*4;
				k = v*6;
				
				triangles[k]   = j;
				triangles[k+1] = j+1;
				triangles[k+2] = j+2;
				triangles[k+3] = j;
				triangles[k+4] = j+2;
				triangles[k+5] = j+3;
		
				uvs1[j]   = new Vector2(-0.5f,-0.5f); // bottom left
				uvs1[j+1] = new Vector2(0.5f,-0.5f); // bottom right
				uvs1[j+2] = new Vector2(0.5f,0.5f); // top right
				uvs1[j+3] = new Vector2(-0.5f,0.5f); // top left
			}
		}
	}
}




public class DebugTimer{
	private float[] times = new float[50];
	
	private double timestamp;
	
	public float elapsed;
	public float maximum;
	public float minimum;
	
	public void Start(){
		timestamp = Time.realtimeSinceStartup;
	}
	
	public void Stop(){
		timestamp = Time.realtimeSinceStartup - timestamp;
		elapsed = (float)(timestamp*1000);
		times[49] = elapsed;
		for(int i = 0; i < 49; i++){
			times[i] = times[i+1];
		}
		
	}
	
	public float Average(){
		float a = 0f;
		maximum = 0f;
		minimum = 99999;
		for(int i = 0; i < 50; i++){
			a += times[i];
			maximum = Mathf.Max (maximum,times[i]);
			minimum = Mathf.Min (minimum,times[i]);
		}
		
		return a/50f;
	}
	
	public void ShowGraph(float offsetY = 0f){
		
		GL.Begin(GL.LINES); 
		Vector3 pos = Camera.main.transform.position + Camera.main.transform.forward*3 + Camera.main.transform.up*offsetY;
		Vector3 up = Camera.main.transform.up;
		Vector3 right = Camera.main.transform.right;
		
		Color[] colors = new Color[5];
		
		colors[0] = new Color(1f,1f,1f,1f);
		colors[1] = new Color(0f,1f,0f,1f);
		colors[2] = new Color(1f,1f,0f,1f);
		colors[3] = new Color(1f,0.5f,0f,1f);
		colors[4] = new Color(1f,0f,0f,1f);
		
		GL.Color(new Color(1f,1f,1f,0.5f));
				
		GL.Vertex(pos);
		GL.Vertex(pos + right * 0.25f);
		
		GL.Vertex(pos + up / 50f);
		GL.Vertex(pos + up / 50f + right * 0.25f);
		
		GL.Vertex(pos + (up*2) / 50f);
		GL.Vertex(pos + (up*2) / 50f + right * 0.25f);
		
		pos = Camera.main.transform.position + Camera.main.transform.forward*3 + Camera.main.transform.up*offsetY;

		int c = 0;
		for(int i = 0; i < 50; i++){
			
			c = Mathf.Min (4,Mathf.FloorToInt(times[i]));
			GL.Color(colors[c]);
			GL.Vertex( pos);
	    	GL.Vertex( pos + up * times[i]/50f );
			pos += right*0.005f;

		}
		GL.End();
		
	}
}
