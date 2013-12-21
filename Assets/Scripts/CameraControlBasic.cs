using UnityEngine;
using System.Collections;

public class CameraControlBasic : MonoBehaviour 
{
	public Vector3 myposition;
	public Transform Player;
	public float CameraMoveSpeed = 2.0f;
	Quaternion InitialRot;
	
	void Awake()
	{
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
			Screen.orientation = ScreenOrientation.LandscapeLeft;
		InitialRot = transform.rotation;
	}
	
	// Use this for initialization
	void Start () 
	{
		myposition = transform.position;
		Player = GameObject.Find("PlayerMesh").transform;
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		myposition.x = Player.position.x;
		myposition.z = Player.position.z;
		myposition.y+=Input.GetAxis("Mouse ScrollWheel")*CameraMoveSpeed;
		if(myposition.y > 100)
			myposition.y = 100;
		if(myposition.y < 10)
			myposition.y = 10;
		transform.rotation = InitialRot;
		transform.position = myposition;
	}
}
