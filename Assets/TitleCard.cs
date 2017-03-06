using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleCard : MonoBehaviour {
	
	public static TitleCard current;
	
	public float timeOnScreen = 10f;
	
	private Controller controller;
	private bool activated;
	
	Animator cardAnimator;

	public bool triggered = false;
	public TriggerListener triggerListener;
	
	private Text msgText;
	
	void Awake () {
		current = this;
		msgText = GameObject.Find ("NotificationText").GetComponent<Text>();
	}
	
	// Use this for initialization
	void Start () {
		controller = Controller.current;
		cardAnimator = GetComponent<Animator>();
		cardAnimator.speed = 0f;
		
		if (Application.loadedLevelName == "scene_city"){
			Invoke ("ShowCard", timeOnScreen);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		
	}
	
	void ShowCard () {
		activated = true;
		cardAnimator.speed = 1f;
		cardAnimator.Play("ui_slideIn");
		
		Invoke ("HideCard", timeOnScreen);
		
	}
	
	void HideCard () {
		cardAnimator.Play("ui_slideOut");
	}
	
	public void Message(string msg) {
		msgText.text = msg;
		CancelInvoke ("ShowCard");
		CancelInvoke ("HideCard");
		Invoke ("ShowCard", 0);
	}
}
