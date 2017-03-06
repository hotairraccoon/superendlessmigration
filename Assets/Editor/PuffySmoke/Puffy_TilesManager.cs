using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class Puffy_TilesManager : EditorWindow
{
	public Texture2D TileMap;
	public int TilesX = 16;
	public int TilesY = 4;
		
	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Puffy Tiles Manager")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(Puffy_TilesManager));
	}

	void OnGUI()
	{
		TileMap = (Texture2D) EditorGUILayout.ObjectField("Image", TileMap, typeof (Texture2D), false);
		TilesX = EditorGUILayout.IntField("Col count",TilesX);
		TilesY = EditorGUILayout.IntField("Row count",TilesY);
		
		if(GUILayout.Button("Convert")) Build ();
	}
	
	public void Build(){
		int i,j,k;
		int px,py;
		int step = TileMap.width / TilesX;
		
		k = 0;
		int sz = Mathf.FloorToInt(Mathf.Sqrt(TilesX*TilesY))*step;
		Texture2D output = new Texture2D(sz,sz);
		Debug.Log (sz);
		
		int ox = 0;
		int oy = 0;
		py = 0;
		for(j=0;j<TilesY;j++){
			px = 0;
			for(i=0;i<TilesX;i++){
			
				output.SetPixels(ox,oy,step,step,TileMap.GetPixels(px,py,step,step));
				ox += step;
				if(ox >= sz){
					ox = 0;
					oy += step;
				}

				px += step;
				k++;
				
				
			}
			py += step;
		}
		
		SaveTextureToFile(output,TileMap.name+".png");
	}
	
	private void SaveTextureToFile(Texture2D texture, string fileName)
	{
		byte[] bytes = texture.EncodeToPNG();
		string filePath = Application.dataPath + "/" + fileName;
		Debug.Log (filePath);
		
		FileStream fileStream = new FileStream(filePath, FileMode.Create);
        BinaryWriter binWriter = new BinaryWriter(fileStream);
        binWriter.Write(bytes);
        binWriter.Close();
        fileStream.Close(); 
	}
}
