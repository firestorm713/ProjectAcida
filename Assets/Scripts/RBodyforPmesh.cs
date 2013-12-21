using UnityEngine;
using System.Collections;

public class RBodyforPmesh : MonoBehaviour 
{
	public float Mass;
	public float Drag;

	// Use this for initialization
	void Start () 
	{
		rigidbody.mass = Mass;
		rigidbody.drag = Drag;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
