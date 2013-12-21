using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour 
{
	Transform Pcamera;
	Vector3 PCameraPos;
	Quaternion PlayerRotation;
	Transform PlayerMesh;
	
	Vector3 Thrust;
	float Mass;
	float velX;
	float velZ;
	public float maxVel = 50;
	
	public float speed = 10.0f;
	
//	private Transform myTransform;
	
	Quaternion FixedRotation;
	
	void Awake()
	{
//		myTransform = transform;
		FixedRotation = transform.rotation;
		FixedRotation.x = 90;	
	}

	// Use this for initialization
	void Start ()
	{
		Pcamera = transform.Find("Main Camera");
		PlayerMesh = GameObject.Find("PlayerMesh").transform;
		PlayerRotation = PlayerMesh.rotation;
		velX = PlayerMesh.rigidbody.velocity.x;
		velZ = PlayerMesh.rigidbody.velocity.z;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		HandleInput();
		PMeshRotate();
		velX = PlayerMesh.rigidbody.velocity.x;
		velZ = PlayerMesh.rigidbody.velocity.z;
		VelocityChecker();
	}
	
	void LateUpdate()
	{
	}
	
	void VelocityChecker()
	{
		if(PlayerMesh.forward.x * PlayerMesh.rigidbody.velocity.x>maxVel)
			velX = maxVel;
		if(PlayerMesh.forward.x * PlayerMesh.rigidbody.velocity.x<-maxVel)
			velX=-maxVel;
		if(PlayerMesh.forward.z * PlayerMesh.rigidbody.velocity.z>maxVel)
			velZ = maxVel;
		if(PlayerMesh.forward.z * PlayerMesh.rigidbody.velocity.z<-maxVel)
			velZ=-maxVel;
		PlayerMesh.rigidbody.velocity = new Vector3(velX, 0, velZ);
	}
	
	void HandleInput()
	{
		if(Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.DownArrow))
		{
			PlayerMesh.rigidbody.AddRelativeForce(0, 0, speed);
		}
		if(Input.GetKey(KeyCode.S))
		{
			PlayerMesh.rigidbody.AddRelativeForce(0, 0, -speed);
		}
		if(Input.GetKey(KeyCode.D))
		{
			PlayerMesh.rigidbody.AddRelativeForce(speed, 0, 0);
		}
		if(Input.GetKey(KeyCode.A))
		{
			PlayerMesh.rigidbody.AddRelativeForce(-speed, 0, 0);
		}
		
	}
	
	void PMeshRotate()
	{
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = -10;
		Quaternion temprotation = Quaternion.LookRotation(PlayerMesh.position - Pcamera.camera.ScreenToWorldPoint(mousePos), Vector3.up);
		PlayerRotation = Quaternion.Slerp(PlayerRotation, temprotation, Time.deltaTime * 2.5f);
		PlayerRotation.z = 0;
		PlayerRotation.x = 0;
		PlayerMesh.rigidbody.MoveRotation(PlayerRotation);
	}
}