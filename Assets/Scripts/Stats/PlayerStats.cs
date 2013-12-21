using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {
	
	public float fCurrentArmor = 1;
	public float fMaxArmor = 100;
	public float fCurrentShields = 1;
	public float fMaxShields = 100;
	public float fCurrentEnergy = 1;
	public float fMaxEnergy = 100;
	public float fEnergyCharge = 3;
	public float fEnergyUse = 5;
	public float fCollisionDmg = 25;
	public bool bShieldCharging = true;
	public float fShieldDownTime = 3;
	public float fArmorDamage = 25;
	public bool bPlayerDead = false;
	public float fDeathDelay = 3;
	GameObject player;
	
	// Use this for initialization
	void Start () 
	{
		player = GameObject.Find("Player");

	}
	
	// Update is called once per frame
	void Update () {
			
		EnergyRecharge();
		if(bShieldCharging==true)
		{
			ShieldsRecharge();
		}
		else
		{
			if(fShieldDownTime > 0)
			{
				fShieldDownTime = fShieldDownTime - Time.deltaTime;
			}
			else if (fShieldDownTime <= 0)
			{
				bShieldCharging = true;
			}
			
		}
		
		if(bPlayerDead==true)
		{
			if(fDeathDelay > 0)
			{
				fDeathDelay = fDeathDelay - Time.deltaTime;
			}
			else
			{
				bPlayerDead = false;
				Application.LoadLevel(2);
			}
		}
		

	}
	
	public void EnergyRecharge() 
	{
		
		
			if(fCurrentEnergy > fMaxEnergy) 
				{
				fCurrentEnergy = fMaxEnergy;
	//			print ("hit 100");
				}
			
			else
				{
				fCurrentEnergy += Time.deltaTime * fEnergyCharge;
				}
	
	
			
			
		}

	
	
	public void EnergyDrain() 
	{
		
		if(fCurrentEnergy < 1)
		{
			fCurrentEnergy = 0;
		//	print ("hit 0");	
		}
		else 
		{
			fCurrentEnergy = fCurrentEnergy - (Time.deltaTime * fEnergyUse);
		}
		
		
	}
	
	public void ShieldsRecharge() 
	{
		if(fCurrentShields > fMaxShields) 
			{
			fCurrentShields = fMaxShields;
	//			print ("hit 100");
			}
		else
		{
			fCurrentShields += Time.deltaTime * fEnergyCharge;
		}
	}	
	public void ShieldDamage()
	{
		if(fCurrentShields > 0)
		{
			fCurrentShields = fCurrentShields - (Time.deltaTime * fCollisionDmg);	
			
		}
	}
	
	public void ArmorDamage() 
	{
		
		if(fCurrentArmor < 1)
		{
			fCurrentArmor = 0;
			print("player died");
				
			
		}
		else 
		{
			fCurrentArmor = fCurrentArmor - (fArmorDamage * Time.deltaTime);
		}
		
		
	}	

}
