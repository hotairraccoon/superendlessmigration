using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Puffy_MeshSpawner : Puffy_ShapeSpawner {
	
	public bool smoothNormals = true;
	
	
	private Mesh mesh;
	private bool hasColors = false;
	
	void Start(){
		
		mesh = GetComponent<MeshFilter>().sharedMesh;
		
		hasColors = mesh.colors.Length>0;
		
		emitter.shapeSpawner = this;
		Init();
	}
	
	public override void Init(){
		if(smoothNormals){
			Smooth();
		}else{
			vertices = mesh.vertices;
			normals = mesh.normals;
			colors = mesh.colors;
			lastSpawnPosition = new Vector3[vertices.Length];
			for(int i=0;i<vertices.Length;i++){
				lastSpawnPosition[i] = _transform.TransformPoint(mesh.vertices[i]);	
				lastSpawnDirection[i] = _transform.TransformPoint(mesh.normals[i]);
			}
		}
	}
	
	void Smooth(){
		Dictionary<string,vertexData> dataList = new Dictionary<string,vertexData>();
		
		string key;
		int i;
		int vertexCount = mesh.vertexCount;
		
		for(i=0; i < vertexCount;i++){
			key = mesh.vertices[i].ToString();
			if(dataList.ContainsKey(key)){
				dataList[key].normal += mesh.normals[i];
				if(hasColors) dataList[key].color += mesh.colors[i];
				dataList[key].count++;
			}else{
				if(hasColors){
					dataList.Add (key,new vertexData(mesh.vertices[i],mesh.normals[i],mesh.colors[i]));
				}else{
					dataList.Add (key,new vertexData(mesh.vertices[i],mesh.normals[i],Color.white));
				}
			}
		}
		int dataCount = dataList.Count;
		
		vertices = new Vector3[dataCount];
		normals = new Vector3[dataCount];
		colors = new Color[dataCount];
		lastSpawnPosition = new Vector3[dataCount];
		lastSpawnDirection = new Vector3[dataCount];
		
		i=0;
		foreach(KeyValuePair<string,vertexData> n in dataList){
			vertices[i] = n.Value.vertex;
			lastSpawnPosition[i] = _transform.TransformPoint(vertices[i]);
			
			normals[i] = n.Value.normal / n.Value.count;
			if(hasColors){
				colors[i] = n.Value.color / n.Value.count;
			}
			
			lastSpawnDirection[i] = _transform.TransformPoint(normals[i]);
			
			i++;
		}
		
		dataList = null;
	}
	
	class vertexData{
		public Vector3 vertex = Vector3.zero;
		public Vector3 normal = Vector3.zero;
		public Color color = Color.white;
		public int count = 1;
			
		public vertexData(Vector3 v, Vector3 n, Color c){
			vertex = v;
			normal = n;
			color = c;
			count = 1;
		}
	}
}
