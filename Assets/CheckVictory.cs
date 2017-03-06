using UnityEngine;
using System.Collections;

public class CheckVictory : MonoBehaviour {
	
	public string nextScene;
	public TriggerContainer triggerContainer;
	
	// Use this for initialization
	
	void Start () 
	{
		triggerContainer = GetComponent<TriggerContainer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
			if (triggerContainer.triggered == true)
		{
			Controller.current.nextLevelName = nextScene;
			
			for (int i = 0; i < Controller.current.flockBirds.Count; i++) 
			{
				DontDestroyOnLoad(Controller.current.flockBirds[i]);
			}
			
			Application.LoadLevel("scene_wormhole");
		}
	}
}
