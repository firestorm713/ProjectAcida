using UnityEngine;
using System.Collections;


public class TitleScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	void OnGUI()
	{
		
		int buttonwidth = 300;
		int buttonheight = 40;
		int menuwidth = 325;
		int menuheight = 250;
		int centerx = Screen.width/2;
		int centery = (Screen.height/2);
		int nexty = centery+buttonheight;
		GUI.skin.box.fontSize = 32;
		GUI.skin.button.fontSize = 24;
		GUI.Box (new Rect (centerx-(menuwidth/2),centery-(menuheight/2)+50,menuwidth,menuheight), "Main Menu");
		if(GUI.Button(
			new Rect(
				(centerx) - buttonwidth/2,
				(centery+15) - buttonheight/2,
				buttonwidth, buttonheight), 
			"Play Game"))
			Application.LoadLevel(1);
		if(GUI.Button(
			new Rect(
				(centerx) - buttonwidth/2,
				(nexty+15 + buttonheight),
				buttonwidth,buttonheight),
			"Exit")) 
			Application.Quit();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
