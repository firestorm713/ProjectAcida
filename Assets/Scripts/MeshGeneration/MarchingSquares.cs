///   Marching Squares: 2D Surface Reconstruction
///
///   Derived from "Polygonising a scalar field (Marching Cubes)" by Paul Bourke
///   http://local.wasp.uwa.edu.au/~pbourke/geometry/polygonise/
///   And a lot of inspiration of how to use it with Unity by Brian R. Cowan.
///   See some of his Work here: http://www.briancowan.net/unity/fx
///
///   usage:
///   This script is used to spawn asteroids
///
///   This script is placed in public domain. The author takes no responsibility for any possible harm.
 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class MarchingSquares : MonoBehaviour 
{
	public float isoLevel = 0.25f;
	public GameObject SplitAsteroid;
	
	public Material AsteroidMaterial;
 
	#region helper classes
 
	///  A Triangle in 2D
	///
	public class Triangle {
		// the triangles vertices
		public Vector2[] p;
		// saves "outside" lines index positions
		public int[] outerline;
		// constructor
		public Triangle() {
			p = new Vector2[3];
			outerline = new int[2];
			outerline[0] = -1;
			outerline[1] = -1;
		}
	}
 
	///  A Square has four Vertices. (p)
	///  For each Vertice there is a float elem [0,1] (see val[] array)
	///  2     3
	///  +-----+
	///  |     |
	///  |     |
	///  +-----+
	///  0     1
	///
	public class SquareCell {
		public Vector2[] p;
		public float[] val;
 
		public SquareCell() {
			p = new Vector2[4];
			val = new float[4];
		}
	}
	
	#endregion helper classes
 
	// render mesh arrays
	ArrayList vert;
	ArrayList uv;
	ArrayList tri;
	ArrayList norm;
	// collider mesh arrays
	ArrayList cvert;
	ArrayList cuv;
	ArrayList ctri;
	ArrayList cnorm;
 
	/// Linearly interpolate the position where an isosurface cuts
	/// an edge between two vertices, each with their own scalar value
	///
	Vector2 VertexInterp(float isolevel, ref SquareCell cell, int pid1, int pid2) {
		Vector2 p1 = cell.p[pid1];
		Vector2 p2 = cell.p[pid2];
		float valp1 = cell.val[pid1];
		float valp2 = cell.val[pid2];
 
		float mu;
		Vector2 p = Vector2.zero;
 
		if (Math.Abs(isolevel-valp1) < 0.00001)
			return(p1);
		if (Math.Abs(isolevel-valp2) < 0.00001)
			return(p2);
		if (Math.Abs(valp1-valp2) < 0.00001)
			return(p1);
 
		mu = (isolevel - valp1) / (valp2 - valp1);
		p.x = p1.x + mu * (p2.x - p1.x);
		p.y = p1.y + mu * (p2.y - p1.y);
		return(p);
	}
 
	/// All cases
	///
	/// Case 0   Case 1   Case 2   Case 3   Case 4   Case 5   Case 6   Case 7
	/// O-----O  O-----O  O-----O  O-----O  O-----#  O-----#  O-----#  O-----#
	/// |     |  |     |  |     |  |     |  |    \|  |    \|  |  |  |  |/    |
	/// |     |  |\    |  |    /|  |-----|  |     |  |\    |  |  |  |  |     |
	/// O-----O  #-----O  O-----#  #-----#  O-----O  #-----O  O-----#  #-----#
	///
	/// Case 8   Case 9   Case 10  Case 11  Case 12  Case 13  Case 14  Case 15
	/// #-----O  #-----O  #-----O  #-----O  #-----#  #-----#  #-----#  #-----#
	/// |/    |  |  |  |  |/    |  |    \|  |-----|  |     |  |     |  |     |
	/// |     |  |  |  |  |    /|  |     |  |     |  |    /|  |\    |  |     |
	/// O-----O  #-----O  O-----#  #-----#  O-----O  #-----O  O-----#  #-----#
	///
	private int Polygonise(SquareCell cell, out Triangle[] triangles, float isoLevel) {
 
		triangles = new Triangle[3]; // => Max 3 Triangles needed
 
		// decide which case we have
 
//		bool case_0  = cell.val[0] <  isoLevel && cell.val[1] <  isoLevel && cell.val[2] <  isoLevel && cell.val[3] <  isoLevel;
		bool case_1  = cell.val[0] >= isoLevel && cell.val[1] <  isoLevel && cell.val[2] <  isoLevel && cell.val[3] <  isoLevel;
		bool case_2  = cell.val[0] <  isoLevel && cell.val[1] >= isoLevel && cell.val[2] <  isoLevel && cell.val[3] <  isoLevel;
		bool case_3  = cell.val[0] >= isoLevel && cell.val[1] >= isoLevel && cell.val[2] <  isoLevel && cell.val[3] <  isoLevel;
		bool case_4  = cell.val[0] <  isoLevel && cell.val[1] <  isoLevel && cell.val[2] <  isoLevel && cell.val[3] >= isoLevel;
		bool case_5  = cell.val[0] >= isoLevel && cell.val[1] <  isoLevel && cell.val[2] <  isoLevel && cell.val[3] >= isoLevel;
		bool case_6  = cell.val[0] <  isoLevel && cell.val[1] >= isoLevel && cell.val[2] <  isoLevel && cell.val[3] >= isoLevel;
		bool case_7  = cell.val[0] >= isoLevel && cell.val[1] >= isoLevel && cell.val[2] <  isoLevel && cell.val[3] >= isoLevel;
		bool case_8  = cell.val[0] <  isoLevel && cell.val[1] <  isoLevel && cell.val[2] >= isoLevel && cell.val[3] <  isoLevel;
		bool case_9  = cell.val[0] >= isoLevel && cell.val[1] <  isoLevel && cell.val[2] >= isoLevel && cell.val[3] <  isoLevel;
		bool case_10 = cell.val[0] <  isoLevel && cell.val[1] >= isoLevel && cell.val[2] >= isoLevel && cell.val[3] <  isoLevel;
		bool case_11 = cell.val[0] >= isoLevel && cell.val[1] >= isoLevel && cell.val[2] >= isoLevel && cell.val[3] <  isoLevel;
		bool case_12 = cell.val[0] <  isoLevel && cell.val[1] <  isoLevel && cell.val[2] >= isoLevel && cell.val[3] >= isoLevel;
		bool case_13 = cell.val[0] >= isoLevel && cell.val[1] <  isoLevel && cell.val[2] >= isoLevel && cell.val[3] >= isoLevel;
		bool case_14 = cell.val[0] <  isoLevel && cell.val[1] >= isoLevel && cell.val[2] >= isoLevel && cell.val[3] >= isoLevel;
		bool case_15 = cell.val[0] >= isoLevel && cell.val[1] >= isoLevel && cell.val[2] >= isoLevel && cell.val[3] >= isoLevel;
 
		// make triangles
		int ntriang = 0;
		if (case_1) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 2, 0);
			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 0, 1);
			triangles[0].p[2] = cell.p[0];
			triangles[0].outerline[0] = 0;
			triangles[0].outerline[1] = 1;
			ntriang++;
		}
		if (case_2) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 0, 1);
			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 1, 3);
			triangles[0].p[2] = cell.p[1];
			triangles[0].outerline[0] = 0;
			triangles[0].outerline[1] = 1;
			ntriang++;
		}
		if (case_3) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 0, 2);
			triangles[0].p[1] = cell.p[1];
			triangles[0].p[2] = cell.p[0];
			// no outer line...
			ntriang++;
 
			triangles[1] = new Triangle();
			triangles[1].p[0] = VertexInterp(isoLevel, ref cell, 0, 2);
			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 1, 3);
			triangles[1].p[2] = cell.p[1];
			triangles[1].outerline[0] = 0;
			triangles[1].outerline[1] = 1;
			ntriang++;
		}
		if (case_4) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 1, 3);
			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
			triangles[0].p[2] = cell.p[3];
			triangles[0].outerline[0] = 0;
			triangles[0].outerline[1] = 1;
			ntriang++;
		}
		if (case_5) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 1, 3);
			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
			triangles[0].p[2] = cell.p[3];
			triangles[0].outerline[0] = 0;
			triangles[0].outerline[1] = 1;
			ntriang++;
 
			triangles[1] = new Triangle();
			triangles[1].p[0] = cell.p[0];
			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 0, 2);
			triangles[1].p[2] = VertexInterp(isoLevel, ref cell, 0, 1);
			triangles[1].outerline[0] = 1;
			triangles[1].outerline[1] = 2;
			ntriang++;
		}
		if (case_6) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 2, 3);
			triangles[0].p[1] = cell.p[3];
			triangles[0].p[2] = cell.p[1];
			// no outer line...
			ntriang++;
 
			triangles[1] = new Triangle();
			triangles[1].p[0] = VertexInterp(isoLevel, ref cell, 0, 1);
			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
			triangles[1].p[2] = cell.p[1];
			triangles[1].outerline[0] = 0;
			triangles[1].outerline[1] = 1;
			ntriang++;
		}
		if (case_7) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 2, 3);
			triangles[0].p[1] = cell.p[3];
			triangles[0].p[2] = cell.p[1];
			// no outer line...
			ntriang++;
 
			triangles[1] = new Triangle();
			triangles[1].p[0] = VertexInterp(isoLevel, ref cell, 0, 2);
			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
			triangles[1].p[2] = cell.p[1];
			triangles[1].outerline[0] = 0;
			triangles[1].outerline[1] = 1;
			ntriang++;
 
			triangles[2] = new Triangle();
			triangles[2].p[0] = cell.p[0];
			triangles[2].p[1] = VertexInterp(isoLevel, ref cell, 0, 2);
			triangles[2].p[2] = cell.p[1];
			// no outer line...
			ntriang++;
		}
		if (case_8) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 2, 3);
			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 0, 2);
			triangles[0].p[2] = cell.p[2];
			triangles[0].outerline[0] = 0;
			triangles[0].outerline[1] = 1;
			ntriang++;
		}
		if (case_9) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = cell.p[0];
			triangles[0].p[1] = cell.p[2];
			triangles[0].p[2] = VertexInterp(isoLevel, ref cell, 0, 1);
			// no outer line...
			ntriang++;
 
			triangles[1] = new Triangle();
			triangles[1].p[0] = cell.p[2];
			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
			triangles[1].p[2] = VertexInterp(isoLevel, ref cell, 0, 1);
			triangles[1].outerline[0] = 1;
			triangles[1].outerline[1] = 2;
			ntriang++;
		}
		if (case_10) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = cell.p[2];
			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
			triangles[0].p[2] = VertexInterp(isoLevel, ref cell, 0, 2);
			triangles[0].outerline[0] = 1;
			triangles[0].outerline[1] = 2;
			ntriang++;
 
			triangles[1] = new Triangle();
			triangles[1].p[0] = VertexInterp(isoLevel, ref cell, 0, 1);
			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 1, 3);
			triangles[1].p[2] = cell.p[1];
			triangles[1].outerline[0] = 0;
			triangles[1].outerline[1] = 1;
			ntriang++;
		}
		if (case_11) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = cell.p[0];
			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 1, 3);
			triangles[0].p[2] = cell.p[1];
			// no outer line...
			ntriang++;
 
			triangles[1] = new Triangle();
			triangles[1].p[0] = VertexInterp(isoLevel, ref cell, 2, 3);
			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 1, 3);
			triangles[1].p[2] = cell.p[0];
			triangles[1].outerline[0] = 0;
			triangles[1].outerline[1] = 1;
			ntriang++;
 
			triangles[2] = new Triangle();
			triangles[2].p[0] = cell.p[2];
			triangles[2].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
			triangles[2].p[2] = cell.p[0];
			// no outer line...
			ntriang++;
		}
		if (case_12) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = cell.p[2];
			triangles[0].p[1] = cell.p[3];
			triangles[0].p[2] = VertexInterp(isoLevel, ref cell, 0, 2);
			// no outer line...
			ntriang++;
 
			triangles[1] = new Triangle();
			triangles[1].p[0] = cell.p[3];
			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 1, 3);
			triangles[1].p[2] = VertexInterp(isoLevel, ref cell, 0, 2);
			triangles[1].outerline[0] = 1;
			triangles[1].outerline[1] = 2;
			ntriang++;
		}
		if (case_13) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 0, 1);
			triangles[0].p[1] = cell.p[0];
			triangles[0].p[2] = cell.p[2];
			// no outer line...
			ntriang++;
 
			triangles[1] = new Triangle();
			triangles[1].p[0] = VertexInterp(isoLevel, ref cell, 1, 3);
			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 0, 1);
			triangles[1].p[2] = cell.p[2];
			triangles[1].outerline[0] = 0;
			triangles[1].outerline[1] = 1;
			ntriang++;
 
			triangles[2] = new Triangle();
			triangles[2].p[0] = VertexInterp(isoLevel, ref cell, 1, 3);
			triangles[2].p[1] = cell.p[2];
			triangles[2].p[2] = cell.p[3];
			// no outer line...
			ntriang++;
		}
		if (case_14) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = cell.p[1];
			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 0, 1);
			triangles[0].p[2] = cell.p[3];
			// no outer line...
			ntriang++;
 
			triangles[1] = new Triangle();
			triangles[1].p[0] = cell.p[3];
			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 0, 1);
			triangles[1].p[2] = VertexInterp(isoLevel, ref cell, 0, 2);
			triangles[1].outerline[0] = 1;
			triangles[1].outerline[1] = 2;
			ntriang++;
 
			triangles[2] = new Triangle();
			triangles[2].p[0] = VertexInterp(isoLevel, ref cell, 0, 2);
			triangles[2].p[1] = cell.p[2];
			triangles[2].p[2] = cell.p[3];
			// no outer line...
			ntriang++;
		}
		if (case_15) {
			triangles[0] = new Triangle();
			triangles[0].p[0] = cell.p[2];
			triangles[0].p[1] = cell.p[1];
			triangles[0].p[2] = cell.p[0];
			// no outer line...
			ntriang++;
 
			triangles[1] = new Triangle();
			triangles[1].p[0] = cell.p[1];
			triangles[1].p[1] = cell.p[2];
			triangles[1].p[2] = cell.p[3];
			// no outer line...
			ntriang++;
		}
 
		return ntriang;
	}
 
	/// this method renders two meshes:
	/// cmesh the collision mesh (just the outline vertices extruded)
	/// rmesh the render mesh (the 2d surface of the given voxel lattices)
	///
	public void MarchSquares(out Mesh cmesh, out Mesh rmesh, ref SquareCell[,] cells, float isolevel) {
 
		Vector2 uvScale = new Vector2(1.0f / cells.GetLength(0), 1.0f / cells.GetLength(1));
		// triangle index counter
		int tricount = 0;
		// collider triangle index counter
		int ctricount = 0;
		// mesh data arrays - just clear when reused
		if (vert == null) vert = new ArrayList(); else vert.Clear();
		if (uv == null)   uv = new ArrayList();   else uv.Clear();
		if (tri == null)  tri = new ArrayList();  else tri.Clear();
		if (norm == null) norm = new ArrayList(); else norm.Clear();
		// collider mesh arrays
		if (cvert == null) cvert = new ArrayList(); else cvert.Clear();
		if (cuv == null)   cuv = new ArrayList();   else cuv.Clear();
		if (ctri == null)  ctri = new ArrayList();  else ctri.Clear();
		if (cnorm == null) cnorm = new ArrayList(); else cnorm.Clear();
 
		for (int i = 0; i < cells.GetLength(0); i++) {
			for (int j = 0; j < cells.GetLength(1); j++) {
 
				SquareCell cell = cells[i,j];
 
				Triangle[] triangles;
				Polygonise(cell, out triangles, isolevel);
 
				for (int k = 0; k < triangles.Length; k++) {
					Triangle triangle = triangles[k];
					if (triangle != null) {
 
						Vector3 p0 = new Vector3(triangle.p[0].x-5, 0, triangle.p[0].y-5);
						Vector3 p1 = new Vector3(triangle.p[1].x-5, 0, triangle.p[1].y-5);
						Vector3 p2 = new Vector3(triangle.p[2].x-5, 0, triangle.p[2].y-5);
 
						/// Start Vertices One ---------------------------------------
						vert.Add(p0);
						vert.Add(p1);
						vert.Add(p2);
						// Triangles
						tri.Add(tricount);
						tri.Add(tricount+1);
						tri.Add(tricount+2);
						// Normals
						Vector3 vn1 = p0 - p1; Vector3 vn2 = p0 - p2;
						Vector3 n = Vector3.Normalize ( Vector3.Cross(vn1,vn2) );
						norm.Add(n); norm.Add(n); norm.Add(n);
						uv.Add(Vector2.Scale(new Vector2 (p0.x, p0.z), new Vector2(uvScale.x, uvScale.y)));
						uv.Add(Vector2.Scale(new Vector2 (p1.x, p1.z), new Vector2(uvScale.x, uvScale.y)));
						uv.Add(Vector2.Scale(new Vector2 (p2.x, p2.z), new Vector2(uvScale.x, uvScale.y)));
						tricount += 3;
						/// END Vertices One ---------------------------------------
 
						if (triangle.outerline[0] != -1) {
							Vector3 o1 = new Vector3(triangle.p[triangle.outerline[0]].x-5, 0, triangle.p[triangle.outerline[0]].y-5);
							Vector3 o2 = new Vector3(triangle.p[triangle.outerline[1]].x-5, 0, triangle.p[triangle.outerline[1]].y-5);
							Vector3 bo1 = o1; o1.y = -1; // o1 transposed one unit down
							Vector3 bo2 = o2; o2.y = -1; // o2 transposed one unit down
							/// Start Vertices Two ---------------------------------------
							cvert.Add(o1);
							cvert.Add(o2);
							cvert.Add(bo1);
							// Triangles
							ctri.Add(ctricount);
							ctri.Add(ctricount+1);
							ctri.Add(ctricount+2);
							// Normals
							Vector3 ovn1 = o1 - o2; Vector3 ovn2 = o1 - bo1;
							Vector3 on = Vector3.Normalize ( Vector3.Cross(ovn1,ovn2) );
							cnorm.Add(on); cnorm.Add(on); cnorm.Add(on);
							cuv.Add(Vector2.zero); cuv.Add(Vector2.zero); cuv.Add(Vector2.zero);
							ctricount += 3;
							/// END Vertices Two ---------------------------------------
 
							/// Start Vertices Three ---------------------------------------
							cvert.Add(bo2);
							cvert.Add(bo1);
							cvert.Add(o2);
							// Triangles
							ctri.Add(ctricount);
							ctri.Add(ctricount+1);
							ctri.Add(ctricount+2);
							// Normals
							Vector3 oovn1 = o2 - bo1; Vector3 oovn2 = o2 - bo2;
							Vector3 oon = Vector3.Normalize ( Vector3.Cross(oovn1,oovn2) )*-1;
							cnorm.Add(oon); cnorm.Add(oon); cnorm.Add(oon);
							cuv.Add(Vector2.zero); cuv.Add(Vector2.zero); cuv.Add(Vector2.zero);
							ctricount += 3;
							/// END Vertices Three ---------------------------------------
						}
					}
				}
			}
		}
 
		// prepare the collision mesh
		cmesh = new Mesh();
		cmesh.vertices = (Vector3[]) cvert.ToArray(typeof(Vector3));
		cmesh.uv = (Vector2[]) cuv.ToArray(typeof(Vector2));
		cmesh.triangles = (int[]) ctri.ToArray(typeof(int));
		cmesh.normals = (Vector3[]) cnorm.ToArray(typeof(Vector3));
 
		// prepare the render mesh
		rmesh = new Mesh();
		rmesh.vertices = (Vector3[]) vert.ToArray(typeof(Vector3));
		rmesh.uv = (Vector2[]) uv.ToArray(typeof(Vector2));
		rmesh.triangles = (int[]) tri.ToArray(typeof(int));
		rmesh.normals = (Vector3[]) norm.ToArray(typeof(Vector3));
	}
 
 
 
 
 
 
 
 
	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	/////////// A simple Example /////////////////////////////////////////////////////////////////////////////
	/// //////////////////////////////////////////////////////////////////////////////////////////////////////
 
	//GameObject Testg;
	GameObject TestRenderg;
	SquareCell[,] cells;
	public float[,] data =
		{{0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f}, 
		 {0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f}, 
		 {0.0f,0.0f,0.0f,0.5f,0.9f,0.9f,0.9f,0.5f,0.0f,0.0f,0.0f}, 
		 {0.0f,0.0f,0.5f,1.0f,1.0f,1.0f,1.0f,1.0f,0.5f,0.0f,0.0f}, 
		 {0.0f,0.0f,0.9f,1.0f,1.0f,1.0f,1.0f,1.0f,0.9f,0.0f,0.0f}, 
		 {0.0f,0.0f,0.9f,1.0f,1.0f,1.0f,1.0f,1.0f,0.9f,0.0f,0.0f}, 
		 {0.0f,0.0f,0.9f,1.0f,1.0f,1.0f,1.0f,1.0f,0.9f,0.0f,0.0f}, 
		 {0.0f,0.0f,0.5f,1.0f,1.0f,1.0f,1.0f,1.0f,0.5f,0.0f,0.0f}, 
		 {0.0f,0.0f,0.0f,0.5f,0.9f,0.9f,0.9f,0.5f,0.0f,0.0f,0.0f}, 
		 {0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f}, 
		 {0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f},};

	public void Noise()
	{
		for(int i = 0; i < data.GetLength(0); i++)
		{
			for (int j = 0; j < data.GetLength(1); j++)
			{
				if(data[i,j]>=0.25f && data[i,j] < 1.0f)
				{
					data[i,j] = UnityEngine.Random.Range(0.25f, 1.0f);
					
				}
			}
		}
	}
	
	public void Start() 
	{
		Noise ();
		Noise ();
		Noise ();
		cells = new SquareCell[data.GetLength(0)-1,data.GetLength(1)-1];
		// put data in cells
		for (int i = 0; i < data.GetLength(0); i++) {
			for (int j = 0; j < data.GetLength(1); j++) {
				// do not process the edges of the data array since cell.dim + 1 == data.dim
				if (i < data.GetLength(0)-1 && j < data.GetLength(1)-1) {
					SquareCell cell = new SquareCell();
					cell.p[0] = new Vector2(i,j);
					cell.p[1] = new Vector2(i+1,j);
					cell.p[2] = new Vector2(i,j+1);
					cell.p[3] = new Vector2(i+1,j+1);
 
					cell.val[0] = data[i,j];
					cell.val[1] = data[i+1,j];
					cell.val[2] = data[i,j+1];
					cell.val[3] = data[i+1,j+1];
 
					cells[i,j] = cell;
				}
			}
		}
 
		// create a gameobject
		//Testg = new GameObject();
		//Testg.name = "msquare";
		//gameObject.transform.position = Vector3.zero;
		//gameObject.transform.rotation = Quaternion.identity;
		// collision meshfilter
		MeshFilter mf = (MeshFilter) gameObject.AddComponent(typeof(MeshFilter));
		// normally you don't want to render the collision mesh
		MeshRenderer mr = (MeshRenderer) gameObject.AddComponent(typeof(MeshRenderer));
		mr.material.color = new Color(.5f,.6f,1f, 1f);
		// collider rigidbody...
		gameObject.AddComponent(typeof(Rigidbody));
		gameObject.rigidbody.isKinematic = false;
		gameObject.rigidbody.useGravity = true;
		gameObject.rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
		gameObject.rigidbody.mass = 10;
		gameObject.rigidbody.drag = 0.1f;
		gameObject.rigidbody.angularDrag = 0.4f;
		gameObject.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		MeshCollider mc = (MeshCollider) gameObject.AddComponent(typeof(MeshCollider));
		mc.sharedMesh = mf.mesh;
		mc.convex = false;
		// create texture sub gameobject
		TestRenderg = new GameObject();
		TestRenderg.transform.parent = gameObject.transform;
		TestRenderg.name = "msquare__rendermesh";
		TestRenderg.transform.position = transform.position;
		TestRenderg.transform.rotation = Quaternion.identity;
		TestRenderg.AddComponent(typeof(MeshFilter));
		MeshRenderer cmr = (MeshRenderer)  TestRenderg.AddComponent(typeof(MeshRenderer));
		cmr.material.color = new Color(1f,.6f,1f, 1f);
		// render
		// for the sake of simplicity we are rendering every frame.
		// obviously you should only render when the data of the cells has changed
		Mesh mesh, cmesh;
		MarchSquares(out mesh, out cmesh, ref cells, 0.5f);
		// update the render mesh
		mf = (MeshFilter) gameObject.GetComponent(typeof(MeshFilter));
		mf.mesh.Clear();
		mf.mesh.vertices = mesh.vertices;
		mf.mesh.uv = mesh.uv;
		mf.mesh.triangles = mesh.triangles;
		mf.mesh.normals = mesh.normals;
		Destroy(mesh);
		// update the collision mesh
		MeshFilter cmf = (MeshFilter) TestRenderg.GetComponent(typeof(MeshFilter));
		cmf.mesh.Clear();
		cmf.mesh.vertices = cmesh.vertices;
		cmf.mesh.uv = cmesh.uv;
		cmf.mesh.triangles = cmesh.triangles;
		cmf.mesh.normals = cmesh.normals;
		Destroy(cmesh);
		mc = (MeshCollider) gameObject.GetComponent(typeof(MeshCollider));
		if (mc != null)
			mc.sharedMesh = mf.mesh;
		
		renderer.material.color = Color.gray;
		mf.renderer.material.color = Color.gray;
		
		gameObject.collider.enabled = false;
		gameObject.collider.enabled = true;
		if(transform.position.y!=1)
		{
			Vector3 temp = transform.position;
			temp.y = 1;
			transform.position = temp;
		}
	}
 
	public void FixedUpdate() {
		// render
		// for the sake of simplicity we are rendering every frame.
		// obviously you should only render when the data of the cells has changed
		Mesh mesh, cmesh;
		MarchSquares(out mesh, out cmesh, ref cells, 0.25f);
		// update the render mesh
		MeshFilter mf = (MeshFilter) gameObject.GetComponent(typeof(MeshFilter));
		mf.mesh.Clear();
		mf.mesh.vertices = mesh.vertices;
		mf.mesh.uv = mesh.uv;
		mf.mesh.triangles = mesh.triangles;
		mf.mesh.normals = mesh.normals;
		Destroy(mesh);
		// update the collision mesh
		MeshFilter cmf = (MeshFilter) TestRenderg.GetComponent(typeof(MeshFilter));
		cmf.mesh.Clear();
		cmf.mesh.vertices = cmesh.vertices;
		cmf.mesh.uv = cmesh.uv;
		cmf.mesh.triangles = cmesh.triangles;
		cmf.mesh.normals = cmesh.normals;
		Destroy(cmesh);
		MeshCollider mc = (MeshCollider) gameObject.GetComponent(typeof(MeshCollider));
		if (mc != null)
			mc.sharedMesh = mf.mesh;
		if(transform.position.y != 1)
		{
			Vector3 temp = transform.position;
			temp.y = 1;
			transform.position = temp;
		}
	}
	
	public void Updatecells()
	{
		for (int i = 0; i < data.GetLength(0); i++) {
			for (int j = 0; j < data.GetLength(1); j++) {
				// do not process the edges of the data array since cell.dim + 1 == data.dim
				if (i < data.GetLength(0)-1 && j < data.GetLength(1)-1) {
					SquareCell cell = cells[i,j];
					cell.val[0] = data[i,j];
					cell.val[1] = data[i+1,j];
					cell.val[2] = data[i,j+1];
					cell.val[3] = data[i+1,j+1];
					cells[i,j] = cell;
				}
			}
		}
		//CheckforBreaks();
	}
	
	void CheckZero()
	{
		bool zero = true;
		//float integrity = 0;
		for (int i = 0; i < data.GetLength(0); i++)
			for (int j = 0; j < data.GetLength(1); j++)
				if(data[i,j]>0.5f)
					zero = false;

		if(zero)
		{
			Debug.Log("Asteroid Destroyed");
			Destroy (gameObject);
		}
	}
	
	float[,] lastdata;
		
	public void Update()
	{
		//if(lastdata!=data)
		Updatecells();
		CheckZero();
		gameObject.collider.enabled = false;
		gameObject.collider.enabled = true;
		//Debug.Log(transform.rotation.y);
		//Vector3 vel = new Vector3(0, 10, 0);
		//Quaternion dRot = Quaternion.Euler(vel * Time.deltaTime);
		//rigidbody.MoveRotation(rigidbody.rotation * dRot);
	}
	
	public void LateUpdate()
	{
		
	}		

