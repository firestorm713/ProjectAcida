using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour 
{
	Vital Armor;
	Vital Shields;
	Vital Energy;
	
	public float BaseArmor = 100;
	public float BaseShields = 100;
	public float BaseEnergy = 100;
	
	public float curShields;
	public float curEnergy;
	public float curArmor;
	

	private bool bShieldDelayed=false;
	public float ShieldRechargeDelay = 2.0f;
	
	public float RechargeRate;
	
	IEnumerator ShieldDelay()
	{
		bShieldDelayed = true;
		yield return new WaitForSeconds(ShieldRechargeDelay);
		bShieldDelayed = false;
	}

	void Awake () 
	{
		Armor = new Vital();
		Shields = new Vital();
		Energy = new Vital();
		Armor.BaseValue = Armor.CurValue = BaseArmor;
		Shields.BaseValue = Shields.CurValue = BaseShields;
		Energy.BaseValue = Energy.CurValue = BaseEnergy;
		curShields = Shields.CurValue;
		curEnergy = Energy.CurValue;
		curArmor = Armor.CurValue;
	}

	
	// Update is called once per frame
	void Update () 
	{
		Armor.AdjustCurrentValue(0);
		Shields.AdjustCurrentValue(0);
		Energy.AdjustCurrentValue(0);	
		if(Shields.CurValue<Shields.AdjustedBaseValue)
			StartCoroutine(ShieldDelay());
		Recharge (Energy);
		if(!bShieldDelayed)
			Recharge(Shields);
		curShields = Shields.CurValue;
		curEnergy = Energy.CurValue;
	}
	
	public void Recharge(Vital v)
	{
		if(v.CurValue<v.BaseValue)
			v.AdjustCurrentValue(RechargeRate*Time.deltaTime);
	}
}