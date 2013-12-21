using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour 
{
	public GameObject Asteroid;
	
	float scale = 1;
	float speed = 1;
	
	Vector3[] baseVertices;
	Perlin noise;
	
	// Use this for initialization
	void Start () 
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		noise = new Perlin();
		baseVertices = mesh.vertices;
		Vector3[] vertices = new Vector3[baseVertices.Length];
		
		float timex = Time.time * speed + 0.1365143f;
		float timey = Time.time * speed + 1.21688f;
		float timez = Time.time * speed + 2.5564f;
		for (int i=0; i<vertices.Length;i++)
		{
			Vector3 vertex = baseVertices[i];
			vertex.x +=noise.Noise(timex + vertex.x, timex + vertex.y, timex + vertex.z) * scale;
			vertex.x +=noise.Noise(timey + vertex.x, timey + vertex.y, timey + vertex.z) * scale;
			vertex.x +=noise.Noise(timez + vertex.x, timez + vertex.y, timez + vertex.z) * scale;
			vertices[i] = vertex;
			
		}
		mesh.vertices = vertices;
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
