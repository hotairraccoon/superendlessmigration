using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DynamicText : MonoBehaviour {

	public Text textField;
	public List<string> textOptions;
	
	void Start () {
		
		textField = gameObject.GetComponent<Text>();
		
		textOptions.Add("Canada goose lay\nsnugly in marshy nest\nwe hear the ever hronk.");
		textOptions.Add("Coastal and lagoon\nsits calm brown pelican\nfish, sudden plunge.");
		textOptions.Add("Is there a mute swan?\nGrace like the dancer, Odette.\nThe bird mimics the girl.");
		textOptions.Add("Beauty found the male bird.\nDull color grosbeak female,\ncall a sharp iik iik.");
		textOptions.Add("Song a very sweet\nbuzz, Spizella arborea\nshe swoops by me.");
		textOptions.Add("We wander woods late\ndizzy in the dark, alas\nWarbler be my guide.");
		textOptions.Add("Darling starling roost\nthere by me the autumn moon\nshow thee hide away.");
		textOptions.Add("Shall I call you, crow,\nbravely perched upon your hay-man\ndark raven with eyes that see all.");
		textOptions.Add("Horned owl, night feathered\nfriend who speaks of hoo hoododo hoooo.\nSlain warrior reborn.");
		
		setText();
		
	}
	
	
	
	void setText(){
		
		textField.text = textOptions[Random.Range(0, textOptions.Count)];
		
	}

}
