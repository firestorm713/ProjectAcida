using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour {
	
	public AudioClip song1;
	public AudioClip song2;
	public float timer=0;

	// Use this for initialization
	void Start () {
		
	audio.clip = song1;

//	print (song1.length+song2.length);	
	}
	
	// Update is called once per frame
	void Update () 
	{
	 	
		 if(!audio.isPlaying)
		 {
			audio.Play();
		 }
         timer += Time.deltaTime;
      	 if(timer > song1.length)
		 {
			audio.clip = song2;
		 } 
		 if(timer > (song1.length+song2.length)) 
		{
			audio.clip = song1;
			timer = 0;
		}
	
	}
	
}
