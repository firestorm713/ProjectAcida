using UnityEngine;
using System.Collections;

public class FadeTexture : MonoBehaviour {
 
private GUITexture gt;
private float _alpha = 0F;
private bool fadeIn = false;
 
// Use this for initialization
void Start() 
{
	int iconwidth = 960;
	int iconheight = 540;
	int centerx = Screen.width/2;
	int centery = (Screen.height/2)+200;
    gt = (GUITexture) gameObject.GetComponent(typeof(GUITexture));
	gt.pixelInset = new Rect(centerx-(iconwidth/2),centery-(iconheight/2),iconwidth,iconheight);
}
 
// Update is called once per frame
void Update() 
{
    print(_alpha);
 
    if(fadeIn)
    {
        _alpha = Mathf.Lerp(_alpha, 1F, Time.deltaTime*0.15F);
        gt.color = new Color(.5F,.5F,.5F,_alpha);
        if(_alpha > .98F) fadeIn = false; 
    }
    else
    {
        _alpha = Mathf.Lerp(_alpha, 0F, Time.deltaTime);
        gt.color = new Color(.5F,.5F,.5F,_alpha);
        if(_alpha < .01F) fadeIn = true; 
    }                   
}
}
