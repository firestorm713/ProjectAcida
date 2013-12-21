using UnityEngine;
using System.Collections;

public class Collide : MonoBehaviour {
	
	GameObject player;
	GameObject shieldparticles;
	GameObject deathparticles;
	GameObject hull;
	GameObject wings;
	GameObject nose;
	GameObject nose2;
	public GameObject newPlayer;
	public AudioClip aShieldSound;
	public AudioClip aArmorSound;
	public AudioClip aDieSound;

	
	// Use this for initialization
	void Start () {
	player = GameObject.Find("Player");
	shieldparticles = GameObject.Find("shieldparticle");
	deathparticles = GameObject.Find("deathparticle");	
	hull = GameObject.Find("hull2");
	wings = GameObject.Find("wingsB");
	nose = GameObject.Find("cockpitB");
	nose2 = GameObject.Find("cockpit");

	
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter(Collision collision)
	{
		PlayerStats sPlayerStats;
    	sPlayerStats = player.GetComponent<PlayerStats>();
		if (sPlayerStats.fCurrentShields > 0 )
		{
			sPlayerStats.ShieldDamage();
			sPlayerStats.bShieldCharging = false;
			sPlayerStats.fShieldDownTime = 3;
			audio.clip = aShieldSound;
			if(!audio.isPlaying) audio.Play();
			shieldparticles.particleSystem.Play();
		}
		else if(sPlayerStats.fCurrentArmor > 0)
		{	
			sPlayerStats.ArmorDamage();
			sPlayerStats.bShieldCharging = false;
			sPlayerStats.fShieldDownTime = 3;
			audio.clip = aArmorSound;
			if(!audio.isPlaying) audio.Play();
		}
		else
		{
			audio.clip = aDieSound;
			if(!audio.isPlaying) audio.Play();
			hull.renderer.enabled = false;
			wings.renderer.enabled = false;
			nose.renderer.enabled = false;
			nose2.renderer.enabled = false;
			deathparticles.particleSystem.Play();
			sPlayerStats.fDeathDelay = 3;
			sPlayerStats.bPlayerDead = true;
	
		}
		
		
	}
	
}
