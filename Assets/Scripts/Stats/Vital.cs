using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vital
{
	public float BaseValue;
	public float CurValue;
	public List<float> modifiers;
	private float _modValue;
	
	public float AdjustedBaseValue
	{
		get
		{
			CalculateModValue();
			return BaseValue + _modValue;
		}
	}
	
	public void CalculateModValue()
	{
		_modValue =0;
		foreach(float i in modifiers)
			_modValue+=i;
	}
	
	public Vital()
	{
		BaseValue=100;
		CurValue=100;
		modifiers = new List<float>();
	}
	
	public void AdjustCurrentValue (float adj)
	{
		CurValue += adj;
		if(CurValue>BaseValue)
			CurValue=BaseValue;
		if(BaseValue<1)
			BaseValue=1;
		if(CurValue<0)
			CurValue=0;
	}
}