using UnityEngine;
using System.Collections;

public class Bangbang : MonoBehaviour {
	
	RaycastHit hit;
	Ray ray;
	LineRenderer lr;
	Transform GunPos;
	Vector3 forwardpos;
	GameObject player;
	
	float hitpointx;
	float hitpointz;
	float asteroidx;
	float asteroidz;
	float asteroidrot;
	float srot;
	float crot;
	float rotx;
	float rotz;
	int gridx;
	int gridy;
	float fCurrentEnergy;
	
	public GameObject HitEffect;
	
	public float GunRange = 10;
	
	public float GunMine = 2;
	
	void Awake()
	{
		GunPos = transform;
	}
	void Start () 
	{
		lr = GetComponent<LineRenderer>();
		forwardpos = GunPos.position + GunPos.forward*GunRange;	// Calculate x units out.
		player = GameObject.Find("Player");
	}

	
	void Update () 
	{
		PlayerStats sPlayerStats;
    	sPlayerStats = player.GetComponent<PlayerStats>();
		
		forwardpos = GunPos.position + GunPos.forward*GunRange;	// Calculate default range of gun
		if(Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
		{
			sPlayerStats.EnergyDrain();
			if(sPlayerStats.fCurrentEnergy > 1)
			{
				if (!audio.isPlaying) audio.Play();
				lr.enabled = true;
				lr.SetPosition(0, GunPos.position);				// Shoot out to the default range
				lr.SetPosition(1, forwardpos);
					
				if(Physics.Raycast(GunPos.position, transform.forward, out hit, GunRange))
				{	
					lr.SetPosition(1, hit.point);
					//Debug.Log(hit.point);
					//Debug.Log(hit.collider.transform.position);
					if(hit.collider.gameObject.tag=="Asteroid")
					{
						gridx = Mathf.RoundToInt(hit.point.x - hit.collider.transform.position.x)+5;
						gridy = Mathf.RoundToInt(hit.point.z - hit.collider.transform.position.z)+5;
						Debug.DrawLine(new Vector3(hit.collider.transform.position.x, 5, hit.collider.transform.position.z), new Vector3(rotx, 5, rotz));
						MarchingSquares ms = hit.collider.gameObject.GetComponent<MarchingSquares>();
						if(ms.data[gridx,gridy]>0)
						{
							ms.data[gridx,gridy]-= GunMine/2 * Time.deltaTime;
						}
						for(int i = -1; i<1; i++)
						{
							for(int j = -1; j<1;j++)
							{
								if(gridx+i>=0&&gridx+i<ms.data.GetLength(0)&&gridy+j>=0&&gridy<ms.data.GetLength(1))
									ms.data[gridx+i,gridy+j] -= GunMine/2 *Time.deltaTime;
							}
						}
					}
				}
			}
			else
			{
				lr.enabled = false;
				audio.Stop();
			}
		}
		else 
		{
			lr.enabled = false;
			audio.Stop();
		}
	}
			
	void rotatepoints(Collider hit, Vector3 hitpoint)
	{
		hitpointx = hitpoint.x;
		hitpointz = hitpoint.z;
		asteroidx = hit.transform.position.x;
		asteroidz = hit.transform.position.z;
		asteroidrot = -1*hit.transform.rotation.y;
		srot = Mathf.Sin(asteroidrot);
		crot = Mathf.Cos(asteroidrot);
		hitpointx -= asteroidx;
		hitpointz -= asteroidz;
		rotx = hitpointx * crot + hitpointz * srot;
		rotz = -1*hitpointx * srot + hitpointz * crot;
		rotx += asteroidx;
		rotz += asteroidz;
	}
}