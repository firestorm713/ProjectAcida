using UnityEngine;
using System.Collections;

public class Banking: MonoBehaviour
{
	Transform Hull;
	Quaternion HullRotation;
	public float tiltAngle = 0.0f;
	
	
	void Start()
	{
		Hull = GameObject.Find("hull2").transform;	
		//HullRotation = Hull.rotation;
		HullRotation=transform.rotation;
	}
	void shipRotate(float angle)
	{
			//Hull.transform.Rotate(0,0,90);
			//float tiltAroundZ = Input.GetAxis("Horizontal");
			//Quaternion target= Quaternion.Euler(0,0,90.0F);
		Quaternion temp = transform.rotation;
		temp.z = tiltAngle;
			HullRotation= Quaternion.Slerp(HullRotation, temp, Time.deltaTime * 2.5f);
			HullRotation.x = 0;
			transform.rotation = HullRotation;
	}
	void Bank()
	{
		tiltAngle=0.0F;
		if(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow)){
			tiltAngle=90.0F;}
		if(Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow)){
			tiltAngle=-90.0F;}
		shipRotate(tiltAngle);
				
	}
	void Update()
	{
		Bank();
	}
}
