using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {
	float fCurrentArmor; 
	float fCurrentEnergy;
	float fCurrentShields;
	float fMaxArmor;
	float fMaxShields;
    float fMaxEnergy;
	
	int ArmorBarHeight = 35;
	int ShieldBarHeight = 35;
	int EnergyBarHeight = 35;
	
	GameObject player;
	public GUIStyle m_sGreenHealthBar;
	public GUIStyle m_sRedHealthBar;
	public GUIStyle m_sShieldBar;
	public GUIStyle m_sEnergyBar;
	public GUIStyle m_sLabel;
	
	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
		PlayerStats sPlayerStats;
    	sPlayerStats = player.GetComponent<PlayerStats>();
		fMaxShields = sPlayerStats.fMaxShields;
		fMaxEnergy = sPlayerStats.fMaxEnergy;
		fMaxArmor = sPlayerStats.fMaxArmor;
	}
		


    void OnGUI() {
 		float BarY=10;
	    float BarX=10;
	 	// Armor
		m_sLabel.normal.textColor = Color.black;
        GUI.Button(new Rect(BarX, BarY,fMaxArmor, ArmorBarHeight), "",m_sRedHealthBar);	
        GUI.Button(new Rect(BarX, BarY, fCurrentArmor, ArmorBarHeight), "",m_sGreenHealthBar);
		GUI.Label(new Rect(BarX, BarY+5, fMaxArmor, 20), "Armor",m_sLabel);
		
		// Shields
		m_sLabel.normal.textColor = Color.white;
		BarY += (ArmorBarHeight + 10);
	//	BarX = (Screen.width / 2) - (fMaxShields/2);
        GUI.Button(new Rect(BarX, (BarY),fMaxShields, ShieldBarHeight), "",m_sRedHealthBar);
        GUI.Button(new Rect(BarX, (BarY), fCurrentShields, ShieldBarHeight), "",m_sShieldBar);
		GUI.Label(new Rect(BarX, BarY+5, fMaxShields, 20), "Shields",m_sLabel);
		
		// Energy 
		m_sLabel.normal.textColor = Color.black;
		BarY+=(ShieldBarHeight + 10);
//		BarX = (Screen.width - fMaxEnergy - 10);
        GUI.Button(new Rect(BarX, (BarY),fMaxEnergy, EnergyBarHeight), "",m_sRedHealthBar);
        GUI.Button(new Rect(BarX, (BarY), fCurrentEnergy, EnergyBarHeight), "",m_sEnergyBar);	
		GUI.Label(new Rect(BarX, BarY+5, fMaxEnergy, 20), "Energy",m_sLabel);
		
        
    }
	void Update () {
		PlayerStats sPlayerStats;
    	sPlayerStats = player.GetComponent<PlayerStats>();
		fCurrentArmor = sPlayerStats.fCurrentArmor;
		fCurrentEnergy = sPlayerStats.fCurrentEnergy;
		fCurrentShields = sPlayerStats.fCurrentShields;

	}
}
