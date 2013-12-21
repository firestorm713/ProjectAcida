using UnityEngine;
using System.Collections;


public class GameOver : MonoBehaviour {

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
		GUI.Box (new Rect (centerx-(menuwidth/2),centery-(menuheight/2)+25,menuwidth,menuheight), "");
		if(GUI.Button(
			new Rect(
				(centerx) - buttonwidth/2,
				(centery+15) - buttonheight/2,
				buttonwidth, buttonheight), 
			"Play Again"))
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
