using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PathWalker : MonoBehaviour {
	public int duration = 15;
	public string pathName = "";
	public PathType type = PathType.CatmullRom;
	public PathMode mode = PathMode.TopDown2D;
	public int resolution = 10;
	public Color color = Color.black;

	private Vector3 lastLocation;
	private Vector3[] path;
	void Start() {
		lastLocation = transform.position;
		if(pathName.Equals("")) {
			iTweenPath ipath = (iTweenPath)gameObject.GetComponent(typeof(iTweenPath));
			path = ipath.nodes.ToArray();
		} else {
			path = iTweenPath.GetPath(pathName);
		}
	}

	void OnScreenTriggerEvent (string name) {
		Debug.Log (" screen has caused us to move: " + name);
		transform.DOPath (path, duration, type, mode, resolution, color);
	}
	// Update is called once per frame
	void Update () {


		Vector3 moveDirection = gameObject.transform.position - lastLocation; 
		float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		lastLocation = transform.position;

	}
}