/*	public List<List<GridLoc> > adjacents;
	int index;
	
	public class GridLoc
	{
		public int x, y;
		public float isovalue;
		public GridLoc(int X, int Y, float Isovalue)
		{
			x = X;
			y = Y;
			isovalue = Isovalue;
		}
		public bool Equals(GridLoc A, GridLoc B)
		{
			if(A.x == B.x && A.y == B.y)
				return true;
			else
				return false;
		}
	}
	
	public void CheckforBreaks()
	{
		if(adjacents==null)
			adjacents = new List<List<GridLoc> >();
		else
			adjacents.Clear();
		index = 0;
		for(int i = 0; i < data.GetLength(0); i++)
		{
			for(int j = 0; j < data.GetLength(1); j++)
			{
				if(data[i,j]>=0)
				{
					GridLoc gl = new GridLoc(i,j,data[i,j]);
					if(!InList(gl))
					{
						List<GridLoc> temp = new List<GridLoc>();
						temp.Add(gl);
						adjacents.Add(temp);
						CheckAdjacency(gl);
						index++;
					}
				}
			}
		}
		if(adjacents.Count>1)
		{
			for(int i = 0; i < adjacents.Count; i++)
			{
				GameObject temp = (GameObject)Instantiate(SplitAsteroid, transform.position, transform.rotation);
				MarchingSquares newasteroid = temp.GetComponent<MarchingSquares>();
				for(int x = 0; x < data.GetLength(0); x++)
				{
					for(int y = 0; y < data.GetLength(1); y++)
					{
						data[x,y] = 0;
					}
				}
				for(int j = 0; j < adjacents[i].Count; j++)
				{
					data[adjacents[i][j].x,adjacents[i][j].y] = adjacents[i][j].isovalue;
				}
				newasteroid.rigidbody.AddForce(new Vector3(UnityEngine.Random.value,UnityEngine.Random.value,UnityEngine.Random.value));
			}
		}
	}
	
	void CheckAdjacency(GridLoc gl)
	{
		for(int i = -1; i < 1; i++)
		{
			for(int j = -1; j < 1; j++)
			{
				int tx=gl.x+i;
				int ty=gl.y+j;
				if(tx >=0 && tx < data.GetLength(0) &&
				   ty >=0 && ty < data.GetLength(1) &&
				   data[tx,ty]>=isoLevel)
				{
					GridLoc temp = new GridLoc(tx,ty,data[tx,ty]);
					if(!InList(temp))
						adjacents[index].Add(temp);
						CheckAdjacency(temp);
				}
			}
		}
	}
	bool InList(GridLoc gl)
	{
		for(int i = 0; i <adjacents.Count; i++)
		{
			for(int j = 0; j < adjacents[i].Count; j++)
			{
				if(gl==adjacents[i][j])
					return true;
			}
		}
		return false;
	}*/
	
}