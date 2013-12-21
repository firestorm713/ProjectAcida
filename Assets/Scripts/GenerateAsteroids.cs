using UnityEngine;
using System.Collections;

public class GenerateAsteroids : MonoBehaviour {
	
	Vector3 randomloc;
	public GameObject asteroidPrefab;
	public int NumberOfAsteroids;
	public float RangeFromShip;
	public float AsteroidRadius;
	public float AsteroidRange;
	

	// Use this for initialization
	Vector3 SetRange()
	{
		randomloc.x = UnityEngine.Random.Range(-1*AsteroidRange, AsteroidRange);
		
		if(randomloc.x <= RangeFromShip && randomloc.x >= 0)
			randomloc.x += RangeFromShip;
		if(randomloc.x <= 0 && randomloc.x >= -1*RangeFromShip)
			randomloc.x -= RangeFromShip;
		randomloc.y = 1;
		
		randomloc.z = UnityEngine.Random.Range(-1*AsteroidRange, AsteroidRange);
		if(randomloc.z <= RangeFromShip && randomloc.z >= 0)
			randomloc.z += RangeFromShip;
		if(randomloc.z <= 0 && randomloc.z >= -1*RangeFromShip)
			randomloc.z -= RangeFromShip;
		
		return randomloc;
	}
	void Start () 
	{
		for(int i = 0; i < NumberOfAsteroids; i++)
		{
			GameObject go;
			Vector3 randomloc = SetRange();
			go = (GameObject)Instantiate (asteroidPrefab, randomloc, Quaternion.identity);
			go.name = "Asteroid " + i;
			Collider[] nearasteroids = Physics.OverlapSphere(go.transform.position,6);
			for(int j = 0; j < nearasteroids.Length; j++)
			{
				Vector3 temp = nearasteroids[j].gameObject.transform.position;
				if(temp.x <= AsteroidRadius)
					temp.x -= AsteroidRadius;
				if(temp.x >= AsteroidRadius)
					temp.x += AsteroidRadius;
				if(temp.z <= AsteroidRadius)
					temp.z += AsteroidRadius;
				nearasteroids[j].gameObject.transform.position = temp;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
