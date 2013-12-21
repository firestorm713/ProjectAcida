using UnityEngine;
using System.Collections;

public class CollisionLog : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnCollisionEnter(Collision collision)
	{
		Debug.Log ("Collision box: " + collision.collider.gameObject.name + " entered.");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
